namespace sushi
{
    partial class FormPoint
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
            lbTotalPoint = new Label();
            flpPoints = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // lbTotalPoint
            // 
            lbTotalPoint.AutoSize = true;
            lbTotalPoint.Location = new Point(89, 9);
            lbTotalPoint.Name = "lbTotalPoint";
            lbTotalPoint.Size = new Size(66, 15);
            lbTotalPoint.TabIndex = 0;
            lbTotalPoint.Text = "총 포인트 :";
            // 
            // flpPoints
            // 
            flpPoints.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flpPoints.AutoScroll = true;
            flpPoints.FlowDirection = FlowDirection.TopDown;
            flpPoints.Location = new Point(0, 27);
            flpPoints.Name = "flpPoints";
            flpPoints.Size = new Size(322, 325);
            flpPoints.TabIndex = 1;
            flpPoints.WrapContents = false;
            // 
            // FormPoint
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(322, 352);
            Controls.Add(flpPoints);
            Controls.Add(lbTotalPoint);
            Name = "FormPoint";
            Text = "포인트 내역";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbTotalPoint;
        private FlowLayoutPanel flpPoints;
    }
}
