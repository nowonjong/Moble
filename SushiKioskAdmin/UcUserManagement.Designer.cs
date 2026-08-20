namespace SushiKioskAdmin.Views
{
    partial class UcUserManagement
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
            components = new System.ComponentModel.Container();
            pnlSearch = new Panel();
            label3 = new Label();
            btnDeleteUser = new Button();
            btnReset = new Button();
            btnSearch = new Button();
            txtSearch = new TextBox();
            panel1 = new Panel();
            btnUpdateUser = new Button();
            txtInputAddress = new TextBox();
            txtInputPhone = new TextBox();
            txtInputName = new TextBox();
            splitter1 = new Splitter();
            lblPoint = new Label();
            label6 = new Label();
            label4 = new Label();
            label2 = new Label();
            label1 = new Label();
            dgvUserList = new DataGridView();
            refreshTimer = new System.Windows.Forms.Timer(components);
            pnlSearch.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUserList).BeginInit();
            SuspendLayout();
            // 
            // pnlSearch
            // 
            pnlSearch.Controls.Add(label3);
            pnlSearch.Controls.Add(btnDeleteUser);
            pnlSearch.Controls.Add(btnReset);
            pnlSearch.Controls.Add(btnSearch);
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Location = new Point(0, 0);
            pnlSearch.Name = "pnlSearch";
            pnlSearch.Size = new Size(848, 50);
            pnlSearch.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(11, 14);
            label3.Name = "label3";
            label3.Size = new Size(128, 20);
            label3.TabIndex = 2;
            label3.Text = "회원명 / 연락처 : ";
            // 
            // btnDeleteUser
            // 
            btnDeleteUser.Location = new Point(739, 9);
            btnDeleteUser.Name = "btnDeleteUser";
            btnDeleteUser.Size = new Size(91, 32);
            btnDeleteUser.TabIndex = 1;
            btnDeleteUser.Text = "회원 삭제";
            btnDeleteUser.UseVisualStyleBackColor = true;
            btnDeleteUser.Click += btnDeleteUser_Click;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(632, 9);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(91, 32);
            btnReset.TabIndex = 1;
            btnReset.Text = "전체 보기";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(525, 9);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(91, 32);
            btnSearch.TabIndex = 1;
            btnSearch.Text = "검색";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(145, 12);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(363, 23);
            txtSearch.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnUpdateUser);
            panel1.Controls.Add(txtInputAddress);
            panel1.Controls.Add(txtInputPhone);
            panel1.Controls.Add(txtInputName);
            panel1.Controls.Add(splitter1);
            panel1.Controls.Add(lblPoint);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 50);
            panel1.Name = "panel1";
            panel1.Size = new Size(280, 400);
            panel1.TabIndex = 1;
            // 
            // btnUpdateUser
            // 
            btnUpdateUser.Location = new Point(146, 197);
            btnUpdateUser.Name = "btnUpdateUser";
            btnUpdateUser.Size = new Size(105, 37);
            btnUpdateUser.TabIndex = 3;
            btnUpdateUser.Text = "회원 정보 수정";
            btnUpdateUser.UseVisualStyleBackColor = true;
            btnUpdateUser.Click += btnUpdateUser_Click;
            // 
            // txtInputAddress
            // 
            txtInputAddress.ImeMode = ImeMode.Hangul;
            txtInputAddress.Location = new Point(77, 102);
            txtInputAddress.Name = "txtInputAddress";
            txtInputAddress.Size = new Size(174, 23);
            txtInputAddress.TabIndex = 2;
            // 
            // txtInputPhone
            // 
            txtInputPhone.Location = new Point(98, 64);
            txtInputPhone.Name = "txtInputPhone";
            txtInputPhone.Size = new Size(153, 23);
            txtInputPhone.TabIndex = 2;
            // 
            // txtInputName
            // 
            txtInputName.Location = new Point(98, 26);
            txtInputName.Name = "txtInputName";
            txtInputName.Size = new Size(153, 23);
            txtInputName.TabIndex = 2;
            // 
            // splitter1
            // 
            splitter1.BackColor = SystemColors.WindowFrame;
            splitter1.BorderStyle = BorderStyle.FixedSingle;
            splitter1.Dock = DockStyle.Top;
            splitter1.Location = new Point(0, 0);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(280, 3);
            splitter1.TabIndex = 1;
            splitter1.TabStop = false;
            // 
            // lblPoint
            // 
            lblPoint.AutoSize = true;
            lblPoint.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblPoint.Location = new Point(145, 140);
            lblPoint.Name = "lblPoint";
            lblPoint.Size = new Size(17, 21);
            lblPoint.TabIndex = 0;
            lblPoint.Text = "-";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label6.Location = new Point(24, 140);
            label6.Name = "label6";
            label6.Size = new Size(112, 21);
            label6.TabIndex = 0;
            label6.Text = "현재 포인트 : ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(24, 102);
            label4.Name = "label4";
            label4.Size = new Size(58, 21);
            label4.TabIndex = 0;
            label4.Text = "주소 : ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(24, 64);
            label2.Name = "label2";
            label2.Size = new Size(74, 21);
            label2.TabIndex = 0;
            label2.Text = "연락처 : ";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(24, 26);
            label1.Name = "label1";
            label1.Size = new Size(74, 21);
            label1.TabIndex = 0;
            label1.Text = "회원명 : ";
            // 
            // dgvUserList
            // 
            dgvUserList.AllowUserToAddRows = false;
            dgvUserList.AllowUserToResizeColumns = false;
            dgvUserList.AllowUserToResizeRows = false;
            dgvUserList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUserList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUserList.Dock = DockStyle.Fill;
            dgvUserList.Location = new Point(280, 50);
            dgvUserList.Name = "dgvUserList";
            dgvUserList.ReadOnly = true;
            dgvUserList.RowHeadersVisible = false;
            dgvUserList.RowHeadersWidth = 30;
            dgvUserList.RowTemplate.Height = 25;
            dgvUserList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUserList.Size = new Size(568, 400);
            dgvUserList.TabIndex = 2;
            dgvUserList.SelectionChanged += dgvUserList_SelectionChanged;
            // 
            // refreshTimer
            // 
            refreshTimer.Enabled = true;
            refreshTimer.Interval = 1000;
            // 
            // UcUserManagement
            // 
            AutoScaleMode = AutoScaleMode.None;
            Controls.Add(dgvUserList);
            Controls.Add(panel1);
            Controls.Add(pnlSearch);
            Name = "UcUserManagement";
            Size = new Size(848, 450);
            Load += UcUserManagement_Load;
            pnlSearch.ResumeLayout(false);
            pnlSearch.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUserList).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlSearch;
        private Button btnSearch;
        private TextBox txtSearch;
        private Panel panel1;
        private Label lblPoint;
        private Label lblGrade;
        private Label lblPhone;
        private Label lblName;
        private Label label6;
        private Label label4;
        private Label label2;
        private Label label1;
        private DataGridView dgvUserList;
        private Button btnReset;
        private Label label3;
        private Splitter splitter1;
        private TextBox textBox1;
        private Button btnDeleteUser;
        private Button btnUpdateUser;
        private TextBox txtInputAddress;
        private TextBox txtInputPhone;
        private TextBox txtInputName;
        private System.Windows.Forms.Timer refreshTimer;
    }
}