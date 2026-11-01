using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Repositories
{
    /// <summary>
    /// Dostop do tabele customers v MySQL prek Dapperja. Ne ve nič o WinForms.
    /// Vsaka metoda odpre svojo povezavo in jo takoj zapre - za pooling poskrbi MySqlConnector,
    /// zato je "odpri pozno, zapri zgodaj" poceni in varno.
    /// </summary>
    // INTERVJU: ConfigureAwait(false) v repozitoriju pomeni "nadaljevanja ne vračaj na UI nit".
    // Knjižnična koda ne potrebuje UI niti, zato je tako hitreje in ni nevarnosti deadlocka,
    // če bi kdo nad to metodo (napačno) poklical .Result. Na UI plasti ConfigureAwait(false)
    // NE uporabljamo, ker moramo po await-u spet dostopati do kontrol.
    public class CustomerRepository : ICustomerRepository
    {
        // Stolpci v bazi so snake_case, lastnosti v C# PascalCase - z aliasi (AS)
        // Dapperju povemo, kam naj preslika posamezen stolpec.
        private const string SelectColumns =
            "SELECT id AS Id, first_name AS FirstName, last_name AS LastName, email AS Email, " +
            "phone AS Phone, updated_at AS UpdatedAt " +
            "FROM customers";

        // Skupni WHERE za iskanje. Če iskalnega niza ni (@Pattern IS NULL), vrne vse.
        private const string SearchWhere =
            " WHERE (@Pattern IS NULL OR first_name LIKE @Pattern OR last_name LIKE @Pattern OR email LIKE @Pattern)";

        private const string InsertSql =
            "INSERT INTO customers (first_name, last_name, email, phone) " +
            "VALUES (@FirstName, @LastName, @Email, @Phone)";

        private readonly string _connectionString;

        /// <summary>
        /// Repozitorij dobi connection string od zunaj (iz App.config prek Program.cs),
        /// sam ne bere konfiguracije - tako ga je lažje uporabiti tudi z drugo bazo.
        /// </summary>
        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Vse stranke, urejene po priimku in imenu (npr. za izvoz v CSV).
        /// </summary>
        public async Task<List<Customer>> GetAllAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                // Dapper sam odpre zaprto povezavo in jo po poizvedbi zapre.
                IEnumerable<Customer> customers = await connection
                    .QueryAsync<Customer>(SelectColumns + " ORDER BY last_name, first_name")
                    .ConfigureAwait(false);

                return customers.ToList();
            }
        }

        /// <summary>
        /// Vrne stranko ali null, če ne obstaja.
        /// </summary>
        public async Task<Customer> GetByIdAsync(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                // INTERVJU: parametri (@Id) namesto lepljenja nizov. Vrednost gre do baze ločeno
                // od besedila SQL, zato je ni mogoče "podtakniti" kot del ukaza (SQL injection),
                // hkrati pa ni težav z narekovaji, decimalno vejico ali obliko datuma.
                return await connection
                    .QuerySingleOrDefaultAsync<Customer>(SelectColumns + " WHERE id = @Id", new { Id = id })
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Straničenje z LIMIT/OFFSET in iskanje z LIKE. Skupno število in stran prebere
        /// v enem obisku baze z QueryMultiple.
        /// </summary>
        public async Task<PagedResult<Customer>> SearchPagedAsync(string term, int pageIndex, int pageSize)
        {
            // INTERVJU: QueryMultiple pošlje dve poizvedbi v enem ukazu (en "round-trip") in
            // rezultate bere zaporedno. Vrstni red Read* klicev se mora ujemati z vrstnim redom SELECT-ov.
            string sql =
                "SELECT COUNT(*) FROM customers" + SearchWhere + ";" +
                SelectColumns + SearchWhere +
                // id na koncu ORDER BY naredi vrstni red enoličen - brez tega bi se lahko stranke
                // z enakim imenom pri prehodu med stranmi ponovile ali izpustile.
                " ORDER BY last_name, first_name, id LIMIT @Limit OFFSET @Offset;";

            var parameters = new
            {
                Pattern = BuildLikePattern(term),
                Limit = pageSize,
                // INTERVJU: OFFSET mora baza vseeno prebrati in zavreči vse prejšnje vrstice,
                // zato je pri zelo velikih tabelah globoka stran počasna. Alternativa je
                // "keyset pagination": WHERE (last_name, id) > (@LastName, @LastId) LIMIT n.
                Offset = pageIndex * pageSize
            };

            using (var connection = new MySqlConnection(_connectionString))
            using (SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, parameters).ConfigureAwait(false))
            {
                var result = new PagedResult<Customer>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                result.TotalCount = await multi.ReadSingleAsync<int>().ConfigureAwait(false);
                result.Items = (await multi.ReadAsync<Customer>().ConfigureAwait(false)).ToList();
                return result;
            }
        }

        /// <summary>
        /// Vstavi stranko in ji nastavi Id in UpdatedAt, ki ju je dodelila baza.
        /// </summary>
        public async Task InsertAsync(Customer customer)
        {
            // LAST_INSERT_ID() je vezan na povezavo (sejo), zato mora teči v istem ukazu/povezavi
            // kot INSERT. Tako dobimo pravi Id tudi, če hkrati vstavljajo drugi uporabniki.
            // Prebremo še updated_at (privzeto vrednost je nastavila baza), ki ga potrebujemo
            // za kasnejše optimistično zaklepanje.
            const string sql =
                InsertSql + "; " +
                "SELECT id AS Id, updated_at AS UpdatedAt FROM customers WHERE id = LAST_INSERT_ID();";

            using (var connection = new MySqlConnection(_connectionString))
            {
                Customer inserted = await connection.QuerySingleAsync<Customer>(sql, customer).ConfigureAwait(false);
                customer.Id = inserted.Id;
                customer.UpdatedAt = inserted.UpdatedAt;
            }
        }

        /// <summary>
        /// Posodobi stranko z optimističnim zaklepanjem. UPDATE uspe samo, če je updated_at v bazi
        /// enak tistemu, ki ga je stranka imela ob branju. Ob uspehu nastavi nov UpdatedAt.
        /// </summary>
        public async Task<UpdateResult> UpdateAsync(Customer customer)
        {
            // updated_at nastavimo izrecno: ON UPDATE CURRENT_TIMESTAMP se sproži samo, če se kak
            // stolpec res spremeni. Mi želimo novo "verzijo" ob vsakem shranjevanju.
            const string updateSql =
                "UPDATE customers " +
                "SET first_name = @FirstName, last_name = @LastName, email = @Email, phone = @Phone, " +
                "    updated_at = CURRENT_TIMESTAMP(6) " +
                "WHERE id = @Id AND updated_at = @UpdatedAt";

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);

                // INTERVJU: transakcija tu zagotovi, da preberemo PRAV naš novi updated_at.
                // UPDATE zaklene vrstico do COMMIT-a, zato je vmes nihče drug ne more spremeniti.
                // Brez transakcije bi lahko med UPDATE in SELECT nekdo drug shranil svojo verzijo,
                // mi pa bi si zapomnili njegov čas in naslednjič tiho prepisali njegove podatke.
                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    // MySqlConnector privzeto vrača število NAJDENIH vrstic (UseAffectedRows=false),
                    // zato 0 pomeni "WHERE ni našel vrstice", ne "ni bilo spremembe".
                    int rows = await connection.ExecuteAsync(updateSql, customer, transaction).ConfigureAwait(false);

                    if (rows == 0)
                    {
                        // Ločimo dva razloga: vrstica ne obstaja več ali jo je vmes spremenil nekdo drug.
                        long count = await connection.ExecuteScalarAsync<long>(
                            "SELECT COUNT(*) FROM customers WHERE id = @Id", new { customer.Id }, transaction)
                            .ConfigureAwait(false);

                        await transaction.RollbackAsync().ConfigureAwait(false);
                        return count == 0 ? UpdateResult.NotFound : UpdateResult.Conflict;
                    }

                    DateTime newUpdatedAt = await connection.ExecuteScalarAsync<DateTime>(
                        "SELECT updated_at FROM customers WHERE id = @Id", new { customer.Id }, transaction)
                        .ConfigureAwait(false);

                    await transaction.CommitAsync().ConfigureAwait(false);

                    // Objekt spremenimo šele po uspešnem COMMIT-u.
                    customer.UpdatedAt = newUpdatedAt;
                    return UpdateResult.Updated;
                }
            }
        }

        /// <summary>
        /// Izbriše stranko. Vrne false, če je ni bilo.
        /// Stranke z naročili baza ne pusti izbrisati (FK ON DELETE RESTRICT) - to prevedemo v razumljivo napako.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    int rows = await connection
                        .ExecuteAsync("DELETE FROM customers WHERE id = @Id", new { Id = id })
                        .ConfigureAwait(false);
                    return rows > 0;
                }
                // Filter izjem (when): ujamemo samo kršitev tujega ključa, ostale napake gredo naprej.
                catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.RowIsReferenced2)
                {
                    throw new InvalidOperationException(
                        "Stranke ni mogoče izbrisati, ker ima naročila.", ex);
                }
            }
        }

        /// <summary>
        /// Pokliče shranjeno proceduro sp_customer_stats: OUT parametri + rezultat (top izdelki).
        /// </summary>
        public async Task<CustomerStats> GetStatsAsync(int customerId)
        {
            // INTERVJU: DynamicParameters omogoča tudi izhodne (OUT) parametre. Vrednosti OUT
            // parametrov so na voljo šele, ko je rezultat procedure v celoti prebran - QueryAsync
            // (privzeto "buffered") ga prebere do konca, zato lahko takoj za njim kličemo Get<T>.
            var parameters = new DynamicParameters();
            parameters.Add("p_customer_id", customerId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("p_order_count", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("p_total_amount", dbType: DbType.Decimal, direction: ParameterDirection.Output);
            parameters.Add("p_last_order", dbType: DbType.DateTime, direction: ParameterDirection.Output);

            using (var connection = new MySqlConnection(_connectionString))
            {
                // CommandType.StoredProcedure: v ukaz napišemo samo ime procedure,
                // MySqlConnector sam sestavi CALL in poveže parametre po imenu.
                IEnumerable<ProductStat> products = await connection.QueryAsync<ProductStat>(
                    "sp_customer_stats", parameters, commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                return new CustomerStats
                {
                    CustomerId = customerId,
                    TopProducts = products.ToList(),
                    OrderCount = parameters.Get<int>("p_order_count"),
                    TotalAmount = parameters.Get<decimal>("p_total_amount"),
                    // Stranka brez naročil: procedura vrne NULL, zato DateTime? (nullable).
                    LastOrderDate = parameters.Get<DateTime?>("p_last_order")
                };
            }
        }

        /// <summary>
        /// Vstavi paket strank v eni transakciji: ali se vstavijo vse iz paketa ali nobena.
        /// </summary>
        public async Task InsertBatchAsync(IList<Customer> customers, CancellationToken cancellationToken)
        {
            if (customers.Count == 0)
            {
                return;
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                using (MySqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
                {
                    // INTERVJU: Dapper s seznamom kot parametrom izvede INSERT enkrat za vsak element
                    // (N obiskov baze). Za res velike uvoze je hitreje več vrstic v enem INSERT ali
                    // MySqlBulkCopy / LOAD DATA. CommandDefinition omogoča podati CancellationToken,
                    // da preklic prekine tudi čakanje na bazo.
                    var command = new CommandDefinition(InsertSql, customers, transaction,
                        cancellationToken: cancellationToken);
                    await connection.ExecuteAsync(command).ConfigureAwait(false);

                    // Brez Commit bi Dispose transakcije naredil rollback.
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Sinhrona različica InsertBatchAsync za BackgroundWorker (DoWork že teče na drugi niti).
        /// </summary>
        public void InsertBatch(IList<Customer> customers)
        {
            if (customers.Count == 0)
            {
                return;
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    connection.Execute(InsertSql, customers, transaction);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Iz uporabnikovega niza naredi vzorec za LIKE ("%niz%"). Prazen niz vrne null (= brez filtra).
        /// </summary>
        // INTERVJU: parameter ščiti pred SQL injection, NE pa pred nadomestnimi znaki LIKE.
        // Če uporabnik vpiše "%" ali "_", bi brez ubežnih znakov zadel vse vrstice, zato jih
        // ubežimo z "\" (privzeti ubežni znak za LIKE v MySQL). Vodilni "%" prepreči uporabo
        // indeksa (iskanje "vsebuje"); "začne se z" (niz%) indeks lahko uporabi.
        private static string BuildLikePattern(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return null;
            }

            string escaped = term.Trim()
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_");

            return "%" + escaped + "%";
        }
    }
}
