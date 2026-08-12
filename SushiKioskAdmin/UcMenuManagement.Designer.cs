namespace SushiKioskAdmin.Views
{
    partial class UcMenuManagement
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            pnlLeft = new Panel();
            btnUpdate = new Button();
            btnAdd = new Button();
            label2 = new Label();
            label1 = new Label();
            txtMenuName = new TextBox();
            cmbCategory = new ComboBox();
            pnlRight = new Panel();
            dgvMenuList = new DataGridView();
            pnlStatusControl = new Panel();
            btnSalesResume = new Button();
            btnSoldOut = new Button();
            pnlLeft.SuspendLayout();
            pnlRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMenuList).BeginInit();
            pnlStatusControl.SuspendLayout();
            SuspendLayout();
            // 
            // pnlLeft
            // 
            pnlLeft.Controls.Add(btnUpdate);
            pnlLeft.Controls.Add(btnAdd);
            pnlLeft.Controls.Add(label2);
            pnlLeft.Controls.Add(label1);
            pnlLeft.Controls.Add(txtMenuName);
            pnlLeft.Controls.Add(cmbCategory);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 0);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Size = new Size(280, 450);
            pnlLeft.TabIndex = 0;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(154, 161);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(105, 44);
            btnUpdate.TabIndex = 4;
            btnUpdate.Text = "선택 메뉴 수정";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += BtnUpdate_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(27, 161);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(105, 44);
            btnAdd.TabIndex = 4;
            btnAdd.Text = "신규 메뉴 등록";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += BtnAdd_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(22, 96);
            label2.Name = "label2";
            label2.Size = new Size(54, 20);
            label2.TabIndex = 2;
            label2.Text = "메뉴명";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(22, 55);
            label1.Name = "label1";
            label1.Size = new Size(69, 20);
            label1.TabIndex = 2;
            label1.Text = "카테고리";
            // 
            // txtMenuName
            // 
            txtMenuName.Location = new Point(109, 93);
            txtMenuName.Name = "txtMenuName";
            txtMenuName.Size = new Size(143, 23);
            txtMenuName.TabIndex = 1;
            // 
            // cmbCategory
            // 
            cmbCategory.FormattingEnabled = true;
            cmbCategory.Items.AddRange(new object[] { "🔴 1,000원 메뉴", "🔴 1,500원 메뉴", "🔴 2,000원 사이드/디저트", "🔴 3,000원 메뉴", "🔴 5,000원 면류", "🔴 6,000원 프리미엄", "\U0001f964 1,000원 음료" });
            cmbCategory.Location = new Point(109, 52);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new Size(143, 23);
            cmbCategory.TabIndex = 0;
            // 
            // pnlRight
            // 
            pnlRight.Controls.Add(dgvMenuList);
            pnlRight.Controls.Add(pnlStatusControl);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Location = new Point(280, 0);
            pnlRight.Name = "pnlRight";
            pnlRight.Size = new Size(568, 450);
            pnlRight.TabIndex = 1;
            // 
            // dgvMenuList
            // 
            dgvMenuList.AllowUserToResizeColumns = false;
            dgvMenuList.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("맑은 고딕", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvMenuList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvMenuList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMenuList.Dock = DockStyle.Fill;
            dgvMenuList.Location = new Point(0, 45);
            dgvMenuList.MultiSelect = false;
            dgvMenuList.Name = "dgvMenuList";
            dgvMenuList.ReadOnly = true;
            dgvMenuList.RowHeadersVisible = false;
            dgvMenuList.RowTemplate.Height = 25;
            dgvMenuList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMenuList.Size = new Size(568, 405);
            dgvMenuList.TabIndex = 1;
            dgvMenuList.SelectionChanged += DgvMenuList_SelectionChanged;
            // 
            // pnlStatusControl
            // 
            pnlStatusControl.Controls.Add(btnSalesResume);
            pnlStatusControl.Controls.Add(btnSoldOut);
            pnlStatusControl.Dock = DockStyle.Top;
            pnlStatusControl.Location = new Point(0, 0);
            pnlStatusControl.Name = "pnlStatusControl";
            pnlStatusControl.Size = new Size(568, 45);
            pnlStatusControl.TabIndex = 0;
            // 
            // btnSalesResume
            // 
            btnSalesResume.Location = new Point(461, 5);
            btnSalesResume.Name = "btnSalesResume";
            btnSalesResume.Size = new Size(91, 33);
            btnSalesResume.TabIndex = 0;
            btnSalesResume.Text = "판매 재개";
            btnSalesResume.UseVisualStyleBackColor = true;
            btnSalesResume.Click += btnSalesResume_Click;
            // 
            // btnSoldOut
            // 
            btnSoldOut.Location = new Point(347, 5);
            btnSoldOut.Name = "btnSoldOut";
            btnSoldOut.Size = new Size(91, 33);
            btnSoldOut.TabIndex = 0;
            btnSoldOut.Text = "메뉴 품절";
            btnSoldOut.UseVisualStyleBackColor = true;
            btnSoldOut.Click += btnSoldOut_Click;
            // 
            // UcMenuManagement
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);
            Name = "UcMenuManagement";
            Size = new Size(848, 450);
            Load += UcMenuManagement_Load;
            pnlLeft.ResumeLayout(false);
            pnlLeft.PerformLayout();
            pnlRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvMenuList).EndInit();
            pnlStatusControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlLeft;
        private ComboBox cmbCategory;
        private Label label1;
        private TextBox txtMenuName;
        private Label label2;
        private Button btnAdd;
        private Button btnUpdate;
        private Panel pnlRight;
        private DataGridView dgvMenuList;
        private Panel pnlStatusControl;
        private Button btnSalesResume;
        private Button btnSoldOut;
    }
}