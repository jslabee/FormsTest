using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Repositories;

namespace WindowsFormsApp1.Services
{
    /// <summary>
    /// Primeri za obrazec "Demo napak", ki potrebujejo bazo. Vsaka metoda izvede primer in vrne
    /// besedilo z rezultatom in razlago (vzrok + rešitev), ki ga obrazec samo prikaže.
    /// </summary>
    public class DemoService
    {
        // Več poskusov, kot je max_connections (30), da se napaka zagotovo pokaže.
        private const int ConnectionAttempts = 50;

        private readonly DemoRepository _demoRepository;
        private readonly CustomerService _customerService;

        /// <summary>
        /// Demo servis potrebuje demo repozitorij in servis strank (za primer optimističnega zaklepanja).
        /// </summary>
        public DemoService(DemoRepository demoRepository, CustomerService customerService)
        {
            if (demoRepository == null)
            {
                throw new ArgumentNullException(nameof(demoRepository));
            }

            if (customerService == null)
            {
                throw new ArgumentNullException(nameof(customerService));
            }

            _demoRepository = demoRepository;
            _customerService = customerService;
        }

        /// <summary>
        /// 1a: povezave brez using - pokaže napako "Too many connections".
        /// </summary>
        public async Task<DemoResult> LeakConnectionsAsync()
        {
            ConnectionDemoResult r = await _demoRepository.OpenConnectionsWithoutUsingAsync(ConnectionAttempts);

            var text = new StringBuilder();
            text.AppendLine("NAPAČNO: new MySqlConnection(...) + Open() brez using / Dispose.");
            text.AppendLine();
            text.AppendLine("Uspešno odprtih povezav: " + r.Opened + " od " + ConnectionAttempts);
            text.AppendLine("Napaka: " + (r.Error ?? "(ni napake - je max_connections večji od 30?)"));
            text.AppendLine();
            text.AppendLine("VZROK: povezava, ki ni zaprta, ostane \"v uporabi\". Pool je ne more ponovno");
            text.AppendLine("uporabiti, zato vsak Open odpre novo fizično povezavo. Ko jih je toliko kot");
            text.AppendLine("max_connections, strežnik zavrne VSE nove povezave - tudi drugih aplikacij.");
            text.AppendLine("Napaka se pogosto pokaže šele pod obremenitvijo, ne pri razvijalcu.");
            text.AppendLine();
            text.AppendLine("REŠITEV: using (var conn = new MySqlConnection(cs)) { ... } - Dispose vrne");
            text.AppendLine("povezavo v pool tudi ob izjemi. (Demo je za seboj povezave pospravil.)");
            return new DemoResult { Text = text.ToString() };
        }

        /// <summary>
        /// 1b: povezave z using - pool uporablja isto fizično povezavo.
        /// </summary>
        public async Task<DemoResult> UsingConnectionsAsync()
        {
            ConnectionDemoResult r = await _demoRepository.OpenConnectionsWithUsingAsync(ConnectionAttempts);

            var text = new StringBuilder();
            text.AppendLine("PRAVILNO: vsaka povezava v using.");
            text.AppendLine();
            text.AppendLine("Odprtih (in zaprtih) povezav: " + r.Opened);
            text.AppendLine("Različnih fizičnih povezav na strežniku (CONNECTION_ID): " + r.DistinctServerConnections);
            text.AppendLine();
            text.AppendLine("Dispose ne zapre fizične povezave, ampak jo vrne v pool. Naslednji Open jo");
            text.AppendLine("dobi nazaj (MySqlConnector jo ponastavi), zato je \"odpri pozno, zapri zgodaj\"");
            text.AppendLine("poceni. Povezave zato NE hranimo v polju razreda.");
            return new DemoResult { Text = text.ToString() };
        }

