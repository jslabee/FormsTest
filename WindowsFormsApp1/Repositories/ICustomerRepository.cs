using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Repositories
{
    /// <summary>
    /// Pogodba za dostop do strank. Servisi poznajo samo ta vmesnik, ne konkretnega razreda.
    /// </summary>
    // INTERVJU: vmesnik omogoča "dependency injection" - servis dobi repozitorij v konstruktorju.
    // V aplikaciji podamo CustomerRepository (MySQL), v testih pa lažni repozitorij v pomnilniku,
    // zato testi tečejo brez baze, hitro in vedno z enakim izidom.
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();

        Task<Customer> GetByIdAsync(int id);

        /// <summary>
        /// Ena stran strank, ki se ujemajo z iskalnim nizom (ime, priimek ali email).
        /// </summary>
        Task<PagedResult<Customer>> SearchPagedAsync(string term, int pageIndex, int pageSize);

        Task InsertAsync(Customer customer);

        /// <summary>
        /// Posodobi stranko z optimističnim zaklepanjem (preveri UpdatedAt).
        /// </summary>
        Task<UpdateResult> UpdateAsync(Customer customer);

        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Statistika stranke prek shranjene procedure sp_customer_stats.
        /// </summary>
        Task<CustomerStats> GetStatsAsync(int customerId);

        /// <summary>
        /// Vstavi paket strank v eni transakciji (uvoz CSV, async različica).
        /// </summary>
        Task InsertBatchAsync(IList<Customer> customers, CancellationToken cancellationToken);

        /// <summary>
        /// Sinhrona različica za BackgroundWorker (stari način, koda že teče na drugi niti).
        /// </summary>
        void InsertBatch(IList<Customer> customers);
    }
}
