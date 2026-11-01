using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Repositories
{
    /// <summary>
    /// Dostop do baze za obrazec "Demo napak". Nekatere metode so NAMERNO napačne
    /// (označene z "NAPAČNO"), da se vidi problem; poleg vsake je pravilna različica.
    /// Te kode ne kopiraj v pravo aplikacijo.
    /// </summary>
    public class DemoRepository
    {
        private const string RefCodeIndexName = "ix_activity_log_ref_code";

        // Koda, ki v tabeli activity_log obstaja (skripta 05_activity_log.sql jo ustvari).
        private const string SearchedRefCode = "REF-0400000";

        private readonly string _connectionString;

        /// <summary>
        /// Connection string dobi od zunaj (Program.cs ga prebere iz App.config).
        /// </summary>
        public DemoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// NAPAČNO: odpira povezave brez using/Dispose, dokler strežnik ne zavrne nove povezave.
        /// </summary>
        public async Task<ConnectionDemoResult> OpenConnectionsWithoutUsingAsync(int attempts)
        {
            var result = new ConnectionDemoResult();

            // Reference hranimo SAMO zato, da lahko demo na koncu za seboj pospravi. V pravi
            // napačni kodi referenc ni: povezava ostane odprta, dokler je GC nekoč ne pobere,
            // pool pa je ne more ponovno uporabiti, ker zanj še vedno velja, da je "v uporabi".
            var leaked = new List<MySqlConnection>();
            try
            {
                for (int i = 0; i < attempts; i++)
                {
                    // INTERVJU: brez using se Close/Dispose ne pokliče. Vsaka nova OpenAsync zato
                    // ne dobi povezave iz poola, ampak odpre NOVO fizično povezavo. Ko jih je
                    // toliko kot max_connections (v docker-compose 30), strežnik vrne napako 1040
                    // "Too many connections" - takrat ne deluje nobena aplikacija na tej bazi.
                    var connection = new MySqlConnection(_connectionString);
                    await connection.OpenAsync().ConfigureAwait(false);
                    leaked.Add(connection);
                    result.Opened++;
                }
            }
            catch (MySqlException ex)
            {
                result.Error = ex.Message;
            }
            finally
            {
                // Pospravimo za demom: vrnemo povezave v pool in pool izpraznimo, da se fizične
                // povezave res zaprejo in strežnik spet sprejema nove.
                foreach (MySqlConnection connection in leaked)
                {
                    connection.Dispose();
                }

                MySqlConnection.ClearAllPools();
            }

            result.DistinctServerConnections = result.Opened;
            return result;
        }

        /// <summary>
        /// PRAVILNO: vsaka povezava je v using. Pool vedno znova uporabi isto fizično povezavo.
        /// </summary>
        public async Task<ConnectionDemoResult> OpenConnectionsWithUsingAsync(int attempts)
        {
            var result = new ConnectionDemoResult();
            var serverIds = new HashSet<long>();

            for (int i = 0; i < attempts; i++)
            {
                // using pokliče Dispose tudi ob izjemi (prevajalnik naredi try/finally).
                // Dispose povezave ne zapre fizično, ampak jo vrne v pool za naslednji Open.
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync().ConfigureAwait(false);

                    // CONNECTION_ID() = Id seje na strežniku. Če je vedno enak, gre za isto fizično povezavo.
                    long serverId = await connection.ExecuteScalarAsync<long>("SELECT CONNECTION_ID()")
                        .ConfigureAwait(false);
                    serverIds.Add(serverId);
                    result.Opened++;
                }
            }

            result.DistinctServerConnections = serverIds.Count;
            return result;
        }

        /// <summary>
        /// NAPAČNO: sestavi SQL z lepljenjem uporabnikovega vnosa. Vrne besedilo, ki gre v bazo.
        /// </summary>
        public static string BuildUnsafeSql(string lastName)
        {
            // INTERVJU: SQL injection. Vnos x' OR '1'='1 da:
            //   ... WHERE last_name = 'x' OR '1'='1'   -> pogoj je vedno resničen, vrne VSE stranke.
            // Narekovaj v vnosu "zapre" niz in preostanek postane del ukaza. Ubežanje narekovajev
            // na roko ni rešitev (kodne strani, ubežni znaki ...) - edina prava rešitev so parametri.
            return "SELECT id AS Id, first_name AS FirstName, last_name AS LastName, email AS Email, phone AS Phone " +
                   "FROM customers WHERE last_name = '" + lastName + "'";
        }

        /// <summary>
        /// NAPAČNO: izvede ranljivo poizvedbo iz BuildUnsafeSql.
        /// </summary>
        public async Task<List<Customer>> FindByLastNameUnsafeAsync(string lastName)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                IEnumerable<Customer> rows = await connection.QueryAsync<Customer>(BuildUnsafeSql(lastName))
                    .ConfigureAwait(false);
                return rows.ToList();
            }
        }

        /// <summary>
        /// PRAVILNO: enaka poizvedba s parametrom. Vnos je vedno samo vrednost, nikoli del ukaza.
        /// </summary>
        public async Task<List<Customer>> FindByLastNameSafeAsync(string lastName)
        {
            const string sql =
                "SELECT id AS Id, first_name AS FirstName, last_name AS LastName, email AS Email, phone AS Phone " +
                "FROM customers WHERE last_name = @LastName";

            using (var connection = new MySqlConnection(_connectionString))
            {
                IEnumerable<Customer> rows = await connection.QueryAsync<Customer>(sql, new { LastName = lastName })
                    .ConfigureAwait(false);
                return rows.ToList();
            }
        }

        /// <summary>
        /// Poišče zapis po ref_code v tabeli s 500.000 vrsticami - brez indeksa ali z njim.
        /// Pred tem indeks ustvari ali odstrani, nato vrne EXPLAIN in izmerjen čas.
        /// </summary>
        public async Task<QueryPlanResult> RunRefCodeQueryAsync(bool withIndex)
        {
            const string sql =
                "SELECT id, customer_id, action, ref_code, created_at FROM activity_log WHERE ref_code = @Code";

            var parameters = new { Code = SearchedRefCode };

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                await EnsureRefCodeIndexAsync(connection, withIndex).ConfigureAwait(false);

                var result = new QueryPlanResult { Sql = sql.Replace("@Code", "'" + SearchedRefCode + "'") };

                // INTERVJU: EXPLAIN pokaže načrt izvajanja. Ključna stolpca:
                //   type = ALL  -> pregled cele tabele (full table scan), rows ~ 500.000
                //   type = ref  -> iskanje po indeksu (key = ime indeksa), rows ~ 1
                using (DbDataReader reader = await connection.ExecuteReaderAsync("EXPLAIN " + sql, parameters)
                    .ConfigureAwait(false))
                {
                    result.Plan = ReadAsTextTable(reader);
                }

                // Poizvedbo poženemo 3-krat in vzamemo najhitrejšo: prvi zagon lahko bere z diska,
                // naslednji iz pomnilnika (buffer pool) - tako primerjamo sam načrt, ne diska.
                long best = long.MaxValue;
                for (int i = 0; i < 3; i++)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    IEnumerable<dynamic> rows = await connection.QueryAsync(sql, parameters).ConfigureAwait(false);
                    stopwatch.Stop();

                    result.RowCount = rows.Count();
                    best = Math.Min(best, stopwatch.ElapsedMilliseconds);
                }

                result.ElapsedMilliseconds = best;
                return result;
            }
        }

        /// <summary>
        /// Prebere poljuben rezultat v DataTable z besedilnimi stolpci (za prikaz v mreži).
        /// </summary>
        // DataTable.Load tu ne pride v poštev: MySQL za nekatere stolpce EXPLAIN-a v metapodatkih
        // javi NOT NULL, nato pa vrne NULL - Load iz metapodatkov naredi omejitve in vrže
        // ConstraintException. Ročno branje stolpcev in vrednosti je zanesljivo.
        private static DataTable ReadAsTextTable(DbDataReader reader)
        {
            var table = new DataTable();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                table.Columns.Add(reader.GetName(i), typeof(string));
            }

            while (reader.Read())
            {
                var values = new object[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    values[i] = reader.IsDBNull(i) ? "NULL" : Convert.ToString(reader.GetValue(i));
                }

                table.Rows.Add(values);
            }

            return table;
        }

        /// <summary>
        /// Ustvari ali odstrani demo indeks na activity_log.ref_code (glede na želeno stanje).
        /// </summary>
        private static async Task EnsureRefCodeIndexAsync(MySqlConnection connection, bool shouldExist)
        {
            long count = await connection.ExecuteScalarAsync<long>(
                "SELECT COUNT(*) FROM information_schema.statistics " +
                "WHERE table_schema = DATABASE() AND table_name = 'activity_log' AND index_name = @IndexName",
                new { IndexName = RefCodeIndexName }).ConfigureAwait(false);

            bool exists = count > 0;

            // Imena tabel/indeksov ne morejo biti parametri (parametri so samo za VREDNOSTI),
            // zato je DDL sestavljen iz konstant - nikoli iz uporabnikovega vnosa.
            if (shouldExist && !exists)
            {
                await connection.ExecuteAsync("CREATE INDEX " + RefCodeIndexName + " ON activity_log (ref_code)")
                    .ConfigureAwait(false);
            }
            else if (!shouldExist && exists)
            {
                await connection.ExecuteAsync("DROP INDEX " + RefCodeIndexName + " ON activity_log")
                    .ConfigureAwait(false);
            }
        }
    }
}