        /// <summary>
        /// 3a: ranljiva poizvedba z lepljenjem niza.
        /// </summary>
        public async Task<DemoResult> SqlInjectionUnsafeAsync(string lastName)
        {
            // Varovalka za demo: MySqlConnector privzeto dovoli več ukazov v enem klicu, zato bi
            // vnos "x'; DROP TABLE customers; --" res izbrisal tabelo. Prav to je nevarnost
            // SQL injection - v demu je ne dovolimo, da si ne pokvarimo testne baze.
            if (lastName != null && lastName.Contains(";"))
            {
                return new DemoResult
                {
                    Text = "Demo ne dovoli znaka \";\". Z njim bi napadalec lahko dodal povsem nov ukaz " +
                           "(npr. DROP TABLE), ker MySqlConnector privzeto izvede več ukazov v enem klicu." +
                           Environment.NewLine + "Poskusi: x' OR '1'='1"
                };
            }

            string sql = DemoRepository.BuildUnsafeSql(lastName);
            List<Customer> rows = await _demoRepository.FindByLastNameUnsafeAsync(lastName);

            var text = new StringBuilder();
            text.AppendLine("NAPAČNO: SQL, sestavljen z lepljenjem vnosa:");
            text.AppendLine(sql);
            text.AppendLine();
            text.AppendLine("Najdenih vrstic: " + rows.Count);
            text.AppendLine();
            text.AppendLine("VZROK: narekovaj v vnosu zapre niz, preostanek vnosa postane del ukaza.");
            text.AppendLine("Z vnosom x' OR '1'='1 je pogoj vedno resničen in poizvedba vrne vse stranke.");
            text.AppendLine("Napadalec lahko tako bere tuje podatke, obide prijavo ali spreminja bazo.");
            text.AppendLine();
            text.AppendLine("REŠITEV: parametri (@LastName). Ročno podvajanje narekovajev NI rešitev.");
            return new DemoResult { Text = text.ToString(), GridData = rows };
        }

        /// <summary>
        /// 3b: parametrizirana poizvedba z istim vnosom.
        /// </summary>
        public async Task<DemoResult> SqlInjectionSafeAsync(string lastName)
        {
            List<Customer> rows = await _demoRepository.FindByLastNameSafeAsync(lastName);

            var text = new StringBuilder();
            text.AppendLine("PRAVILNO: parametrizirana poizvedba:");
            text.AppendLine("SELECT ... FROM customers WHERE last_name = @LastName");
            text.AppendLine("@LastName = \"" + lastName + "\"");
            text.AppendLine();
            text.AppendLine("Najdenih vrstic: " + rows.Count);
            text.AppendLine();
            text.AppendLine("Vnos je vedno samo VREDNOST, s katero se primerja last_name - nikoli del ukaza.");
            text.AppendLine("Iščemo torej stranko, ki se dobesedno piše \"" + lastName + "\" (je ni).");
            text.AppendLine("Poskusi še pravi priimek, npr. Novak.");
            return new DemoResult { Text = text.ToString(), GridData = rows };
        }

        /// <summary>
        /// 4a/4b: iskanje po ref_code brez indeksa ali z njim + EXPLAIN.
        /// </summary>
        public async Task<DemoResult> QueryPlanAsync(bool withIndex)
        {
            QueryPlanResult r = await _demoRepository.RunRefCodeQueryAsync(withIndex);

            var text = new StringBuilder();
            text.AppendLine(withIndex ? "Z INDEKSOM na ref_code:" : "BREZ INDEKSA na ref_code (tabela ima 500.000 vrstic):");
            text.AppendLine(r.Sql);
            text.AppendLine();
            text.AppendLine("Najdenih vrstic: " + r.RowCount + ", čas (najboljši od 3): " + r.ElapsedMilliseconds + " ms");
            text.AppendLine("EXPLAIN: " + DescribePlan(r.Plan));
            text.AppendLine();

            if (withIndex)
            {
                text.AppendLine("type = ref, key = ix_activity_log_ref_code: MySQL v B-drevesu indeksa najde");
                text.AppendLine("vrstico v nekaj korakih (rows ~ 1). Cena indeksa: več prostora in počasnejši");
                text.AppendLine("INSERT/UPDATE, ker mora baza vzdrževati tudi indeks.");
            }
            else
            {
                text.AppendLine("VZROK: type = ALL pomeni full table scan - MySQL prebere VSE vrstice in vsako");
                text.AppendLine("primerja (rows ~ 500.000). Čas raste linearno s tabelo; z več uporabniki hkrati");
                text.AppendLine("taka poizvedba porablja CPU in disk ter upočasni vse ostale.");
                text.AppendLine();
                text.AppendLine("REŠITEV: indeks na stolpcu iz WHERE / JOIN / ORDER BY (gumb 4b).");
                text.AppendLine("Pozor: LIKE '%niz' ali funkcija nad stolpcem (WHERE UPPER(ref_code) = ...)");
                text.AppendLine("indeksa ne more uporabiti.");
            }

            return new DemoResult { Text = text.ToString(), GridData = r.Plan };
        }

