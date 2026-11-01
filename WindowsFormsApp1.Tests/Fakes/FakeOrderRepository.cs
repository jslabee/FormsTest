using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Repositories;

namespace WindowsFormsApp1.Tests.Fakes
{
    /// <summary>
    /// Lažni repozitorij naročil v pomnilniku. Zna simulirati napako baze (ExceptionToThrow),
    /// da test preveri obnašanje servisa, ko shranjevanje ne uspe.
    /// </summary>
    public class FakeOrderRepository : IOrderRepository
    {
        private int _nextId = 100;

        /// <summary>
        /// Shranjena naročila ("tabela").
        /// </summary>
        public List<Order> Orders { get; } = new List<Order>();

        /// <summary>
        /// Število klicev CreateAsync (tudi neuspešnih).
        /// </summary>
        public int CreateCalls { get; private set; }

        /// <summary>
        /// Naročilo, kot ga je servis poslal v CreateAsync (zadnji klic).
        /// </summary>
        public Order LastCreated { get; private set; }

        /// <summary>
        /// Če ni null, CreateAsync vrže to izjemo (kot da bi baza zavrnila INSERT in naredila rollback).
        /// </summary>
        public Exception ExceptionToThrow { get; set; }

        public Task<List<Order>> GetByCustomerAsync(int customerId)
        {
            return Task.FromResult(Orders.Where(o => o.CustomerId == customerId).ToList());
        }

        public Task<Order> GetByIdAsync(int orderId)
        {
            return Task.FromResult(Orders.FirstOrDefault(o => o.Id == orderId));
        }

        public Task<int> CreateAsync(Order order)
        {
            CreateCalls++;
            LastCreated = order;

            if (ExceptionToThrow != null)
            {
                // Task.FromException: izjema pride ven ob await, enako kot pri pravi async metodi.
                return Task.FromException<int>(ExceptionToThrow);
            }

            Order stored = order.Clone();
            stored.Id = _nextId++;
            stored.OrderDate = new DateTime(2026, 11, 1, 12, 0, 0);
            Orders.Add(stored);
            return Task.FromResult(stored.Id);
        }
    }
}
