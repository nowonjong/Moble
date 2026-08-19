namespace SushiKioskAdmin
{
    partial class MainAdminForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlTop = new Panel();
            btnExit = new Button();
            lblNotice = new Label();
            lblTitle = new Label();
            pnlSidebar = new Panel();
            btnNavReport = new Button();
            btnNavStock = new Button();
            btnNavUser = new Button();
            btnNavHistory = new Button();
            btnNavMenu = new Button();
            btnNavTable = new Button();
            btnNavOrder = new Button();
            pnlMainContainer = new Panel();
            noticeBlinkTimer = new System.Windows.Forms.Timer(components);
            pnlTop.SuspendLayout();
            pnlSidebar.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.FromArgb(45, 45, 48);
            pnlTop.Controls.Add(btnExit);
            pnlTop.Controls.Add(lblNotice);
            pnlTop.Controls.Add(lblTitle);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(1008, 50);
            pnlTop.TabIndex = 0;
            // 
            // btnExit
            // 
            btnExit.BackColor = SystemColors.AppWorkspace;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Location = new Point(887, 12);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(99, 23);
            btnExit.TabIndex = 4;
            btnExit.Text = "프로그램 종료";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // lblNotice
            // 
            lblNotice.AutoSize = true;
            lblNotice.Font = new Font("맑은 고딕", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblNotice.ForeColor = Color.Yellow;
            lblNotice.Location = new Point(475, 16);
            lblNotice.Name = "lblNotice";
            lblNotice.Size = new Size(150, 17);
            lblNotice.TabIndex = 3;
            lblNotice.Text = "신규 주문 [0건] 대기 중";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(21, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(197, 25);
            lblTitle.TabIndex = 3;
            lblTitle.Text = "초밥집 관리자 시스템";
            // 
            // pnlSidebar
            // 
            pnlSidebar.Controls.Add(btnNavReport);
            pnlSidebar.Controls.Add(btnNavStock);
            pnlSidebar.Controls.Add(btnNavUser);
            pnlSidebar.Controls.Add(btnNavHistory);
            pnlSidebar.Controls.Add(btnNavMenu);
            pnlSidebar.Controls.Add(btnNavTable);
            pnlSidebar.Controls.Add(btnNavOrder);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 50);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(160, 679);
            pnlSidebar.TabIndex = 1;
            // 
            // btnNavReport
            // 
            btnNavReport.FlatAppearance.BorderSize = 0;
            btnNavReport.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 204);
            btnNavReport.FlatStyle = FlatStyle.Flat;
            btnNavReport.Location = new Point(3, 426);
            btnNavReport.Name = "btnNavReport";
            btnNavReport.Size = new Size(154, 58);
            btnNavReport.TabIndex = 0;
            btnNavReport.Text = "7. 매출 리포트";
            btnNavReport.UseVisualStyleBackColor = true;
            btnNavReport.Click += btnNavReport_Click;
            // 
            // btnNavStock
            // 
            btnNavStock.FlatAppearance.BorderSize = 0;
            btnNavStock.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 204);
            btnNavStock.FlatStyle = FlatStyle.Flat;
            btnNavStock.Location = new Point(3, 362);
            btnNavStock.Name = "btnNavStock";
            btnNavStock.Size = new Size(154, 58);
            btnNavStock.TabIndex = 0;
            btnNavStock.Text = "6. 재고 관리";
            btnNavStock.UseVisualStyleBackColor = true;
            btnNavStock.Click += btnNavStock_Click;
            // 
            // btnNavUser
            // 
            btnNavUser.FlatAppearance.BorderSize = 0;
            btnNavUser.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 204);
            btnNavUser.FlatStyle = FlatStyle.Flat;
            btnNavUser.Location = new Point(3, 298);
            btnNavUser.Name = "btnNavUser";
            btnNavUser.Size = new Size(154, 58);
            btnNavUser.TabIndex = 0;
            btnNavUser.Text = "5. 회원 관리";
            btnNavUser.UseVisualStyleBackColor = true;
            btnNavUser.Click += btnNavUser_Click;
            // 
            // btnNavHistory
            // 
            btnNavHistory.FlatAppearance.BorderSize = 0;
            btnNavHistory.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 204);
            btnNavHistory.FlatStyle = FlatStyle.Flat;
            btnNavHistory.Location = new Point(3, 234);
            btnNavHistory.Name = "btnNavHistory";
            btnNavHistory.Size = new Size(154, 58);
            btnNavHistory.TabIndex = 0;
            btnNavHistory.Text = "4. 과거 주문 내역 및\r\n영수증 조회\r\n";
            btnNavHistory.UseVisualStyleBackColor = true;
            btnNavHistory.Click += btnNavHistory_Click;
            // 
            // btnNavMenu
            // 
            btnNavMenu.FlatAppearance.BorderSize = 0;
            btnNavMenu.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 204);
            btnNavMenu.FlatStyle = FlatStyle.Flat;
            btnNavMenu.Location = new Point(3, 170);
            btnNavMenu.Name = "btnNavMenu";
            btnNavMenu.Size = new Size(154, 58);
            btnNavMenu.TabIndex = 0;
            btnNavMenu.Text = "3. 메뉴 관리";
            btnNavMenu.UseVisualStyleBackColor = true;
            btnNavMenu.Click += btnNavMenu_Click;
            // 
            // btnNavTable
            // 
            btnNavTable.FlatAppearance.BorderSize = 0;
            btnNavTable.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 204);
            btnNavTable.FlatStyle = FlatStyle.Flat;
            btnNavTable.Location = new Point(3, 106);
            btnNavTable.Name = "btnNavTable";
            btnNavTable.Size = new Size(154, 58);
            btnNavTable.TabIndex = 0;
            btnNavTable.Text = "2. 테이블 현황";
            btnNavTable.UseVisualStyleBackColor = true;
            btnNavTable.Click += btnNavTable_Click;
            // 
            // btnNavOrder
            // 
            btnNavOrder.FlatAppearance.BorderSize = 0;
            btnNavOrder.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 204);
            btnNavOrder.FlatStyle = FlatStyle.Flat;
            btnNavOrder.Location = new Point(3, 42);
            btnNavOrder.Name = "btnNavOrder";
            btnNavOrder.Size = new Size(154, 58);
            btnNavOrder.TabIndex = 0;
            btnNavOrder.Text = "1. 실시간 주문";
            btnNavOrder.UseVisualStyleBackColor = true;
            btnNavOrder.Click += btnNavOrder_Click;
            // 
            // pnlMainContainer
            // 
            pnlMainContainer.Dock = DockStyle.Fill;
            pnlMainContainer.Location = new Point(160, 50);
            pnlMainContainer.Name = "pnlMainContainer";
            pnlMainContainer.Size = new Size(848, 679);
            pnlMainContainer.TabIndex = 2;
            // 
            // noticeBlinkTimer
            // 
            noticeBlinkTimer.Interval = 500;
            noticeBlinkTimer.Tick += noticeBlinkTimer_Tick;
            // 
            // MainAdminForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 729);
            Controls.Add(pnlMainContainer);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlTop);
            Name = "MainAdminForm";
            Text = "초밥집 관리자 시스템";
            Load += MainAdminForm_Load;
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlSidebar.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlTop;
        private Panel pnlSidebar;
        private Button btnNavReport;
        private Button btnNavStock;
        private Button btnNavUser;
        private Button btnNavHistory;
        private Button btnNavMenu;
        private Button btnNavTable;
        private Button btnNavOrder;
        private Panel pnlMainContainer;
        private Label lblTitle;
        private Button btnExit;
        private Label lblNotice;
        private System.Windows.Forms.Timer noticeBlinkTimer;
    }
}
