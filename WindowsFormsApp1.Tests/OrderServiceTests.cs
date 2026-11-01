using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;
using WindowsFormsApp1.Tests.Fakes;

namespace WindowsFormsApp1.Tests
{
    /// <summary>
    /// Testi za OrderService: validacija naročila, "Simuliraj napako" in obnašanje ob napaki baze.
    /// Same transakcije (rollback) tu ne testiramo - to je naloga integracijskega testa z bazo.
    /// </summary>
    [TestClass]
    public class OrderServiceTests
    {
        private FakeOrderRepository _repository;
        private OrderService _service;

        [TestInitialize]
        public void SetUp()
        {
            _repository = new FakeOrderRepository();
            _service = new OrderService(_repository);
        }

        /// <summary>
        /// Veljavno naročilo z dvema postavkama (skupaj 2 x 10,00 + 1 x 5,50 = 25,50).
        /// </summary>
        private static Order CreateValidOrder()
        {
            var order = new Order { CustomerId = 1, Note = "  test  " };
            order.Items.Add(new OrderItem { ProductName = " Miška ", Quantity = 2, UnitPrice = 10.00m });
            order.Items.Add(new OrderItem { ProductName = "Podloga", Quantity = 1, UnitPrice = 5.50m });
            return order;
        }

        [TestMethod]
        public async Task CreateAsync_ValidOrder_SavesAndReturnsOrderFromRepository()
        {
            Order order = CreateValidOrder();

            Order saved = await _service.CreateAsync(order, false);

            Assert.AreEqual(1, _repository.CreateCalls);
            Assert.IsTrue(saved.Id > 0);
            Assert.AreEqual(2, saved.Items.Count);
            Assert.AreEqual(25.50m, saved.Total);
            // Servis obreže besedila pred shranjevanjem.
            Assert.AreEqual("test", saved.Note);
            Assert.AreEqual("Miška", saved.Items[0].ProductName);
        }

        [TestMethod]
        public async Task CreateAsync_NoItems_ThrowsAndDoesNotCallRepository()
        {
            var order = new Order { CustomerId = 1 };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _service.CreateAsync(order, false));

            Assert.AreEqual(0, _repository.CreateCalls);
        }

        [TestMethod]
        public async Task CreateAsync_ZeroQuantity_Throws()
        {
            Order order = CreateValidOrder();
            order.Items[1].Quantity = 0;

            ArgumentException ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => _service.CreateAsync(order, false));

            StringAssert.Contains(ex.Message, "Postavka 2");
        }

        [TestMethod]
        public async Task CreateAsync_SimulateError_SendsInvalidLastItemButKeepsCallersOrderUnchanged()
        {
            Order order = CreateValidOrder();

            await _service.CreateAsync(order, true);

            // V repozitorij je šla kopija z dodatno, namerno napačno postavko NA KONCU ...
            OrderItem last = _repository.LastCreated.Items.Last();
            Assert.AreEqual(3, _repository.LastCreated.Items.Count);
            Assert.AreEqual(-1, last.Quantity);
            Assert.AreEqual(OrderService.SimulatedErrorProductName, last.ProductName);

            // ... objekt klicatelja (obrazca) pa ostane nespremenjen, da ponovni poskus deluje.
            Assert.AreEqual(2, order.Items.Count);
            Assert.AreNotSame(order, _repository.LastCreated);
        }

        [TestMethod]
        public async Task CreateAsync_RepositoryFails_PropagatesExceptionAndSavesNothing()
        {
            // Simulacija: baza zavrne INSERT (npr. CHECK omejitev) in transakcija se razveljavi.
            _repository.ExceptionToThrow = new InvalidOperationException("Check constraint violated");

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(CreateValidOrder(), false));

            Assert.AreEqual(0, _repository.Orders.Count);
        }

        [TestMethod]
        public void OrderItem_ChangeQuantity_RaisesPropertyChangedForLineTotal()
        {
            // BindingList osveži izračunan stolpec samo, če postavka sporoči spremembo LineTotal.
            var item = new OrderItem { Quantity = 1, UnitPrice = 3m };
            var changed = new System.Collections.Generic.List<string>();
            item.PropertyChanged += (s, e) => changed.Add(e.PropertyName);

            item.Quantity = 4;

            CollectionAssert.Contains(changed, "Quantity");
            CollectionAssert.Contains(changed, "LineTotal");
            Assert.AreEqual(12m, item.LineTotal);
        }
    }
}
