namespace Kiosk
{
    partial class Payment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Payment));
            btn_back = new Button();
            btn_allDelete = new Button();
            btn_KakaoPay = new Button();
            btn_naverPay = new Button();
            btn_card = new Button();
            label2 = new Label();
            label1 = new Label();
            label4 = new Label();
            label5 = new Label();
            btn_SamsungPay = new Button();
            label3 = new Label();
            btn_coupon = new Button();
            roundedPanel1 = new Kiosk.Controls.RoundedPanel();
            axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            roundedPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)axWindowsMediaPlayer1).BeginInit();
            SuspendLayout();
            // 
            // btn_back
            // 
            btn_back.BackColor = SystemColors.ActiveBorder;
            btn_back.FlatStyle = FlatStyle.Flat;
            btn_back.Location = new Point(224, 454);
            btn_back.Name = "btn_back";
            btn_back.Size = new Size(200, 68);
            btn_back.TabIndex = 20;
            btn_back.Text = "이전";
            btn_back.UseVisualStyleBackColor = false;
            btn_back.Click += btn_back_Click;
            // 
            // btn_allDelete
            // 
            btn_allDelete.BackColor = Color.FromArgb(211, 47, 47);
            btn_allDelete.FlatStyle = FlatStyle.Flat;
            btn_allDelete.Location = new Point(48, 454);
            btn_allDelete.Name = "btn_allDelete";
            btn_allDelete.Size = new Size(103, 68);
            btn_allDelete.TabIndex = 19;
            btn_allDelete.Text = "전체 취소";
            btn_allDelete.UseVisualStyleBackColor = false;
            // 
            // btn_KakaoPay
            // 
            btn_KakaoPay.BackColor = SystemColors.ControlLightLight;
            btn_KakaoPay.FlatStyle = FlatStyle.Flat;
            btn_KakaoPay.Location = new Point(238, 126);
            btn_KakaoPay.Name = "btn_KakaoPay";
            btn_KakaoPay.Size = new Size(186, 91);
            btn_KakaoPay.TabIndex = 18;
            btn_KakaoPay.Text = "카카오 페이";
            btn_KakaoPay.UseVisualStyleBackColor = false;
            // 
            // btn_naverPay
            // 
            btn_naverPay.BackColor = SystemColors.ControlLightLight;
            btn_naverPay.FlatStyle = FlatStyle.Flat;
            btn_naverPay.Location = new Point(143, 126);
            btn_naverPay.Name = "btn_naverPay";
            btn_naverPay.Size = new Size(89, 91);
            btn_naverPay.TabIndex = 16;
            btn_naverPay.Text = "네이버 페이";
            btn_naverPay.UseVisualStyleBackColor = false;
            // 
            // btn_card
            // 
            btn_card.BackColor = SystemColors.ControlLightLight;
            btn_card.FlatStyle = FlatStyle.Flat;
            btn_card.Location = new Point(46, 126);
            btn_card.Name = "btn_card";
            btn_card.Size = new Size(89, 91);
            btn_card.TabIndex = 15;
            btn_card.Text = "카드 결제";
            btn_card.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            label2.BackColor = SystemColors.GradientActiveCaption;
            label2.Location = new Point(46, 95);
            label2.Name = "label2";
            label2.Size = new Size(378, 28);
            label2.TabIndex = 14;
            label2.Text = "결제 방식";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = SystemColors.MenuHighlight;
            label1.Location = new Point(35, 41);
            label1.Name = "label1";
            label1.Size = new Size(0, 15);
            label1.TabIndex = 13;
            // 
            // label4
            // 
            label4.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label4.Location = new Point(28, 60);
            label4.Name = "label4";
            label4.Size = new Size(249, 23);
            label4.TabIndex = 22;
            label4.Text = "결제 방식을 선택해주세요 ! ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("맑은 고딕", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label5.ForeColor = SystemColors.MenuHighlight;
            label5.Location = new Point(28, 45);
            label5.Name = "label5";
            label5.Size = new Size(44, 15);
            label5.TabIndex = 21;
            label5.Text = "Step2.";
            // 
            // btn_SamsungPay
            // 
            btn_SamsungPay.BackColor = SystemColors.ControlLightLight;
            btn_SamsungPay.FlatStyle = FlatStyle.Flat;
            btn_SamsungPay.Location = new Point(46, 223);
            btn_SamsungPay.Name = "btn_SamsungPay";
            btn_SamsungPay.Size = new Size(186, 91);
            btn_SamsungPay.TabIndex = 23;
            btn_SamsungPay.Text = "삼성페이";
            btn_SamsungPay.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            label3.BackColor = SystemColors.GradientActiveCaption;
            label3.Location = new Point(46, 326);
            label3.Name = "label3";
            label3.Size = new Size(378, 28);
            label3.TabIndex = 24;
            label3.Text = "상품권 결제";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btn_coupon
            // 
            btn_coupon.BackColor = SystemColors.ControlLightLight;
            btn_coupon.FlatStyle = FlatStyle.Flat;
            btn_coupon.Location = new Point(48, 357);
            btn_coupon.Name = "btn_coupon";
            btn_coupon.Size = new Size(186, 91);
            btn_coupon.TabIndex = 25;
            btn_coupon.Text = "쿠폰 / 상품권 복합 결제";
            btn_coupon.UseVisualStyleBackColor = false;
            btn_coupon.Click += button8_Click;
            // 
            // roundedPanel1
            // 
            roundedPanel1.BorderColor = Color.Black;
            roundedPanel1.BorderRadius = 90;
            roundedPanel1.BorderSize = 1F;
            roundedPanel1.BottomBorderRadius = 40;
            roundedPanel1.Controls.Add(axWindowsMediaPlayer1);
            roundedPanel1.Controls.Add(label4);
            roundedPanel1.Controls.Add(btn_coupon);
            roundedPanel1.Controls.Add(label2);
            roundedPanel1.Controls.Add(btn_KakaoPay);
            roundedPanel1.Controls.Add(btn_card);
            roundedPanel1.Controls.Add(label3);
            roundedPanel1.Controls.Add(btn_naverPay);
            roundedPanel1.Controls.Add(btn_SamsungPay);
            roundedPanel1.Controls.Add(btn_allDelete);
            roundedPanel1.Controls.Add(btn_back);
            roundedPanel1.Controls.Add(label5);
            roundedPanel1.Location = new Point(41, 12);
            roundedPanel1.Name = "roundedPanel1";
            roundedPanel1.RoundBottomLeft = true;
            roundedPanel1.RoundBottomRight = true;
            roundedPanel1.RoundTopLeft = true;
            roundedPanel1.RoundTopRight = true;
            roundedPanel1.ShadowColor = Color.FromArgb(60, 0, 0, 0);
            roundedPanel1.ShadowDepth = 8;
            roundedPanel1.ShowShadow = true;
            roundedPanel1.Size = new Size(980, 572);
            roundedPanel1.TabIndex = 26;
            // 
            // axWindowsMediaPlayer1
            // 
            axWindowsMediaPlayer1.Enabled = true;
            axWindowsMediaPlayer1.Location = new Point(430, 126);
            axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            axWindowsMediaPlayer1.OcxState = (AxHost.State)resources.GetObject("axWindowsMediaPlayer1.OcxState");
            axWindowsMediaPlayer1.Size = new Size(488, 338);
            axWindowsMediaPlayer1.TabIndex = 27;
            // 
            // Payment
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1219, 700);
            Controls.Add(roundedPanel1);
            Controls.Add(label1);
            Name = "Payment";
            Text = "Payment";
            Load += Payment_Load;
            roundedPanel1.ResumeLayout(false);
            roundedPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)axWindowsMediaPlayer1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_back;
        private Button btn_allDelete;
        private Button btn_KakaoPay;
        private Button btn_naverPay;
        private Button btn_card;
        private Label label2;
        private Label label1;
        private Label label4;
        private Label label5;
        private Button btn_SamsungPay;
        private Label label3;
        private Button btn_coupon;
        private Controls.RoundedPanel roundedPanel1;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
    }
}