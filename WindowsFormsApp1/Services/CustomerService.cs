using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Repositories;

namespace WindowsFormsApp1.Services
{
    /// <summary>
    /// Poslovna logika nad strankami: iskanje s straničenjem, dodajanje, urejanje, brisanje,
    /// statistika in izvoz. Obrazec kliče samo to plast; za bazo poskrbi repozitorij.
    /// </summary>
    public class CustomerService
    {
        /// <summary>
        /// Število strank na stran v glavnem oknu. Majhno, da se straničenje vidi že s testnimi podatki.
        /// </summary>
        public const int DefaultPageSize = 10;

        private readonly ICustomerRepository _repository;

        /// <summary>
        /// Repozitorij dobimo v konstruktorju (constructor injection) - servis ne ve, ali je to
        /// MySQL ali lažni repozitorij iz testov.
        /// </summary>
        public CustomerService(ICustomerRepository repository)
        {
            // Napaka v sestavljanju aplikacije naj se pokaže takoj, ne šele ob prvem klicu.
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            _repository = repository;
        }

        /// <summary>
        /// Ena stran strank, ki vsebujejo iskani niz v imenu, priimku ali emailu.
        /// </summary>
        public Task<PagedResult<Customer>> SearchAsync(string term, int pageIndex, int pageSize)
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Velikost strani mora biti pozitivna.");
            }

            // Negativen indeks strani (npr. "Nazaj" na prvi strani) tiho popravimo na prvo stran.
            return _repository.SearchPagedAsync(term, Math.Max(0, pageIndex), pageSize);
        }

        /// <summary>
        /// Stranka po Id ali null.
        /// </summary>
        public Task<Customer> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Preveri, poenoti in shrani novo stranko. Po klicu ima stranka Id iz baze.
        /// </summary>
        public async Task AddAsync(Customer customer)
        {
            EnsureValid(customer);
            CustomerValidator.Normalize(customer);
            await _repository.InsertAsync(customer);
        }

        /// <summary>
        /// Shrani spremembe stranke. Vrže ConcurrencyException, če jo je vmes spremenil nekdo drug,
        /// in InvalidOperationException, če je ni več.
        /// </summary>
        public async Task UpdateAsync(Customer customer)
        {
            EnsureValid(customer);
            CustomerValidator.Normalize(customer);

            UpdateResult result = await _repository.UpdateAsync(customer);

            // Repozitorij vrne izid, servis ga prevede v pomen za uporabnika.
            if (result == UpdateResult.NotFound)
            {
                throw new InvalidOperationException("Stranka ne obstaja več - vmes jo je nekdo izbrisal.");
            }

            if (result == UpdateResult.Conflict)
            {
                throw new ConcurrencyException(
                    "Stranko \"" + customer.FullName + "\" je medtem spremenil drug uporabnik. " +
                    "Vaše spremembe niso shranjene; podatki bodo osveženi, nato urejanje ponovite.");
            }
        }

        /// <summary>
        /// Izbriše stranko. Če je že izbrisana, je cilj vseeno dosežen - zato rezultata ne preverjamo.
        /// </summary>
        public Task DeleteAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Statistika stranke (shranjena procedura).
        /// </summary>
        public Task<CustomerStats> GetStatsAsync(int customerId)
        {
            return _repository.GetStatsAsync(customerId);
        }

        /// <summary>
        /// Izvozi vse stranke v CSV datoteko. Vrne število izvoženih strank.
        /// </summary>
        public async Task<int> ExportToCsvAsync(string path)
        {
            List<Customer> customers = await _repository.GetAllAsync();
            string csv = CustomerCsv.Write(customers);

            // UTF-8 Z BOM (new UTF8Encoding(true)): po BOM Excel prepozna kodiranje in pravilno
            // prikaže šumnike. Brez BOM bi Excel datoteko bral v ANSI (cp1250) in pokvaril č, š, ž.
            using (var writer = new StreamWriter(path, false, new UTF8Encoding(true)))
            {
                // Asinhrono pisanje: pri večji datoteki UI med pisanjem ne zamrzne.
                await writer.WriteAsync(csv);
            }

            return customers.Count;
        }

        // Varovalka: tudi če obrazec validacijo preskoči, neveljavna stranka ne pride v bazo.
        private static void EnsureValid(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            List<string> errors = CustomerValidator.Validate(customer);
            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join(Environment.NewLine, errors));
            }
        }
    }
}
