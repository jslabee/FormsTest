using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Forms;
using WindowsFormsApp1.Repositories;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1
{
    /// <summary>
    /// Vstopna točka aplikacije: nastavi globalno obravnavo napak, sestavi servise
    /// ("composition root") in zažene glavno okno.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Glavna vstopna točka aplikacije.
        /// </summary>
        // [STAThread]: WinForms (in COM komponente, npr. odložišče, dialogi za datoteke)
        // zahtevajo "single-threaded apartment" na UI niti.
        [STAThread]
        static void Main()
        {
            // INTERVJU: globalna obravnava napak - zadnja varovalka za izjeme, ki jih koda ni ujela.
            //  - Application.ThreadException: izjeme na UI niti (event handlerji, tudi async void).
            //    Aplikacija lahko teče naprej.
            //  - AppDomain.UnhandledException: izjeme na DRUGIH nitih (Thread, Timer iz poola).
            //    Tu aplikacije ni mogoče rešiti - proces se konča; lahko le zapišemo log.
            //  - TaskScheduler.UnobservedTaskException: izjeme v Taskih, na katere nihče ni čakal.
            // Vse tri je treba nastaviti PRED ustvarjanjem prvega obrazca.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppLogger.Info("Zagon aplikacije.");

            MainForm mainForm;
            try
            {
                mainForm = CreateMainForm();
            }
            catch (Exception ex)
            {
                // Napaka pred Application.Run (npr. manjka connection string) ni na sporočilni zanki,
                // zato je ThreadException ne bi ujel - obravnavamo jo tu.
                AppLogger.Error("Aplikacije ni mogoče zagnati.", ex);
                MessageBox.Show("Aplikacije ni mogoče zagnati:" + Environment.NewLine + ex.Message,
                    "Napaka", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(mainForm);
            AppLogger.Info("Konec aplikacije.");
        }

        /// <summary>
        /// Sestavi repozitorije in servise ter jih poda glavnemu oknu.
        /// </summary>
        // INTERVJU: "composition root" - edino mesto, kjer se odločimo za konkretne razrede
        // (MySQL repozitoriji). Vsi ostali razredi dobijo odvisnosti v konstruktorju
        // (dependency injection brez DI ogrodja, t. i. "pure DI").
        private static MainForm CreateMainForm()
        {
            // Connection string beremo iz App.config (<connectionStrings>), ne trdo kodiramo.
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["TestDb"];
            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new ConfigurationErrorsException("V App.config manjka connection string \"TestDb\".");
            }

            string connectionString = settings.ConnectionString;

            var customerRepository = new CustomerRepository(connectionString);
            var orderRepository = new OrderRepository(connectionString);
            var demoRepository = new DemoRepository(connectionString);

            var customerService = new CustomerService(customerRepository);
            var orderService = new OrderService(orderRepository);
            var importService = new CsvImportService(customerRepository, ReadIntSetting("ImportDemoDelayMs", 0));
            var demoService = new DemoService(demoRepository, customerService);

            return new MainForm(customerService, orderService, importService, demoService);
        }

        /// <summary>
        /// Prebere celoštevilsko nastavitev iz &lt;appSettings&gt;; če je ni ali ni število, vrne privzeto vrednost.
        /// </summary>
        private static int ReadIntSetting(string key, int defaultValue)
        {
            int value;
            return int.TryParse(ConfigurationManager.AppSettings[key], out value) ? value : defaultValue;
        }

        /// <summary>
        /// Neujeta izjema na UI niti: zapišemo v log, uporabnika obvestimo, aplikacija teče naprej.
        /// </summary>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            AppLogger.Error("Neujeta izjema na UI niti (Application.ThreadException).", e.Exception);

            MessageBox.Show(
                "Prišlo je do nepričakovane napake. Aplikacija bo poskusila nadaljevati." +
                Environment.NewLine + Environment.NewLine + e.Exception.Message +
                Environment.NewLine + Environment.NewLine + "Podrobnosti so v log datoteki:" +
                Environment.NewLine + AppLogger.CurrentLogFile,
                "Nepričakovana napaka", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Neujeta izjema na drugi niti: proces se bo končal, lahko le zapišemo log.
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // ExceptionObject je tipa object (lahko bi bila tudi "ne-CLS" izjema), zato "as".
            var exception = e.ExceptionObject as Exception;
            AppLogger.Error("Neujeta izjema na drugi niti (AppDomain.UnhandledException), IsTerminating = " +
                            e.IsTerminating + ".", exception);

            MessageBox.Show(
                "Kritična napaka - aplikacija se bo zaprla." + Environment.NewLine + Environment.NewLine +
                (exception != null ? exception.Message : Convert.ToString(e.ExceptionObject)) +
                Environment.NewLine + Environment.NewLine + "Log: " + AppLogger.CurrentLogFile,
                "Kritična napaka", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        /// <summary>
        /// Izjema v Tasku, na katerega nihče ni čakal (sproži se ob GC). Zapišemo in označimo kot opaženo.
        /// </summary>
        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            AppLogger.Error("Neopažena izjema v Tasku (TaskScheduler.UnobservedTaskException).", e.Exception);

            // Od .NET 4.5 neopažena izjema privzeto ne podre procesa; SetObserved to potrdi.
            e.SetObserved();
        }
    }
}
