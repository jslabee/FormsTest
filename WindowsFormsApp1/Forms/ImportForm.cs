using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    /// <summary>
    /// Uvoz strank iz CSV na dva načina, da se vidi razlika:
    ///  1. async/await + IProgress&lt;T&gt; + CancellationToken (sodobni način),
    ///  2. BackgroundWorker (stari način, pogost v legacy kodi).
    /// Dnevnik (AppendLog) prikazuje tudi Invoke/InvokeRequired pri klicu iz druge niti.
    /// </summary>
    public partial class ImportForm : Form
    {
        private readonly CsvImportService _importService;

        // Vir preklica za async različico; null, ko uvoz ne teče.
        private CancellationTokenSource _cancellation;

        // Zadnje poročilo o napredku - ob preklicu iz njega vemo, koliko je že shranjeno.
        private ImportProgress _lastProgress;

        private bool _running;

        /// <summary>
        /// Servis za uvoz dobi obrazec od zunaj. Kot privzeto datoteko predlaga testni CSV iz mape Data.
        /// </summary>
        public ImportForm(CsvImportService importService)
        {
            InitializeComponent();

            if (importService == null)
            {
                throw new ArgumentNullException(nameof(importService));
            }

            _importService = importService;

            // Testna datoteka se ob gradnji kopira poleg .exe (Data\uvoz_strank.csv).
            string sample = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "uvoz_strank.csv");
            if (File.Exists(sample))
            {
                txtFile.Text = sample;
            }

            AppendLog("UI nit ima Id " + Thread.CurrentThread.ManagedThreadId + ".");
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtFile.Text))
            {
                ofdCsv.InitialDirectory = Path.GetDirectoryName(txtFile.Text);
            }

            if (ofdCsv.ShowDialog(this) == DialogResult.OK)
            {
                txtFile.Text = ofdCsv.FileName;
            }
        }

        private async void btnImportAsync_Click(object sender, EventArgs e)
        {
            if (!CheckFile())
            {
                return;
            }

            _cancellation = new CancellationTokenSource();
            SetRunning(true);
            AppendLog(Environment.NewLine + "=== Uvoz z async/await ===");

            // INTERVJU: Progress<T> si ob ustvarjanju zapomni SynchronizationContext trenutne niti
            // (tu UI nit). Ko servis z druge niti pokliče Report, Progress pošlje klic OnProgress
            // na UI nit - zato v OnProgress smemo neposredno do kontrol, brez Invoke.
            var progress = new Progress<ImportProgress>(OnProgress);

            try
            {
                // INTERVJU: await sprosti UI nit (okno se riše, gumb Prekliči deluje). Ko je
                // uvoz končan, se izvajanje nadaljuje TUKAJ, spet na UI niti.
                ImportResult result = await _importService.ImportAsync(txtFile.Text, progress, _cancellation.Token);
                AppendLog("Končano: uvoženih " + result.Imported + ", preskočenih " + result.Skipped +
                          " od " + result.Total + " vrstic.");
                lblProgress.Text = "Uvoz končan.";
            }
            catch (OperationCanceledException)
            {
                // Preklic ni napaka - pričakovan izid, zato ga ne logiramo kot ERROR.
                ReportCancelled();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                // CancellationTokenSource je IDisposable (interno ima lahko časovnik / WaitHandle).
                _cancellation.Dispose();
                _cancellation = null;
                SetRunning(false);
            }
        }

        private void btnImportBgw_Click(object sender, EventArgs e)
        {
            if (!CheckFile() || bgwImport.IsBusy)
            {
                return;
            }

            SetRunning(true);
            AppendLog(Environment.NewLine + "=== Uvoz z BackgroundWorker ===");

            // RunWorkerAsync takoj vrne; DoWork se izvede na niti iz poola.
            // Argument gre v DoWork kot e.Argument - DoWork naj ne bere kontrol (druga nit!).
            bgwImport.RunWorkerAsync(txtFile.Text);
        }

        // INTERVJU: BackgroundWorker ima tri dogodke:
        //   DoWork             - na DELOVNI niti (tu NE smemo do kontrol),
        //   ProgressChanged    - na UI niti (BackgroundWorker sam poskrbi za prenos),
        //   RunWorkerCompleted - na UI niti; izjema iz DoWork pride sem kot e.Error.
        // Danes ga nadomešča async/await, v legacy kodi pa ga srečaš povsod.
        private void bgwImport_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            string path = (string)e.Argument;

            // Klic iz delovne niti: AppendLog zazna InvokeRequired == true in uporabi Invoke.
            AppendLog("DoWork teče na niti " + Thread.CurrentThread.ManagedThreadId +
                      " (InvokeRequired = " + InvokeRequired + ")");

            ImportResult result = _importService.Import(
                path,
                p => worker.ReportProgress(p.Percent, p),     // UserState prenese celoten objekt
                () => worker.CancellationPending);           // CancelAsync samo postavi to zastavico

            // Preklic sporočimo z e.Cancel - RunWorkerCompleted ga dobi kot e.Cancelled.
            if (result.Cancelled)
            {
                e.Cancel = true;
            }

            e.Result = result;
        }

        private void bgwImport_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Smo na UI niti - BackgroundWorker je klic prenesel sam.
            OnProgress((ImportProgress)e.UserState);
        }

        private void bgwImport_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // INTERVJU: vrstni red preverjanja je pomemben. Če je e.Error ali e.Cancelled,
            // branje e.Result vrže izjemo (TargetInvocationException / InvalidOperationException).
            if (e.Error != null)
            {
                ShowError(e.Error);
            }
            else if (e.Cancelled)
            {
                ReportCancelled();
            }
            else
            {
                var result = (ImportResult)e.Result;
                AppendLog("Končano: uvoženih " + result.Imported + ", preskočenih " + result.Skipped +
                          " od " + result.Total + " vrstic.");
                lblProgress.Text = "Uvoz končan.";
            }

            SetRunning(false);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Preklic je samo PROŠNJA: servis jo opazi ob naslednjem preverjanju (med paketi).
            if (_cancellation != null)
            {
                _cancellation.Cancel();
            }

            if (bgwImport.IsBusy)
            {
                bgwImport.CancelAsync();
            }

            btnCancel.Enabled = false;
            AppendLog("Zahtevan preklic ...");
        }

        private void ImportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Zapiranje med uvozom bi pustilo delovno nit, ki kliče kontrole uničenega obrazca.
            if (_running)
            {
                e.Cancel = true;
                MessageBox.Show(this, "Uvoz še teče. Najprej ga prekličite.", "Uvoz",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Prikaže napredek. Kliče se vedno na UI niti (prek Progress&lt;T&gt; ali ProgressChanged).
        /// </summary>
        private void OnProgress(ImportProgress progress)
        {
            _lastProgress = progress;
            progressBar.Value = Math.Min(100, Math.Max(0, progress.Percent));
            lblProgress.Text = "Obdelanih " + progress.Processed + " / " + progress.Total +
                               " (uvoženih " + progress.Imported + ", preskočenih " + progress.Skipped + ")";

            AppendLog("[nit " + Thread.CurrentThread.ManagedThreadId + "] " + progress.Percent + " % - " + progress.Message);
        }

        /// <summary>
        /// Doda vrstico v dnevnik. Varno za klic iz katerekoli niti.
        /// </summary>
        // INTERVJU: kontrole WinForms smejo spreminjati samo iz niti, ki jih je ustvarila (UI nit),
        // ker Win32 okna niso varna za večnitni dostop. InvokeRequired pove, ali smo na drugi niti;
        // Invoke takrat klic pošlje UI niti in POČAKA, da se izvede (BeginInvoke bi ne čakal).
        // Pozor: Invoke iz delovne niti, medtem ko UI nit čaka na to delovno nit (npr. .Wait()),
        // je deadlock - obe niti čakata druga na drugo.
        private void AppendLog(string text)
        {
            if (InvokeRequired)
            {
                // Obrazec je med tem morda že zaprt - Invoke bi vrgel ObjectDisposedException.
                if (!IsDisposed)
                {
                    Invoke(new Action<string>(AppendLog), text);
                }

                return;
            }

            txtLog.AppendText(text + Environment.NewLine);
        }

        private void ReportCancelled()
        {
            int imported = _lastProgress != null ? _lastProgress.Imported : 0;
            AppendLog("Preklicano. Že shranjeni paketi ostanejo v bazi (vsak paket je svoja transakcija): " +
                      imported + " strank.");
            lblProgress.Text = "Uvoz preklican.";
        }

        private bool CheckFile()
        {
            if (string.IsNullOrWhiteSpace(txtFile.Text) || !File.Exists(txtFile.Text))
            {
                MessageBox.Show(this, "Izberite obstoječo CSV datoteko.", "Uvoz",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SetRunning(bool running)
        {
            _running = running;
            btnImportAsync.Enabled = !running;
            btnImportBgw.Enabled = !running;
            btnBrowse.Enabled = !running;
            btnClose.Enabled = !running;
            btnCancel.Enabled = running;

            if (running)
            {
                _lastProgress = null;
                progressBar.Value = 0;
            }
        }

        private void ShowError(Exception ex)
        {
            AppLogger.Error("Napaka pri uvozu", ex);
            AppendLog("NAPAKA: " + ex.Message);
            MessageBox.Show(this, "Napaka pri uvozu:" + Environment.NewLine + ex.Message, "Napaka",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
