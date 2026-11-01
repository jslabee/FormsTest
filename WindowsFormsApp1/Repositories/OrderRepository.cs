using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Repositories
{
    /// <summary>
    /// Dostop do tabel orders in order_items. Prikazuje tri tipične Dapper vzorce:
    /// multi-mapping (JOIN + splitOn), QueryMultiple in transakcijo čez več tabel.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private const string OrderColumns =
            "o.id AS Id, o.customer_id AS CustomerId, o.order_date AS OrderDate, o.status AS Status, o.note AS Note";

        private const string ItemColumns =
            "i.id AS Id, i.order_id AS OrderId, i.product_name AS ProductName, i.quantity AS Quantity, i.unit_price AS UnitPrice";

        private readonly string _connectionString;

        /// <summary>
        /// Connection string dobi od zunaj (Program.cs ga prebere iz App.config).
        /// </summary>
        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Vsa naročila stranke s postavkami, najnovejša najprej - z eno poizvedbo (JOIN).
        /// </summary>
        public async Task<List<Order>> GetByCustomerAsync(int customerId)
        {
            // LEFT JOIN, da dobimo tudi naročila brez postavk.
            const string sql =
                "SELECT " + OrderColumns + ", " + ItemColumns + " " +
                "FROM orders o " +
                "LEFT JOIN order_items i ON i.order_id = o.id " +
                "WHERE o.customer_id = @CustomerId " +
                "ORDER BY o.order_date DESC, o.id DESC, i.id";

            // JOIN vrne eno vrstico na POSTAVKO, zato se isto naročilo pojavi večkrat.
            // Slovar poskrbi, da vsako naročilo ustvarimo enkrat in mu le dodajamo postavke;
            // seznam ohrani vrstni red iz ORDER BY.
            var ordersById = new Dictionary<int, Order>();
            var orders = new List<Order>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                // INTERVJU: multi-mapping. Dapper vsako vrstico razreže na dva objekta (Order, OrderItem).
                // splitOn: "Id" pove, pri katerem stolpcu se začne drugi objekt - pri DRUGEM stolpcu
                // z imenom Id (i.id). Prvi Id (o.id) pripada naročilu. Če se drugi del začne z
                // drugačnim imenom, ga moramo navesti, sicer Dapper vrže "split on" napako.
                await connection.QueryAsync<Order, OrderItem, Order>(
                    sql,
                    (order, item) =>
                    {
                        Order existing;
                        if (!ordersById.TryGetValue(order.Id, out existing))
                        {
                            existing = order;
                            ordersById.Add(order.Id, existing);
                            orders.Add(existing);
                        }

                        // Pri LEFT JOIN brez postavk so stolpci postavke NULL.
                        if (item != null && item.Id != 0)
                        {
                            existing.Items.Add(item);
                        }

                        return existing;
                    },
                    new { CustomerId = customerId },
                    splitOn: "Id").ConfigureAwait(false);
            }

            return orders;
        }

        /// <summary>
        /// Eno naročilo s postavkami - dve poizvedbi v enem obisku baze (QueryMultiple).
        /// </summary>
        public async Task<Order> GetByIdAsync(int orderId)
        {
            // INTERVJU: QueryMultiple je alternativa JOIN-u. Glava se ne ponavlja v vsaki vrstici
            // (manj prenesenih podatkov), mapiranje je preprostejše, a sta to dve ločeni poizvedbi.
            const string sql =
                "SELECT " + OrderColumns + " FROM orders o WHERE o.id = @Id;" +
                "SELECT " + ItemColumns + " FROM order_items i WHERE i.order_id = @Id ORDER BY i.id;";

            using (var connection = new MySqlConnection(_connectionString))
            using (SqlMapper.GridReader multi = await connection.QueryMultipleAsync(sql, new { Id = orderId }).ConfigureAwait(false))
            {
                // Rezultate beremo v enakem vrstnem redu, kot so SELECT-i v ukazu.
                Order order = await multi.ReadSingleOrDefaultAsync<Order>().ConfigureAwait(false);
                if (order == null)
                {
                    return null;
                }

                order.Items = (await multi.ReadAsync<OrderItem>().ConfigureAwait(false)).ToList();
                return order;
            }
        }

        /// <summary>
        /// Shrani glavo naročila in vse postavke v ENI transakciji.
        /// Če katerikoli INSERT ne uspe, se razveljavi vse (ni naročila brez postavk ali s polovico postavk).
        /// </summary>
        public async Task<int> CreateAsync(Order order)
        {
            const string orderSql =
                "INSERT INTO orders (customer_id, status, note) VALUES (@CustomerId, @Status, @Note); " +
                "SELECT LAST_INSERT_ID();";

            const string itemSql =
                "INSERT INTO order_items (order_id, product_name, quantity, unit_price) " +
                "VALUES (@OrderId, @ProductName, @Quantity, @UnitPrice)";

            using (var connection = new MySqlConnection(_connectionString))
            {
                // Pri transakciji moramo povezavo odpreti sami - transakcija živi na odprti povezavi.
                await connection.OpenAsync().ConfigureAwait(false);

                using (MySqlTransaction transaction = await connection.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        // INTERVJU: vsak Dapper klic znotraj transakcije MORA dobiti parameter
                        // transaction:. Če ga pozabimo, MySqlConnector vrže napako, ker ukaz na
                        // povezavi z aktivno transakcijo ne sme teči mimo nje.
                        ulong newId = await connection.ExecuteScalarAsync<ulong>(orderSql, order, transaction)
                            .ConfigureAwait(false);
                        int orderId = checked((int)newId);

                        // Postavkam podamo Id glave. Ustvarimo nove objekte za parametre, da ne
                        // spreminjamo objektov klicatelja, če se transakcija razveljavi.
                        var itemParameters = order.Items.Select(i => new
                        {
                            OrderId = orderId,
                            i.ProductName,
                            i.Quantity,
                            i.UnitPrice
                        }).ToList();

                        await connection.ExecuteAsync(itemSql, itemParameters, transaction).ConfigureAwait(false);

                        await transaction.CommitAsync().ConfigureAwait(false);
                        return orderId;
                    }
                    catch
                    {
                        // INTERVJU: rollback. Dispose nepotrjene transakcije bi naredil rollback tudi sam,
                        // izrecni klic pa jasno pokaže namen. Če rollback sam ne uspe (npr. prekinjena
                        // povezava), te napake ne pokažemo - pomembnejša je prvotna, zato "throw;".
                        try
                        {
                            await transaction.RollbackAsync().ConfigureAwait(false);
                        }
                        catch (Exception)
                        {
                            // Namerno prezremo; strežnik ob prekinjeni povezavi transakcijo razveljavi sam.
                        }

                        // "throw;" (ne "throw ex;") ohrani prvotni stack trace.
                        throw;
                    }
                }
            }
        }
    }
}
