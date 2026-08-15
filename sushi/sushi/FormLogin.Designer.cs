namespace sushi
{
    partial class FormLogin
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
            label2 = new Label();
            tbLoginPhone = new TextBox();
            tbLoginPassword = new TextBox();
            btnLogin = new Button();
            bthCancel = new Button();
            chbShowPassword = new CheckBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(30, 50);
            label1.Name = "label1";
            label1.Size = new Size(55, 15);
            label1.TabIndex = 0;
            label1.Text = "전화번호";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(30, 86);
            label2.Name = "label2";
            label2.Size = new Size(55, 15);
            label2.TabIndex = 0;
            label2.Text = "비밀번호";
            // 
            // tbLoginPhone
            // 
            tbLoginPhone.Location = new Point(93, 47);
            tbLoginPhone.Name = "tbLoginPhone";
            tbLoginPhone.PlaceholderText = "번호를 입력해주세요.";
            tbLoginPhone.Size = new Size(151, 23);
            tbLoginPhone.TabIndex = 1;
            // 
            // tbLoginPassword
            // 
            tbLoginPassword.Location = new Point(93, 83);
            tbLoginPassword.Name = "tbLoginPassword";
            tbLoginPassword.PlaceholderText = "비밀번호를 입력해주세요.";
            tbLoginPassword.Size = new Size(151, 23);
            tbLoginPassword.TabIndex = 2;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(39, 161);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(75, 23);
            btnLogin.TabIndex = 3;
            btnLogin.Text = "로그인";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // bthCancel
            // 
            bthCancel.Location = new Point(160, 161);
            bthCancel.Name = "bthCancel";
            bthCancel.Size = new Size(75, 23);
            bthCancel.TabIndex = 4;
            bthCancel.Text = "취소";
            bthCancel.UseVisualStyleBackColor = true;
            bthCancel.Click += bthCancel_Click;
            // 
            // chbShowPassword
            // 
            chbShowPassword.AutoSize = true;
            chbShowPassword.Location = new Point(93, 118);
            chbShowPassword.Name = "chbShowPassword";
            chbShowPassword.Size = new Size(98, 19);
            chbShowPassword.TabIndex = 5;
            chbShowPassword.Text = "비밀번호표시";
            chbShowPassword.UseVisualStyleBackColor = true;
            chbShowPassword.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // FormLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(283, 228);
            Controls.Add(chbShowPassword);
            Controls.Add(bthCancel);
            Controls.Add(btnLogin);
            Controls.Add(tbLoginPassword);
            Controls.Add(tbLoginPhone);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormLogin";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "로그인";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox tbLoginPhone;
        private TextBox tbLoginPassword;
        private Button btnLogin;
        private Button bthCancel;
        private CheckBox chbShowPassword;
    }
}