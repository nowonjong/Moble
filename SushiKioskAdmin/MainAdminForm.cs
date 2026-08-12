using SushiKioskAdmin.Views;

namespace SushiKioskAdmin
{
    public partial class MainAdminForm : Form
    {
        // 현재 선택된 사이드바 버튼을 저장하는 변수
        private Button currentSelectedButton;

        public MainAdminForm()
        {
            InitializeComponent();

            // 1. 폼 크기 및 시작 위치 설정 (1024x768 포스기 해상도)
            this.Size = new Size(1024, 768);
            this.MinimumSize = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupSidebarStyle();
        }
        private void SetupSidebarStyle()
        {
            // 사이드바 패널 자체 배경색 지정 (어두운 네이비/그레이)
            pnlSidebar.BackColor = Color.FromArgb(45, 45, 48);

            Button[] navButtons = { btnNavOrder, btnNavTable, btnNavMenu, btnNavHistory, btnNavUser, btnNavStock, btnNavReport };

            foreach (var btn in navButtons)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0; // 테두리 완전히 제거
                btn.Margin = new Padding(0);        // 버튼 간격(여백) 제거
                btn.Padding = new Padding(0);
                btn.Height = 50;                   // 버튼 높이 균일하게 고정
                btn.Dock = DockStyle.Top;          // 위에서부터 착착 붙게 배치 (필요시)

                // 기본 안 선택 상태 색상
                btn.BackColor = Color.FromArgb(45, 45, 48);
                btn.ForeColor = Color.White;

                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 65);
            }
        }

        private void MainAdminForm_Load(object sender, EventArgs e)
        {
            // 2. 관리자 폼 실행 시 기본 첫 화면으로 '실시간 주문 현황판' 로드
            ShowView(new UcOrderBoard(), btnNavOrder);
        }

        private void ShowView(UserControl view, Button clickedButton)
        {
            // 기존에 띄워져 있던 이전 화면 제거 및 메모리 정리
            pnlMainContainer.Controls.Clear();

            // 새로 띄울 서브 화면 설정
            view.Dock = DockStyle.Fill; // 우측 패널 크기에 딱 맞게 채움
            pnlMainContainer.Controls.Add(view);
            view.BringToFront();

            // 클릭된 사이드바 버튼 색상 강조
            HighlightButton(clickedButton);
        }

        private void HighlightButton(Button btn)
        {
            // 1. 이전 버튼 원복 (어두운 배경색)
            if (currentSelectedButton != null)
            {
                currentSelectedButton.BackColor = Color.FromArgb(45, 45, 48);
                currentSelectedButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 65);
            }

            // 2. 현재 선택된 버튼 강조 (밝은 파란색)
            currentSelectedButton = btn;
            if (currentSelectedButton != null)
            {
                Color activeColor = Color.FromArgb(0, 122, 204); // 밝은 파란색

                currentSelectedButton.BackColor = activeColor;
                currentSelectedButton.FlatAppearance.MouseOverBackColor = activeColor;
            }
        }

        /// <summary>
        /// 상단바 알림 메시지를 변경하는 함수 (외부/서브 화면에서 호출 가능) -> 양수 값이면 신규 주문 대기 중, 0이면 모든 주문 처리 완료
        /// </summary>
        public void UpdateNotice(int waitingCount)
        {
            if (waitingCount > 0)
            {
                lblNotice.Text = $"🔔 신규 주문 [{waitingCount}건] 대기 중!";
                lblNotice.ForeColor = Color.Yellow;
            }
            else
            {
                lblNotice.Text = "✅ 모든 주문이 처리되었습니다.";
                lblNotice.ForeColor = Color.LightGreen;
            }
        }

        private void btnNavOrder_Click(object sender, EventArgs e)
        {
            ShowView(new UcOrderBoard(), (Button)sender);
        }

        private void btnNavTable_Click(object sender, EventArgs e)
        {
            ShowView(new UcTableMonitor(), (Button)sender);
        }

        private void btnNavMenu_Click(object sender, EventArgs e)
        {
            ShowView(new UcMenuManagement(), (Button)sender);
        }

        private void btnNavHistory_Click(object sender, EventArgs e)
        {
            ShowView(new UcOrderHistory(), (Button)sender);
        }

        private void btnNavUser_Click(object sender, EventArgs e)
        {
            ShowView(new UcUserManagement(), (Button)sender);
        }

        private void btnNavStock_Click(object sender, EventArgs e)
        {
            ShowView(new UcStockManagement(), (Button)sender);
        }

        private void btnNavReport_Click(object sender, EventArgs e)
        {
            ShowView(new UcSalesReport(), (Button)sender);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("관리자 시스템을 종료하시겠습니까?", "시스템 종료",
                                          MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

    }
}
