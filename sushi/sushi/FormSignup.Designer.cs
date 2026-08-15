namespace sushi
{
    partial class FormSignup
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
            label4 = new Label();
            label6 = new Label();
            label11 = new Label();
            chbShowPassword = new CheckBox();
            tbPassword = new TextBox();
            tbPasswordConfirm = new TextBox();
            tbAddress = new TextBox();
            chbEventAgree = new CheckBox();
            btnSignup = new Button();
            btnCancel = new Button();
            chbPrivacyAgree = new CheckBox();
            label2 = new Label();
            tbName = new TextBox();
            tbPhone = new TextBox();
            label8 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(215, 18);
            label1.Name = "label1";
            label1.Size = new Size(88, 25);
            label1.TabIndex = 0;
            label1.Text = "회원가입";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label4.Location = new Point(60, 159);
            label4.Name = "label4";
            label4.Size = new Size(74, 21);
            label4.TabIndex = 1;
            label4.Text = "비밀번호";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label6.Location = new Point(60, 199);
            label6.Name = "label6";
            label6.Size = new Size(112, 21);
            label6.TabIndex = 1;
            label6.Text = "비밀번호 확인";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label11.Location = new Point(60, 239);
            label11.Name = "label11";
            label11.Size = new Size(42, 21);
            label11.TabIndex = 1;
            label11.Text = "주소";
            // 
            // chbShowPassword
            // 
            chbShowPassword.AutoSize = true;
            chbShowPassword.Font = new Font("맑은 고딕", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            chbShowPassword.Location = new Point(351, 161);
            chbShowPassword.Name = "chbShowPassword";
            chbShowPassword.Size = new Size(110, 21);
            chbShowPassword.TabIndex = 2;
            chbShowPassword.Text = "비밀번호 표시";
            chbShowPassword.UseVisualStyleBackColor = true;
            chbShowPassword.CheckedChanged += chbShowPassword_CheckedChanged;
            // 
            // tbPassword
            // 
            tbPassword.Location = new Point(182, 158);
            tbPassword.Name = "tbPassword";
            tbPassword.Size = new Size(150, 23);
            tbPassword.TabIndex = 3;
            // 
            // tbPasswordConfirm
            // 
            tbPasswordConfirm.Location = new Point(182, 199);
            tbPasswordConfirm.Name = "tbPasswordConfirm";
            tbPasswordConfirm.Size = new Size(150, 23);
            tbPasswordConfirm.TabIndex = 3;
            // 
            // tbAddress
            // 
            tbAddress.Location = new Point(180, 241);
            tbAddress.Name = "tbAddress";
            tbAddress.Size = new Size(300, 23);
            tbAddress.TabIndex = 3;
            // 
            // chbEventAgree
            // 
            chbEventAgree.AutoSize = true;
            chbEventAgree.Location = new Point(112, 332);
            chbEventAgree.Name = "chbEventAgree";
            chbEventAgree.Size = new Size(178, 19);
            chbEventAgree.TabIndex = 6;
            chbEventAgree.Text = "이벤트 정보 수신 동의(선택)";
            chbEventAgree.UseVisualStyleBackColor = true;
            // 
            // btnSignup
            // 
            btnSignup.Location = new Point(147, 389);
            btnSignup.Name = "btnSignup";
            btnSignup.Size = new Size(90, 38);
            btnSignup.TabIndex = 7;
            btnSignup.Text = "가입하기";
            btnSignup.UseVisualStyleBackColor = true;
            btnSignup.Click += btnSignup_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(294, 389);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 38);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "취소";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // chbPrivacyAgree
            // 
            chbPrivacyAgree.AutoSize = true;
            chbPrivacyAgree.Location = new Point(112, 301);
            chbPrivacyAgree.Name = "chbPrivacyAgree";
            chbPrivacyAgree.Size = new Size(191, 19);
            chbPrivacyAgree.TabIndex = 8;
            chbPrivacyAgree.Text = "개인정보 수집•이용 동의(필수)";
            chbPrivacyAgree.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(60, 74);
            label2.Name = "label2";
            label2.Size = new Size(58, 21);
            label2.TabIndex = 1;
            label2.Text = "회원명";
            // 
            // tbName
            // 
            tbName.Location = new Point(182, 72);
            tbName.Name = "tbName";
            tbName.Size = new Size(150, 23);
            tbName.TabIndex = 10;
            // 
            // tbPhone
            // 
            tbPhone.Location = new Point(182, 112);
            tbPhone.Name = "tbPhone";
            tbPhone.Size = new Size(150, 23);
            tbPhone.TabIndex = 12;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label8.Location = new Point(60, 114);
            label8.Name = "label8";
            label8.Size = new Size(58, 21);
            label8.TabIndex = 11;
            label8.Text = "연락처";
            // 
            // FormSignup
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(522, 490);
            Controls.Add(tbPhone);
            Controls.Add(label8);
            Controls.Add(tbName);
            Controls.Add(chbPrivacyAgree);
            Controls.Add(btnCancel);
            Controls.Add(btnSignup);
            Controls.Add(chbEventAgree);
            Controls.Add(tbAddress);
            Controls.Add(tbPasswordConfirm);
            Controls.Add(tbPassword);
            Controls.Add(chbShowPassword);
            Controls.Add(label11);
            Controls.Add(label6);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "FormSignup";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormSignup";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label4;
        private Label label6;
        private Label label11;
        private CheckBox chbShowPassword;
        private TextBox tbPassword;
        private TextBox tbPasswordConfirm;
        private TextBox tbAddress;
        private Button btnSignup;
        private Button btnCancel;
        private CheckBox chbPrivacyAgree;
        private CheckBox chbEventAgree;
        private Label label2;
        private TextBox tbName;
        private TextBox tbPhone;
        private Label label8;
    }
}