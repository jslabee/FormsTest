namespace WindowsFormsApp1.Forms
{
    partial class MainForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvCustomers = new System.Windows.Forms.DataGridView();
            this.colFirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPhone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsCustomers = new System.Windows.Forms.BindingSource(this.components);
            this.flpSearch = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnStats = new System.Windows.Forms.Button();
            this.btnNewOrder = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuImport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDemoErrors = new System.Windows.Forms.ToolStripMenuItem();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.flpPaging = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.lblPage = new System.Windows.Forms.Label();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.splitOrders = new System.Windows.Forms.SplitContainer();
            this.dgvOrders = new System.Windows.Forms.DataGridView();
            this.colOrderId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrderDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrderStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrderItemCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrderTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrderNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsOrders = new System.Windows.Forms.BindingSource(this.components);
            this.dgvOrderItems = new System.Windows.Forms.DataGridView();
            this.colItemProduct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemUnitPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemLineTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsOrderItems = new System.Windows.Forms.BindingSource(this.components);
            this.lblOrders = new System.Windows.Forms.Label();
            this.tmrSearch = new System.Windows.Forms.Timer(this.components);
            this.sfdExport = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCustomers)).BeginInit();
            this.flpSearch.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.flpPaging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitOrders)).BeginInit();
            this.splitOrders.Panel1.SuspendLayout();
            this.splitOrders.Panel2.SuspendLayout();
            this.splitOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsOrderItems)).BeginInit();
            this.SuspendLayout();
            //
            // dgvCustomers
            //
            this.dgvCustomers.AllowUserToAddRows = false;
            this.dgvCustomers.AllowUserToDeleteRows = false;
            this.dgvCustomers.AllowUserToResizeRows = false;
            this.dgvCustomers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCustomers.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFirstName,
            this.colLastName,
            this.colEmail,
            this.colPhone});
            this.dgvCustomers.DataSource = this.bsCustomers;
            this.dgvCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCustomers.Location = new System.Drawing.Point(0, 0);
            this.dgvCustomers.MultiSelect = false;
            this.dgvCustomers.Name = "dgvCustomers";
            this.dgvCustomers.ReadOnly = true;
            this.dgvCustomers.RowHeadersVisible = false;
            this.dgvCustomers.RowHeadersWidth = 82;
            this.dgvCustomers.RowTemplate.Height = 33;
            this.dgvCustomers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomers.Size = new System.Drawing.Size(1368, 376);
            this.dgvCustomers.TabIndex = 0;
            this.dgvCustomers.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomers_CellDoubleClick);
            //
            // colFirstName
            //
            this.colFirstName.DataPropertyName = "FirstName";
            this.colFirstName.HeaderText = "Ime";
            this.colFirstName.MinimumWidth = 10;
            this.colFirstName.Name = "colFirstName";
            this.colFirstName.ReadOnly = true;
            //
            // colLastName
            //
            this.colLastName.DataPropertyName = "LastName";
            this.colLastName.HeaderText = "Priimek";
            this.colLastName.MinimumWidth = 10;
            this.colLastName.Name = "colLastName";
            this.colLastName.ReadOnly = true;
            //
            // colEmail
            //
            this.colEmail.DataPropertyName = "Email";
            this.colEmail.HeaderText = "Email";
            this.colEmail.MinimumWidth = 10;
            this.colEmail.Name = "colEmail";
            this.colEmail.ReadOnly = true;
            //
            // colPhone
            //
            this.colPhone.DataPropertyName = "Phone";
            this.colPhone.HeaderText = "Telefon";
            this.colPhone.MinimumWidth = 10;
            this.colPhone.Name = "colPhone";
            this.colPhone.ReadOnly = true;
            //
            // bsCustomers
            //
            this.bsCustomers.CurrentChanged += new System.EventHandler(this.bsCustomers_CurrentChanged);
            //
            // flpSearch
            //
            this.flpSearch.Controls.Add(this.lblSearch);
            this.flpSearch.Controls.Add(this.txtSearch);
            this.flpSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpSearch.Location = new System.Drawing.Point(0, 40);
            this.flpSearch.Name = "flpSearch";
            this.flpSearch.Padding = new System.Windows.Forms.Padding(16, 12, 16, 0);
            this.flpSearch.Size = new System.Drawing.Size(1600, 72);
            this.flpSearch.TabIndex = 2;
            this.flpSearch.WrapContents = false;
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(16, 24);
            this.lblSearch.Margin = new System.Windows.Forms.Padding(0, 12, 12, 0);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(300, 25);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Iskanje (ime, priimek, email):";
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(334, 18);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(6);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(500, 31);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            //
            // flpButtons
            //
            this.flpButtons.Controls.Add(this.btnAdd);
            this.flpButtons.Controls.Add(this.btnEdit);
            this.flpButtons.Controls.Add(this.btnDelete);
            this.flpButtons.Controls.Add(this.btnStats);
            this.flpButtons.Controls.Add(this.btnNewOrder);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpButtons.Location = new System.Drawing.Point(1368, 112);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.flpButtons.Size = new System.Drawing.Size(232, 756);
            this.flpButtons.TabIndex = 1;
            //
            // btnAdd
            //
            this.btnAdd.Location = new System.Drawing.Point(22, 6);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(188, 56);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Dodaj";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            //
            // btnEdit
            //
            this.btnEdit.Location = new System.Drawing.Point(22, 74);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(6);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(188, 56);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "Uredi";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            //
            // btnDelete
            //
            this.btnDelete.Location = new System.Drawing.Point(22, 142);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(6);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(188, 56);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Izbriši";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            //
            // btnStats
            //
            this.btnStats.Location = new System.Drawing.Point(22, 234);
            this.btnStats.Margin = new System.Windows.Forms.Padding(6, 30, 6, 6);
            this.btnStats.Name = "btnStats";
            this.btnStats.Size = new System.Drawing.Size(188, 56);
            this.btnStats.TabIndex = 3;
            this.btnStats.Text = "Statistika";
            this.btnStats.UseVisualStyleBackColor = true;
            this.btnStats.Click += new System.EventHandler(this.btnStats_Click);
            //
            // btnNewOrder
            //
            this.btnNewOrder.Location = new System.Drawing.Point(22, 302);
            this.btnNewOrder.Margin = new System.Windows.Forms.Padding(6);
            this.btnNewOrder.Name = "btnNewOrder";
            this.btnNewOrder.Size = new System.Drawing.Size(188, 56);
            this.btnNewOrder.TabIndex = 4;
            this.btnNewOrder.Text = "Novo naročilo";
            this.btnNewOrder.UseVisualStyleBackColor = true;
            this.btnNewOrder.Click += new System.EventHandler(this.btnNewOrder_Click);
            //
            // statusStrip
            //
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 868);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1600, 32);
            this.statusStrip.TabIndex = 3;
            //
            // lblStatus
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 25);
            //
            // menuStrip
            //
            this.menuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuTools});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1600, 40);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip";
            //
            // mnuFile
            //
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuImport,
            this.mnuExport,
            this.mnuFileSeparator,
            this.mnuExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(122, 36);
            this.mnuFile.Text = "&Datoteka";
            //
            // mnuImport
            //
            this.mnuImport.Name = "mnuImport";
            this.mnuImport.Size = new System.Drawing.Size(398, 44);
            this.mnuImport.Text = "&Uvoz strank iz CSV ...";
            this.mnuImport.Click += new System.EventHandler(this.mnuImport_Click);
            //
            // mnuExport
            //
            this.mnuExport.Name = "mnuExport";
            this.mnuExport.Size = new System.Drawing.Size(398, 44);
            this.mnuExport.Text = "&Izvoz strank v CSV ...";
            this.mnuExport.Click += new System.EventHandler(this.mnuExport_Click);
            //
            // mnuFileSeparator
            //
            this.mnuFileSeparator.Name = "mnuFileSeparator";
            this.mnuFileSeparator.Size = new System.Drawing.Size(395, 6);
            //
            // mnuExit
            //
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(398, 44);
            this.mnuExit.Text = "I&zhod";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            //
            // mnuTools
            //
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDemoErrors});
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new System.Drawing.Size(103, 36);
            this.mnuTools.Text = "&Orodja";
            //
            // mnuDemoErrors
            //
            this.mnuDemoErrors.Name = "mnuDemoErrors";
            this.mnuDemoErrors.Size = new System.Drawing.Size(316, 44);
            this.mnuDemoErrors.Text = "&Demo napak ...";
            this.mnuDemoErrors.Click += new System.EventHandler(this.mnuDemoErrors_Click);
            //
            // splitMain
            //
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 112);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // splitMain.Panel1
            //
            this.splitMain.Panel1.Controls.Add(this.dgvCustomers);
            this.splitMain.Panel1.Controls.Add(this.flpPaging);
            //
            // splitMain.Panel2
            //
            this.splitMain.Panel2.Controls.Add(this.splitOrders);
            this.splitMain.Panel2.Controls.Add(this.lblOrders);
            this.splitMain.Size = new System.Drawing.Size(1368, 756);
            this.splitMain.SplitterDistance = 440;
            this.splitMain.SplitterWidth = 8;
            this.splitMain.TabIndex = 0;
            //
            // flpPaging
            //
            this.flpPaging.Controls.Add(this.btnPrevPage);
            this.flpPaging.Controls.Add(this.lblPage);
            this.flpPaging.Controls.Add(this.btnNextPage);
            this.flpPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flpPaging.Location = new System.Drawing.Point(0, 376);
            this.flpPaging.Name = "flpPaging";
            this.flpPaging.Padding = new System.Windows.Forms.Padding(10, 4, 10, 0);
            this.flpPaging.Size = new System.Drawing.Size(1368, 64);
            this.flpPaging.TabIndex = 1;
            this.flpPaging.WrapContents = false;
            //
            // btnPrevPage
            //
            this.btnPrevPage.Location = new System.Drawing.Point(16, 10);
            this.btnPrevPage.Margin = new System.Windows.Forms.Padding(6);
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(160, 46);
            this.btnPrevPage.TabIndex = 0;
            this.btnPrevPage.Text = "< Nazaj";
            this.btnPrevPage.UseVisualStyleBackColor = true;
            this.btnPrevPage.Click += new System.EventHandler(this.btnPrevPage_Click);
            //
            // lblPage
            //
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(194, 20);
            this.lblPage.Margin = new System.Windows.Forms.Padding(12, 16, 12, 0);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(114, 25);
            this.lblPage.TabIndex = 1;
            this.lblPage.Text = "Stran 1 / 1";
            //
            // btnNextPage
            //
            this.btnNextPage.Location = new System.Drawing.Point(326, 10);
            this.btnNextPage.Margin = new System.Windows.Forms.Padding(6);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(160, 46);
            this.btnNextPage.TabIndex = 2;
            this.btnNextPage.Text = "Naprej >";
            this.btnNextPage.UseVisualStyleBackColor = true;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            //
            // splitOrders
            //
            this.splitOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitOrders.Location = new System.Drawing.Point(0, 44);
            this.splitOrders.Name = "splitOrders";
            //
            // splitOrders.Panel1
            //
            this.splitOrders.Panel1.Controls.Add(this.dgvOrders);
            //
            // splitOrders.Panel2
            //
            this.splitOrders.Panel2.Controls.Add(this.dgvOrderItems);
            this.splitOrders.Size = new System.Drawing.Size(1368, 264);
            this.splitOrders.SplitterDistance = 820;
            this.splitOrders.SplitterWidth = 8;
            this.splitOrders.TabIndex = 1;
            //
            // dgvOrders
            //
            this.dgvOrders.AllowUserToAddRows = false;
            this.dgvOrders.AllowUserToDeleteRows = false;
            this.dgvOrders.AllowUserToResizeRows = false;
            this.dgvOrders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOrders.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colOrderId,
            this.colOrderDate,
            this.colOrderStatus,
            this.colOrderItemCount,
            this.colOrderTotal,
            this.colOrderNote});
            this.dgvOrders.DataSource = this.bsOrders;
            this.dgvOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOrders.Location = new System.Drawing.Point(0, 0);
            this.dgvOrders.MultiSelect = false;
            this.dgvOrders.Name = "dgvOrders";
            this.dgvOrders.ReadOnly = true;
            this.dgvOrders.RowHeadersVisible = false;
            this.dgvOrders.RowHeadersWidth = 82;
            this.dgvOrders.RowTemplate.Height = 33;
            this.dgvOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOrders.Size = new System.Drawing.Size(820, 264);
            this.dgvOrders.TabIndex = 0;
            //
            // colOrderId
            //
            this.colOrderId.DataPropertyName = "Id";
            this.colOrderId.FillWeight = 40F;
            this.colOrderId.HeaderText = "Št.";
            this.colOrderId.MinimumWidth = 10;
            this.colOrderId.Name = "colOrderId";
            this.colOrderId.ReadOnly = true;
            //
            // colOrderDate
            //
            this.colOrderDate.DataPropertyName = "OrderDate";
            dataGridViewCellStyle1.Format = "dd.MM.yyyy HH:mm";
            this.colOrderDate.DefaultCellStyle = dataGridViewCellStyle1;
            this.colOrderDate.FillWeight = 110F;
            this.colOrderDate.HeaderText = "Datum";
            this.colOrderDate.MinimumWidth = 10;
            this.colOrderDate.Name = "colOrderDate";
            this.colOrderDate.ReadOnly = true;
            //
            // colOrderStatus
            //
            this.colOrderStatus.DataPropertyName = "Status";
            this.colOrderStatus.FillWeight = 80F;
            this.colOrderStatus.HeaderText = "Status";
            this.colOrderStatus.MinimumWidth = 10;
            this.colOrderStatus.Name = "colOrderStatus";
            this.colOrderStatus.ReadOnly = true;
            //
            // colOrderItemCount
            //
            this.colOrderItemCount.DataPropertyName = "ItemCount";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colOrderItemCount.DefaultCellStyle = dataGridViewCellStyle2;
            this.colOrderItemCount.FillWeight = 55F;
            this.colOrderItemCount.HeaderText = "Postavk";
            this.colOrderItemCount.MinimumWidth = 10;
            this.colOrderItemCount.Name = "colOrderItemCount";
            this.colOrderItemCount.ReadOnly = true;
            //
            // colOrderTotal
            //
            this.colOrderTotal.DataPropertyName = "Total";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "N2";
            this.colOrderTotal.DefaultCellStyle = dataGridViewCellStyle3;
            this.colOrderTotal.FillWeight = 80F;
            this.colOrderTotal.HeaderText = "Znesek";
            this.colOrderTotal.MinimumWidth = 10;
            this.colOrderTotal.Name = "colOrderTotal";
            this.colOrderTotal.ReadOnly = true;
            //
            // colOrderNote
            //
            this.colOrderNote.DataPropertyName = "Note";
            this.colOrderNote.FillWeight = 150F;
            this.colOrderNote.HeaderText = "Opomba";
            this.colOrderNote.MinimumWidth = 10;
            this.colOrderNote.Name = "colOrderNote";
            this.colOrderNote.ReadOnly = true;
            //
            // dgvOrderItems
            //
            this.dgvOrderItems.AllowUserToAddRows = false;
            this.dgvOrderItems.AllowUserToDeleteRows = false;
            this.dgvOrderItems.AllowUserToResizeRows = false;
            this.dgvOrderItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOrderItems.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvOrderItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrderItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colItemProduct,
            this.colItemQuantity,
            this.colItemUnitPrice,
            this.colItemLineTotal});
            this.dgvOrderItems.DataSource = this.bsOrderItems;
            this.dgvOrderItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOrderItems.Location = new System.Drawing.Point(0, 0);
            this.dgvOrderItems.MultiSelect = false;
            this.dgvOrderItems.Name = "dgvOrderItems";
            this.dgvOrderItems.ReadOnly = true;
            this.dgvOrderItems.RowHeadersVisible = false;
            this.dgvOrderItems.RowHeadersWidth = 82;
            this.dgvOrderItems.RowTemplate.Height = 33;
            this.dgvOrderItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOrderItems.Size = new System.Drawing.Size(540, 264);
            this.dgvOrderItems.TabIndex = 0;
            //
            // colItemProduct
            //
            this.colItemProduct.DataPropertyName = "ProductName";
            this.colItemProduct.FillWeight = 160F;
            this.colItemProduct.HeaderText = "Izdelek";
            this.colItemProduct.MinimumWidth = 10;
            this.colItemProduct.Name = "colItemProduct";
            this.colItemProduct.ReadOnly = true;
            //
            // colItemQuantity
            //
            this.colItemQuantity.DataPropertyName = "Quantity";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colItemQuantity.DefaultCellStyle = dataGridViewCellStyle4;
            this.colItemQuantity.FillWeight = 60F;
            this.colItemQuantity.HeaderText = "Količina";
            this.colItemQuantity.MinimumWidth = 10;
            this.colItemQuantity.Name = "colItemQuantity";
            this.colItemQuantity.ReadOnly = true;
            //
            // colItemUnitPrice
            //
            this.colItemUnitPrice.DataPropertyName = "UnitPrice";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N2";
            this.colItemUnitPrice.DefaultCellStyle = dataGridViewCellStyle5;
            this.colItemUnitPrice.FillWeight = 70F;
            this.colItemUnitPrice.HeaderText = "Cena";
            this.colItemUnitPrice.MinimumWidth = 10;
            this.colItemUnitPrice.Name = "colItemUnitPrice";
            this.colItemUnitPrice.ReadOnly = true;
            //
            // colItemLineTotal
            //
            this.colItemLineTotal.DataPropertyName = "LineTotal";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N2";
            this.colItemLineTotal.DefaultCellStyle = dataGridViewCellStyle6;
            this.colItemLineTotal.FillWeight = 70F;
            this.colItemLineTotal.HeaderText = "Znesek";
            this.colItemLineTotal.MinimumWidth = 10;
            this.colItemLineTotal.Name = "colItemLineTotal";
            this.colItemLineTotal.ReadOnly = true;
            //
            // lblOrders
            //
            this.lblOrders.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblOrders.Location = new System.Drawing.Point(0, 0);
            this.lblOrders.Name = "lblOrders";
            this.lblOrders.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblOrders.Size = new System.Drawing.Size(1368, 44);
            this.lblOrders.TabIndex = 0;
            this.lblOrders.Text = "Naročila izbrane stranke";
            this.lblOrders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tmrSearch
            //
            this.tmrSearch.Interval = 400;
            this.tmrSearch.Tick += new System.EventHandler(this.tmrSearch_Tick);
            //
            // sfdExport
            //
            this.sfdExport.DefaultExt = "csv";
            this.sfdExport.Filter = "CSV datoteke (*.csv)|*.csv|Vse datoteke (*.*)|*.*";
            this.sfdExport.Title = "Izvoz strank v CSV";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1600, 900);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.flpButtons);
            this.Controls.Add(this.flpSearch);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(1200, 700);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Upravljanje strank in naročil";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsCustomers)).EndInit();
            this.flpSearch.ResumeLayout(false);
            this.flpSearch.PerformLayout();
            this.flpButtons.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.flpPaging.ResumeLayout(false);
            this.flpPaging.PerformLayout();
            this.splitOrders.Panel1.ResumeLayout(false);
            this.splitOrders.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitOrders)).EndInit();
            this.splitOrders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsOrderItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCustomers;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEmail;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhone;
        private System.Windows.Forms.BindingSource bsCustomers;
        private System.Windows.Forms.FlowLayoutPanel flpSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnStats;
        private System.Windows.Forms.Button btnNewOrder;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuImport;
        private System.Windows.Forms.ToolStripMenuItem mnuExport;
        private System.Windows.Forms.ToolStripSeparator mnuFileSeparator;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuDemoErrors;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.FlowLayoutPanel flpPaging;
        private System.Windows.Forms.Button btnPrevPage;
        private System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.SplitContainer splitOrders;
        private System.Windows.Forms.DataGridView dgvOrders;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrderId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrderDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrderStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrderItemCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrderTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrderNote;
        private System.Windows.Forms.BindingSource bsOrders;
        private System.Windows.Forms.DataGridView dgvOrderItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemUnitPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemLineTotal;
        private System.Windows.Forms.BindingSource bsOrderItems;
        private System.Windows.Forms.Label lblOrders;
        private System.Windows.Forms.Timer tmrSearch;
        private System.Windows.Forms.SaveFileDialog sfdExport;
    }
}