        /// <summary>
        /// 6a: dva "uporabnika" urejata isto stranko - drugi shrani prvi, prvi dobi ConcurrencyException.
        /// </summary>
        public async Task<DemoResult> ConcurrencyConflictAsync()
        {
            PagedResult<Customer> page = await _customerService.SearchAsync(null, 0, 1);
            if (page.Items.Count == 0)
            {
                return new DemoResult { Text = "V bazi ni nobene stranke." };
            }

            // Oba uporabnika prebereta isto stranko - imata enak UpdatedAt.
            Customer userA = page.Items[0];
            Customer userB = userA.Clone();

            var text = new StringBuilder();
            text.AppendLine("Uporabnik A in uporabnik B odpreta stranko \"" + userA.FullName + "\".");
            text.AppendLine("Oba imata updated_at = " + userA.UpdatedAt.ToString("HH:mm:ss.ffffff"));
            text.AppendLine();

            // B shrani prvi (podatkov ne spremenimo, UPDATE pa vseeno nastavi nov updated_at).
            await _customerService.UpdateAsync(userB);
            text.AppendLine("B shrani -> uspe. Nov updated_at = " + userB.UpdatedAt.ToString("HH:mm:ss.ffffff"));

            try
            {
                // A shrani s starim updated_at -> WHERE id = @Id AND updated_at = @UpdatedAt ne najde vrstice.
                await _customerService.UpdateAsync(userA);
                text.AppendLine("A shrani -> uspe (NEPRIČAKOVANO).");
            }
            catch (ConcurrencyException ex)
            {
                text.AppendLine("A shrani -> ConcurrencyException:");
                text.AppendLine("  " + ex.Message);
            }

            text.AppendLine();
            text.AppendLine("Brez preverjanja updated_at bi A tiho prepisal spremembe uporabnika B (\"lost update\").");
            text.AppendLine("Optimistično: nič ne zaklepamo vnaprej, konflikt zaznamo ob shranjevanju.");
            text.AppendLine("Pesimistično (SELECT ... FOR UPDATE) bi vrstico zaklenili za ves čas urejanja -");
            text.AppendLine("v namizni aplikaciji, kjer uporabnik lahko gre na kavo, je to slaba ideja.");
            return new DemoResult { Text = text.ToString() };
        }

        /// <summary>
        /// Kratek povzetek EXPLAIN-a: type, key in rows prve vrstice načrta.
        /// </summary>
        private static string DescribePlan(DataTable plan)
        {
            if (plan == null || plan.Rows.Count == 0)
            {
                return "(ni načrta)";
            }

            DataRow row = plan.Rows[0];
            return "type = " + GetValue(plan, row, "type") +
                   ", key = " + GetValue(plan, row, "key") +
                   ", rows = " + GetValue(plan, row, "rows");
        }

        private static string GetValue(DataTable table, DataRow row, string column)
        {
            if (!table.Columns.Contains(column) || row.IsNull(column))
            {
                return "NULL";
            }

            return Convert.ToString(row[column]);
        }
    }
}
