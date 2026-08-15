namespace sushi
{
    partial class FormPayment
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
            label1 = new Label();
            tbOrderMenu = new TextBox();
            lbOrder = new Label();
            lbadress = new Label();
            tbaddress = new TextBox();
            label3 = new Label();
            tbRequest = new TextBox();
            cbPay = new ComboBox();
            lbPayInfo = new ListBox();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label12 = new Label();
            lbpay = new Label();
            btnPay = new Button();
            groupBox1 = new GroupBox();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            lbTotalPoint = new Label();
            label2 = new Label();
            label4 = new Label();
            lbTotal = new Label();
            lbPoint = new Label();
            label9 = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(124, 9);
            label1.Name = "label1";
            label1.Size = new Size(88, 25);
            label1.TabIndex = 0;
            label1.Text = "주문하기";
            // 
            // tbOrderMenu
            // 
            tbOrderMenu.Location = new Point(1, 90);
            tbOrderMenu.Multiline = true;
            tbOrderMenu.Name = "tbOrderMenu";
            tbOrderMenu.ReadOnly = true;
            tbOrderMenu.ScrollBars = ScrollBars.Vertical;
            tbOrderMenu.Size = new Size(351, 126);
            tbOrderMenu.TabIndex = 1;
            // 
            // lbOrder
            // 
            lbOrder.AutoSize = true;
            lbOrder.Location = new Point(3, 39);
            lbOrder.Name = "lbOrder";
            lbOrder.Size = new Size(55, 15);
            lbOrder.TabIndex = 2;
            lbOrder.Text = "주문방법";
            // 
            // lbadress
            // 
            lbadress.AutoSize = true;
            lbadress.Location = new Point(4, 237);
            lbadress.Name = "lbadress";
            lbadress.Size = new Size(55, 15);
            lbadress.TabIndex = 3;
            lbadress.Text = "배달주소";
            // 
            // tbaddress
            // 
            tbaddress.Location = new Point(63, 233);
            tbaddress.Multiline = true;
            tbaddress.Name = "tbaddress";
            tbaddress.PlaceholderText = "주소를 입력해주세요.";
            tbaddress.Size = new Size(256, 54);
            tbaddress.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 310);
            label3.Name = "label3";
            label3.Size = new Size(55, 15);
            label3.TabIndex = 3;
            label3.Text = "요청사항";
            // 
            // tbRequest
            // 
            tbRequest.Location = new Point(64, 307);
            tbRequest.Multiline = true;
            tbRequest.Name = "tbRequest";
            tbRequest.Size = new Size(255, 84);
            tbRequest.TabIndex = 6;
            // 
            // cbPay
            // 
            cbPay.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPay.FormattingEnabled = true;
            cbPay.Items.AddRange(new object[] { "간편결제", "신용/체크카드" });
            cbPay.Location = new Point(76, 411);
            cbPay.Name = "cbPay";
            cbPay.Size = new Size(103, 23);
            cbPay.TabIndex = 7;
            cbPay.SelectedIndexChanged += cbPay_SelectedIndexChanged;
            // 
            // lbPayInfo
            // 
            lbPayInfo.FormattingEnabled = true;
            lbPayInfo.ItemHeight = 15;
            lbPayInfo.Location = new Point(194, 410);
            lbPayInfo.Name = "lbPayInfo";
            lbPayInfo.Size = new Size(120, 79);
            lbPayInfo.TabIndex = 8;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(4, 414);
            label5.Name = "label5";
            label5.Size = new Size(55, 15);
            label5.TabIndex = 11;
            label5.Text = "결제수단";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(-5, 560);
            label6.Name = "label6";
            label6.Size = new Size(367, 15);
            label6.TabIndex = 10;
            label6.Text = "------------------------------------------------------------------------";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(3, 70);
            label7.Name = "label7";
            label7.Size = new Size(59, 15);
            label7.TabIndex = 2;
            label7.Text = "담은 메뉴";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(-5, 54);
            label8.Name = "label8";
            label8.Size = new Size(362, 15);
            label8.TabIndex = 12;
            label8.Text = "-----------------------------------------------------------------------";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(33, 652);
            label12.Name = "label12";
            label12.Size = new Size(55, 15);
            label12.TabIndex = 13;
            label12.Text = "결제금액";
            // 
            // lbpay
            // 
            lbpay.AutoSize = true;
            lbpay.Location = new Point(237, 649);
            lbpay.Name = "lbpay";
            lbpay.Size = new Size(39, 15);
            lbpay.TabIndex = 13;
            lbpay.Text = "label9";
            // 
            // btnPay
            // 
            btnPay.Location = new Point(81, 670);
            btnPay.Name = "btnPay";
            btnPay.Size = new Size(189, 56);
            btnPay.TabIndex = 14;
            btnPay.Text = "결제하기";
            btnPay.UseVisualStyleBackColor = true;
            btnPay.Click += btnPay_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(radioButton2);
            groupBox1.Controls.Add(radioButton1);
            groupBox1.Location = new Point(136, 500);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(183, 57);
            groupBox1.TabIndex = 17;
            groupBox1.TabStop = false;
            groupBox1.Text = "포인트 사용여부";
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(101, 24);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(61, 19);
            radioButton2.TabIndex = 0;
            radioButton2.TabStop = true;
            radioButton2.Text = "미사용";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(24, 24);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(49, 19);
            radioButton1.TabIndex = 0;
            radioButton1.TabStop = true;
            radioButton1.Text = "사용";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // lbTotalPoint
            // 
            lbTotalPoint.AutoSize = true;
            lbTotalPoint.Location = new Point(33, 517);
            lbTotalPoint.Name = "lbTotalPoint";
            lbTotalPoint.Size = new Size(84, 15);
            lbTotalPoint.TabIndex = 18;
            lbTotalPoint.Text = "총포인트 :  0P";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(33, 619);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 13;
            label2.Text = "포인트할인";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(33, 590);
            label4.Name = "label4";
            label4.Size = new Size(55, 15);
            label4.TabIndex = 13;
            label4.Text = "주문금액";
            // 
            // lbTotal
            // 
            lbTotal.AutoSize = true;
            lbTotal.Location = new Point(237, 590);
            lbTotal.Name = "lbTotal";
            lbTotal.Size = new Size(26, 15);
            lbTotal.TabIndex = 13;
            lbTotal.Text = "0원";
            // 
            // lbPoint
            // 
            lbPoint.AutoSize = true;
            lbPoint.Location = new Point(238, 619);
            lbPoint.Name = "lbPoint";
            lbPoint.Size = new Size(26, 15);
            lbPoint.TabIndex = 13;
            lbPoint.Text = "0원";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(33, 532);
            label9.Name = "label9";
            label9.Size = new Size(67, 15);
            label9.TabIndex = 19;
            label9.Text = "(회원 전용)";
            // 
            // FormPayment
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(354, 725);
            Controls.Add(label9);
            Controls.Add(lbTotalPoint);
            Controls.Add(groupBox1);
            Controls.Add(btnPay);
            Controls.Add(lbPoint);
            Controls.Add(lbTotal);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label12);
            Controls.Add(lbpay);
            Controls.Add(label8);
            Controls.Add(label5);
            Controls.Add(label6);
            Controls.Add(lbPayInfo);
            Controls.Add(cbPay);
            Controls.Add(tbRequest);
            Controls.Add(tbaddress);
            Controls.Add(label3);
            Controls.Add(lbadress);
            Controls.Add(label7);
            Controls.Add(lbOrder);
            Controls.Add(tbOrderMenu);
            Controls.Add(label1);
            Name = "FormPayment";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormPayment";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox tbOrderMenu;
        private Label lbOrder;
        private Label lbadress;
        private TextBox tbaddress;
        private Label label3;
        private TextBox tbRequest;
        private ComboBox cbPay;
        private ListBox lbPayInfo;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label12;
        private Label lbpay;
        private Button btnPay;
        private GroupBox groupBox1;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private Label lbTotalPoint;
        private Label label2;
        private Label label4;
        private Label lbTotal;
        private Label lbPoint;
        private Label label9;
    }
}