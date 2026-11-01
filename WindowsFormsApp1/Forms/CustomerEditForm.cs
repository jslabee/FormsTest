using System;
using System.ComponentModel;
using System.Windows.Forms;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Services;

namespace WindowsFormsApp1.Forms
{
    /// <summary>
    /// Modalni obrazec za vnos nove ali urejanje obstoječe stranke.
    /// Dela na kopiji, zato preklic ne spremeni ničesar. Validacija z ErrorProvider.
    /// </summary>
    public partial class CustomerEditForm : Form
    {
        /// <summary>
        /// Stranka s podatki iz obrazca. Veljavna samo, ko je DialogResult == OK.
        /// </summary>
        public Customer Customer { get; private set; }

        /// <summary>
        /// Odpre obrazec za novo stranko (customer == null) ali za urejanje kopije obstoječe.
        /// </summary>
        public CustomerEditForm(Customer customer)
        {
            InitializeComponent();

            bool isNew = customer == null;

            // Kopija: če uporabnik prekliče, original v seznamu ostane nespremenjen.
            // Kopija ohrani tudi UpdatedAt, ki ga servis potrebuje za optimistično zaklepanje.
            Customer = isNew ? new Customer() : customer.Clone();
            Text = isNew ? "Nova stranka" : "Urejanje stranke";

            txtFirstName.Text = Customer.FirstName;
            txtLastName.Text = Customer.LastName;
            txtEmail.Text = Customer.Email;
            txtPhone.Text = Customer.Phone;
        }

        // INTERVJU: Validating se sproži, ko kontrola izgublja fokus (če ima CausesValidation = true).
        // e.Cancel = true pomeni "vrednost ni veljavna". Ker je AutoValidate = EnableAllowFocusChange,
        // uporabnik lahko kljub temu nadaljuje na drugo polje (ni "ujet"), ValidateChildren() pa
        // ob shranjevanju vrne false. ErrorProvider ob polju pokaže ikono z opisom napake.
        private void txtFirstName_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !ShowFieldError(txtFirstName, CustomerValidator.ValidateFirstName(txtFirstName.Text));
        }

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !ShowFieldError(txtEmail, CustomerValidator.ValidateEmail(txtEmail.Text));
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // ValidateChildren sproži Validating za VSE kontrole - tudi za tiste, ki jih
            // uporabnik sploh ni obiskal (npr. prazno Ime, če je takoj kliknil Shrani).
            if (!ValidateChildren())
            {
                FocusFirstError();
                return; // obrazec ostane odprt
            }

            Customer.FirstName = txtFirstName.Text;
            Customer.LastName = txtLastName.Text;
            Customer.Email = txtEmail.Text;
            Customer.Phone = txtPhone.Text;

            // Pravila so v Services; obrazec jih uporabi še enkrat na celotnem objektu
            // (enaka pravila kot servis, ki je zadnja varovalka pred bazo).
            if (CustomerValidator.Validate(Customer).Count > 0)
            {
                FocusFirstError();
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void CustomerEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Znana past: neveljavna kontrola (Validating z e.Cancel = true) lahko prepreči zapiranje
            // okna s križcem. Zapiranje dovolimo vedno - shranjevanje gre itak samo prek btnOk.
            e.Cancel = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // Prazen handler, ki ga je ustvaril Designer (dvoklik na radioButton1). Kontrola nima
            // namena; ko jo odstraniš v Designerju, izbriši tudi to metodo.
        }

        /// <summary>
        /// Nastavi ali pobriše napako ob polju. Vrne true, če je polje veljavno.
        /// </summary>
        private bool ShowFieldError(Control control, string error)
        {
            // Prazen niz pobriše ikono napake.
            errorProvider.SetError(control, error ?? string.Empty);
            return error == null;
        }

        private void FocusFirstError()
        {
            if (!string.IsNullOrEmpty(errorProvider.GetError(txtFirstName)))
            {
                txtFirstName.Focus();
            }
            else if (!string.IsNullOrEmpty(errorProvider.GetError(txtEmail)))
            {
                txtEmail.Focus();
            }
        }
    }
}
