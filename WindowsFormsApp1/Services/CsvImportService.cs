using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Repositories;

namespace WindowsFormsApp1.Services
{
    /// <summary>
    /// Uvoz strank iz CSV v paketih. Isti postopek ima dve obliki:
    ///  - ImportAsync: sodobni način (async/await, IProgress&lt;T&gt;, CancellationToken),
    ///  - Import: sinhrona oblika za BackgroundWorker (stari način, pogost v legacy kodi).
    /// Servis ne pozna WinForms - napredek sporoča prek IProgress / delegata.
    /// </summary>
    public class CsvImportService
    {
        /// <summary>
        /// Število vrstic v enem paketu (ena transakcija). Manjši paket = pogostejši napredek
        /// in hitrejši odziv na preklic; večji = manj obiskov baze.
        /// </summary>
        public const int BatchSize = 25;

        private readonly ICustomerRepository _repository;
        private readonly int _delayPerBatchMs;

        /// <summary>
        /// Ustvari servis za uvoz.
        /// </summary>
        /// <param name="repository">Kam shranjujemo stranke.</param>
        /// <param name="delayPerBatchMs">
        /// Umetna zakasnitev po vsakem paketu (samo za demo, da se ProgressBar in preklic vidita;
        /// nastavi se v App.config, v testih je 0).
        /// </param>
        public CsvImportService(ICustomerRepository repository, int delayPerBatchMs)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            _repository = repository;
            _delayPerBatchMs = Math.Max(0, delayPerBatchMs);
        }

        /// <summary>
        /// Asinhroni uvoz. Ob preklicu vrže OperationCanceledException; paketi, ki so bili do
        /// takrat shranjeni, ostanejo v bazi (vsak paket je svoja transakcija).
        /// </summary>
        public async Task<ImportResult> ImportAsync(string path, IProgress<ImportProgress> progress,
            CancellationToken cancellationToken)
        {
            // INTERVJU: branje in razčlenjevanje datoteke je CPU/disk delo - s Task.Run ga
            // prestavimo na nit iz poola, da UI nit ostane prosta za risanje okna.
            List<CsvCustomerRow> rows = await Task.Run(() => ReadRows(path), cancellationToken)
                .ConfigureAwait(false);

            var result = new ImportResult { Total = rows.Count };
            Report(progress, result, 0, "Prebranih vrstic: " + rows.Count);

            int processed = 0;
            foreach (List<CsvCustomerRow> batch in SplitIntoBatches(rows))
            {
                // INTERVJU: preklic je "sodelovalen" - token sam ničesar ne ustavi, koda ga mora
                // preverjati. ThrowIfCancellationRequested vrže OperationCanceledException,
                // ki jo klicatelj ujame kot "preklicano" (ne kot napako).
                cancellationToken.ThrowIfCancellationRequested();

                string message;
                List<Customer> valid = ValidateBatch(batch, result, out message);

                // Token gre tudi v repozitorij - preklic prekine tudi čakanje na bazo.
                await _repository.InsertBatchAsync(valid, cancellationToken).ConfigureAwait(false);
                result.Imported += valid.Count;
                processed += batch.Count;

                // INTERVJU: zaradi ConfigureAwait(false) ta zanka teče na niti iz poola, ne na UI niti.
                // To je v redu, ker Progress<T> (ustvarjen v obrazcu) sam prenese klic na UI nit
                // (prek SynchronizationContext, ki si ga je zapomnil ob ustvarjanju).
                Report(progress, result, processed, message);

                if (_delayPerBatchMs > 0)
                {
                    // Task.Delay s tokenom: preklic prekine tudi čakanje.
                    await Task.Delay(_delayPerBatchMs, cancellationToken).ConfigureAwait(false);
                }
            }

            return result;
        }

