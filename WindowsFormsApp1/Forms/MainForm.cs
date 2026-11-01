using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    /// <summary>
    /// Glavno okno: zgoraj stranke (iskanje, straničenje, urejanje), spodaj naročila izbrane
    /// stranke in postavke izbranega naročila (master-detail). Obrazec skrbi samo za prikaz;
    /// vse operacije gredo skozi servise iz mape Services.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly CustomerService _customerService;
        private readonly OrderService _orderService;
        private readonly CsvImportService _importService;
        private readonly DemoService _demoService;

        // INTERVJU: BindingList<T> (za razliko od List<T>) sporoča spremembe (ListChanged).
        // BindingSource in DataGridView se nanje odzoveta sama: dodan, odstranjen ali zamenjan
        // element se takoj pokaže v mreži, brez ponovnega nastavljanja DataSource.
        private readonly BindingList<Customer> _customers = new BindingList<Customer>();
        private readonly BindingList<Order> _orders = new BindingList<Order>();

        // Straničenje: indeks trenutne strani (od 0) in skupno število strani.
        private int _pageIndex;
        private int _pageCount = 1;

        // INTERVJU: "race condition" pri asinhronem nalaganju. Če uporabnik hitro klikne dve
        // stranki, se lahko odgovor za prvo vrne ZA odgovorom za drugo in prepiše pravilne podatke.
        // Vsaka zahteva dobi zaporedno številko; rezultat prikažemo samo, če je še zadnja.
        private int _customersRequestId;
        private int _ordersRequestId;

        private bool _busy;
        private string _statusText = string.Empty;

        /// <summary>
        /// Servise dobi obrazec od zunaj (Program.cs) - obrazec ne sestavlja repozitorijev
        /// in ne bere connection stringa. Designer konstruktorja ne kliče, zato parametri ne motijo.
        /// </summary>
        public MainForm(CustomerService customerService, OrderService orderService,
            CsvImportService importService, DemoService demoService)
        {
            InitializeComponent();

            if (customerService == null) throw new ArgumentNullException(nameof(customerService));
            if (orderService == null) throw new ArgumentNullException(nameof(orderService));
            if (importService == null) throw new ArgumentNullException(nameof(importService));
            if (demoService == null) throw new ArgumentNullException(nameof(demoService));

            _customerService = customerService;
            _orderService = orderService;
            _importService = importService;
            _demoService = demoService;

            // AutoGenerateColumns v Designerju ni na voljo (ni v Properties oknu), zato ga nastavimo tukaj.
            // Brez tega bi mreža ob vezavi dodala še stolpce za Id, UpdatedAt, CustomerId ...
            dgvCustomers.AutoGenerateColumns = false;
            dgvOrders.AutoGenerateColumns = false;
            dgvOrderItems.AutoGenerateColumns = false;

            // Vezava: mreža -> BindingSource -> BindingList. Seznamov ne zamenjujemo, samo polnimo.
            bsCustomers.DataSource = _customers;
            bsOrders.DataSource = _orders;

            // INTERVJU: master-detail z BindingSource. Vir postavk je BindingSource naročil,
            // DataMember pa lastnost Order.Items. Ko uporabnik izbere drugo naročilo (spremeni se
            // bsOrders.Current), bsOrderItems sam pokaže postavke tega naročila - brez kode.
            // Nastavimo v kodi (ne v Designerju), ker DataMember potrebuje vir z znanim tipom.
            bsOrderItems.DataSource = bsOrders;
            bsOrderItems.DataMember = "Items";
        }

        // INTERVJU: async void je sprejemljiv SAMO pri event handlerjih (podpis določa .NET).
        // Izjeme iz async void ni mogoče ujeti pri klicatelju, zato jih ujamemo v metodi sami
        // (ali pa jih ujame globalni Application.ThreadException).
        private async void MainForm_Load(object sender, EventArgs e)
        {
            // Znana past WinForms: pri gnezdenem SplitContainer-ju samodejno skaliranje (DPI) ne
            // preračuna SplitterDistance - pri 100 % zaslonu bi desna mreža (postavke) izginila.
            // V Load je skaliranje že končano, zato razdaljo nastavimo sorazmerno (60 % : 40 %).
            splitOrders.SplitterDistance = splitOrders.Width * 60 / 100;

            await ReloadCustomersAsync(null);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // INTERVJU: "debounce". Ne iščemo ob vsaki črki (vsaka bi bila poizvedba v bazo),
            // ampak šele, ko uporabnik 400 ms ne tipka. Stop + Start timer ponastavi.
            tmrSearch.Stop();
            tmrSearch.Start();
        }

        private async void tmrSearch_Tick(object sender, EventArgs e)
        {
            // System.Windows.Forms.Timer sproži Tick na UI niti, zato smemo neposredno do kontrol.
            tmrSearch.Stop();

            // Nov iskalni niz -> nazaj na prvo stran.
            _pageIndex = 0;
            await ReloadCustomersAsync(null);
        }

        private async void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (_pageIndex > 0)
            {
                _pageIndex--;
                await ReloadCustomersAsync(null);
            }
        }

        private async void btnNextPage_Click(object sender, EventArgs e)
        {
            if (_pageIndex < _pageCount - 1)
            {
                _pageIndex++;
                await ReloadCustomersAsync(null);
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new CustomerEditForm(null))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                Customer customer = form.Customer;
                bool saved = await RunSaveAsync(() => _customerService.AddAsync(customer));
                if (saved)
                {
                    // Nova stranka je morda na drugi strani (urejeno po priimku) - osvežimo trenutno.
                    await ReloadCustomersAsync(customer.Id);
                    SetStatus("Dodana stranka: " + customer.FullName);
                }
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            await EditSelectedCustomerAsync();
        }

        private async void dgvCustomers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // e.RowIndex == -1 pomeni dvoklik na glavo stolpca.
            if (e.RowIndex >= 0)
            {
                await EditSelectedCustomerAsync();
            }
        }

        /// <summary>
        /// Odpre obrazec za urejanje izbrane stranke in shrani spremembe (z optimističnim zaklepanjem).
        /// </summary>
        // async Task (ne async void), ker to ni event handler - klicatelj lahko počaka in ujame napake.
        private async Task EditSelectedCustomerAsync()
        {
            Customer selected = GetSelectedCustomer();
            if (selected == null || _busy)
            {
                return;
            }

            using (var form = new CustomerEditForm(selected))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                Customer edited = form.Customer;
                bool saved = await RunSaveAsync(() => _customerService.UpdateAsync(edited));
                if (saved)
                {
                    // Zamenjava elementa v BindingList sproži ListChanged(ItemChanged) in mreža
                    // osveži samo to vrstico - brez ponovnega branja cele strani iz baze.
                    int index = IndexOfCustomer(edited.Id);
                    if (index >= 0)
                    {
                        _customers[index] = edited;
                    }

                    SetStatus("Shranjena stranka: " + edited.FullName);
                }
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            Customer selected = GetSelectedCustomer();
            if (selected == null)
            {
                return;
            }

            DialogResult answer = MessageBox.Show(this, "Ali res želite izbrisati stranko \"" + selected.FullName + "\"?",
                "Potrditev brisanja", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (answer != DialogResult.Yes)
            {
                return;
            }

            bool deleted = await RunSaveAsync(() => _customerService.DeleteAsync(selected.Id));
            if (deleted)
            {
                // Po brisanju se stran "zamakne" - ponovno naložimo, da je stran spet polna.
                await ReloadCustomersAsync(null);
            }
        }

        private async void btnStats_Click(object sender, EventArgs e)
        {
            Customer selected = GetSelectedCustomer();
            if (selected == null)
            {
                return;
            }

            SetBusy(true, "Računam statistiko ...");
            try
            {
                CustomerStats stats = await _customerService.GetStatsAsync(selected.Id);
                MessageBox.Show(this, FormatStats(selected, stats), "Statistika stranke",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError("Napaka pri branju statistike", ex);
            }
            finally
            {
                SetBusy(false, null);
            }
        }

        private async void btnNewOrder_Click(object sender, EventArgs e)
        {
            Customer selected = GetSelectedCustomer();
            if (selected == null)
            {
                return;
            }

            int? newOrderId = null;

            // Obrazec za naročilo shrani sam (da ob napaki ostane odprt in vnos ni izgubljen).
            using (var form = new OrderEditForm(_orderService, selected))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    newOrderId = form.SavedOrder.Id;
                }
            }

            if (newOrderId.HasValue)
            {
                await LoadOrdersAsync(newOrderId);
                SetStatus("Shranjeno naročilo št. " + newOrderId.Value);
            }
        }

        private void bsCustomers_CurrentChanged(object sender, EventArgs e)
        {
            UpdateButtons();

            // Klic brez await ("fire and forget", "_ =" je discard). Varno samo zato, ker
            // LoadOrdersAsync sam ujame vse svoje napake - sicer bi izjema v Tasku ostala neopažena.
            _ = LoadOrdersAsync(null);
        }

        private async void mnuImport_Click(object sender, EventArgs e)
        {
            using (var form = new ImportForm(_importService))
            {
                form.ShowDialog(this);
            }

            // Uvoz je morda dodal stranke - osvežimo seznam.
            await ReloadCustomersAsync(null);
        }

        private async void mnuExport_Click(object sender, EventArgs e)
        {
            sfdExport.FileName = "stranke_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            if (sfdExport.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            SetBusy(true, "Izvažam ...");
            try
            {
                int count = await _customerService.ExportToCsvAsync(sfdExport.FileName);
                MessageBox.Show(this, "Izvoženih strank: " + count + Environment.NewLine + sfdExport.FileName,
                    "Izvoz v CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError("Napaka pri izvozu", ex);
            }
            finally
            {
                SetBusy(false, null);
            }
        }

        private async void mnuDemoErrors_Click(object sender, EventArgs e)
        {
            using (var form = new DemoErrorsForm(_demoService))
            {
                form.ShowDialog(this);
            }

            // Demo optimističnega zaklepanja spremeni updated_at - preberemo sveže podatke.
            await ReloadCustomersAsync(null);
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Prebere trenutno stran strank iz baze in po želji izbere stranko z danim Id.
        /// </summary>
        private async Task ReloadCustomersAsync(int? selectId)
        {
            int requestId = ++_customersRequestId;
            Customer current = GetSelectedCustomer();
            int? keepId = selectId ?? (current != null ? current.Id : (int?)null);

            lblStatus.Text = "Nalagam stranke ...";
            try
            {
                PagedResult<Customer> page = await _customerService.SearchAsync(
                    txtSearch.Text, _pageIndex, CustomerService.DefaultPageSize);

                // Medtem se je začelo novo nalaganje (npr. uporabnik je tipkal naprej) - ta rezultat je star.
                if (requestId != _customersRequestId)
                {
                    return;
                }

                // Po brisanju ali uvozu je trenutna stran lahko za koncem - pojdi na zadnjo obstoječo.
                if (page.Items.Count == 0 && _pageIndex > 0 && page.TotalCount > 0)
                {
                    _pageIndex = page.PageCount - 1;
                    await ReloadCustomersAsync(selectId);
                    return;
                }

                _pageCount = page.PageCount;
                FillBindingList(_customers, page.Items);
                SelectCustomer(keepId);

                lblPage.Text = "Stran " + (_pageIndex + 1) + " / " + _pageCount;
                SetStatus("Najdenih strank: " + page.TotalCount);
            }
            catch (Exception ex)
            {
                SetStatus("Napaka pri nalaganju strank.");
                ShowError("Napaka pri nalaganju strank", ex);
            }
            finally
            {
                UpdateButtons();
            }
        }

        /// <summary>
        /// Prebere naročila izbrane stranke. Postavke izbranega naročila pokaže bsOrderItems sam.
        /// </summary>
        private async Task LoadOrdersAsync(int? selectOrderId)
        {
            int requestId = ++_ordersRequestId;
            Customer customer = GetSelectedCustomer();

            if (customer == null)
            {
                FillBindingList(_orders, new List<Order>());
                lblOrders.Text = "Naročila: ni izbrane stranke";
                return;
            }

            lblOrders.Text = "Naročila stranke " + customer.FullName + " - nalagam ...";
            try
            {
                List<Order> orders = await _orderService.GetByCustomerAsync(customer.Id);
                if (requestId != _ordersRequestId)
                {
                    return;
                }

                FillBindingList(_orders, orders);

                if (selectOrderId.HasValue)
                {
                    int index = orders.FindIndex(o => o.Id == selectOrderId.Value);
                    if (index >= 0)
                    {
                        bsOrders.Position = index;
                    }
                }

                lblOrders.Text = "Naročila stranke " + customer.FullName + ": " + orders.Count;
            }
            catch (Exception ex)
            {
                if (requestId == _ordersRequestId)
                {
                    lblOrders.Text = "Napaka pri nalaganju naročil.";
                    ShowError("Napaka pri nalaganju naročil", ex);
                }
            }
        }

        /// <summary>
        /// Zamenja vsebino BindingList-a z novimi elementi in mrežo osveži samo enkrat.
        /// </summary>
        private static void FillBindingList<T>(BindingList<T> list, IEnumerable<T> items)
        {
            // INTERVJU: brez RaiseListChangedEvents = false bi vsak Add sprožil ListChanged in
            // mreža bi se ob 100 elementih narisala 100-krat. Na koncu ResetBindings sporoči
            // eno samo "vse se je spremenilo" (ListChangedType.Reset).
            list.RaiseListChangedEvents = false;
            list.Clear();
            foreach (T item in items)
            {
                list.Add(item);
            }

            list.RaiseListChangedEvents = true;
            list.ResetBindings();
        }

        /// <summary>
        /// Skupni ovoj za operacije, ki shranjujejo v bazo: zaklene UI in ujame napake.
        /// Vrne true, če je operacija uspela.
        /// </summary>
        private async Task<bool> RunSaveAsync(Func<Task> action)
        {
            SetBusy(true, "Shranjujem ...");
            try
            {
                await action();
                return true;
            }
            catch (ConcurrencyException ex)
            {
                // Konflikt ni "napaka programa" - uporabnika obvestimo in pokažemo sveže podatke.
                MessageBox.Show(this, ex.Message, "Sočasna sprememba",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetBusy(false, null);
                await ReloadCustomersAsync(null);
                return false;
            }
            catch (Exception ex)
            {
                ShowError("Napaka pri shranjevanju", ex);
                return false;
            }
            finally
            {
                SetBusy(false, null);
            }
        }

        /// <summary>
        /// Izbere stranko z danim Id, če je na trenutni strani; sicer prvo vrstico.
        /// </summary>
        private void SelectCustomer(int? id)
        {
            if (!id.HasValue)
            {
                return;
            }

            int index = IndexOfCustomer(id.Value);
            if (index >= 0)
            {
                // Position na BindingSource premakne tudi izbrano vrstico v mreži (in sproži CurrentChanged).
                bsCustomers.Position = index;
            }
        }

        private int IndexOfCustomer(int id)
        {
            for (int i = 0; i < _customers.Count; i++)
            {
                if (_customers[i].Id == id)
                {
                    return i;
                }
            }

            return -1;
        }

        private Customer GetSelectedCustomer()
        {
            // Current je element, na katerem stoji BindingSource (= izbrana vrstica); null pri praznem seznamu.
            return bsCustomers.Current as Customer;
        }

        private void UpdateButtons()
        {
            bool hasSelection = GetSelectedCustomer() != null && !_busy;
            btnEdit.Enabled = hasSelection;
            btnDelete.Enabled = hasSelection;
            btnStats.Enabled = hasSelection;
            btnNewOrder.Enabled = hasSelection;
            btnPrevPage.Enabled = !_busy && _pageIndex > 0;
            btnNextPage.Enabled = !_busy && _pageIndex < _pageCount - 1;
        }

        /// <summary>
        /// Med dolgotrajno operacijo onemogoči ukaze, da uporabnik ne sproži iste stvari dvakrat.
        /// </summary>
        private void SetBusy(bool busy, string message)
        {
            _busy = busy;
            UseWaitCursor = busy;
            btnAdd.Enabled = !busy;
            menuStrip.Enabled = !busy;
            splitMain.Enabled = !busy;

            UpdateButtons();
            lblStatus.Text = busy ? message : _statusText;
        }

        private void SetStatus(string text)
        {
            _statusText = text;
            lblStatus.Text = text;
        }

        /// <summary>
        /// Napako zapiše v log in jo pokaže uporabniku.
        /// </summary>
        private void ShowError(string title, Exception ex)
        {
            AppLogger.Error(title, ex);
            MessageBox.Show(this, title + ":" + Environment.NewLine + ex.Message,
                "Napaka", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Besedilo za okno s statistiko stranke.
        /// </summary>
        private static string FormatStats(Customer customer, CustomerStats stats)
        {
            var text = new StringBuilder();
            text.AppendLine("Stranka: " + customer.FullName);
            text.AppendLine();
            text.AppendLine("Število naročil (brez storniranih): " + stats.OrderCount);
            text.AppendLine("Skupni znesek: " + stats.TotalAmount.ToString("N2") + " €");
            text.AppendLine("Zadnje naročilo: " +
                (stats.LastOrderDate.HasValue ? stats.LastOrderDate.Value.ToString("dd.MM.yyyy HH:mm") : "-"));

            if (stats.TopProducts.Count > 0)
            {
                text.AppendLine();
                text.AppendLine("Največ porabljeno za:");
                foreach (ProductStat product in stats.TopProducts)
                {
                    text.AppendLine("  " + product.ProductName + " - " + product.TotalQuantity + " kos, " +
                                    product.TotalAmount.ToString("N2") + " €");
                }
            }

            text.AppendLine();
            text.AppendLine("(podatki iz shranjene procedure sp_customer_stats)");
            return text.ToString();
        }
    }
}
