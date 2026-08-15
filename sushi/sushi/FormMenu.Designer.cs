namespace sushi
{
    partial class FormMenu
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
            tableLayoutPanel1 = new TableLayoutPanel();
            panel1 = new Panel();
            label3 = new Label();
            btnDrink = new Button();
            btnSide = new Button();
            button3 = new Button();
            button4 = new Button();
            button2 = new Button();
            button1 = new Button();
            lbOrder = new Label();
            label1 = new Label();
            panel3 = new Panel();
            btnOrder = new Button();
            label2 = new Label();
            flpMenu = new FlowLayoutPanel();
            label4 = new Label();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);
            tableLayoutPanel1.Controls.Add(panel3, 0, 2);
            tableLayoutPanel1.Controls.Add(flpMenu, 0, 1);
            tableLayoutPanel1.Location = new Point(1, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 128F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 79F));
            tableLayoutPanel1.Size = new Size(363, 753);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(label3);
            panel1.Controls.Add(btnDrink);
            panel1.Controls.Add(btnSide);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button4);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(lbOrder);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(357, 122);
            panel1.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(-10, 21);
            label3.Name = "label3";
            label3.Size = new Size(402, 15);
            label3.TabIndex = 3;
            label3.Text = "-------------------------------------------------------------------------------";
            // 
            // btnDrink
            // 
            btnDrink.Location = new Point(237, 77);
            btnDrink.Name = "btnDrink";
            btnDrink.Size = new Size(120, 42);
            btnDrink.TabIndex = 2;
            btnDrink.Text = "음료";
            btnDrink.UseVisualStyleBackColor = true;
            // 
            // btnSide
            // 
            btnSide.Location = new Point(119, 77);
            btnSide.Name = "btnSide";
            btnSide.Size = new Size(120, 42);
            btnSide.TabIndex = 2;
            btnSide.Text = "사이드/면/디저트";
            btnSide.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(237, 36);
            button3.Name = "button3";
            button3.Size = new Size(120, 42);
            button3.TabIndex = 2;
            button3.Text = "롤/마끼";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(0, 77);
            button4.Name = "button4";
            button4.Size = new Size(120, 42);
            button4.TabIndex = 2;
            button4.Text = "단품/기타초밥";
            button4.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(119, 36);
            button2.Name = "button2";
            button2.Size = new Size(120, 42);
            button2.TabIndex = 1;
            button2.Text = "해산물";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(0, 36);
            button1.Name = "button1";
            button1.Size = new Size(120, 42);
            button1.TabIndex = 0;
            button1.Text = "활어/참치";
            button1.UseVisualStyleBackColor = true;
            // 
            // lbOrder
            // 
            lbOrder.AutoSize = true;
            lbOrder.Location = new Point(292, 6);
            lbOrder.Name = "lbOrder";
            lbOrder.Size = new Size(55, 15);
            lbOrder.TabIndex = 1;
            lbOrder.Text = "주문방법";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 6);
            label1.Name = "label1";
            label1.Size = new Size(43, 15);
            label1.TabIndex = 1;
            label1.Text = "초밥집";
            // 
            // panel3
            // 
            panel3.Controls.Add(label4);
            panel3.Controls.Add(btnOrder);
            panel3.Controls.Add(label2);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(3, 677);
            panel3.Name = "panel3";
            panel3.Size = new Size(357, 73);
            panel3.TabIndex = 2;
            // 
            // btnOrder
            // 
            btnOrder.Location = new Point(230, 1);
            btnOrder.Name = "btnOrder";
            btnOrder.Size = new Size(126, 73);
            btnOrder.TabIndex = 1;
            btnOrder.Text = "주문하기";
            btnOrder.UseVisualStyleBackColor = true;
            btnOrder.Click += btnOrder_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 39);
            label2.Name = "label2";
            label2.Size = new Size(126, 15);
            label2.TabIndex = 0;
            label2.Text = "장바구니 0개 / 총 0원";
            // 
            // flpMenu
            // 
            flpMenu.AutoScroll = true;
            flpMenu.Dock = DockStyle.Fill;
            flpMenu.FlowDirection = FlowDirection.TopDown;
            flpMenu.Location = new Point(3, 131);
            flpMenu.Name = "flpMenu";
            flpMenu.Size = new Size(357, 540);
            flpMenu.TabIndex = 3;
            flpMenu.WrapContents = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 13);
            label4.Name = "label4";
            label4.Size = new Size(137, 15);
            label4.TabIndex = 2;
            label4.Text = "최소주문금액 : 12000원";
            // 
            // FormMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(363, 753);
            Controls.Add(tableLayoutPanel1);
            Name = "FormMenu";
            StartPosition = FormStartPosition.CenterParent;
            Text = "메뉴";
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private Button button3;
        private Button button2;
        private Button button1;
        private Label lbOrder;
        private Label label1;
        private Panel panel3;
        private Button btnOrder;
        private Label label2;
        private FlowLayoutPanel flpMenu;
        private Button btnDrink;
        private Button btnSide;
        private Button button4;
        private Label label3;
        private Label label4;
    }
}