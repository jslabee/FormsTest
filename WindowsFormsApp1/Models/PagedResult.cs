using System;
using System.Collections.Generic;

namespace WindowsFormsApp1.Models
{
    /// <summary>
    /// Ena stran rezultatov + skupno število zadetkov. Skupno število potrebujemo za prikaz
    /// "Stran 2 / 5" in za omogočanje gumbov Naprej/Nazaj.
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }

        // Indeks strani je od 0 (tako se lažje izračuna OFFSET = PageIndex * PageSize).
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public PagedResult()
        {
            Items = new List<T>();
        }

        /// <summary>
        /// Število strani; vsaj 1, da prikaz "Stran 1 / 1" deluje tudi pri praznem rezultatu.
        /// </summary>
        public int PageCount
        {
            get
            {
                if (PageSize <= 0)
                {
                    return 1;
                }

                return Math.Max(1, (TotalCount + PageSize - 1) / PageSize);
            }
        }
    }
}
