namespace sushi
{
    partial class PointHistoryItem
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
            lbDate = new Label();
            lbReason = new Label();
            lbPoints = new Label();
            label4 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(19, 28);
            label1.Name = "label1";
            label1.Size = new Size(66, 15);
            label1.TabIndex = 0;
            label1.Text = "처리일시 : ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(19, 58);
            label2.Name = "label2";
            label2.Size = new Size(71, 15);
            label2.TabIndex = 0;
            label2.Text = "구분/사유 : ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(19, 88);
            label3.Name = "label3";
            label3.Size = new Size(78, 15);
            label3.TabIndex = 0;
            label3.Text = "변동포인트 : ";
            // 
            // lbDate
            // 
            lbDate.AutoSize = true;
            lbDate.Location = new Point(103, 28);
            lbDate.Name = "lbDate";
            lbDate.Size = new Size(39, 15);
            lbDate.TabIndex = 1;
            lbDate.Text = "label4";
            // 
            // lbReason
            // 
            lbReason.AutoSize = true;
            lbReason.Location = new Point(103, 58);
            lbReason.Name = "lbReason";
            lbReason.Size = new Size(39, 15);
            lbReason.TabIndex = 1;
            lbReason.Text = "label4";
            // 
            // lbPoints
            // 
            lbPoints.AutoSize = true;
            lbPoints.Location = new Point(103, 88);
            lbPoints.Name = "lbPoints";
            lbPoints.Size = new Size(39, 15);
            lbPoints.TabIndex = 1;
            lbPoints.Text = "label4";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(-3, 4);
            label4.Name = "label4";
            label4.Size = new Size(327, 15);
            label4.TabIndex = 2;
            label4.Text = "----------------------------------------------------------------";
            // 
            // PointHistoryItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label4);
            Controls.Add(lbPoints);
            Controls.Add(lbReason);
            Controls.Add(lbDate);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "PointHistoryItem";
            Size = new Size(321, 118);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label lbDate;
        private Label lbReason;
        private Label lbPoints;
        private Label label4;
    }
}
