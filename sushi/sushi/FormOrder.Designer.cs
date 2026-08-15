namespace sushi
{
    partial class FormOrder
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
            flpOrders = new FlowLayoutPanel();
            label1 = new Label();
            SuspendLayout();
            // 
            // flpOrders
            // 
            flpOrders.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flpOrders.AutoScroll = true;
            flpOrders.FlowDirection = FlowDirection.TopDown;
            flpOrders.Location = new Point(-2, 27);
            flpOrders.Name = "flpOrders";
            flpOrders.Size = new Size(320, 327);
            flpOrders.TabIndex = 0;
            flpOrders.WrapContents = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(141, 9);
            label1.Name = "label1";
            label1.Size = new Size(55, 15);
            label1.TabIndex = 1;
            label1.Text = "주문내역";
            // 
            // FormOrder
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(322, 352);
            Controls.Add(label1);
            Controls.Add(flpOrders);
            Name = "FormOrder";
            StartPosition = FormStartPosition.CenterParent;
            Text = "주문내역";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel flpOrders;
        private Label label1;
    }
}