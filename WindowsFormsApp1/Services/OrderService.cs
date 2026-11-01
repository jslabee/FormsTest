using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Repositories;

namespace WindowsFormsApp1.Services
{
    /// <summary>
    /// Poslovna logika naročil: branje naročil stranke in shranjevanje novega naročila.
    /// </summary>
    public class OrderService
    {
        /// <summary>
        /// Naziv namerno napačne postavke, ki jo doda "Simuliraj napako".
        /// </summary>
        public const string SimulatedErrorProductName = "*** NAMERNA NAPAKA (količina -1) ***";

        private readonly IOrderRepository _repository;

        /// <summary>
        /// Repozitorij dobimo v konstruktorju, da ga lahko testi zamenjajo z lažnim.
        /// </summary>
        public OrderService(IOrderRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            _repository = repository;
        }

        /// <summary>
        /// Naročila stranke s postavkami, najnovejša najprej.
        /// </summary>
        public Task<List<Order>> GetByCustomerAsync(int customerId)
        {
            return _repository.GetByCustomerAsync(customerId);
        }

        /// <summary>
        /// Preveri in shrani naročilo (glava + postavke v eni transakciji). Vrne naročilo,
        /// kot je shranjeno v bazi (z Id-ji in datumom, ki ga je nastavila baza).
        /// </summary>
        /// <param name="order">Naročilo, ki ga je sestavil uporabnik.</param>
        /// <param name="simulateError">
        /// Za demo: za veljavnimi postavkami doda še postavko s količino -1. Baza jo zavrne
        /// (CHECK omejitev), zato se razveljavi celotna transakcija - tudi že vstavljena glava in postavke.
        /// </param>
        public async Task<Order> CreateAsync(Order order, bool simulateError)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            List<string> errors = OrderValidator.Validate(order);
            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join(Environment.NewLine, errors));
            }

            // Delamo na kopiji: ne spremenimo objekta obrazca (pri ponovnem poskusu brez napake
            // napačna postavka ne sme ostati v naročilu).
            Order toSave = order.Clone();
            toSave.Note = string.IsNullOrWhiteSpace(toSave.Note) ? null : toSave.Note.Trim();
            foreach (OrderItem item in toSave.Items)
            {
                item.ProductName = item.ProductName.Trim();
            }

            if (simulateError)
            {
                // Zadnja postavka: glava in vse prejšnje postavke se v transakciji že vstavijo,
                // šele nato pride napaka - tako je rollback res viden (nič od tega ne ostane v bazi).
                toSave.Items.Add(new OrderItem
                {
                    ProductName = SimulatedErrorProductName,
                    Quantity = -1,
                    UnitPrice = 0m
                });
            }

            int orderId = await _repository.CreateAsync(toSave);

            // Preberemo nazaj iz baze: tako ima naročilo datum in Id-je postavk, ki jih je dodelila baza.
            Order saved = await _repository.GetByIdAsync(orderId);
            if (saved == null)
            {
                throw new InvalidOperationException("Shranjenega naročila ni mogoče prebrati iz baze.");
            }

            return saved;
        }
    }
}
