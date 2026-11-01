using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    /// <summary>
    /// Vnos novega naročila s postavkami. Shrani sam (prek OrderService), da ob napaki ostane
    /// odprt in uporabnik ne izgubi vnosa. Kljukica "Simuliraj napako" pokaže rollback transakcije.
    /// </summary>
    public partial class OrderEditForm : Form
    {
        private readonly OrderService _orderService;
        private readonly Customer _customer;

        // Postavke, ki jih uporabnik ureja v mreži (BindingList -> nova vrstica, brisanje, ListChanged).
        private readonly BindingList<OrderItem> _items = new BindingList<OrderItem>();

        private bool _saving;

        /// <summary>
        /// Naročilo, kot je shranjeno v bazi. Veljavno samo, ko je DialogResult == OK.
        /// </summary>
        public Order SavedOrder { get; private set; }

        /// <summary>
        /// Odpre prazen obrazec za naročilo dane stranke.
        /// </summary>
        public OrderEditForm(OrderService orderService, Customer customer)
        {
            InitializeComponent();

            if (orderService == null) throw new ArgumentNullException(nameof(orderService));
            if (customer == null) throw new ArgumentNullException(nameof(customer));

            _orderService = orderService;
            _customer = customer;

            dgvItems.AutoGenerateColumns = false;

            // INTERVJU: AddingNew omogoča, da nova vrstica v mreži dobi smiselne privzete vrednosti
            // (količina 1), namesto da bi BindingList klical prazen konstruktor (količina 0).
            _items.AddingNew += Items_AddingNew;
            bsItems.DataSource = _items;

            Text = "Novo naročilo - " + customer.FullName;
            lblCustomer.Text = customer.FullName + " (Id " + customer.Id + ")";
            txtNote.MaxLength = OrderValidator.MaxNoteLength;
            UpdateTotal();
        }

        private void Items_AddingNew(object sender, AddingNewEventArgs e)
        {
            e.NewObject = new OrderItem { Quantity = 1 };
        }

        private void bsItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            // Sproži se ob dodajanju/brisanju vrstice IN ob spremembi lastnosti postavke
            // (ker OrderItem implementira INotifyPropertyChanged) - zato je vsota vedno sveža.
            UpdateTotal();
        }

        // INTERVJU: DataError se sproži, ko mreža vrednosti iz celice ne more pretvoriti v tip
        // lastnosti (npr. "abc" v int Quantity). Brez tega handlerja WinForms pokaže grd sistemski
        // dialog z izjemo. Napako pokažemo ob vrstici, celica pa ostane v urejanju (e.Cancel = true).
        private void dgvItems_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string column = dgvItems.Columns[e.ColumnIndex].HeaderText;
            dgvItems.Rows[e.RowIndex].ErrorText = "Neveljavna vrednost v stolpcu \"" + column + "\" - vpišite število.";
            e.ThrowException = false;
            e.Cancel = true;
        }

        private void dgvItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Vrednost je bila uspešno potrjena - pobrišemo morebitno staro napako vrstice.
            dgvItems.Rows[e.RowIndex].ErrorText = string.Empty;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            // Celica, ki jo uporabnik še ureja, še ni zapisana v OrderItem - najprej jo potrdimo.
            if (!dgvItems.EndEdit())
            {
                return;
            }

            bsItems.EndEdit();

            Order order = BuildOrder();
            if (!ShowValidationErrors(order))
            {
                return;
            }

            SetSaving(true);
            try
            {
                SavedOrder = await _orderService.CreateAsync(order, chkSimulateError.Checked);

                // Nastavitev DialogResult zapre modalni obrazec; ShowDialog v MainForm vrne OK.
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                AppLogger.Error("Naročilo ni shranjeno", ex);
                MessageBox.Show(this,
                    "Naročilo NI shranjeno." + Environment.NewLine + Environment.NewLine +
                    "Glava in postavke se shranjujejo v eni transakciji, zato se je ob napaki razveljavilo " +
                    "vse (ROLLBACK) - v bazi ni ostalo nič od tega naročila. Vnos v obrazcu je ohranjen." +
                    Environment.NewLine + Environment.NewLine +
                    "Napaka: " + ex.Message,
                    "Napaka pri shranjevanju", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetSaving(false);
            }
        }

        private void OrderEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Med shranjevanjem ne dovolimo zapreti okna - rezultat shranjevanja bi se izgubil.
            if (_saving)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Sestavi naročilo iz vnosnih polj in postavk v mreži.
        /// </summary>
        private Order BuildOrder()
        {
            return new Order
            {
                CustomerId = _customer.Id,
                Note = txtNote.Text,
                Items = _items.ToList()
            };
        }

        /// <summary>
        /// Prikaže napake z ErrorProvider (ob poljih) in ErrorText (ob vrsticah mreže).
        /// Vrne true, če je naročilo veljavno.
        /// </summary>
        private bool ShowValidationErrors(Order order)
        {
            List<string> errors = OrderValidator.Validate(order);

            // Napake posameznih vrstic: rdeč klicaj v glavi vrstice z opisom v tooltipu.
            // Indeks vrstice v mreži = indeks v _items, ker mreža ni sortirana.
            for (int i = 0; i < _items.Count; i++)
            {
                List<string> itemErrors = OrderValidator.ValidateItem(_items[i], i + 1);
                dgvItems.Rows[i].ErrorText = string.Join(" ", itemErrors);
            }

            string noteError = errors.FirstOrDefault(m => m.StartsWith("Opomba"));
            errorProvider.SetError(txtNote, noteError ?? string.Empty);

            List<string> otherErrors = errors.Where(m => m != noteError).ToList();
            errorProvider.SetError(dgvItems, string.Join(Environment.NewLine, otherErrors));

            if (errors.Count > 0)
            {
                MessageBox.Show(this, string.Join(Environment.NewLine, errors), "Napaka pri vnosu",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void UpdateTotal()
        {
            lblTotal.Text = "Skupaj: " + _items.Sum(i => i.LineTotal).ToString("N2") + " €";
        }

        private void SetSaving(bool saving)
        {
            _saving = saving;
            UseWaitCursor = saving;
            btnSave.Enabled = !saving;
            btnCancel.Enabled = !saving;
            dgvItems.Enabled = !saving;
            txtNote.Enabled = !saving;
            chkSimulateError.Enabled = !saving;
        }
    }
}
