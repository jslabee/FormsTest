namespace WindowsFormsApp1.Models
{
    /// <summary>
    /// Stanje uvoza, ki ga servis sporoča obrazcu (prek IProgress&lt;T&gt; ali BackgroundWorker.ReportProgress).
    /// Razred je nespremenljiv po ustvarjanju: vsako poročilo je nov objekt, zato ga UI nit lahko
    /// bere brez zaklepanja, medtem ko delovna nit že pripravlja naslednjega.
    /// </summary>
    public class ImportProgress
    {
        public ImportProgress(int processed, int total, int imported, int skipped, string message)
        {
            Processed = processed;
            Total = total;
            Imported = imported;
            Skipped = skipped;
            Message = message;
        }

        public int Processed { get; private set; }
        public int Total { get; private set; }
        public int Imported { get; private set; }
        public int Skipped { get; private set; }
        public string Message { get; private set; }

        /// <summary>
        /// Odstotek za ProgressBar (0-100).
        /// </summary>
        public int Percent
        {
            get { return Total == 0 ? 100 : Processed * 100 / Total; }
        }
    }

    /// <summary>
    /// Končni rezultat uvoza.
    /// </summary>
    public class ImportResult
    {
        public int Total { get; set; }
        public int Imported { get; set; }
        public int Skipped { get; set; }

        // Pri BackgroundWorker različici preklic ne vrže izjeme, ampak se zapiše sem.
        public bool Cancelled { get; set; }
    }

    /// <summary>
    /// Ena vrstica CSV datoteke: prebrana stranka in številka vrstice (za sporočila o napakah).
    /// </summary>
    public class CsvCustomerRow
    {
        public int LineNumber { get; set; }
        public Customer Customer { get; set; }
    }
}
