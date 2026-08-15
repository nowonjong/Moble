namespace sushi
{
    partial class OrderHistoryItem
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            tbMenu = new TextBox();
            lbOrderDate = new Label();
            lbOrderType = new Label();
            lbPayAmount = new Label();
            label5 = new Label();
            tbRequest = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 16);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 0;
            label1.Text = "주문일시 :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(16, 44);
            label2.Name = "label2";
            label2.Size = new Size(62, 15);
            label2.TabIndex = 0;
            label2.Text = "주문방식 :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(16, 72);
            label3.Name = "label3";
            label3.Size = new Size(55, 15);
            label3.TabIndex = 0;
            label3.Text = "주문메뉴";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(16, 219);
            label4.Name = "label4";
            label4.Size = new Size(62, 15);
            label4.TabIndex = 0;
            label4.Text = "결제금액 :";
            // 
            // tbMenu
            // 
            tbMenu.BackColor = Color.White;
            tbMenu.BorderStyle = BorderStyle.None;
            tbMenu.Location = new Point(77, 74);
            tbMenu.Multiline = true;
            tbMenu.Name = "tbMenu";
            tbMenu.ReadOnly = true;
            tbMenu.ScrollBars = ScrollBars.Vertical;
            tbMenu.Size = new Size(260, 77);
            tbMenu.TabIndex = 1;
            tbMenu.TabStop = false;
            // 
            // lbOrderDate
            // 
            lbOrderDate.AutoSize = true;
            lbOrderDate.Location = new Point(85, 18);
            lbOrderDate.Name = "lbOrderDate";
            lbOrderDate.Size = new Size(39, 15);
            lbOrderDate.TabIndex = 2;
            lbOrderDate.Text = "label5";
            // 
            // lbOrderType
            // 
            lbOrderType.AutoSize = true;
            lbOrderType.Location = new Point(85, 44);
            lbOrderType.Name = "lbOrderType";
            lbOrderType.Size = new Size(39, 15);
            lbOrderType.TabIndex = 2;
            lbOrderType.Text = "label5";
            // 
            // lbPayAmount
            // 
            lbPayAmount.AutoSize = true;
            lbPayAmount.Location = new Point(85, 219);
            lbPayAmount.Name = "lbPayAmount";
            lbPayAmount.Size = new Size(39, 15);
            lbPayAmount.TabIndex = 2;
            lbPayAmount.Text = "label5";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(16, 163);
            label5.Name = "label5";
            label5.Size = new Size(62, 15);
            label5.TabIndex = 0;
            label5.Text = "요청사항 :";
            // 
            // tbRequest
            // 
            tbRequest.BackColor = Color.White;
            tbRequest.BorderStyle = BorderStyle.None;
            tbRequest.Location = new Point(84, 164);
            tbRequest.Multiline = true;
            tbRequest.Name = "tbRequest";
            tbRequest.ReadOnly = true;
            tbRequest.ScrollBars = ScrollBars.Vertical;
            tbRequest.Size = new Size(253, 44);
            tbRequest.TabIndex = 3;
            tbRequest.TabStop = false;
            // 
            // OrderHistoryItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(tbRequest);
            Controls.Add(lbPayAmount);
            Controls.Add(lbOrderType);
            Controls.Add(lbOrderDate);
            Controls.Add(tbMenu);
            Controls.Add(label4);
            Controls.Add(label5);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Margin = new Padding(2, 5, 0, 10);
            Name = "OrderHistoryItem";
            Size = new Size(338, 243);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox tbMenu;
        private Label lbOrderDate;
        private Label lbOrderType;
        private Label lbPayAmount;
        private Label label5;
        private TextBox tbRequest;
    }
}
