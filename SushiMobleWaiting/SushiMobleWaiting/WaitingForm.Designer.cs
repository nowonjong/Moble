namespace SushiMobleWaiting
{
    partial class WaitingForm
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
            pnlWaitingInfo = new Panel();
            lblTableStatus = new Label();
            panel2 = new Panel();
            panel1 = new Panel();
            lblWaitingCount = new Label();
            lblWaitingTitle = new Label();
            lblGuide = new Label();
            lblStoreName = new Label();
            pnlPhoneInput = new Panel();
            tlpKeypad = new TableLayoutPanel();
            btnClear = new Button();
            btn0 = new Button();
            btnBack = new Button();
            btn9 = new Button();
            btn6 = new Button();
            btn8 = new Button();
            btn1 = new Button();
            btn5 = new Button();
            btn2 = new Button();
            btn7 = new Button();
            btn3 = new Button();
            btn4 = new Button();
            lblPhoneTitle = new Label();
            btnRegisterWaiting = new Button();
            lblPhoneNumber = new Label();
            pnlWaitingInfo.SuspendLayout();
            pnlPhoneInput.SuspendLayout();
            tlpKeypad.SuspendLayout();
            SuspendLayout();
            // 
            // pnlWaitingInfo
            // 
            pnlWaitingInfo.Controls.Add(lblTableStatus);
            pnlWaitingInfo.Controls.Add(panel2);
            pnlWaitingInfo.Controls.Add(panel1);
            pnlWaitingInfo.Controls.Add(lblWaitingCount);
            pnlWaitingInfo.Controls.Add(lblWaitingTitle);
            pnlWaitingInfo.Controls.Add(lblGuide);
            pnlWaitingInfo.Controls.Add(lblStoreName);
            pnlWaitingInfo.Dock = DockStyle.Left;
            pnlWaitingInfo.Location = new Point(0, 0);
            pnlWaitingInfo.Name = "pnlWaitingInfo";
            pnlWaitingInfo.Size = new Size(420, 450);
            pnlWaitingInfo.TabIndex = 0;
            // 
            // lblTableStatus
            // 
            lblTableStatus.AutoSize = true;
            lblTableStatus.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblTableStatus.Location = new Point(128, 381);
            lblTableStatus.Name = "lblTableStatus";
            lblTableStatus.Size = new Size(156, 30);
            lblTableStatus.TabIndex = 2;
            lblTableStatus.Text = "빈 테이블 : 0개";
            // 
            // panel2
            // 
            panel2.Location = new Point(450, 112);
            panel2.Name = "panel2";
            panel2.Size = new Size(350, 263);
            panel2.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.Location = new Point(450, 123);
            panel1.Name = "panel1";
            panel1.Size = new Size(350, 241);
            panel1.TabIndex = 1;
            // 
            // lblWaitingCount
            // 
            lblWaitingCount.AutoSize = true;
            lblWaitingCount.Font = new Font("맑은 고딕", 27.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblWaitingCount.Location = new Point(171, 286);
            lblWaitingCount.Name = "lblWaitingCount";
            lblWaitingCount.Size = new Size(80, 50);
            lblWaitingCount.TabIndex = 0;
            lblWaitingCount.Text = "0팀";
            // 
            // lblWaitingTitle
            // 
            lblWaitingTitle.AutoSize = true;
            lblWaitingTitle.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblWaitingTitle.Location = new Point(143, 242);
            lblWaitingTitle.Name = "lblWaitingTitle";
            lblWaitingTitle.Size = new Size(125, 30);
            lblWaitingTitle.TabIndex = 0;
            lblWaitingTitle.Text = "현재 웨이팅";
            // 
            // lblGuide
            // 
            lblGuide.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblGuide.Location = new Point(38, 139);
            lblGuide.Name = "lblGuide";
            lblGuide.Size = new Size(340, 80);
            lblGuide.TabIndex = 0;
            lblGuide.Text = "휴대폰 번호를 입력하시면\n웨이팅 등록 후 문자로 안내해드립니다.";
            lblGuide.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblStoreName
            // 
            lblStoreName.AutoSize = true;
            lblStoreName.Font = new Font("맑은 고딕", 24F, FontStyle.Bold, GraphicsUnit.Point);
            lblStoreName.Location = new Point(88, 42);
            lblStoreName.Name = "lblStoreName";
            lblStoreName.Size = new Size(234, 45);
            lblStoreName.TabIndex = 0;
            lblStoreName.Text = "SUSHI MOBLE";
            // 
            // pnlPhoneInput
            // 
            pnlPhoneInput.Controls.Add(tlpKeypad);
            pnlPhoneInput.Controls.Add(lblPhoneTitle);
            pnlPhoneInput.Controls.Add(btnRegisterWaiting);
            pnlPhoneInput.Controls.Add(lblPhoneNumber);
            pnlPhoneInput.Dock = DockStyle.Fill;
            pnlPhoneInput.Location = new Point(420, 0);
            pnlPhoneInput.Name = "pnlPhoneInput";
            pnlPhoneInput.Size = new Size(380, 450);
            pnlPhoneInput.TabIndex = 0;
            // 
            // tlpKeypad
            // 
            tlpKeypad.ColumnCount = 3;
            tlpKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tlpKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            tlpKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            tlpKeypad.Controls.Add(btnClear, 0, 3);
            tlpKeypad.Controls.Add(btn0, 1, 3);
            tlpKeypad.Controls.Add(btnBack, 2, 3);
            tlpKeypad.Controls.Add(btn9, 2, 2);
            tlpKeypad.Controls.Add(btn6, 2, 1);
            tlpKeypad.Controls.Add(btn8, 1, 2);
            tlpKeypad.Controls.Add(btn1, 0, 0);
            tlpKeypad.Controls.Add(btn5, 1, 1);
            tlpKeypad.Controls.Add(btn2, 1, 0);
            tlpKeypad.Controls.Add(btn7, 0, 2);
            tlpKeypad.Controls.Add(btn3, 2, 0);
            tlpKeypad.Controls.Add(btn4, 0, 1);
            tlpKeypad.Font = new Font("맑은 고딕", 18F, FontStyle.Bold, GraphicsUnit.Point);
            tlpKeypad.Location = new Point(0, 115);
            tlpKeypad.Name = "tlpKeypad";
            tlpKeypad.RowCount = 4;
            tlpKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpKeypad.Size = new Size(380, 246);
            tlpKeypad.TabIndex = 4;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(3, 186);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(120, 54);
            btnClear.TabIndex = 1;
            btnClear.Text = "C";
            btnClear.UseVisualStyleBackColor = true;
            // 
            // btn0
            // 
            btn0.Location = new Point(129, 186);
            btn0.Name = "btn0";
            btn0.Size = new Size(120, 54);
            btn0.TabIndex = 1;
            btn0.Text = "0";
            btn0.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(255, 186);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(122, 54);
            btnBack.TabIndex = 1;
            btnBack.Text = "<-";
            btnBack.UseVisualStyleBackColor = true;
            // 
            // btn9
            // 
            btn9.Location = new Point(255, 125);
            btn9.Name = "btn9";
            btn9.Size = new Size(122, 54);
            btn9.TabIndex = 1;
            btn9.Text = "9";
            btn9.UseVisualStyleBackColor = true;
            // 
            // btn6
            // 
            btn6.Location = new Point(255, 64);
            btn6.Name = "btn6";
            btn6.Size = new Size(122, 54);
            btn6.TabIndex = 1;
            btn6.Text = "6";
            btn6.UseVisualStyleBackColor = true;
            // 
            // btn8
            // 
            btn8.Location = new Point(129, 125);
            btn8.Name = "btn8";
            btn8.Size = new Size(120, 54);
            btn8.TabIndex = 1;
            btn8.Text = "8";
            btn8.UseVisualStyleBackColor = true;
            // 
            // btn1
            // 
            btn1.Location = new Point(3, 3);
            btn1.Name = "btn1";
            btn1.Size = new Size(120, 54);
            btn1.TabIndex = 1;
            btn1.Text = "1";
            btn1.UseVisualStyleBackColor = true;
            // 
            // btn5
            // 
            btn5.Location = new Point(129, 64);
            btn5.Name = "btn5";
            btn5.Size = new Size(120, 54);
            btn5.TabIndex = 1;
            btn5.Text = "5";
            btn5.UseVisualStyleBackColor = true;
            // 
            // btn2
            // 
            btn2.Location = new Point(129, 3);
            btn2.Name = "btn2";
            btn2.Size = new Size(120, 54);
            btn2.TabIndex = 1;
            btn2.Text = "2";
            btn2.UseVisualStyleBackColor = true;
            // 
            // btn7
            // 
            btn7.Location = new Point(3, 125);
            btn7.Name = "btn7";
            btn7.Size = new Size(120, 54);
            btn7.TabIndex = 1;
            btn7.Text = "7";
            btn7.UseVisualStyleBackColor = true;
            // 
            // btn3
            // 
            btn3.Location = new Point(255, 3);
            btn3.Name = "btn3";
            btn3.Size = new Size(122, 54);
            btn3.TabIndex = 1;
            btn3.Text = "3";
            btn3.UseVisualStyleBackColor = true;
            // 
            // btn4
            // 
            btn4.Location = new Point(3, 64);
            btn4.Name = "btn4";
            btn4.Size = new Size(120, 54);
            btn4.TabIndex = 1;
            btn4.Text = "4";
            btn4.UseVisualStyleBackColor = true;
            // 
            // lblPhoneTitle
            // 
            lblPhoneTitle.AutoSize = true;
            lblPhoneTitle.Font = new Font("맑은 고딕", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblPhoneTitle.Location = new Point(52, 9);
            lblPhoneTitle.Name = "lblPhoneTitle";
            lblPhoneTitle.Size = new Size(279, 30);
            lblPhoneTitle.TabIndex = 3;
            lblPhoneTitle.Text = "휴대폰 번호를 입력해주세요";
            // 
            // btnRegisterWaiting
            // 
            btnRegisterWaiting.Font = new Font("맑은 고딕", 18F, FontStyle.Bold, GraphicsUnit.Point);
            btnRegisterWaiting.Location = new Point(100, 381);
            btnRegisterWaiting.Name = "btnRegisterWaiting";
            btnRegisterWaiting.Size = new Size(179, 57);
            btnRegisterWaiting.TabIndex = 2;
            btnRegisterWaiting.Text = "웨이팅 등록";
            btnRegisterWaiting.UseVisualStyleBackColor = true;
            // 
            // lblPhoneNumber
            // 
            lblPhoneNumber.AutoSize = true;
            lblPhoneNumber.BorderStyle = BorderStyle.FixedSingle;
            lblPhoneNumber.Font = new Font("맑은 고딕", 21.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblPhoneNumber.Location = new Point(63, 56);
            lblPhoneNumber.Name = "lblPhoneNumber";
            lblPhoneNumber.Size = new Size(230, 42);
            lblPhoneNumber.TabIndex = 0;
            lblPhoneNumber.Text = "010 - ____ - ____";
            lblPhoneNumber.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // WaitingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 450);
            Controls.Add(pnlPhoneInput);
            Controls.Add(pnlWaitingInfo);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximumSize = new Size(1000, 650);
            Name = "WaitingForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "스시 모블 웨이팅";
            pnlWaitingInfo.ResumeLayout(false);
            pnlWaitingInfo.PerformLayout();
            pnlPhoneInput.ResumeLayout(false);
            pnlPhoneInput.PerformLayout();
            tlpKeypad.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlWaitingInfo;
        private Panel pnlPhoneInput;
        private Panel panel2;
        private Panel panel1;
        private Label lblWaitingCount;
        private Label lblWaitingTitle;
        private Label lblGuide;
        private Label lblStoreName;
        private Button btn6;
        private Button btn5;
        private Button btn4;
        private Button btn2;
        private Button btn1;
        private Label lblPhoneNumber;
        private Button btnRegisterWaiting;
        private Button btnBack;
        private Button btn9;
        private Button btn0;
        private Button btn8;
        private Button btnClear;
        private Button btn7;
        private Button btn3;
        private Label lblPhoneTitle;
        private TableLayoutPanel tlpKeypad;
        private Label lblTableStatus;
    }
}
