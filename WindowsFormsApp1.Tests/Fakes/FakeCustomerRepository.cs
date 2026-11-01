using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Repositories;

namespace WindowsFormsApp1.Tests.Fakes
{
    /// <summary>
    /// Lažni repozitorij strank v pomnilniku. Namesto baze hrani seznam in si zapomni klice,
    /// da lahko test preveri, KAJ je servis poslal v "bazo".
    /// </summary>
    // INTERVJU: "fake" (lažna implementacija) proti "mock" (knjižnica, npr. Moq). Ročni fake je
    // preprost, berljiv in brez dodatnih paketov; mock je hitrejši za pisanje pri velikih vmesnikih.
    // Oboje je mogoče le, ker servis dobi ICustomerRepository v konstruktorju.
    public class FakeCustomerRepository : ICustomerRepository
    {
        private int _nextId = 1;

        /// <summary>
        /// "Tabela" strank.
        /// </summary>
        public List<Customer> Customers { get; } = new List<Customer>();

        /// <summary>
        /// Paketi, ki jih je servis vstavil z InsertBatch(Async) - za teste uvoza.
        /// </summary>
        public List<List<Customer>> InsertedBatches { get; } = new List<List<Customer>>();

        /// <summary>
        /// Kaj naj vrne naslednji UpdateAsync (za simulacijo konflikta ali izbrisane vrstice).
        /// </summary>
        public UpdateResult NextUpdateResult { get; set; } = UpdateResult.Updated;

        /// <summary>
        /// Parametri zadnjega klica SearchPagedAsync.
        /// </summary>
        public int LastPageIndex { get; private set; }

        public Task<List<Customer>> GetAllAsync()
        {
            // Task.FromResult: sinhrona implementacija asinhronega vmesnika (brez pravega čakanja).
            return Task.FromResult(Customers.ToList());
        }

        public Task<Customer> GetByIdAsync(int id)
        {
            return Task.FromResult(Customers.FirstOrDefault(c => c.Id == id));
        }

        public Task<PagedResult<Customer>> SearchPagedAsync(string term, int pageIndex, int pageSize)
        {
            LastPageIndex = pageIndex;
            var result = new PagedResult<Customer>
            {
                TotalCount = Customers.Count,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Items = Customers.Skip(pageIndex * pageSize).Take(pageSize).ToList()
            };
            return Task.FromResult(result);
        }

        public Task InsertAsync(Customer customer)
        {
            customer.Id = _nextId++;
            customer.UpdatedAt = DateTime.Now;
            Customers.Add(customer);
            return Task.CompletedTask;
        }

        public Task<UpdateResult> UpdateAsync(Customer customer)
        {
            return Task.FromResult(NextUpdateResult);
        }

        public Task<bool> DeleteAsync(int id)
        {
            return Task.FromResult(Customers.RemoveAll(c => c.Id == id) > 0);
        }

        public Task<CustomerStats> GetStatsAsync(int customerId)
        {
            return Task.FromResult(new CustomerStats { CustomerId = customerId });
        }

        public Task InsertBatchAsync(IList<Customer> customers, CancellationToken cancellationToken)
        {
            // Tudi lažni repozitorij spoštuje preklic, kot bi ga pravi (MySqlConnector).
            cancellationToken.ThrowIfCancellationRequested();
            InsertBatch(customers);
            return Task.CompletedTask;
        }

        public void InsertBatch(IList<Customer> customers)
        {
            InsertedBatches.Add(customers.ToList());
            foreach (Customer customer in customers)
            {
                customer.Id = _nextId++;
                Customers.Add(customer);
            }
        }
    }
}
