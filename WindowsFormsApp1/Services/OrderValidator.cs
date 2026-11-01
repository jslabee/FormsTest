using System.Collections.Generic;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Services
{
    /// <summary>
    /// Pravila za veljavno naročilo. Enaka pravila preverja tudi baza (CHECK omejitve),
    /// a v aplikaciji jih preverimo prej, da uporabnik dobi razumljivo sporočilo.
    /// </summary>
    public static class OrderValidator
    {
        /// <summary>
        /// Največja dolžina opombe (enako kot stolpec orders.note).
        /// </summary>
        public const int MaxNoteLength = 500;

        /// <summary>
        /// Vrne vse napake naročila (prazen seznam = naročilo je veljavno).
        /// </summary>
        public static List<string> Validate(Order order)
        {
            var errors = new List<string>();

            if (order.CustomerId <= 0)
            {
                errors.Add("Naročilo mora pripadati stranki.");
            }

            if (order.Note != null && order.Note.Length > MaxNoteLength)
            {
                errors.Add("Opomba je predolga (največ " + MaxNoteLength + " znakov).");
            }

            if (order.Items.Count == 0)
            {
                errors.Add("Naročilo mora imeti vsaj eno postavko.");
            }

            // Številka postavke za uporabnika je od 1, ne od 0.
            for (int i = 0; i < order.Items.Count; i++)
            {
                errors.AddRange(ValidateItem(order.Items[i], i + 1));
            }

            return errors;
        }

        /// <summary>
        /// Napake ene postavke; obrazec jih uporabi tudi za ErrorText posamezne vrstice v mreži.
        /// </summary>
        public static List<string> ValidateItem(OrderItem item, int lineNumber)
        {
            var errors = new List<string>();
            string prefix = "Postavka " + lineNumber + ": ";

            if (string.IsNullOrWhiteSpace(item.ProductName))
            {
                errors.Add(prefix + "naziv izdelka je obvezen.");
            }

            if (item.Quantity <= 0)
            {
                errors.Add(prefix + "količina mora biti večja od 0.");
            }

            if (item.UnitPrice < 0)
            {
                errors.Add(prefix + "cena ne sme biti negativna.");
            }

            return errors;
        }
    }
}
