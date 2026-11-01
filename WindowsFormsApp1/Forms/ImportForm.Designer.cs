namespace WindowsFormsApp1.Forms
{
    partial class ImportForm
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
            this.tlpFile = new System.Windows.Forms.TableLayoutPanel();
            this.lblFile = new System.Windows.Forms.Label();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.flpActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnImportAsync = new System.Windows.Forms.Button();
            this.btnImportBgw = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlProgress = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.flpBottom = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.bgwImport = new System.ComponentModel.BackgroundWorker();
            this.ofdCsv = new System.Windows.Forms.OpenFileDialog();
            this.tlpFile.SuspendLayout();
            this.flpActions.SuspendLayout();
            this.pnlProgress.SuspendLayout();
            this.flpBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // tlpFile
            //
            this.tlpFile.ColumnCount = 3;
            this.tlpFile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpFile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpFile.Controls.Add(this.lblFile, 0, 0);
            this.tlpFile.Controls.Add(this.txtFile, 1, 0);
            this.tlpFile.Controls.Add(this.btnBrowse, 2, 0);
            this.tlpFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpFile.Location = new System.Drawing.Point(0, 0);
            this.tlpFile.Name = "tlpFile";
            this.tlpFile.Padding = new System.Windows.Forms.Padding(16, 12, 16, 0);
            this.tlpFile.RowCount = 1;
            this.tlpFile.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFile.Size = new System.Drawing.Size(1100, 76);
            this.tlpFile.TabIndex = 0;
            //
            // lblFile
            //
            this.lblFile.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(16, 25);
            this.lblFile.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(144, 25);
            this.lblFile.TabIndex = 0;
            this.lblFile.Text = "Datoteka CSV:";
            //
            // txtFile
            //
            this.txtFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFile.Location = new System.Drawing.Point(178, 22);
            this.txtFile.Margin = new System.Windows.Forms.Padding(6);
            this.txtFile.Name = "txtFile";
            this.txtFile.ReadOnly = true;
            this.txtFile.Size = new System.Drawing.Size(706, 31);
            this.txtFile.TabIndex = 1;
            //
            // btnBrowse
            //
            this.btnBrowse.Location = new System.Drawing.Point(896, 18);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(6);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(182, 46);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Prebrskaj ...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            //
            // flpActions
            //
            this.flpActions.Controls.Add(this.btnImportAsync);
            this.flpActions.Controls.Add(this.btnImportBgw);
            this.flpActions.Controls.Add(this.btnCancel);
            this.flpActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.flpActions.Location = new System.Drawing.Point(0, 76);
            this.flpActions.Name = "flpActions";
            this.flpActions.Padding = new System.Windows.Forms.Padding(10, 6, 10, 0);
            this.flpActions.Size = new System.Drawing.Size(1100, 76);
            this.flpActions.TabIndex = 1;
            this.flpActions.WrapContents = false;
            //
            // btnImportAsync
            //
            this.btnImportAsync.Location = new System.Drawing.Point(16, 12);
            this.btnImportAsync.Margin = new System.Windows.Forms.Padding(6);
            this.btnImportAsync.Name = "btnImportAsync";
            this.btnImportAsync.Size = new System.Drawing.Size(330, 52);
            this.btnImportAsync.TabIndex = 0;
            this.btnImportAsync.Text = "Uvozi (async/await + IProgress)";
            this.btnImportAsync.UseVisualStyleBackColor = true;
            this.btnImportAsync.Click += new System.EventHandler(this.btnImportAsync_Click);
            //
            // btnImportBgw
            //
            this.btnImportBgw.Location = new System.Drawing.Point(358, 12);
            this.btnImportBgw.Margin = new System.Windows.Forms.Padding(6);
            this.btnImportBgw.Name = "btnImportBgw";
            this.btnImportBgw.Size = new System.Drawing.Size(330, 52);
            this.btnImportBgw.TabIndex = 1;
            this.btnImportBgw.Text = "Uvozi (BackgroundWorker)";
            this.btnImportBgw.UseVisualStyleBackColor = true;
            this.btnImportBgw.Click += new System.EventHandler(this.btnImportBgw_Click);
            //
            // btnCancel
            //
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(700, 12);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(180, 52);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Prekliči";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // pnlProgress
            //
            this.pnlProgress.Controls.Add(this.progressBar);
            this.pnlProgress.Controls.Add(this.lblProgress);
            this.pnlProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProgress.Location = new System.Drawing.Point(0, 152);
            this.pnlProgress.Name = "pnlProgress";
            this.pnlProgress.Padding = new System.Windows.Forms.Padding(16, 4, 16, 12);
            this.pnlProgress.Size = new System.Drawing.Size(1100, 96);
            this.pnlProgress.TabIndex = 2;
            //
            // progressBar
            //
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar.Location = new System.Drawing.Point(16, 40);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1068, 40);
            this.progressBar.TabIndex = 1;
            //
            // lblProgress
            //
            this.lblProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblProgress.Location = new System.Drawing.Point(16, 4);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(1068, 36);
            this.lblProgress.TabIndex = 0;
            this.lblProgress.Text = "Izberite datoteko in način uvoza.";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtLog
            //
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(0, 248);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(1100, 414);
            this.txtLog.TabIndex = 3;
            //
            // flpBottom
            //
            this.flpBottom.Controls.Add(this.btnClose);
            this.flpBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flpBottom.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpBottom.Location = new System.Drawing.Point(0, 662);
            this.flpBottom.Name = "flpBottom";
            this.flpBottom.Padding = new System.Windows.Forms.Padding(16);
            this.flpBottom.Size = new System.Drawing.Size(1100, 88);
            this.flpBottom.TabIndex = 4;
            //
            // btnClose
            //
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(898, 22);
            this.btnClose.Margin = new System.Windows.Forms.Padding(6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(180, 46);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Zapri";
            this.btnClose.UseVisualStyleBackColor = true;
            //
            // bgwImport
            //
            this.bgwImport.WorkerReportsProgress = true;
            this.bgwImport.WorkerSupportsCancellation = true;
            this.bgwImport.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwImport_DoWork);
            this.bgwImport.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgwImport_ProgressChanged);
            this.bgwImport.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwImport_RunWorkerCompleted);
            //
            // ofdCsv
            //
            this.ofdCsv.DefaultExt = "csv";
            this.ofdCsv.Filter = "CSV datoteke (*.csv)|*.csv|Vse datoteke (*.*)|*.*";
            this.ofdCsv.Title = "Izberite CSV datoteko s strankami";
            //
            // ImportForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1100, 750);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.pnlProgress);
            this.Controls.Add(this.flpActions);
            this.Controls.Add(this.tlpFile);
            this.Controls.Add(this.flpBottom);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "ImportForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Uvoz strank iz CSV";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImportForm_FormClosing);
            this.tlpFile.ResumeLayout(false);
            this.tlpFile.PerformLayout();
            this.flpActions.ResumeLayout(false);
            this.pnlProgress.ResumeLayout(false);
            this.flpBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpFile;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.FlowLayoutPanel flpActions;
        private System.Windows.Forms.Button btnImportAsync;
        private System.Windows.Forms.Button btnImportBgw;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlProgress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.FlowLayoutPanel flpBottom;
        private System.Windows.Forms.Button btnClose;
        private System.ComponentModel.BackgroundWorker bgwImport;
        private System.Windows.Forms.OpenFileDialog ofdCsv;
    }
}
