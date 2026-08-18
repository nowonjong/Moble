namespace Kiosk
{
    partial class Firstform
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Firstform));
            btn_togo = new Button();
            label1 = new Label();
            roundedPanel1 = new Kiosk.Controls.RoundedPanel();
            btn_KorCh = new Button();
            btn_JapCh = new Button();
            btn_EngCh = new Button();
            btn_herein = new Button();
            pictureBox1 = new PictureBox();
            roundedPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // btn_togo
            // 
            btn_togo.BackColor = SystemColors.GradientActiveCaption;
            btn_togo.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            btn_togo.Location = new Point(572, 482);
            btn_togo.Name = "btn_togo";
            btn_togo.Size = new Size(224, 100);
            btn_togo.TabIndex = 2;
            btn_togo.Text = "포장 주문";
            btn_togo.UseVisualStyleBackColor = false;
            btn_togo.Click += btn_togo_Click;
            // 
            // label1
            // 
            label1.BackColor = Color.FromArgb(252, 245, 242);
            label1.Font = new Font("맑은 고딕", 20.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(357, 68);
            label1.Name = "label1";
            label1.Size = new Size(430, 41);
            label1.TabIndex = 10;
            label1.Text = "원하시는 서비스를 선택해주세요! ";
            // 
            // roundedPanel1
            // 
            roundedPanel1.BackColor = SystemColors.Info;
            roundedPanel1.BorderColor = Color.Black;
            roundedPanel1.BorderRadius = 90;
            roundedPanel1.BorderSize = 1F;
            roundedPanel1.BottomBorderRadius = 40;
            roundedPanel1.Controls.Add(btn_KorCh);
            roundedPanel1.Controls.Add(btn_JapCh);
            roundedPanel1.Controls.Add(btn_EngCh);
            roundedPanel1.Controls.Add(btn_togo);
            roundedPanel1.Controls.Add(btn_herein);
            roundedPanel1.Controls.Add(label1);
            roundedPanel1.Controls.Add(pictureBox1);
            roundedPanel1.Location = new Point(34, 31);
            roundedPanel1.Name = "roundedPanel1";
            roundedPanel1.RoundBottomLeft = true;
            roundedPanel1.RoundBottomRight = true;
            roundedPanel1.RoundTopLeft = true;
            roundedPanel1.RoundTopRight = true;
            roundedPanel1.ShadowColor = Color.FromArgb(60, 0, 0, 0);
            roundedPanel1.ShadowDepth = 8;
            roundedPanel1.ShowShadow = true;
            roundedPanel1.Size = new Size(1144, 604);
            roundedPanel1.TabIndex = 11;
            // 
            // btn_KorCh
            // 
            btn_KorCh.BackColor = SystemColors.ActiveCaption;
            btn_KorCh.Location = new Point(791, 14);
            btn_KorCh.Name = "btn_KorCh";
            btn_KorCh.Size = new Size(94, 35);
            btn_KorCh.TabIndex = 10;
            btn_KorCh.Text = "한국어";
            btn_KorCh.UseVisualStyleBackColor = false;
            // 
            // btn_JapCh
            // 
            btn_JapCh.BackColor = SystemColors.ActiveCaption;
            btn_JapCh.Location = new Point(991, 14);
            btn_JapCh.Name = "btn_JapCh";
            btn_JapCh.Size = new Size(94, 35);
            btn_JapCh.TabIndex = 10;
            btn_JapCh.Text = "日本語";
            btn_JapCh.UseVisualStyleBackColor = false;
            btn_JapCh.Click += btn_JapCh_Click;
            // 
            // btn_EngCh
            // 
            btn_EngCh.BackColor = SystemColors.ActiveCaption;
            btn_EngCh.Location = new Point(891, 14);
            btn_EngCh.Name = "btn_EngCh";
            btn_EngCh.Size = new Size(94, 35);
            btn_EngCh.TabIndex = 9;
            btn_EngCh.Text = "English";
            btn_EngCh.UseVisualStyleBackColor = false;
            // 
            // btn_herein
            // 
            btn_herein.BackColor = Color.SandyBrown;
            btn_herein.Font = new Font("맑은 고딕", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            btn_herein.Location = new Point(330, 482);
            btn_herein.Margin = new Padding(4, 5, 4, 5);
            btn_herein.Name = "btn_herein";
            btn_herein.Size = new Size(224, 100);
            btn_herein.TabIndex = 0;
            btn_herein.Text = "매장 식사";
            btn_herein.UseVisualStyleBackColor = false;
            btn_herein.Click += btn_start_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(-22, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1166, 612);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 11;
            pictureBox1.TabStop = false;
            // 
            // Firstform
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(74, 93, 78);
            ClientSize = new Size(1219, 671);
            Controls.Add(roundedPanel1);
            Name = "Firstform";
            Text = "Form1";
            roundedPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button btn_togo;
        private Label label1;
        private Controls.RoundedPanel roundedPanel1;
        private Button btn_KorCh;
        private Button btn_JapCh;
        private Button btn_EngCh;
        private Button btn_herein;
        private PictureBox pictureBox1;
    }
}
