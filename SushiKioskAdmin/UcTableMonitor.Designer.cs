namespace SushiKioskAdmin.Views
{
    partial class UcTableMonitor
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
            pnlSummary = new Panel();
            lblEmptyTables = new Label();
            lblOccupiedTables = new Label();
            lblTotalTables = new Label();
            flpTables = new FlowLayoutPanel();
            refreshTimer = new System.Windows.Forms.Timer(components);
            pnlSummary.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSummary
            // 
            pnlSummary.Controls.Add(lblEmptyTables);
            pnlSummary.Controls.Add(lblOccupiedTables);
            pnlSummary.Controls.Add(lblTotalTables);
            pnlSummary.Dock = DockStyle.Top;
            pnlSummary.Location = new Point(0, 0);
            pnlSummary.Name = "pnlSummary";
            pnlSummary.Size = new Size(848, 60);
            pnlSummary.TabIndex = 0;
            // 
            // lblEmptyTables
            // 
            lblEmptyTables.AutoSize = true;
            lblEmptyTables.Font = new Font("맑은 고딕", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblEmptyTables.Location = new Point(320, 22);
            lblEmptyTables.Name = "lblEmptyTables";
            lblEmptyTables.Size = new Size(93, 17);
            lblEmptyTables.TabIndex = 0;
            lblEmptyTables.Text = "빈 테이블: 7개";
            // 
            // lblOccupiedTables
            // 
            lblOccupiedTables.AutoSize = true;
            lblOccupiedTables.Font = new Font("맑은 고딕", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblOccupiedTables.Location = new Point(187, 22);
            lblOccupiedTables.Name = "lblOccupiedTables";
            lblOccupiedTables.Size = new Size(80, 17);
            lblOccupiedTables.TabIndex = 0;
            lblOccupiedTables.Text = "사용 중: 3개";
            // 
            // lblTotalTables
            // 
            lblTotalTables.AutoSize = true;
            lblTotalTables.Font = new Font("맑은 고딕", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblTotalTables.Location = new Point(35, 22);
            lblTotalTables.Name = "lblTotalTables";
            lblTotalTables.Size = new Size(113, 17);
            lblTotalTables.TabIndex = 0;
            lblTotalTables.Text = "전체 테이블: 10개";
            // 
            // flpTables
            // 
            flpTables.AutoScroll = true;
            flpTables.Dock = DockStyle.Fill;
            flpTables.Location = new Point(0, 60);
            flpTables.Name = "flpTables";
            flpTables.Size = new Size(848, 390);
            flpTables.TabIndex = 1;
            // 
            // refreshTimer
            // 
            refreshTimer.Enabled = true;
            refreshTimer.Interval = 1000;
            refreshTimer.Tick += refreshTimer_Tick;
            // 
            // UcTableMonitor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(flpTables);
            Controls.Add(pnlSummary);
            Name = "UcTableMonitor";
            Size = new Size(848, 450);
            pnlSummary.ResumeLayout(false);
            pnlSummary.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlSummary;
        private Label lblTotalTables;
        private Label lblEmptyTables;
        private Label lblOccupiedTables;
        private FlowLayoutPanel flpTables;
        private System.Windows.Forms.Timer refreshTimer;
    }
}