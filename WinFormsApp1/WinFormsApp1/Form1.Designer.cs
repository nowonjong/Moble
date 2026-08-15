namespace WinFormsApp1
{
    partial class Form1
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
            txtServerIP = new TextBox();
            btnConnect = new Button();
            txtMessage = new TextBox();
            txtLog = new TextBox();
            btnSend = new Button();
            SuspendLayout();
            // 
            // txtServerIP
            // 
            txtServerIP.Location = new Point(125, 164);
            txtServerIP.Multiline = true;
            txtServerIP.Name = "txtServerIP";
            txtServerIP.Size = new Size(275, 57);
            txtServerIP.TabIndex = 0;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(422, 180);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 23);
            btnConnect.TabIndex = 1;
            btnConnect.Text = "connect";
            btnConnect.UseVisualStyleBackColor = true;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(125, 246);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(275, 59);
            txtMessage.TabIndex = 0;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(125, 333);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.Size = new Size(275, 59);
            txtLog.TabIndex = 0;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(422, 259);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 2;
            btnSend.Text = "send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSend);
            Controls.Add(btnConnect);
            Controls.Add(txtLog);
            Controls.Add(txtMessage);
            Controls.Add(txtServerIP);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtServerIP;
        private Button btnConnect;
        private TextBox txtMessage;
        private TextBox txtLog;
        private Button btnSend;
    }
}
