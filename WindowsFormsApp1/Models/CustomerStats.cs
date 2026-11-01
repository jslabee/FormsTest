using System;
using System.Collections.Generic;

namespace WindowsFormsApp1.Models
{
    /// <summary>
    /// Statistika stranke, ki jo vrne shranjena procedura sp_customer_stats:
    /// prve tri lastnosti pridejo iz OUT parametrov, TopProducts iz rezultata (result set).
    /// </summary>
    public class CustomerStats
    {
        public int CustomerId { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalAmount { get; set; }

        // Nullable: stranka brez naročil nima zadnjega naročila (procedura vrne NULL).
        public DateTime? LastOrderDate { get; set; }

        public List<ProductStat> TopProducts { get; set; }

        public CustomerStats()
        {
            TopProducts = new List<ProductStat>();
        }
    }

    /// <summary>
    /// Ena vrstica rezultata procedure: koliko je stranka kupila posameznega izdelka.
    /// </summary>
    public class ProductStat
    {
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
