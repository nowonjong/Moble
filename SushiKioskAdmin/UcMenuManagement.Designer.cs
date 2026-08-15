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
            picMenuImage = new PictureBox();
            btnUpdate = new Button();
            btnAdd = new Button();
            label4 = new Label();
            label3 = new Label();
            txtJapanese = new TextBox();
            label2 = new Label();
            txtEnglish = new TextBox();
            label1 = new Label();
            txtMenuName = new TextBox();
            cmbCategory = new ComboBox();
            pnlRight = new Panel();
            dgvMenuList = new DataGridView();
            pnlStatusControl = new Panel();
            btnSalesResume = new Button();
            btnSoldOut = new Button();
            pnlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picMenuImage).BeginInit();
            pnlRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMenuList).BeginInit();
            pnlStatusControl.SuspendLayout();
            SuspendLayout();
            // 
            // pnlLeft
            // 
            pnlLeft.Controls.Add(picMenuImage);
            pnlLeft.Controls.Add(btnUpdate);
            pnlLeft.Controls.Add(btnAdd);
            pnlLeft.Controls.Add(label4);
            pnlLeft.Controls.Add(label3);
            pnlLeft.Controls.Add(txtJapanese);
            pnlLeft.Controls.Add(label2);
            pnlLeft.Controls.Add(txtEnglish);
            pnlLeft.Controls.Add(label1);
            pnlLeft.Controls.Add(txtMenuName);
            pnlLeft.Controls.Add(cmbCategory);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 0);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Size = new Size(280, 500);
            pnlLeft.TabIndex = 0;
            // 
            // picMenuImage
            // 
            picMenuImage.BorderStyle = BorderStyle.FixedSingle;
            picMenuImage.Location = new Point(22, 196);
            picMenuImage.Name = "picMenuImage";
            picMenuImage.Size = new Size(230, 201);
            picMenuImage.TabIndex = 5;
            picMenuImage.TabStop = false;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(148, 426);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(105, 44);
            btnUpdate.TabIndex = 4;
            btnUpdate.Text = "선택 메뉴 수정";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += BtnUpdate_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(27, 426);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(105, 44);
            btnAdd.TabIndex = 4;
            btnAdd.Text = "신규 메뉴 등록";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += BtnAdd_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("맑은 고딕", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(22, 163);
            label4.Name = "label4";
            label4.Size = new Size(94, 17);
            label4.TabIndex = 2;
            label4.Text = "메뉴명(일본어)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(22, 129);
            label3.Name = "label3";
            label3.Size = new Size(81, 17);
            label3.TabIndex = 2;
            label3.Text = "메뉴명(영어)";
            // 
            // txtJapanese
            // 
            txtJapanese.Location = new Point(121, 160);
            txtJapanese.Name = "txtJapanese";
            txtJapanese.Size = new Size(131, 23);
            txtJapanese.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(22, 92);
            label2.Name = "label2";
            label2.Size = new Size(54, 20);
            label2.TabIndex = 2;
            label2.Text = "메뉴명";
            // 
            // txtEnglish
            // 
            txtEnglish.Location = new Point(121, 124);
            txtEnglish.Name = "txtEnglish";
            txtEnglish.Size = new Size(131, 23);
            txtEnglish.TabIndex = 1;
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
            txtMenuName.Location = new Point(121, 88);
            txtMenuName.Name = "txtMenuName";
            txtMenuName.Size = new Size(131, 23);
            txtMenuName.TabIndex = 1;
            // 
            // cmbCategory
            // 
            cmbCategory.FormattingEnabled = true;
            cmbCategory.Items.AddRange(new object[] { "🔴 1,000원 메뉴", "🔴 1,500원 메뉴", "🔴 2,000원 사이드/디저트", "🔴 3,000원 메뉴", "🔴 5,000원 면류", "🔴 6,000원 프리미엄", "\U0001f964 1,000원 음료" });
            cmbCategory.Location = new Point(121, 52);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new Size(131, 23);
            cmbCategory.TabIndex = 0;
            // 
            // pnlRight
            // 
            pnlRight.Controls.Add(dgvMenuList);
            pnlRight.Controls.Add(pnlStatusControl);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Location = new Point(280, 0);
            pnlRight.Name = "pnlRight";
            pnlRight.Size = new Size(568, 500);
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
            dgvMenuList.Size = new Size(568, 455);
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
            Size = new Size(848, 500);
            Load += UcMenuManagement_Load;
            pnlLeft.ResumeLayout(false);
            pnlLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picMenuImage).EndInit();
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
        private Label label4;
        private Label label3;
        private TextBox txtJapanese;
        private TextBox txtEnglish;
        private PictureBox picMenuImage;
    }
}