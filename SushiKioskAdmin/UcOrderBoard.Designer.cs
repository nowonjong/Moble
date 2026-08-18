namespace SushiKioskAdmin.Views
{
    partial class UcOrderBoard
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
            pnlFilter = new Panel();
            rdoWaiting = new RadioButton();
            rdoKiosk = new RadioButton();
            rdoApp = new RadioButton();
            rdoAll = new RadioButton();
            pnlControl = new Panel();
            btnPickUpDone = new Button();
            btnCookDone = new Button();
            btnReject = new Button();
            btnAccept = new Button();
            dgvOrders = new DataGridView();
            refreshTimer = new System.Windows.Forms.Timer(components);
            pnlFilter.SuspendLayout();
            pnlControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvOrders).BeginInit();
            SuspendLayout();
            // 
            // pnlFilter
            // 
            pnlFilter.Controls.Add(rdoWaiting);
            pnlFilter.Controls.Add(rdoKiosk);
            pnlFilter.Controls.Add(rdoApp);
            pnlFilter.Controls.Add(rdoAll);
            pnlFilter.Dock = DockStyle.Top;
            pnlFilter.Location = new Point(0, 0);
            pnlFilter.Name = "pnlFilter";
            pnlFilter.Size = new Size(848, 50);
            pnlFilter.TabIndex = 0;
            // 
            // rdoWaiting
            // 
            rdoWaiting.AutoSize = true;
            rdoWaiting.Location = new Point(424, 16);
            rdoWaiting.Name = "rdoWaiting";
            rdoWaiting.Size = new Size(77, 19);
            rdoWaiting.TabIndex = 0;
            rdoWaiting.Text = "접수 대기";
            rdoWaiting.UseVisualStyleBackColor = true;
            rdoWaiting.CheckedChanged += FilterOrders_CheckedChanged;
            // 
            // rdoKiosk
            // 
            rdoKiosk.AutoSize = true;
            rdoKiosk.Location = new Point(166, 16);
            rdoKiosk.Name = "rdoKiosk";
            rdoKiosk.Size = new Size(85, 19);
            rdoKiosk.TabIndex = 0;
            rdoKiosk.Text = "키오스크만";
            rdoKiosk.UseVisualStyleBackColor = true;
            rdoKiosk.CheckedChanged += FilterOrders_CheckedChanged;
            // 
            // rdoApp
            // 
            rdoApp.AutoSize = true;
            rdoApp.Location = new Point(299, 16);
            rdoApp.Name = "rdoApp";
            rdoApp.Size = new Size(77, 19);
            rdoApp.TabIndex = 0;
            rdoApp.Text = "앱 주문만";
            rdoApp.UseVisualStyleBackColor = true;
            rdoApp.CheckedChanged += FilterOrders_CheckedChanged;
            // 
            // rdoAll
            // 
            rdoAll.AutoSize = true;
            rdoAll.Checked = true;
            rdoAll.Location = new Point(41, 16);
            rdoAll.Name = "rdoAll";
            rdoAll.Size = new Size(77, 19);
            rdoAll.TabIndex = 0;
            rdoAll.TabStop = true;
            rdoAll.Text = "전체 보기";
            rdoAll.UseVisualStyleBackColor = true;
            rdoAll.CheckedChanged += FilterOrders_CheckedChanged;
            // 
            // pnlControl
            // 
            pnlControl.Controls.Add(btnPickUpDone);
            pnlControl.Controls.Add(btnCookDone);
            pnlControl.Controls.Add(btnReject);
            pnlControl.Controls.Add(btnAccept);
            pnlControl.Dock = DockStyle.Bottom;
            pnlControl.Location = new Point(0, 390);
            pnlControl.Name = "pnlControl";
            pnlControl.Size = new Size(848, 60);
            pnlControl.TabIndex = 2;
            // 
            // btnPickUpDone
            // 
            btnPickUpDone.Location = new Point(747, 11);
            btnPickUpDone.Name = "btnPickUpDone";
            btnPickUpDone.Size = new Size(91, 38);
            btnPickUpDone.TabIndex = 3;
            btnPickUpDone.Text = "픽업 완료";
            btnPickUpDone.UseVisualStyleBackColor = true;
            btnPickUpDone.Click += btnPickUpDone_Click;
            // 
            // btnCookDone
            // 
            btnCookDone.Location = new Point(643, 11);
            btnCookDone.Name = "btnCookDone";
            btnCookDone.Size = new Size(91, 38);
            btnCookDone.TabIndex = 3;
            btnCookDone.Text = "조리 완료";
            btnCookDone.UseVisualStyleBackColor = true;
            btnCookDone.Click += btnCookDone_Click;
            // 
            // btnReject
            // 
            btnReject.Location = new Point(539, 11);
            btnReject.Name = "btnReject";
            btnReject.Size = new Size(91, 38);
            btnReject.TabIndex = 3;
            btnReject.Text = "주문 거절";
            btnReject.UseVisualStyleBackColor = true;
            btnReject.Click += btnReject_Click;
            // 
            // btnAccept
            // 
            btnAccept.Location = new Point(435, 11);
            btnAccept.Name = "btnAccept";
            btnAccept.Size = new Size(91, 38);
            btnAccept.TabIndex = 3;
            btnAccept.Text = "주문 수락";
            btnAccept.UseVisualStyleBackColor = true;
            btnAccept.Click += btnAccept_Click;
            // 
            // dgvOrders
            // 
            dgvOrders.AllowUserToAddRows = false;
            dgvOrders.AllowUserToResizeColumns = false;
            dgvOrders.AllowUserToResizeRows = false;
            dgvOrders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOrders.Dock = DockStyle.Fill;
            dgvOrders.Location = new Point(0, 50);
            dgvOrders.MultiSelect = false;
            dgvOrders.Name = "dgvOrders";
            dgvOrders.ReadOnly = true;
            dgvOrders.RowHeadersVisible = false;
            dgvOrders.RowTemplate.Height = 25;
            dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrders.Size = new Size(848, 340);
            dgvOrders.TabIndex = 3;
            // 
            // refreshTimer
            // 
            refreshTimer.Enabled = true;
            refreshTimer.Interval = 1000;
            refreshTimer.Tick += refreshTimer_Tick;
            // 
            // UcOrderBoard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(dgvOrders);
            Controls.Add(pnlControl);
            Controls.Add(pnlFilter);
            Name = "UcOrderBoard";
            Size = new Size(848, 450);
            Load += UcOrderBoard_Load;
            pnlFilter.ResumeLayout(false);
            pnlFilter.PerformLayout();
            pnlControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvOrders).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlFilter;
        private RadioButton rdoWaiting;
        private RadioButton rdoKiosk;
        private RadioButton rdoApp;
        private RadioButton rdoAll;
        private Panel pnlControl;
        private Button btnCookDone;
        private Button btnReject;
        private Button btnAccept;
        private DataGridView dgvOrders;
        private Button btnPickUpDone;
        private System.Windows.Forms.Timer refreshTimer;
    }
}