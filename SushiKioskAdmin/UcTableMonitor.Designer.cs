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
            label2 = new Label();
            lblOccupiedTables = new Label();
            lblTotalTables = new Label();
            label1 = new Label();
            flpTables = new FlowLayoutPanel();
            refreshTimer = new System.Windows.Forms.Timer(components);
            label3 = new Label();
            pnlSummary.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSummary
            // 
            pnlSummary.Controls.Add(lblEmptyTables);
            pnlSummary.Controls.Add(lblOccupiedTables);
            pnlSummary.Controls.Add(lblTotalTables);
            pnlSummary.Controls.Add(label1);
            pnlSummary.Controls.Add(label2);
            pnlSummary.Controls.Add(label3);
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
            lblEmptyTables.Location = new Point(394, 22);
            lblEmptyTables.Name = "lblEmptyTables";
            lblEmptyTables.Size = new Size(28, 17);
            lblEmptyTables.TabIndex = 0;
            lblEmptyTables.Text = "7개";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(187, 22);
            label2.Name = "label2";
            label2.Size = new Size(55, 17);
            label2.TabIndex = 0;
            label2.Text = "사용 중:";
            // 
            // lblOccupiedTables
            // 
            lblOccupiedTables.AutoSize = true;
            lblOccupiedTables.Font = new Font("맑은 고딕", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblOccupiedTables.Location = new Point(248, 22);
            lblOccupiedTables.Name = "lblOccupiedTables";
            lblOccupiedTables.Size = new Size(28, 17);
            lblOccupiedTables.TabIndex = 0;
            lblOccupiedTables.Text = "3개";
            // 
            // lblTotalTables
            // 
            lblTotalTables.AutoSize = true;
            lblTotalTables.Font = new Font("맑은 고딕", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblTotalTables.Location = new Point(122, 22);
            lblTotalTables.Name = "lblTotalTables";
            lblTotalTables.Size = new Size(35, 17);
            lblTotalTables.TabIndex = 0;
            lblTotalTables.Text = "10개";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(35, 22);
            label1.Name = "label1";
            label1.Size = new Size(81, 17);
            label1.TabIndex = 0;
            label1.Text = "전체 테이블:";
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
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(320, 22);
            label3.Name = "label3";
            label3.Size = new Size(68, 17);
            label3.TabIndex = 0;
            label3.Text = "빈 테이블:";
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
        private Label label2;
        private Label label1;
        private Label label3;
    }
}