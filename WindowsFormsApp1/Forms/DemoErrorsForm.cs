using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    /// <summary>
    /// "Demo napak": vsak gumb pokaže en tipičen problem (in praviloma še pravilno rešitev).
    /// Primeri z bazo tečejo prek DemoService; primeri z nitmi so tu, ker gre za UI probleme.
    /// </summary>
    public partial class DemoErrorsForm : Form
    {
        private readonly DemoService _demoService;

        // Ali se primer trenutno izvaja (gumbi so onemogočeni, zapiranje ni dovoljeno).
        private bool _busy;

        /// <summary>
        /// Demo servis dobi obrazec od zunaj.
        /// </summary>
        public DemoErrorsForm(DemoService demoService)
        {
            InitializeComponent();

            if (demoService == null)
            {
                throw new ArgumentNullException(nameof(demoService));
            }

            _demoService = demoService;
            ShowText("Izberite primer na levi. Rezultat in razlaga se prikažeta tukaj," + Environment.NewLine +
                     "podatki (npr. EXPLAIN) pa v mreži spodaj.");
        }

        // ---------------------------------------------------------------------------------
        // 1. Puščanje povezav
        // ---------------------------------------------------------------------------------

        private async void btnLeakConnections_Click(object sender, EventArgs e)
        {
            await RunDemoAsync(() => _demoService.LeakConnectionsAsync());
        }

        private async void btnUsingConnections_Click(object sender, EventArgs e)
        {
            await RunDemoAsync(() => _demoService.UsingConnectionsAsync());
        }

        // ---------------------------------------------------------------------------------
        // 2. Deadlock zaradi .Result
        // ---------------------------------------------------------------------------------

        private void btnDeadlock_Click(object sender, EventArgs e)
        {
            DialogResult answer = MessageBox.Show(this,
                "Ta primer ZAMRZNE aplikacijo za vedno (deadlock)." + Environment.NewLine +
                "Ustaviti jo boste morali z Visual Studio (Stop) ali v upravitelju opravil." +
                Environment.NewLine + Environment.NewLine + "Nadaljujem?",
                "Opozorilo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (answer != DialogResult.Yes)
            {
                return;
            }

            AppLogger.Info("Demo deadlock: klic .Result na UI niti - aplikacija bo zamrznila.");

            // INTERVJU: zakaj deadlock?
            //  1. .Result BLOKIRA UI nit, dokler Task ni končan.
            //  2. GetValueAsync čaka na Task.Delay BREZ ConfigureAwait(false), zato si je zapomnil
            //     SynchronizationContext UI niti in želi nadaljevati NA UI niti.
            //  3. UI nit je blokirana v .Result in ne more izvesti nadaljevanja -> Task se nikoli
            //     ne konča -> .Result nikoli ne vrne. Obe strani čakata druga na drugo.
            // Rešitev: "async all the way" (await namesto .Result/.Wait()); v knjižnicah še
            // ConfigureAwait(false). Task.Run(() => ...).Result se "izogne" deadlocku, a še vedno
            // blokira UI - to je obvoz, ne rešitev.
            int value = GetValueAsync().Result;

            ShowText("Te vrstice ne boste nikoli videli: " + value);
        }

        private async void btnAwait_Click(object sender, EventArgs e)
        {
            ShowText("Čakam z await ... (okno se med tem normalno odziva)");

            // await ne blokira UI niti: metoda se tu "ustavi", UI nit obdeluje sporočila, ko je
            // Task končan, pa se nadaljevanje izvede na UI niti (zato smemo potem do kontrol).
            int value = await GetValueAsync();

            var text = new StringBuilder();
            text.AppendLine("PRAVILNO: int value = await GetValueAsync();  -> " + value);
            text.AppendLine();
            text.AppendLine("Med čakanjem je bila UI nit prosta (okno se je risalo, gumbi so delovali).");
            text.AppendLine("Nadaljevanje po await teče spet na UI niti, ker je SynchronizationContext ohranjen.");
            text.AppendLine();
            text.AppendLine("Pravila:");
            text.AppendLine(" - v WinForms nikoli .Result / .Wait() / .GetAwaiter().GetResult() na UI niti,");
            text.AppendLine(" - async void samo za event handlerje, drugje async Task,");
            text.AppendLine(" - v knjižnicah (repozitoriji) ConfigureAwait(false) - nadaljevanje ne potrebuje UI niti.");
            ShowText(text.ToString());
        }

        /// <summary>
        /// Asinhrona metoda BREZ ConfigureAwait(false) - nadaljevanje želi nazaj na UI nit.
        /// </summary>
        private async Task<int> GetValueAsync()
        {
            await Task.Delay(500);
            return 42;
        }

        // ---------------------------------------------------------------------------------
        // 3. SQL injection
        // ---------------------------------------------------------------------------------

        private async void btnSqlUnsafe_Click(object sender, EventArgs e)
        {
            string input = txtSqlInput.Text;
            await RunDemoAsync(() => _demoService.SqlInjectionUnsafeAsync(input));
        }

        private async void btnSqlSafe_Click(object sender, EventArgs e)
        {
            string input = txtSqlInput.Text;
            await RunDemoAsync(() => _demoService.SqlInjectionSafeAsync(input));
        }

        // ---------------------------------------------------------------------------------
        // 4. Poizvedba brez indeksa
        // ---------------------------------------------------------------------------------

        private async void btnQueryNoIndex_Click(object sender, EventArgs e)
        {
            await RunDemoAsync(() => _demoService.QueryPlanAsync(false));
        }

        private async void btnQueryWithIndex_Click(object sender, EventArgs e)
        {
            await RunDemoAsync(() => _demoService.QueryPlanAsync(true));
        }

        // ---------------------------------------------------------------------------------
        // 5. Dostop do kontrole iz druge niti
        // ---------------------------------------------------------------------------------

        private async void btnCrossThread_Click(object sender, EventArgs e)
        {
            // INTERVJU: WinForms preverjanje dostopa iz druge niti je privzeto vklopljeno samo, ko
            // teče razhroščevalnik (CheckForIllegalCrossThreadCalls = Debugger.IsAttached). Brez
            // razhroščevalnika napačna koda "včasih deluje", včasih pa povzroči nenavadne napake
            // ali zamrznitev - zato preverjanje za demo vklopimo izrecno.
            Control.CheckForIllegalCrossThreadCalls = true;

            try
            {
                // Task.Run izvede lambda na niti iz poola - NE na UI niti.
                await Task.Run(() =>
                {
                    lblThreadTarget.Text = "Nastavljeno iz niti " + Thread.CurrentThread.ManagedThreadId;
                });

                ShowText("Izjeme ni bilo (nepričakovano).");
            }
            catch (InvalidOperationException ex)
            {
                // Izjema z delovne niti je shranjena v Tasku in se ponovno vrže ob await - na UI niti.
                var text = new StringBuilder();
                text.AppendLine("NAPAČNO: lblThreadTarget.Text = ... znotraj Task.Run");
                text.AppendLine();
                text.AppendLine(ex.GetType().Name + ":");
                text.AppendLine(ex.Message);
                text.AppendLine();
                text.AppendLine("VZROK: kontrola (Win32 okno) pripada niti, ki jo je ustvarila (UI nit).");
                text.AppendLine("Dostop iz druge niti ni varen - sporočila oknu bi se lahko prepletla.");
                text.AppendLine();
                text.AppendLine("REŠITEV: Control.Invoke / BeginInvoke (gumb 5b), IProgress<T> ali");
                text.AppendLine("await (nadaljevanje po await teče spet na UI niti).");
                ShowText(text.ToString());
            }
        }

        private async void btnInvoke_Click(object sender, EventArgs e)
        {
            SetBusy(true);
            ShowText("Delovna nit petkrat posodobi oznako prek Invoke ...");

            int uiThreadId = Thread.CurrentThread.ManagedThreadId;
            await Task.Run(() =>
            {
                for (int i = 1; i <= 5; i++)
                {
                    // Simulacija dela na delovni niti.
                    Thread.Sleep(400);
                    SetThreadTargetText("Korak " + i + "/5 - poslala nit " + Thread.CurrentThread.ManagedThreadId);
                }
            });

            SetBusy(false);

            var text = new StringBuilder();
            text.AppendLine("PRAVILNO: delovna nit je oznako posodabljala prek Invoke.");
            text.AppendLine("UI nit: " + uiThreadId + ", delovna nit: druga številka (glej oznako).");
            text.AppendLine();
            text.AppendLine("if (control.InvokeRequired)");
            text.AppendLine("    control.Invoke(new Action<string>(SetText), text);  // prenesi na UI nit");
            text.AppendLine("else");
            text.AppendLine("    control.Text = text;                                // že smo na UI niti");
            text.AppendLine();
            text.AppendLine("Invoke počaka, da UI nit klic izvede; BeginInvoke samo pošlje sporočilo in nadaljuje.");
            ShowText(text.ToString());
        }

        /// <summary>
        /// Nastavi besedilo oznake - varno iz katerekoli niti (vzorec InvokeRequired/Invoke).
        /// </summary>
        private void SetThreadTargetText(string text)
        {
            // InvokeRequired je ena redkih lastnosti kontrole, ki jo smemo brati iz druge niti.
            if (lblThreadTarget.InvokeRequired)
            {
                lblThreadTarget.Invoke(new Action<string>(SetThreadTargetText), text);
                return;
            }

            lblThreadTarget.Text = text;
        }

        // ---------------------------------------------------------------------------------
        // 6. Optimistično zaklepanje, 7. globalne napake
        // ---------------------------------------------------------------------------------

        private async void btnConcurrency_Click(object sender, EventArgs e)
        {
            await RunDemoAsync(() => _demoService.ConcurrencyConflictAsync());
        }

        private void btnUnhandledUi_Click(object sender, EventArgs e)
        {
            ShowText("Vržena je bila izjema, ki je nihče ne ujame." + Environment.NewLine +
                     "Ujel jo je Application.ThreadException (Program.cs), jo zapisal v log in pokazal sporočilo." +
                     Environment.NewLine + "Aplikacija teče naprej. Log: " + AppLogger.CurrentLogFile);

            // INTERVJU: izjema na UI niti, ki je nihče ne ujame, pride do Application.ThreadException
            // (ker je v Program.cs nastavljen UnhandledExceptionMode.CatchException). Pod
            // razhroščevalnikom se VS morda ustavi že ob metu - nadaljuj s F5 ali zaženi s Ctrl+F5.
            throw new InvalidOperationException("Namerna neujeta izjema iz gumba 7a (UI nit).");
        }

        private void btnUnhandledThread_Click(object sender, EventArgs e)
        {
            DialogResult answer = MessageBox.Show(this,
                "Izjema na drugi niti (new Thread) se ne da \"rešiti\": AppDomain.UnhandledException jo " +
                "zapiše v log, nato se aplikacija ZAPRE." + Environment.NewLine + Environment.NewLine + "Nadaljujem?",
                "Opozorilo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (answer != DialogResult.Yes)
            {
                return;
            }

            // INTERVJU: izjema na niti, ki ni UI nit in ni Task, konča CEL proces (od .NET 2.0).
            // Zato mora koda na lastnih nitih (Thread, ThreadPool.QueueUserWorkItem, System.Threading.Timer)
            // vedno imeti svoj try/catch. Pri Task se izjema shrani v Task in pride ven ob await.
            var thread = new Thread(() =>
            {
                throw new InvalidOperationException("Namerna neujeta izjema na delovni niti (gumb 7b).");
            });
            thread.IsBackground = true;
            thread.Start();
        }

        // ---------------------------------------------------------------------------------
        // Pomožne metode
        // ---------------------------------------------------------------------------------

        /// <summary>
        /// Izvede primer iz DemoService: zaklene gumbe, prikaže rezultat ali napako.
        /// </summary>
        private async Task RunDemoAsync(Func<Task<DemoResult>> demo)
        {
            SetBusy(true);
            ShowText("Izvajam ...");
            dgvResult.DataSource = null;

            try
            {
                DemoResult result = await demo();
                ShowText(result.Text);
                dgvResult.DataSource = result.GridData;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Napaka v demu", ex);
                ShowText("NAPAKA:" + Environment.NewLine + ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ShowText(string text)
        {
            // TextBox potrebuje \r\n za prelom vrstice; normaliziramo, če bi kje bil samo \n.
            txtOutput.Text = (text ?? string.Empty).Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
        }

        private void DemoErrorsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Med izvajanjem primera ne zapiramo: nadaljevanje po await bi pisalo v uničene kontrole.
            if (_busy)
            {
                e.Cancel = true;
            }
        }

        private void SetBusy(bool busy)
        {
            _busy = busy;
            UseWaitCursor = busy;
            flpDemos.Enabled = !busy;
        }
    }
}
