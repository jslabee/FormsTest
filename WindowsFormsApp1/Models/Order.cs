using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApp1.Models
{
    /// <summary>
    /// Glava naročila (tabela orders) s seznamom postavk (tabela order_items).
    /// Postavke napolni repozitorij z Dapper multi-mappingom ali z QueryMultiple.
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }

        // INTERVJU: lastnost tipa List<T> omogoča master-detail vezavo v WinForms:
        // bsOrderItems.DataSource = bsOrders, bsOrderItems.DataMember = "Items".
        // BindingSource postavk sam sledi trenutno izbranemu naročilu.
        public List<OrderItem> Items { get; set; }

        public Order()
        {
            // Seznam vedno obstaja, da klicatelju ni treba preverjati null.
            Items = new List<OrderItem>();
            Status = "Novo";
        }

        /// <summary>
        /// Skupni znesek naročila. Ni stolpec v bazi - izračuna se iz postavk (ena resnica).
        /// </summary>
        public decimal Total
        {
            get { return Items.Sum(i => i.LineTotal); }
        }

        /// <summary>
        /// Število postavk (za prikaz v mreži naročil).
        /// </summary>
        public int ItemCount
        {
            get { return Items.Count; }
        }

        /// <summary>
        /// Globoka kopija (tudi postavke). Servis jo uporabi, ko mora naročilu pred shranjevanjem
        /// nekaj dodati (npr. namerno napačno postavko), ne da bi spremenil objekt klicatelja.
        /// </summary>
        public Order Clone()
        {
            var copy = (Order)MemberwiseClone();
            copy.Items = Items.Select(i => i.Clone()).ToList();
            return copy;
        }
    }
}
