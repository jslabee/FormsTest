using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Repositories
{
    /// <summary>
    /// Pogodba za dostop do naročil in postavk.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Vsa naročila stranke s postavkami (ena poizvedba z JOIN, Dapper multi-mapping).
        /// </summary>
        Task<List<Order>> GetByCustomerAsync(int customerId);

        /// <summary>
        /// Eno naročilo s postavkami (dve poizvedbi v enem klicu, Dapper QueryMultiple).
        /// Vrne null, če naročila ni.
        /// </summary>
        Task<Order> GetByIdAsync(int orderId);

        /// <summary>
        /// Shrani glavo in vse postavke v eni transakciji. Vrne Id novega naročila.
        /// </summary>
        Task<int> CreateAsync(Order order);
    }
}
