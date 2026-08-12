using System;
using System.Drawing;
using System.Windows.Forms;

namespace SushiKioskAdmin.Views
{
    public partial class UcTableMonitor : UserControl
    {
        public UcTableMonitor()
        {
            InitializeComponent();
            LoadTableCards();
        }

        // ==========================================
        // 1. 테이블 카드 동적 생성 및 화면 배치
        // ==========================================

        private void LoadTableCards()
        {
            flpTables.Controls.Clear();

            // 총 10개 테이블 동적 생성
            for (int i = 1; i <= 34; i++)
            {
                // 테스트용 초기 상태 설정 (1, 3, 5번 테이블은 착석 중)
                bool isOccupied = (i == 1 || i == 3 || i == 5);
                string amount = isOccupied ? $"{(i * 12000):N0}원" : "0원";
                string statusText = isOccupied ? "식사 중" : "빈 테이블";

                // 테이블 UI 버튼 카드 생성
                Button btnTable = new Button
                {
                    Width = 160,
                    Height = 130,
                    Margin = new Padding(10),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                    TextAlign = ContentAlignment.TopLeft,
                    Text = $" Table {i:D2}\n\n [{statusText}]\n 금액: {amount}",
                    Tag = i, // 테이블 번호 저장
                    BackColor = isOccupied ? Color.FromArgb(231, 76, 60) : Color.FromArgb(46, 204, 113), // 빨강(사용중), 초록(빈테이블)
                    ForeColor = Color.White
                };

                btnTable.FlatAppearance.BorderSize = 0;
                btnTable.Click += TableCard_Click;

                flpTables.Controls.Add(btnTable);
            }
        }

        // ==========================================
        // 2. 디자이너 연결 이벤트 핸들러 (테이블 클릭)
        // ==========================================

        private void TableCard_Click(object sender, EventArgs e)
        {
            if (sender is Button btnTable && btnTable.Tag is int tableNo)
            {
                // 이미 빈 테이블인 경우 처리 제외
                if (btnTable.Text.Contains("빈 테이블"))
                {
                    MessageBox.Show($"Table {tableNo:D2}번은 현재 빈 테이블입니다.", "안내");
                    return;
                }

                // 퇴장 및 테이블 비우기 확인 창
                DialogResult result = MessageBox.Show(
                    $"Table {tableNo:D2}번의 퇴장 처리(테이블 비우기)를 진행하시겠습니까?",
                    "테이블 정산 관리",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // 초록색 빈 테이블 상태로 업데이트
                    btnTable.BackColor = Color.FromArgb(46, 204, 113);
                    btnTable.Text = $" Table {tableNo:D2}\n\n [빈 테이블]\n 금액: 0원";

                    MessageBox.Show($"Table {tableNo:D2}번 정산이 완료되어 빈 테이블로 변경되었습니다.", "알림");
                }
            }
        }
    }
}