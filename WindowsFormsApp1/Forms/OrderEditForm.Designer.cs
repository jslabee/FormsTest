namespace WindowsFormsApp1.Forms
{
    partial class OrderEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tlpHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lblCustomerCaption = new System.Windows.Forms.Label();
            this.lblCustomer = new System.Windows.Forms.Label();
            this.lblNote = new System.Windows.Forms.Label();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.lblItems = new System.Windows.Forms.Label();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.colProduct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnitPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLineTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsItems = new System.Windows.Forms.BindingSource(this.components);
            this.lblTotal = new System.Windows.Forms.Label();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkSimulateError = new System.Windows.Forms.CheckBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tlpHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).BeginInit();
            this.flpButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            //
            // tlpHeader
            //
            this.tlpHeader.ColumnCount = 2;
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHeader.Controls.Add(this.lblCustomerCaption, 0, 0);
            this.tlpHeader.Controls.Add(this.lblCustomer, 1, 0);
            this.tlpHeader.Controls.Add(this.lblNote, 0, 1);
            this.tlpHeader.Controls.Add(this.txtNote, 1, 1);
            this.tlpHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpHeader.Location = new System.Drawing.Point(0, 0);
            this.tlpHeader.Name = "tlpHeader";
            this.tlpHeader.Padding = new System.Windows.Forms.Padding(24, 16, 56, 0);
            this.tlpHeader.RowCount = 2;
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpHeader.Size = new System.Drawing.Size(1200, 120);
            this.tlpHeader.TabIndex = 0;
            //
            // lblCustomerCaption
            //
            this.lblCustomerCaption.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCustomerCaption.AutoSize = true;
            this.lblCustomerCaption.Location = new System.Drawing.Point(24, 28);
            this.lblCustomerCaption.Margin = new System.Windows.Forms.Padding(0, 12, 16, 12);
            this.lblCustomerCaption.Name = "lblCustomerCaption";
            this.lblCustomerCaption.Size = new System.Drawing.Size(88, 25);
            this.lblCustomerCaption.TabIndex = 0;
            this.lblCustomerCaption.Text = "Stranka:";
            //
            // lblCustomer
            //
            this.lblCustomer.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomer.Location = new System.Drawing.Point(134, 28);
            this.lblCustomer.Margin = new System.Windows.Forms.Padding(6, 12, 6, 12);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(24, 25);
            this.lblCustomer.TabIndex = 1;
            this.lblCustomer.Text = "-";
            //
            // lblNote
            //
            this.lblNote.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNote.AutoSize = true;
            this.lblNote.Location = new System.Drawing.Point(24, 77);
            this.lblNote.Margin = new System.Windows.Forms.Padding(0, 12, 16, 12);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(86, 25);
            this.lblNote.TabIndex = 2;
            this.lblNote.Text = "Opomba:";
            //
            // txtNote
            //
            this.txtNote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNote.Location = new System.Drawing.Point(134, 71);
            this.txtNote.Margin = new System.Windows.Forms.Padding(6);
            this.txtNote.MaxLength = 500;
            this.txtNote.Name = "txtNote";
            this.txtNote.Size = new System.Drawing.Size(1004, 31);
            this.txtNote.TabIndex = 3;
            //
            // lblItems
            //
            this.lblItems.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblItems.Location = new System.Drawing.Point(0, 120);
            this.lblItems.Name = "lblItems";
            this.lblItems.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.lblItems.Size = new System.Drawing.Size(1200, 44);
            this.lblItems.TabIndex = 1;
            this.lblItems.Text = "Postavke (novo postavko vpišite v zadnjo, prazno vrstico; Delete izbriše izbrano vrstico):";
            this.lblItems.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // dgvItems
            //
            this.dgvItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvItems.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProduct,
            this.colQuantity,
            this.colUnitPrice,
            this.colLineTotal});
            this.dgvItems.DataSource = this.bsItems;
            this.dgvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItems.Location = new System.Drawing.Point(0, 164);
            this.dgvItems.MultiSelect = false;
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersWidth = 60;
            this.dgvItems.RowTemplate.Height = 33;
            this.dgvItems.Size = new System.Drawing.Size(1200, 500);
            this.dgvItems.TabIndex = 2;
            this.dgvItems.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellEndEdit);
            this.dgvItems.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvItems_DataError);
            //
            // colProduct
            //
            this.colProduct.DataPropertyName = "ProductName";
            this.colProduct.FillWeight = 200F;
            this.colProduct.HeaderText = "Izdelek";
            this.colProduct.MaxInputLength = 200;
            this.colProduct.MinimumWidth = 10;
            this.colProduct.Name = "colProduct";
            //
            // colQuantity
            //
            this.colQuantity.DataPropertyName = "Quantity";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colQuantity.DefaultCellStyle = dataGridViewCellStyle1;
            this.colQuantity.FillWeight = 60F;
            this.colQuantity.HeaderText = "Količina";
            this.colQuantity.MinimumWidth = 10;
            this.colQuantity.Name = "colQuantity";
            //
            // colUnitPrice
            //
            this.colUnitPrice.DataPropertyName = "UnitPrice";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "N2";
            this.colUnitPrice.DefaultCellStyle = dataGridViewCellStyle2;
            this.colUnitPrice.FillWeight = 80F;
            this.colUnitPrice.HeaderText = "Cena";
            this.colUnitPrice.MinimumWidth = 10;
            this.colUnitPrice.Name = "colUnitPrice";
            //
            // colLineTotal
            //
            this.colLineTotal.DataPropertyName = "LineTotal";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Format = "N2";
            this.colLineTotal.DefaultCellStyle = dataGridViewCellStyle3;
            this.colLineTotal.FillWeight = 80F;
            this.colLineTotal.HeaderText = "Znesek";
            this.colLineTotal.MinimumWidth = 10;
            this.colLineTotal.Name = "colLineTotal";
            this.colLineTotal.ReadOnly = true;
            //
            // bsItems
            //
            this.bsItems.ListChanged += new System.ComponentModel.ListChangedEventHandler(this.bsItems_ListChanged);
            //
            // lblTotal
            //
            this.lblTotal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(0, 664);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Padding = new System.Windows.Forms.Padding(0, 0, 40, 0);
            this.lblTotal.Size = new System.Drawing.Size(1200, 48);
            this.lblTotal.TabIndex = 3;
            this.lblTotal.Text = "Skupaj: 0,00 €";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // flpButtons
            //
            this.flpButtons.Controls.Add(this.btnCancel);
            this.flpButtons.Controls.Add(this.btnSave);
            this.flpButtons.Controls.Add(this.chkSimulateError);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpButtons.Location = new System.Drawing.Point(0, 712);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Padding = new System.Windows.Forms.Padding(16);
            this.flpButtons.Size = new System.Drawing.Size(1200, 88);
            this.flpButtons.TabIndex = 4;
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(998, 22);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(180, 46);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Prekliči";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(806, 22);
            this.btnSave.Margin = new System.Windows.Forms.Padding(6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(180, 46);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Shrani";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // chkSimulateError
            //
            this.chkSimulateError.AutoSize = true;
            this.chkSimulateError.ForeColor = System.Drawing.Color.DarkRed;
            this.chkSimulateError.Location = new System.Drawing.Point(462, 28);
            this.chkSimulateError.Margin = new System.Windows.Forms.Padding(6, 12, 30, 6);
            this.chkSimulateError.Name = "chkSimulateError";
            this.chkSimulateError.Size = new System.Drawing.Size(308, 29);
            this.chkSimulateError.TabIndex = 0;
            this.chkSimulateError.Text = "Simuliraj napako (rollback)";
            this.chkSimulateError.UseVisualStyleBackColor = true;
            //
            // errorProvider
            //
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            //
            // OrderEditForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.lblItems);
            this.Controls.Add(this.tlpHeader);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.flpButtons);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "OrderEditForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Novo naročilo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OrderEditForm_FormClosing);
            this.tlpHeader.ResumeLayout(false);
            this.tlpHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).EndInit();
            this.flpButtons.ResumeLayout(false);
            this.flpButtons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpHeader;
        private System.Windows.Forms.Label lblCustomerCaption;
        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.Label lblItems;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnitPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLineTotal;
        private System.Windows.Forms.BindingSource bsItems;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkSimulateError;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
