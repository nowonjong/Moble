namespace SushiKioskAdmin.Views
{
    partial class UcStockManagement
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
            pnlLeft = new Panel();
            label4 = new Label();
            btnOrder = new Button();
            numOrderQty = new NumericUpDown();
            lblStock = new Label();
            lblItem = new Label();
            label2 = new Label();
            label1 = new Label();
            pnlTop = new Panel();
            splitter1 = new Splitter();
            label3 = new Label();
            btnCancelOrder = new Button();
            dgvStockList = new DataGridView();
            pnlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numOrderQty).BeginInit();
            pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvStockList).BeginInit();
            SuspendLayout();
            // 
            // pnlLeft
            // 
            pnlLeft.Controls.Add(label4);
            pnlLeft.Controls.Add(btnOrder);
            pnlLeft.Controls.Add(numOrderQty);
            pnlLeft.Controls.Add(lblStock);
            pnlLeft.Controls.Add(lblItem);
            pnlLeft.Controls.Add(label2);
            pnlLeft.Controls.Add(label1);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 0);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Size = new Size(280, 450);
            pnlLeft.TabIndex = 0;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label4.Location = new Point(12, 10);
            label4.Name = "label4";
            label4.Size = new Size(88, 25);
            label4.TabIndex = 1;
            label4.Text = "주문하기";
            // 
            // btnOrder
            // 
            btnOrder.Location = new Point(148, 182);
            btnOrder.Name = "btnOrder";
            btnOrder.Size = new Size(109, 41);
            btnOrder.TabIndex = 3;
            btnOrder.Text = "주문 재고 요청";
            btnOrder.UseVisualStyleBackColor = true;
            btnOrder.Click += btnOrder_Click;
            // 
            // numOrderQty
            // 
            numOrderQty.Location = new Point(25, 138);
            numOrderQty.Name = "numOrderQty";
            numOrderQty.Size = new Size(120, 23);
            numOrderQty.TabIndex = 2;
            // 
            // lblStock
            // 
            lblStock.AutoSize = true;
            lblStock.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblStock.Location = new Point(123, 95);
            lblStock.Name = "lblStock";
            lblStock.Size = new Size(17, 21);
            lblStock.TabIndex = 1;
            lblStock.Text = "-";
            // 
            // lblItem
            // 
            lblItem.AutoSize = true;
            lblItem.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblItem.Location = new Point(123, 57);
            lblItem.Name = "lblItem";
            lblItem.Size = new Size(17, 21);
            lblItem.TabIndex = 1;
            lblItem.Text = "-";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(25, 95);
            label2.Name = "label2";
            label2.Size = new Size(96, 21);
            label2.TabIndex = 0;
            label2.Text = "현재 재고 : ";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(25, 57);
            label1.Name = "label1";
            label1.Size = new Size(74, 21);
            label1.TabIndex = 0;
            label1.Text = "품목명 : ";
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(splitter1);
            pnlTop.Controls.Add(label3);
            pnlTop.Controls.Add(btnCancelOrder);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(280, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(568, 40);
            pnlTop.TabIndex = 2;
            // 
            // splitter1
            // 
            splitter1.BackColor = SystemColors.ControlDarkDark;
            splitter1.BorderStyle = BorderStyle.FixedSingle;
            splitter1.Location = new Point(0, 0);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(3, 40);
            splitter1.TabIndex = 2;
            splitter1.TabStop = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(8, 8);
            label3.Name = "label3";
            label3.Size = new Size(140, 25);
            label3.TabIndex = 1;
            label3.Text = "재고 주문 내역";
            // 
            // btnCancelOrder
            // 
            btnCancelOrder.Location = new Point(471, 3);
            btnCancelOrder.Name = "btnCancelOrder";
            btnCancelOrder.Size = new Size(83, 34);
            btnCancelOrder.TabIndex = 0;
            btnCancelOrder.Text = "주문 취소";
            btnCancelOrder.UseVisualStyleBackColor = true;
            btnCancelOrder.Click += btnCancelOrder_Click;
            // 
            // dgvStockList
            // 
            dgvStockList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvStockList.Dock = DockStyle.Fill;
            dgvStockList.Location = new Point(280, 40);
            dgvStockList.Name = "dgvStockList";
            dgvStockList.RowTemplate.Height = 25;
            dgvStockList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStockList.Size = new Size(568, 410);
            dgvStockList.TabIndex = 3;
            dgvStockList.SelectionChanged += dgvStockList_SelectionChanged;
            // 
            // UcStockManagement
            // 
            AutoScaleMode = AutoScaleMode.None;
            Controls.Add(dgvStockList);
            Controls.Add(pnlTop);
            Controls.Add(pnlLeft);
            Name = "UcStockManagement";
            Size = new Size(848, 450);
            pnlLeft.ResumeLayout(false);
            pnlLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numOrderQty).EndInit();
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvStockList).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlLeft;
        private Label lblItem;
        private Label label1;
        private Label lblStock;
        private Label label2;
        private Button btnOrder;
        private NumericUpDown numOrderQty;
        private Panel pnlTop;
        private Button btnCancelOrder;
        private DataGridView dgvStockList;
        private Label label3;
        private Label label4;
        private Splitter splitter1;
    }
}