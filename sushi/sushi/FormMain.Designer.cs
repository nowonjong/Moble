namespace sushi
{
    partial class FormMain
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
            btnLogin = new Button();
            btnSignup = new Button();
            label1 = new Label();
            btnGuest = new Button();
            SuspendLayout();
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(314, 133);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(153, 78);
            btnLogin.TabIndex = 0;
            btnLogin.Text = "로그인";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // btnSignup
            // 
            btnSignup.Location = new Point(314, 236);
            btnSignup.Name = "btnSignup";
            btnSignup.Size = new Size(153, 78);
            btnSignup.TabIndex = 0;
            btnSignup.Text = "회원가입";
            btnSignup.UseVisualStyleBackColor = true;
            btnSignup.Click += btnSignup_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("맑은 고딕", 36F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(306, 44);
            label1.Name = "label1";
            label1.Size = new Size(172, 65);
            label1.TabIndex = 1;
            label1.Text = "초밥집";
            // 
            // btnGuest
            // 
            btnGuest.Location = new Point(314, 336);
            btnGuest.Name = "btnGuest";
            btnGuest.Size = new Size(153, 78);
            btnGuest.TabIndex = 2;
            btnGuest.Text = "비회원 로그인";
            btnGuest.UseVisualStyleBackColor = true;
            btnGuest.Click += btnGuest_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnGuest);
            Controls.Add(label1);
            Controls.Add(btnSignup);
            Controls.Add(btnLogin);
            Name = "FormMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FormLogin";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnLogin;
        private Button btnSignup;
        private Label label1;
        private Button btnGuest;
    }
}