using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;
using WindowsFormsApp1.Tests.Fakes;

namespace WindowsFormsApp1.Tests
{
    /// <summary>
    /// Testi za CustomerService: validacija, poenotenje podatkov in prevod izidov repozitorija
    /// (konflikt, izbrisana vrstica) v izjeme. Baza ni potrebna - uporabljamo FakeCustomerRepository.
    /// </summary>
    // INTERVJU: vzorec Arrange / Act / Assert (pripravi, izvedi, preveri). Ime testa pove
    // Metoda_Pogoj_PričakovanIzid, da je ob padcu takoj jasno, kaj ne deluje.
    [TestClass]
    public class CustomerServiceTests
    {
        private FakeCustomerRepository _repository;
        private CustomerService _service;

        /// <summary>
        /// Pred vsakim testom nov repozitorij in servis - testi so med seboj neodvisni.
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            _repository = new FakeCustomerRepository();
            _service = new CustomerService(_repository);
        }

        [TestMethod]
        public async Task AddAsync_ValidCustomer_NormalizesAndInserts()
        {
            // Arrange
            var customer = new Customer { FirstName = "  Janez ", LastName = " Novak ", Email = "   ", Phone = "" };

            // Act
            await _service.AddAsync(customer);

            // Assert: presledki obrezani, prazna neobvezna polja so null, Id je dodelil repozitorij.
            Assert.AreEqual(1, _repository.Customers.Count);
            Assert.AreEqual("Janez", customer.FirstName);
            Assert.AreEqual("Novak", customer.LastName);
            Assert.IsNull(customer.Email);
            Assert.IsNull(customer.Phone);
            Assert.IsTrue(customer.Id > 0);
        }

        [TestMethod]
        public async Task AddAsync_MissingFirstName_ThrowsAndDoesNotInsert()
        {
            var customer = new Customer { FirstName = " ", LastName = "Novak" };

            // ThrowsExceptionAsync počaka na Task in preveri tip izjeme.
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _service.AddAsync(customer));

            Assert.AreEqual(0, _repository.Customers.Count, "Neveljavna stranka ne sme priti v bazo.");
        }

        [TestMethod]
        public async Task AddAsync_InvalidEmail_Throws()
        {
            var customer = new Customer { FirstName = "Janez", Email = "ni-email" };

            ArgumentException ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => _service.AddAsync(customer));

            StringAssert.Contains(ex.Message, "Email");
        }

        [TestMethod]
        public async Task UpdateAsync_Conflict_ThrowsConcurrencyException()
        {
            // Repozitorij javi, da je vrstico vmes spremenil nekdo drug.
            _repository.NextUpdateResult = UpdateResult.Conflict;
            var customer = new Customer { Id = 5, FirstName = "Janez" };

            await Assert.ThrowsExceptionAsync<ConcurrencyException>(() => _service.UpdateAsync(customer));
        }

        [TestMethod]
        public async Task UpdateAsync_NotFound_ThrowsInvalidOperationException()
        {
            _repository.NextUpdateResult = UpdateResult.NotFound;
            var customer = new Customer { Id = 5, FirstName = "Janez" };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.UpdateAsync(customer));
        }

        [TestMethod]
        public async Task UpdateAsync_Updated_DoesNotThrow()
        {
            _repository.NextUpdateResult = UpdateResult.Updated;
            var customer = new Customer { Id = 5, FirstName = "Janez" };

            await _service.UpdateAsync(customer);

            // Brez izjeme = uspeh. Assert ni potreben, a ga dodamo za berljivost.
            Assert.AreEqual("Janez", customer.FirstName);
        }

        [TestMethod]
        public async Task SearchAsync_NegativePageIndex_RequestsFirstPage()
        {
            await _service.SearchAsync(null, -3, 10);

            Assert.AreEqual(0, _repository.LastPageIndex);
        }

        [TestMethod]
        public async Task SearchAsync_ZeroPageSize_ThrowsArgumentOutOfRange()
        {
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _service.SearchAsync(null, 0, 0));
        }

        [TestMethod]
        public void Constructor_NullRepository_ThrowsArgumentNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CustomerService(null));
        }

        [TestMethod]
        public void PagedResult_PageCount_RoundsUp()
        {
            var page = new PagedResult<Customer> { TotalCount = 21, PageSize = 10 };

            Assert.AreEqual(3, page.PageCount);
        }
    }
}
