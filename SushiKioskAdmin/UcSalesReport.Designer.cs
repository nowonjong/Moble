namespace SushiKioskAdmin.Views
{
    partial class UcSalesReport
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
            pnlTop = new Panel();
            cmbPeriodUnit = new ComboBox();
            label1 = new Label();
            btnSearch = new Button();
            dtpEndDate = new DateTimePicker();
            dtpStartDate = new DateTimePicker();
            btnExportCsv = new Button();
            pnlBottom = new Panel();
            lblTotalDiscount = new Label();
            lblTotalOrders = new Label();
            btnExportPdf = new Button();
            lblTotalSales = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            pnlChartArea = new Panel();
            dgvSalesReport = new DataGridView();
            pnlTop.SuspendLayout();
            pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSalesReport).BeginInit();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(cmbPeriodUnit);
            pnlTop.Controls.Add(label1);
            pnlTop.Controls.Add(btnSearch);
            pnlTop.Controls.Add(dtpEndDate);
            pnlTop.Controls.Add(dtpStartDate);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(848, 45);
            pnlTop.TabIndex = 0;
            // 
            // cmbPeriodUnit
            // 
            cmbPeriodUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPeriodUnit.FormattingEnabled = true;
            cmbPeriodUnit.Location = new Point(493, 10);
            cmbPeriodUnit.Name = "cmbPeriodUnit";
            cmbPeriodUnit.Size = new Size(121, 23);
            cmbPeriodUnit.TabIndex = 3;
            cmbPeriodUnit.SelectedIndexChanged += cmbPeriodUnit_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(231, 10);
            label1.Name = "label1";
            label1.Size = new Size(20, 20);
            label1.TabIndex = 1;
            label1.Text = "~";
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(714, 9);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(101, 28);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "조회";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // dtpEndDate
            // 
            dtpEndDate.Location = new Point(265, 10);
            dtpEndDate.Name = "dtpEndDate";
            dtpEndDate.Size = new Size(200, 23);
            dtpEndDate.TabIndex = 0;
            // 
            // dtpStartDate
            // 
            dtpStartDate.Location = new Point(19, 10);
            dtpStartDate.Name = "dtpStartDate";
            dtpStartDate.Size = new Size(200, 23);
            dtpStartDate.TabIndex = 0;
            // 
            // btnExportCsv
            // 
            btnExportCsv.Location = new Point(570, 6);
            btnExportCsv.Name = "btnExportCsv";
            btnExportCsv.Size = new Size(118, 34);
            btnExportCsv.TabIndex = 2;
            btnExportCsv.Text = "CSV 저장";
            btnExportCsv.UseVisualStyleBackColor = true;
            btnExportCsv.Click += btnExportCsv_Click;
            // 
            // pnlBottom
            // 
            pnlBottom.Controls.Add(lblTotalDiscount);
            pnlBottom.Controls.Add(lblTotalOrders);
            pnlBottom.Controls.Add(btnExportPdf);
            pnlBottom.Controls.Add(btnExportCsv);
            pnlBottom.Controls.Add(lblTotalSales);
            pnlBottom.Controls.Add(label4);
            pnlBottom.Controls.Add(label3);
            pnlBottom.Controls.Add(label2);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 405);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(848, 45);
            pnlBottom.TabIndex = 1;
            // 
            // lblTotalDiscount
            // 
            lblTotalDiscount.AutoSize = true;
            lblTotalDiscount.Font = new Font("맑은 고딕", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblTotalDiscount.ForeColor = Color.Red;
            lblTotalDiscount.Location = new Point(458, 12);
            lblTotalDiscount.Name = "lblTotalDiscount";
            lblTotalDiscount.Size = new Size(38, 20);
            lblTotalDiscount.TabIndex = 0;
            lblTotalDiscount.Text = "0 원";
            // 
            // lblTotalOrders
            // 
            lblTotalOrders.AutoSize = true;
            lblTotalOrders.Font = new Font("맑은 고딕", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblTotalOrders.ForeColor = Color.DodgerBlue;
            lblTotalOrders.Location = new Point(297, 12);
            lblTotalOrders.Name = "lblTotalOrders";
            lblTotalOrders.Size = new Size(38, 20);
            lblTotalOrders.TabIndex = 0;
            lblTotalOrders.Text = "0 건";
            // 
            // btnExportPdf
            // 
            btnExportPdf.Location = new Point(708, 6);
            btnExportPdf.Name = "btnExportPdf";
            btnExportPdf.Size = new Size(118, 34);
            btnExportPdf.TabIndex = 2;
            btnExportPdf.Text = "PDF 저장";
            btnExportPdf.UseVisualStyleBackColor = true;
            btnExportPdf.Click += btnExportPdf_Click;
            // 
            // lblTotalSales
            // 
            lblTotalSales.AutoSize = true;
            lblTotalSales.Font = new Font("맑은 고딕", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblTotalSales.ForeColor = Color.DodgerBlue;
            lblTotalSales.Location = new Point(103, 12);
            lblTotalSales.Name = "lblTotalSales";
            lblTotalSales.Size = new Size(38, 20);
            lblTotalSales.TabIndex = 0;
            lblTotalSales.Text = "0 원";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("맑은 고딕", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label4.Location = new Point(359, 12);
            label4.Name = "label4";
            label4.Size = new Size(103, 20);
            label4.TabIndex = 0;
            label4.Text = "할인된 금액 : ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(227, 12);
            label3.Name = "label3";
            label3.Size = new Size(73, 20);
            label3.TabIndex = 0;
            label3.Text = "총 건수 : ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(19, 12);
            label2.Name = "label2";
            label2.Size = new Size(88, 20);
            label2.TabIndex = 0;
            label2.Text = "총 매출액 : ";
            // 
            // pnlChartArea
            // 
            pnlChartArea.Dock = DockStyle.Top;
            pnlChartArea.Location = new Point(0, 45);
            pnlChartArea.Name = "pnlChartArea";
            pnlChartArea.Size = new Size(848, 220);
            pnlChartArea.TabIndex = 2;
            // 
            // dgvSalesReport
            // 
            dgvSalesReport.AllowUserToAddRows = false;
            dgvSalesReport.AllowUserToResizeColumns = false;
            dgvSalesReport.AllowUserToResizeRows = false;
            dgvSalesReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSalesReport.Dock = DockStyle.Fill;
            dgvSalesReport.Location = new Point(0, 265);
            dgvSalesReport.Name = "dgvSalesReport";
            dgvSalesReport.ReadOnly = true;
            dgvSalesReport.RowHeadersVisible = false;
            dgvSalesReport.RowTemplate.Height = 25;
            dgvSalesReport.Size = new Size(848, 140);
            dgvSalesReport.TabIndex = 3;
            // 
            // UcSalesReport
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dgvSalesReport);
            Controls.Add(pnlChartArea);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            Name = "UcSalesReport";
            Size = new Size(848, 450);
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSalesReport).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlTop;
        private Button btnExportCsv;
        private Label label1;
        private DateTimePicker dtpEndDate;
        private DateTimePicker dtpStartDate;
        private ComboBox cmbPeriodUnit;
        private Panel pnlBottom;
        private Label label2;
        private Label lblTotalOrders;
        private Label lblTotalSales;
        private Label label3;
        private Button btnExportPdf;
        private Panel pnlChartArea;
        private DataGridView dgvSalesReport;
        private Button btnSearch;
        private Label lblTotalDiscount;
        private Label label4;
    }
}