namespace sushi
{
    partial class FormHome
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
            btnHome = new Button();
            btnOrder = new Button();
            btnPoint = new Button();
            pnlContent = new Panel();
            pnlHome = new Panel();
            btnPickup = new Button();
            btnDelivery = new Button();
            label2 = new Label();
            lbMember = new Label();
            pnlContent.SuspendLayout();
            pnlHome.SuspendLayout();
            SuspendLayout();
            // 
            // btnHome
            // 
            btnHome.Location = new Point(-1, 393);
            btnHome.Name = "btnHome";
            btnHome.Size = new Size(115, 46);
            btnHome.TabIndex = 4;
            btnHome.Text = "홈";
            btnHome.UseVisualStyleBackColor = true;
            btnHome.Click += btnHome_Click;
            // 
            // btnOrder
            // 
            btnOrder.Location = new Point(112, 393);
            btnOrder.Name = "btnOrder";
            btnOrder.Size = new Size(115, 46);
            btnOrder.TabIndex = 4;
            btnOrder.Text = "주문내역";
            btnOrder.UseVisualStyleBackColor = true;
            btnOrder.Click += btnOrder_Click;
            // 
            // btnPoint
            // 
            btnPoint.Location = new Point(225, 393);
            btnPoint.Name = "btnPoint";
            btnPoint.Size = new Size(115, 46);
            btnPoint.TabIndex = 5;
            btnPoint.Text = "포인트내역";
            btnPoint.UseVisualStyleBackColor = true;
            btnPoint.Click += btnPoint_Click;
            // 
            // pnlContent
            // 
            pnlContent.Controls.Add(pnlHome);
            pnlContent.Location = new Point(-1, 0);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(341, 394);
            pnlContent.TabIndex = 6;
            // 
            // pnlHome
            // 
            pnlHome.Controls.Add(btnPickup);
            pnlHome.Controls.Add(btnDelivery);
            pnlHome.Controls.Add(label2);
            pnlHome.Controls.Add(lbMember);
            pnlHome.Location = new Point(2, 3);
            pnlHome.Name = "pnlHome";
            pnlHome.Size = new Size(338, 391);
            pnlHome.TabIndex = 0;
            // 
            // btnPickup
            // 
            btnPickup.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnPickup.Location = new Point(191, 168);
            btnPickup.Name = "btnPickup";
            btnPickup.Size = new Size(118, 66);
            btnPickup.TabIndex = 4;
            btnPickup.Text = "🛍포장 주문";
            btnPickup.UseVisualStyleBackColor = true;
            btnPickup.Click += btnPickup_Click;
            // 
            // btnDelivery
            // 
            btnDelivery.Font = new Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnDelivery.Location = new Point(29, 168);
            btnDelivery.Name = "btnDelivery";
            btnDelivery.Size = new Size(118, 66);
            btnDelivery.TabIndex = 5;
            btnDelivery.Text = "\U0001f6f5 배달 주문";
            btnDelivery.UseVisualStyleBackColor = true;
            btnDelivery.Click += btnDelivery_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(70, 55);
            label2.Name = "label2";
            label2.Size = new Size(43, 15);
            label2.TabIndex = 2;
            label2.Text = "초밥집";
            // 
            // lbMember
            // 
            lbMember.AutoSize = true;
            lbMember.Location = new Point(235, 55);
            lbMember.Name = "lbMember";
            lbMember.Size = new Size(43, 15);
            lbMember.TabIndex = 3;
            lbMember.Text = "회원명";
            // 
            // FormHome
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(339, 438);
            Controls.Add(pnlContent);
            Controls.Add(btnPoint);
            Controls.Add(btnOrder);
            Controls.Add(btnHome);
            Name = "FormHome";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormHome";
            pnlContent.ResumeLayout(false);
            pnlHome.ResumeLayout(false);
            pnlHome.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button btnHome;
        private Button btnOrder;
        private Button btnPoint;
        private Panel pnlContent;
        private Panel pnlHome;
        private Button btnPickup;
        private Button btnDelivery;
        private Label label2;
        private Label lbMember;
    }
}
