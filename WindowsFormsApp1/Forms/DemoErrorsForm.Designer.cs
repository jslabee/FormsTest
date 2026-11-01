namespace WindowsFormsApp1.Forms
{
    partial class DemoErrorsForm
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
            this.flpDemos = new System.Windows.Forms.FlowLayoutPanel();
            this.grpConnections = new System.Windows.Forms.GroupBox();
            this.btnLeakConnections = new System.Windows.Forms.Button();
            this.btnUsingConnections = new System.Windows.Forms.Button();
            this.grpDeadlock = new System.Windows.Forms.GroupBox();
            this.btnDeadlock = new System.Windows.Forms.Button();
            this.btnAwait = new System.Windows.Forms.Button();
            this.grpSqlInjection = new System.Windows.Forms.GroupBox();
            this.lblSqlInput = new System.Windows.Forms.Label();
            this.txtSqlInput = new System.Windows.Forms.TextBox();
            this.btnSqlUnsafe = new System.Windows.Forms.Button();
            this.btnSqlSafe = new System.Windows.Forms.Button();
            this.grpIndex = new System.Windows.Forms.GroupBox();
            this.btnQueryNoIndex = new System.Windows.Forms.Button();
            this.btnQueryWithIndex = new System.Windows.Forms.Button();
            this.grpCrossThread = new System.Windows.Forms.GroupBox();
            this.lblThreadTarget = new System.Windows.Forms.Label();
            this.btnCrossThread = new System.Windows.Forms.Button();
            this.btnInvoke = new System.Windows.Forms.Button();
            this.grpOther = new System.Windows.Forms.GroupBox();
            this.btnConcurrency = new System.Windows.Forms.Button();
            this.btnUnhandledUi = new System.Windows.Forms.Button();
            this.btnUnhandledThread = new System.Windows.Forms.Button();
            this.splitOutput = new System.Windows.Forms.SplitContainer();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.dgvResult = new System.Windows.Forms.DataGridView();
            this.flpDemos.SuspendLayout();
            this.grpConnections.SuspendLayout();
            this.grpDeadlock.SuspendLayout();
            this.grpSqlInjection.SuspendLayout();
            this.grpIndex.SuspendLayout();
            this.grpCrossThread.SuspendLayout();
            this.grpOther.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitOutput)).BeginInit();
            this.splitOutput.Panel1.SuspendLayout();
            this.splitOutput.Panel2.SuspendLayout();
            this.splitOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).BeginInit();
            this.SuspendLayout();
            //
            // flpDemos
            //
            this.flpDemos.AutoScroll = true;
            this.flpDemos.Controls.Add(this.grpConnections);
            this.flpDemos.Controls.Add(this.grpDeadlock);
            this.flpDemos.Controls.Add(this.grpSqlInjection);
            this.flpDemos.Controls.Add(this.grpIndex);
            this.flpDemos.Controls.Add(this.grpCrossThread);
            this.flpDemos.Controls.Add(this.grpOther);
            this.flpDemos.Dock = System.Windows.Forms.DockStyle.Left;
            this.flpDemos.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpDemos.Location = new System.Drawing.Point(0, 0);
            this.flpDemos.Name = "flpDemos";
            this.flpDemos.Padding = new System.Windows.Forms.Padding(12);
            this.flpDemos.Size = new System.Drawing.Size(590, 1000);
            this.flpDemos.TabIndex = 0;
            this.flpDemos.WrapContents = false;
            //
            // grpConnections
            //
            this.grpConnections.Controls.Add(this.btnLeakConnections);
            this.grpConnections.Controls.Add(this.btnUsingConnections);
            this.grpConnections.Location = new System.Drawing.Point(18, 18);
            this.grpConnections.Margin = new System.Windows.Forms.Padding(6);
            this.grpConnections.Name = "grpConnections";
            this.grpConnections.Size = new System.Drawing.Size(540, 112);
            this.grpConnections.TabIndex = 0;
            this.grpConnections.TabStop = false;
            this.grpConnections.Text = "1. Puščanje povezav (max_connections = 30)";
            //
            // btnLeakConnections
            //
            this.btnLeakConnections.Location = new System.Drawing.Point(16, 40);
            this.btnLeakConnections.Name = "btnLeakConnections";
            this.btnLeakConnections.Size = new System.Drawing.Size(248, 56);
            this.btnLeakConnections.TabIndex = 0;
            this.btnLeakConnections.Text = "1a. Brez using";
            this.btnLeakConnections.UseVisualStyleBackColor = true;
            this.btnLeakConnections.Click += new System.EventHandler(this.btnLeakConnections_Click);
            //
            // btnUsingConnections
            //
            this.btnUsingConnections.Location = new System.Drawing.Point(276, 40);
            this.btnUsingConnections.Name = "btnUsingConnections";
            this.btnUsingConnections.Size = new System.Drawing.Size(248, 56);
            this.btnUsingConnections.TabIndex = 1;
            this.btnUsingConnections.Text = "1b. Z using (pravilno)";
            this.btnUsingConnections.UseVisualStyleBackColor = true;
            this.btnUsingConnections.Click += new System.EventHandler(this.btnUsingConnections_Click);
            //
            // grpDeadlock
            //
            this.grpDeadlock.Controls.Add(this.btnDeadlock);
            this.grpDeadlock.Controls.Add(this.btnAwait);
            this.grpDeadlock.Location = new System.Drawing.Point(18, 142);
            this.grpDeadlock.Margin = new System.Windows.Forms.Padding(6);
            this.grpDeadlock.Name = "grpDeadlock";
            this.grpDeadlock.Size = new System.Drawing.Size(540, 112);
            this.grpDeadlock.TabIndex = 1;
            this.grpDeadlock.TabStop = false;
            this.grpDeadlock.Text = "2. Deadlock zaradi .Result (2a ZAMRZNE aplikacijo!)";
            //
            // btnDeadlock
            //
            this.btnDeadlock.ForeColor = System.Drawing.Color.DarkRed;
            this.btnDeadlock.Location = new System.Drawing.Point(16, 40);
            this.btnDeadlock.Name = "btnDeadlock";
            this.btnDeadlock.Size = new System.Drawing.Size(248, 56);
            this.btnDeadlock.TabIndex = 0;
            this.btnDeadlock.Text = "2a. .Result (!)";
            this.btnDeadlock.UseVisualStyleBackColor = true;
            this.btnDeadlock.Click += new System.EventHandler(this.btnDeadlock_Click);
            //
            // btnAwait
            //
            this.btnAwait.Location = new System.Drawing.Point(276, 40);
            this.btnAwait.Name = "btnAwait";
            this.btnAwait.Size = new System.Drawing.Size(248, 56);
            this.btnAwait.TabIndex = 1;
            this.btnAwait.Text = "2b. await (pravilno)";
            this.btnAwait.UseVisualStyleBackColor = true;
            this.btnAwait.Click += new System.EventHandler(this.btnAwait_Click);
            //
            // grpSqlInjection
            //
            this.grpSqlInjection.Controls.Add(this.lblSqlInput);
            this.grpSqlInjection.Controls.Add(this.txtSqlInput);
            this.grpSqlInjection.Controls.Add(this.btnSqlUnsafe);
            this.grpSqlInjection.Controls.Add(this.btnSqlSafe);
            this.grpSqlInjection.Location = new System.Drawing.Point(18, 266);
            this.grpSqlInjection.Margin = new System.Windows.Forms.Padding(6);
            this.grpSqlInjection.Name = "grpSqlInjection";
            this.grpSqlInjection.Size = new System.Drawing.Size(540, 170);
            this.grpSqlInjection.TabIndex = 2;
            this.grpSqlInjection.TabStop = false;
            this.grpSqlInjection.Text = "3. SQL injection";
            //
            // lblSqlInput
            //
            this.lblSqlInput.AutoSize = true;
            this.lblSqlInput.Location = new System.Drawing.Point(16, 46);
            this.lblSqlInput.Name = "lblSqlInput";
            this.lblSqlInput.Size = new System.Drawing.Size(83, 25);
            this.lblSqlInput.TabIndex = 0;
            this.lblSqlInput.Text = "Priimek:";
            //
            // txtSqlInput
            //
            this.txtSqlInput.Location = new System.Drawing.Point(110, 40);
            this.txtSqlInput.Name = "txtSqlInput";
            this.txtSqlInput.Size = new System.Drawing.Size(414, 31);
            this.txtSqlInput.TabIndex = 1;
            this.txtSqlInput.Text = "x\' OR \'1\'=\'1";
            //
            // btnSqlUnsafe
            //
            this.btnSqlUnsafe.Location = new System.Drawing.Point(16, 96);
            this.btnSqlUnsafe.Name = "btnSqlUnsafe";
            this.btnSqlUnsafe.Size = new System.Drawing.Size(248, 56);
            this.btnSqlUnsafe.TabIndex = 2;
            this.btnSqlUnsafe.Text = "3a. Lepljenje niza";
            this.btnSqlUnsafe.UseVisualStyleBackColor = true;
            this.btnSqlUnsafe.Click += new System.EventHandler(this.btnSqlUnsafe_Click);
            //
            // btnSqlSafe
            //
            this.btnSqlSafe.Location = new System.Drawing.Point(276, 96);
            this.btnSqlSafe.Name = "btnSqlSafe";
            this.btnSqlSafe.Size = new System.Drawing.Size(248, 56);
            this.btnSqlSafe.TabIndex = 3;
            this.btnSqlSafe.Text = "3b. Parametri (pravilno)";
            this.btnSqlSafe.UseVisualStyleBackColor = true;
            this.btnSqlSafe.Click += new System.EventHandler(this.btnSqlSafe_Click);
            //
            // grpIndex
            //
            this.grpIndex.Controls.Add(this.btnQueryNoIndex);
            this.grpIndex.Controls.Add(this.btnQueryWithIndex);
            this.grpIndex.Location = new System.Drawing.Point(18, 448);
            this.grpIndex.Margin = new System.Windows.Forms.Padding(6);
            this.grpIndex.Name = "grpIndex";
            this.grpIndex.Size = new System.Drawing.Size(540, 112);
            this.grpIndex.TabIndex = 3;
            this.grpIndex.TabStop = false;
            this.grpIndex.Text = "4. Počasna poizvedba brez indeksa (EXPLAIN)";
            //
            // btnQueryNoIndex
            //
            this.btnQueryNoIndex.Location = new System.Drawing.Point(16, 40);
            this.btnQueryNoIndex.Name = "btnQueryNoIndex";
            this.btnQueryNoIndex.Size = new System.Drawing.Size(248, 56);
            this.btnQueryNoIndex.TabIndex = 0;
            this.btnQueryNoIndex.Text = "4a. Brez indeksa";
            this.btnQueryNoIndex.UseVisualStyleBackColor = true;
            this.btnQueryNoIndex.Click += new System.EventHandler(this.btnQueryNoIndex_Click);
            //
            // btnQueryWithIndex
            //
            this.btnQueryWithIndex.Location = new System.Drawing.Point(276, 40);
            this.btnQueryWithIndex.Name = "btnQueryWithIndex";
            this.btnQueryWithIndex.Size = new System.Drawing.Size(248, 56);
            this.btnQueryWithIndex.TabIndex = 1;
            this.btnQueryWithIndex.Text = "4b. Z indeksom";
            this.btnQueryWithIndex.UseVisualStyleBackColor = true;
            this.btnQueryWithIndex.Click += new System.EventHandler(this.btnQueryWithIndex_Click);
            //
            // grpCrossThread
            //
            this.grpCrossThread.Controls.Add(this.lblThreadTarget);
            this.grpCrossThread.Controls.Add(this.btnCrossThread);
            this.grpCrossThread.Controls.Add(this.btnInvoke);
            this.grpCrossThread.Location = new System.Drawing.Point(18, 572);
            this.grpCrossThread.Margin = new System.Windows.Forms.Padding(6);
            this.grpCrossThread.Name = "grpCrossThread";
            this.grpCrossThread.Size = new System.Drawing.Size(540, 160);
            this.grpCrossThread.TabIndex = 4;
            this.grpCrossThread.TabStop = false;
            this.grpCrossThread.Text = "5. Dostop do kontrole iz druge niti";
            //
            // lblThreadTarget
            //
            this.lblThreadTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblThreadTarget.Location = new System.Drawing.Point(16, 40);
            this.lblThreadTarget.Name = "lblThreadTarget";
            this.lblThreadTarget.Size = new System.Drawing.Size(508, 40);
            this.lblThreadTarget.TabIndex = 0;
            this.lblThreadTarget.Text = "Ciljna kontrola (Label)";
            this.lblThreadTarget.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnCrossThread
            //
            this.btnCrossThread.Location = new System.Drawing.Point(16, 90);
            this.btnCrossThread.Name = "btnCrossThread";
            this.btnCrossThread.Size = new System.Drawing.Size(248, 56);
            this.btnCrossThread.TabIndex = 1;
            this.btnCrossThread.Text = "5a. Brez Invoke";
            this.btnCrossThread.UseVisualStyleBackColor = true;
            this.btnCrossThread.Click += new System.EventHandler(this.btnCrossThread_Click);
            //
            // btnInvoke
            //
            this.btnInvoke.Location = new System.Drawing.Point(276, 90);
            this.btnInvoke.Name = "btnInvoke";
            this.btnInvoke.Size = new System.Drawing.Size(248, 56);
            this.btnInvoke.TabIndex = 2;
            this.btnInvoke.Text = "5b. Z Invoke (pravilno)";
            this.btnInvoke.UseVisualStyleBackColor = true;
            this.btnInvoke.Click += new System.EventHandler(this.btnInvoke_Click);
            //
            // grpOther
            //
            this.grpOther.Controls.Add(this.btnConcurrency);
            this.grpOther.Controls.Add(this.btnUnhandledUi);
            this.grpOther.Controls.Add(this.btnUnhandledThread);
            this.grpOther.Location = new System.Drawing.Point(18, 744);
            this.grpOther.Margin = new System.Windows.Forms.Padding(6);
            this.grpOther.Name = "grpOther";
            this.grpOther.Size = new System.Drawing.Size(540, 176);
            this.grpOther.TabIndex = 5;
            this.grpOther.TabStop = false;
            this.grpOther.Text = "6.-7. Sočasnost in globalne napake";
            //
            // btnConcurrency
            //
            this.btnConcurrency.Location = new System.Drawing.Point(16, 40);
            this.btnConcurrency.Name = "btnConcurrency";
            this.btnConcurrency.Size = new System.Drawing.Size(508, 56);
            this.btnConcurrency.TabIndex = 0;
            this.btnConcurrency.Text = "6. Optimistično zaklepanje (konflikt)";
            this.btnConcurrency.UseVisualStyleBackColor = true;
            this.btnConcurrency.Click += new System.EventHandler(this.btnConcurrency_Click);
            //
            // btnUnhandledUi
            //
            this.btnUnhandledUi.Location = new System.Drawing.Point(16, 104);
            this.btnUnhandledUi.Name = "btnUnhandledUi";
            this.btnUnhandledUi.Size = new System.Drawing.Size(248, 56);
            this.btnUnhandledUi.TabIndex = 1;
            this.btnUnhandledUi.Text = "7a. Neujeta izjema (UI)";
            this.btnUnhandledUi.UseVisualStyleBackColor = true;
            this.btnUnhandledUi.Click += new System.EventHandler(this.btnUnhandledUi_Click);
            //
            // btnUnhandledThread
            //
            this.btnUnhandledThread.ForeColor = System.Drawing.Color.DarkRed;
            this.btnUnhandledThread.Location = new System.Drawing.Point(276, 104);
            this.btnUnhandledThread.Name = "btnUnhandledThread";
            this.btnUnhandledThread.Size = new System.Drawing.Size(248, 56);
            this.btnUnhandledThread.TabIndex = 2;
            this.btnUnhandledThread.Text = "7b. Na niti (ZAPRE!)";
            this.btnUnhandledThread.UseVisualStyleBackColor = true;
            this.btnUnhandledThread.Click += new System.EventHandler(this.btnUnhandledThread_Click);
            //
            // splitOutput
            //
            this.splitOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitOutput.Location = new System.Drawing.Point(590, 0);
            this.splitOutput.Name = "splitOutput";
            this.splitOutput.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // splitOutput.Panel1
            //
            this.splitOutput.Panel1.Controls.Add(this.txtOutput);
            //
            // splitOutput.Panel2
            //
            this.splitOutput.Panel2.Controls.Add(this.dgvResult);
            this.splitOutput.Size = new System.Drawing.Size(1010, 1000);
            this.splitOutput.SplitterDistance = 600;
            this.splitOutput.SplitterWidth = 8;
            this.splitOutput.TabIndex = 1;
            //
            // txtOutput
            //
            this.txtOutput.BackColor = System.Drawing.SystemColors.Window;
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(0, 0);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(1010, 600);
            this.txtOutput.TabIndex = 0;
            this.txtOutput.WordWrap = false;
            //
            // dgvResult
            //
            this.dgvResult.AllowUserToAddRows = false;
            this.dgvResult.AllowUserToDeleteRows = false;
            this.dgvResult.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvResult.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvResult.Location = new System.Drawing.Point(0, 0);
            this.dgvResult.Name = "dgvResult";
            this.dgvResult.ReadOnly = true;
            this.dgvResult.RowHeadersVisible = false;
            this.dgvResult.RowHeadersWidth = 82;
            this.dgvResult.RowTemplate.Height = 33;
            this.dgvResult.Size = new System.Drawing.Size(1010, 392);
            this.dgvResult.TabIndex = 0;
            //
            // DemoErrorsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1600, 1000);
            this.Controls.Add(this.splitOutput);
            this.Controls.Add(this.flpDemos);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1200, 700);
            this.Name = "DemoErrorsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Demo napak";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DemoErrorsForm_FormClosing);
            this.flpDemos.ResumeLayout(false);
            this.grpConnections.ResumeLayout(false);
            this.grpDeadlock.ResumeLayout(false);
            this.grpSqlInjection.ResumeLayout(false);
            this.grpSqlInjection.PerformLayout();
            this.grpIndex.ResumeLayout(false);
            this.grpCrossThread.ResumeLayout(false);
            this.grpOther.ResumeLayout(false);
            this.splitOutput.Panel1.ResumeLayout(false);
            this.splitOutput.Panel1.PerformLayout();
            this.splitOutput.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitOutput)).EndInit();
            this.splitOutput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpDemos;
        private System.Windows.Forms.GroupBox grpConnections;
        private System.Windows.Forms.Button btnLeakConnections;
        private System.Windows.Forms.Button btnUsingConnections;
        private System.Windows.Forms.GroupBox grpDeadlock;
        private System.Windows.Forms.Button btnDeadlock;
        private System.Windows.Forms.Button btnAwait;
        private System.Windows.Forms.GroupBox grpSqlInjection;
        private System.Windows.Forms.Label lblSqlInput;
        private System.Windows.Forms.TextBox txtSqlInput;
        private System.Windows.Forms.Button btnSqlUnsafe;
        private System.Windows.Forms.Button btnSqlSafe;
        private System.Windows.Forms.GroupBox grpIndex;
        private System.Windows.Forms.Button btnQueryNoIndex;
        private System.Windows.Forms.Button btnQueryWithIndex;
        private System.Windows.Forms.GroupBox grpCrossThread;
        private System.Windows.Forms.Label lblThreadTarget;
        private System.Windows.Forms.Button btnCrossThread;
        private System.Windows.Forms.Button btnInvoke;
        private System.Windows.Forms.GroupBox grpOther;
        private System.Windows.Forms.Button btnConcurrency;
        private System.Windows.Forms.Button btnUnhandledUi;
        private System.Windows.Forms.Button btnUnhandledThread;
        private System.Windows.Forms.SplitContainer splitOutput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.DataGridView dgvResult;
    }
}
