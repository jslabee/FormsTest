using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowsFormsApp1.Models
{
    /// <summary>
    /// Postavka naročila. Implementira INotifyPropertyChanged, ker jo uporabnik ureja neposredno
    /// v DataGridView-u (obrazec za vnos naročila).
    /// </summary>
    // INTERVJU: BindingList<T> sproži ListChanged (ItemChanged) samo, če element sporoči spremembo
    // prek INotifyPropertyChanged. Brez tega se izračunan stolpec "Znesek" in skupna vsota
    // ne bi osvežila, ko uporabnik spremeni količino ali ceno.
    public class OrderItem : INotifyPropertyChanged
    {
        private string _productName;
        private int _quantity;
        private decimal _unitPrice;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }
        public int OrderId { get; set; }

        public string ProductName
        {
            get { return _productName; }
            set { SetField(ref _productName, value); }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                // LineTotal je odvisen od količine, zato sporočimo spremembo obeh lastnosti.
                if (SetField(ref _quantity, value))
                {
                    OnPropertyChanged(nameof(LineTotal));
                }
            }
        }

        // decimal, ne double: denar mora biti točen (0.1 + 0.2 v double ni 0.3).
        public decimal UnitPrice
        {
            get { return _unitPrice; }
            set
            {
                if (SetField(ref _unitPrice, value))
                {
                    OnPropertyChanged(nameof(LineTotal));
                }
            }
        }

        /// <summary>
        /// Znesek postavke = količina x cena. Izračunan, ne shranjen v bazi.
        /// </summary>
        public decimal LineTotal
        {
            get { return Quantity * UnitPrice; }
        }

        /// <summary>
        /// Plitva kopija (vse lastnosti so vrednostni tipi ali string). Dogodka PropertyChanged
        /// ne kopiramo - nova postavka še nima naročnikov.
        /// </summary>
        public OrderItem Clone()
        {
            return new OrderItem
            {
                Id = Id,
                OrderId = OrderId,
                ProductName = ProductName,
                Quantity = Quantity,
                UnitPrice = UnitPrice
            };
        }

        /// <summary>
        /// Nastavi polje in sproži PropertyChanged samo, če se vrednost res spremeni.
        /// Vrne true, če je bila sprememba (da lahko klicatelj sporoči še odvisne lastnosti).
        /// </summary>
        // [CallerMemberName] prevajalnik sam vstavi ime lastnosti - ni "magičnih nizov".
        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged(string propertyName)
        {
            // Lokalna kopija delegata: če se vmes kdo odjavi, ne dobimo NullReferenceException.
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
