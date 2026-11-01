using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;
using WindowsFormsApp1.Tests.Fakes;

namespace WindowsFormsApp1.Tests
{
    /// <summary>
    /// Testi za uvoz CSV: paketi, preskakovanje neveljavnih vrstic, napredek in preklic
    /// (async in sinhrona/BackgroundWorker različica).
    /// </summary>
    [TestClass]
    public class CsvImportServiceTests
    {
        private string _path;
        private FakeCustomerRepository _repository;
        private CsvImportService _service;

        /// <summary>
        /// Pripravi začasno CSV datoteko s 60 vrsticami, od katerih sta 2 neveljavni.
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            var csv = new StringBuilder();
            csv.AppendLine(CustomerCsv.Header);
            for (int i = 1; i <= 60; i++)
            {
                // Vrstici 10 in 20: brez imena oz. z napačnim emailom -> preskočeni.
                string firstName = i == 10 ? string.Empty : "Ime" + i;
                string email = i == 20 ? "napacen" : "oseba" + i + "@example.si";
                csv.AppendLine(firstName + ";Priimek" + i + ";" + email + ";041 000 " + i.ToString("000"));
            }

            _path = Path.Combine(Path.GetTempPath(), "uvoz_test_" + Guid.NewGuid().ToString("N") + ".csv");
            File.WriteAllText(_path, csv.ToString(), new UTF8Encoding(true));

            _repository = new FakeCustomerRepository();
            // Zakasnitev 0 - testi morajo biti hitri.
            _service = new CsvImportService(_repository, 0);
        }

        /// <summary>
        /// Po vsakem testu pobrišemo začasno datoteko.
        /// </summary>
        [TestCleanup]
        public void TearDown()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }
        }

        [TestMethod]
        public async Task ImportAsync_ImportsValidRowsInBatchesAndSkipsInvalid()
        {
            var reports = new List<ImportProgress>();

            ImportResult result = await _service.ImportAsync(_path, new SynchronousProgress(reports), CancellationToken.None);

            Assert.AreEqual(60, result.Total);
            Assert.AreEqual(58, result.Imported);
            Assert.AreEqual(2, result.Skipped);

            // 60 vrstic / 25 na paket = 3 paketi (25 + 25 + 10).
            Assert.AreEqual(3, _repository.InsertedBatches.Count);

            // Prvo poročilo (po branju datoteke) + eno na paket; zadnje je 100 %.
            Assert.AreEqual(4, reports.Count);
            Assert.AreEqual(100, reports.Last().Percent);
        }

        [TestMethod]
        public async Task ImportAsync_CancelledAfterFirstBatch_ThrowsAndStopsImporting()
        {
            var cancellation = new CancellationTokenSource();
            var reports = new List<ImportProgress>();

            // Preklic sprožimo iz poročila o napredku po prvem paketu (2. poročilo).
            var progress = new SynchronousProgress(reports, () =>
            {
                if (reports.Count == 2)
                {
                    cancellation.Cancel();
                }
            });

            // Pozor: ThrowsExceptionAsync zahteva TOČNO ta tip; Task.Delay bi vrgel izpeljan
            // TaskCanceledException, zato v servisu preverjamo s ThrowIfCancellationRequested.
            await Assert.ThrowsExceptionAsync<OperationCanceledException>(
                () => _service.ImportAsync(_path, progress, cancellation.Token));

            // Prvi paket je ostal shranjen (vsak paket je svoja transakcija), ostalih ni.
            Assert.AreEqual(1, _repository.InsertedBatches.Count);
        }

        [TestMethod]
        public void Import_Sync_CancellationRequested_ReturnsCancelledResult()
        {
            int reports = 0;

            // BackgroundWorker način: preklic je zastavica, ki jo servis preverja med paketi.
            ImportResult result = _service.Import(_path, p => reports++, () => reports >= 2);

            Assert.IsTrue(result.Cancelled);
            Assert.AreEqual(1, _repository.InsertedBatches.Count);
        }

        /// <summary>
        /// IProgress, ki poročilo obdela TAKOJ (sinhrono). Progress&lt;T&gt; bi brez UI niti klic
        /// poslal na nit iz poola in test bi lahko preveril rezultat, preden poročilo prispe.
        /// </summary>
        private class SynchronousProgress : IProgress<ImportProgress>
        {
            private readonly List<ImportProgress> _reports;
            private readonly Action _afterReport;

            public SynchronousProgress(List<ImportProgress> reports, Action afterReport = null)
            {
                _reports = reports;
                _afterReport = afterReport;
            }

            public void Report(ImportProgress value)
            {
                _reports.Add(value);
                if (_afterReport != null)
                {
                    _afterReport();
                }
            }
        }
    }
}
