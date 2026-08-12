namespace SushiKioskAdmin.Views
{
    partial class UcOrderHistory
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
            pnlSearch = new Panel();
            btnSearch = new Button();
            cmbOrderType = new ComboBox();
            label1 = new Label();
            dtpEnd = new DateTimePicker();
            dtpStart = new DateTimePicker();
            dgvHistoryList = new DataGridView();
            panel1 = new Panel();
            panel2 = new Panel();
            btnPrintReceipt = new Button();
            txtReceipt = new RichTextBox();
            pnlSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistoryList).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSearch
            // 
            pnlSearch.Controls.Add(btnSearch);
            pnlSearch.Controls.Add(cmbOrderType);
            pnlSearch.Controls.Add(label1);
            pnlSearch.Controls.Add(dtpEnd);
            pnlSearch.Controls.Add(dtpStart);
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Location = new Point(0, 0);
            pnlSearch.Name = "pnlSearch";
            pnlSearch.Size = new Size(848, 60);
            pnlSearch.TabIndex = 0;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(742, 14);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(83, 31);
            btnSearch.TabIndex = 3;
            btnSearch.Text = "조회";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // cmbOrderType
            // 
            cmbOrderType.FormattingEnabled = true;
            cmbOrderType.Items.AddRange(new object[] { "전체", "앱(배달/포장)", "키오스크(매장/포장)" });
            cmbOrderType.Location = new Point(572, 16);
            cmbOrderType.Name = "cmbOrderType";
            cmbOrderType.Size = new Size(125, 23);
            cmbOrderType.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(261, 15);
            label1.Name = "label1";
            label1.Size = new Size(21, 21);
            label1.TabIndex = 1;
            label1.Text = "~";
            // 
            // dtpEnd
            // 
            dtpEnd.Location = new Point(304, 16);
            dtpEnd.Name = "dtpEnd";
            dtpEnd.Size = new Size(200, 23);
            dtpEnd.TabIndex = 0;
            // 
            // dtpStart
            // 
            dtpStart.Location = new Point(40, 16);
            dtpStart.Name = "dtpStart";
            dtpStart.Size = new Size(200, 23);
            dtpStart.TabIndex = 0;
            // 
            // dgvHistoryList
            // 
            dgvHistoryList.AllowUserToAddRows = false;
            dgvHistoryList.AllowUserToResizeColumns = false;
            dgvHistoryList.AllowUserToResizeRows = false;
            dgvHistoryList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistoryList.Dock = DockStyle.Left;
            dgvHistoryList.Location = new Point(0, 60);
            dgvHistoryList.MultiSelect = false;
            dgvHistoryList.Name = "dgvHistoryList";
            dgvHistoryList.ReadOnly = true;
            dgvHistoryList.RowHeadersVisible = false;
            dgvHistoryList.RowTemplate.Height = 25;
            dgvHistoryList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistoryList.Size = new Size(536, 390);
            dgvHistoryList.TabIndex = 1;
            dgvHistoryList.SelectionChanged += dgvHistoryList_SelectionChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(txtReceipt);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(536, 60);
            panel1.Name = "panel1";
            panel1.Size = new Size(312, 390);
            panel1.TabIndex = 2;
            // 
            // panel2
            // 
            panel2.Controls.Add(btnPrintReceipt);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 340);
            panel2.Name = "panel2";
            panel2.Size = new Size(312, 50);
            panel2.TabIndex = 1;
            // 
            // btnPrintReceipt
            // 
            btnPrintReceipt.Location = new Point(191, 5);
            btnPrintReceipt.Name = "btnPrintReceipt";
            btnPrintReceipt.Size = new Size(107, 37);
            btnPrintReceipt.TabIndex = 0;
            btnPrintReceipt.Text = "영수증 재발행";
            btnPrintReceipt.UseVisualStyleBackColor = true;
            btnPrintReceipt.Click += btnPrintReceipt_Click;
            // 
            // txtReceipt
            // 
            txtReceipt.Dock = DockStyle.Fill;
            txtReceipt.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            txtReceipt.Location = new Point(0, 0);
            txtReceipt.Name = "txtReceipt";
            txtReceipt.ReadOnly = true;
            txtReceipt.Size = new Size(312, 390);
            txtReceipt.TabIndex = 0;
            txtReceipt.Text = "";
            // 
            // UcOrderHistory
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel1);
            Controls.Add(dgvHistoryList);
            Controls.Add(pnlSearch);
            Name = "UcOrderHistory";
            Size = new Size(848, 450);
            Load += UcOrderHistory_Load;
            pnlSearch.ResumeLayout(false);
            pnlSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvHistoryList).EndInit();
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlSearch;
        private Button btnSearch;
        private ComboBox cmbOrderType;
        private Label label1;
        private DateTimePicker dtpEnd;
        private DateTimePicker dtpStart;
        private DataGridView dgvHistoryList;
        private Panel panel1;
        private Panel panel2;
        private Button btnPrintReceipt;
        private RichTextBox txtReceipt;
    }
}