        /// <summary>
        /// Sinhroni uvoz za BackgroundWorker.DoWork (koda že teče na drugi niti, zato sme blokirati).
        /// Ob preklicu ne vrže izjeme, ampak vrne rezultat s Cancelled = true.
        /// </summary>
        /// <param name="path">Pot do CSV datoteke.</param>
        /// <param name="reportProgress">Klic za napredek (BackgroundWorker.ReportProgress).</param>
        /// <param name="isCancellationRequested">Ali je uporabnik zahteval preklic (CancellationPending).</param>
        public ImportResult Import(string path, Action<ImportProgress> reportProgress, Func<bool> isCancellationRequested)
        {
            List<CsvCustomerRow> rows = ReadRows(path);
            var result = new ImportResult { Total = rows.Count };
            reportProgress(CreateProgress(result, 0, "Prebranih vrstic: " + rows.Count));

            int processed = 0;
            foreach (List<CsvCustomerRow> batch in SplitIntoBatches(rows))
            {
                // Stari način preklica: periodično preverjamo zastavico (BackgroundWorker.CancellationPending).
                if (isCancellationRequested())
                {
                    result.Cancelled = true;
                    return result;
                }

                string message;
                List<Customer> valid = ValidateBatch(batch, result, out message);

                _repository.InsertBatch(valid);
                result.Imported += valid.Count;
                processed += batch.Count;

                reportProgress(CreateProgress(result, processed, message));

                if (_delayPerBatchMs > 0)
                {
                    // Thread.Sleep je tu dovoljen: blokira delovno nit, ne UI niti.
                    Thread.Sleep(_delayPerBatchMs);
                }
            }

            return result;
        }

        /// <summary>
        /// Prebere CSV datoteko. Kodiranje: UTF-8 (z BOM ali brez).
        /// </summary>
        // Opomba: Excel "CSV (ločeno s podpičjem)" shrani v ANSI (cp1250) - šumniki bi se pokvarili.
        // Uporabnik naj izbere "CSV UTF-8". detectEncodingFromByteOrderMarks prepozna BOM.
        private static List<CsvCustomerRow> ReadRows(string path)
        {
            using (var reader = new StreamReader(path, Encoding.UTF8, true))
            {
                return CustomerCsv.Parse(reader);
            }
        }

        /// <summary>
        /// Razdeli vrstice na pakete po BatchSize. "yield return" vrača pakete enega za drugim,
        /// brez vnaprejšnje izdelave vseh seznamov.
        /// </summary>
        private static IEnumerable<List<CsvCustomerRow>> SplitIntoBatches(List<CsvCustomerRow> rows)
        {
            for (int start = 0; start < rows.Count; start += BatchSize)
            {
                yield return rows.Skip(start).Take(BatchSize).ToList();
            }
        }

        /// <summary>
        /// Poenoti in preveri vrstice paketa. Neveljavne preskoči in jih opiše v sporočilu.
        /// </summary>
        private static List<Customer> ValidateBatch(List<CsvCustomerRow> batch, ImportResult result, out string message)
        {
            var valid = new List<Customer>();
            var problems = new List<string>();

            foreach (CsvCustomerRow row in batch)
            {
                CustomerValidator.Normalize(row.Customer);
                List<string> errors = CustomerValidator.Validate(row.Customer);

                if (errors.Count == 0)
                {
                    valid.Add(row.Customer);
                }
                else
                {
                    result.Skipped++;
                    problems.Add("  vrstica " + row.LineNumber + ": " + string.Join(" ", errors));
                }
            }

            message = "Paket: uvoženih " + valid.Count + ", preskočenih " + problems.Count;
            if (problems.Count > 0)
            {
                message += Environment.NewLine + string.Join(Environment.NewLine, problems);
            }

            return valid;
        }

        private static void Report(IProgress<ImportProgress> progress, ImportResult result, int processed, string message)
        {
            // IProgress je neobvezen - klicatelj lahko poda null, če napredka ne potrebuje.
            if (progress != null)
            {
                progress.Report(CreateProgress(result, processed, message));
            }
        }

        private static ImportProgress CreateProgress(ImportResult result, int processed, string message)
        {
            return new ImportProgress(processed, result.Total, result.Imported, result.Skipped, message);
        }
    }
}
