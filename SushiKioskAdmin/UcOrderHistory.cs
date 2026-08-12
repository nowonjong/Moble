using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SushiKioskAdmin.Views
{
    public partial class UcOrderHistory : UserControl
    {
        private DataTable historyTable;

        public UcOrderHistory()
        {
            InitializeComponent();
            InitHistoryData();
        }

        // ==========================================
        // 1. 초기화 및 더미 데이터 로드
        // ==========================================

        private void InitHistoryData()
        {
            // 검색 조건 컨트롤 초기화 (기본 일주일 전~현재)
            cmbOrderType.Items.Clear();
            cmbOrderType.Items.AddRange(new string[] { "전체", "앱", "키오스크" });
            cmbOrderType.SelectedIndex = 0;

            dtpStart.Value = DateTime.Now.AddDays(-7);
            dtpEnd.Value = DateTime.Now;

            // 데이터테이블 컬럼 구조 생성
            historyTable = new DataTable();
            historyTable.Columns.Add("영수증번호", typeof(string));
            historyTable.Columns.Add("결제일시", typeof(DateTime)); // 날짜 검색용 DateTime 타입 지정
            historyTable.Columns.Add("출처", typeof(string));
            historyTable.Columns.Add("수령방식", typeof(string));
            historyTable.Columns.Add("결제금액", typeof(int));
            historyTable.Columns.Add("결제수단", typeof(string));

            // 샘플 더미 데이터 추가
            historyTable.Rows.Add("ORD-20260810-01", DateTime.Parse("2026-08-10 12:15:20"), "키오스크", "매장(T03)", 13000, "신용카드");
            historyTable.Rows.Add("ORD-20260810-02", DateTime.Parse("2026-08-10 12:30:45"), "앱", "배달", "24000", "앱선결제");
            historyTable.Rows.Add("ORD-20260811-01", DateTime.Parse("2026-08-11 11:45:10"), "키오스크", "포장", 8500, "신용카드");
            historyTable.Rows.Add("ORD-20260811-02", DateTime.Parse("2026-08-11 12:02:33"), "앱", "포장", 18500, "앱선결제");

            // 그리드뷰 바인딩 및 표시 설정
            dgvHistoryList.DataSource = historyTable;
            dgvHistoryList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistoryList.Columns["결제금액"].DefaultCellStyle.Format = "N0";
            dgvHistoryList.Columns["결제일시"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";

            // 헤더 회색 고정 (연파란색 스타일 제거)
            dgvHistoryList.EnableHeadersVisualStyles = false;
            dgvHistoryList.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvHistoryList.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;

            // 열 헤더 높이를 원하는 크기로 지정
            dgvHistoryList.ColumnHeadersHeight = 30;

            // 높이를 자동으로 늘어나지 않게 고정
            dgvHistoryList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        // ==========================================
        // 2. 디자이너 연결 이벤트 핸들러
        // ==========================================

        // [조회] 버튼 클릭 시 출처 및 날짜 조건 필터링
        private void btnSearch_Click(object sender, EventArgs e)
        {
            DataView dv = historyTable.DefaultView;
            string selectedType = cmbOrderType.SelectedItem?.ToString() ?? "전체";

            DateTime startDate = dtpStart.Value.Date;
            DateTime endDate = dtpEnd.Value.Date.AddDays(1).AddSeconds(-1); // 당일 23:59:59까지 포함

            // 출처 조건 생성
            string typeFilter = selectedType == "전체" ? "" : $"출처 = '{selectedType}'";

            // 날짜 범위 조건 생성
            string dateFilter = $"결제일시 >= '{startDate:yyyy-MM-dd HH:mm:ss}' AND 결제일시 <= '{endDate:yyyy-MM-dd HH:mm:ss}'";

            // 최종 필터 결합
            if (string.IsNullOrEmpty(typeFilter))
                dv.RowFilter = dateFilter;
            else
                dv.RowFilter = $"{typeFilter} AND {dateFilter}";

            MessageBox.Show("조회가 완료되었습니다.", "안내");
        }

        // 그리드 항목 선택 변경 시 영수증 템플릿 생성
        private void dgvHistoryList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHistoryList.SelectedRows.Count == 0) return;

            if (dgvHistoryList.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                string orderNo = rowView["영수증번호"].ToString();
                string orderDate = Convert.ToDateTime(rowView["결제일시"]).ToString("yyyy-MM-dd HH:mm:ss");
                string source = rowView["출처"].ToString();
                string type = rowView["수령방식"].ToString();
                int totalAmount = Convert.ToInt32(rowView["결제금액"]);
                string payMethod = rowView["결제수단"].ToString();

                // 영수증 텍스트 구성
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("==========================================");
                sb.AppendLine("            [ 초밥 키오스크 영수증 ]        ");
                sb.AppendLine("==========================================");
                sb.AppendLine($"영수증번호 : {orderNo}");
                sb.AppendLine($"결제일시 : {orderDate}");
                sb.AppendLine($"주문유형 : [{source}] - {type}");
                sb.AppendLine("------------------------------------------");
                sb.AppendLine(" 상품명                  수량     금액");
                sb.AppendLine("------------------------------------------");

                // 영수증번호에 따른 품목 상세 (더미)
                if (orderNo.EndsWith("01"))
                {
                    sb.AppendLine(" 광어초밥 (3,000)          2    6,000원");
                    sb.AppendLine(" 연어뱃살초밥 (3,000)      2    6,000원");
                    sb.AppendLine(" 음료(콜라) (1,000)        1    1,000원");
                }
                else
                {
                    sb.AppendLine(" 참치대뱃살초밥 (6,000)    3   18,000원");
                    sb.AppendLine(" 미니 우동 (5,000)         1    5,000원");
                    sb.AppendLine(" 음료(사이다) (1,000)      1    1,000원");
                }

                sb.AppendLine("------------------------------------------");
                sb.AppendLine($" 합계금액 :                     {totalAmount:N0}원");
                sb.AppendLine($" 결제수단 :                     {payMethod}");
                sb.AppendLine("==========================================");
                sb.AppendLine("         이용해 주셔서 감사합니다!        ");
                sb.AppendLine("==========================================");

                txtReceipt.Text = sb.ToString();
            }
        }

        // [영수증 재발행] 버튼 클릭
        private void btnPrintReceipt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReceipt.Text))
            {
                MessageBox.Show("출력할 영수증을 목록에서 먼저 선택해 주세요.", "안내");
                return;
            }

            MessageBox.Show("영수증 프린터로 인쇄 명령을 전송했습니다.", "영수증 재발행", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UcOrderHistory_Load(object sender, EventArgs e)
        {
            // 1. 헤더 영역의 높이를 고정하여 두 줄로 늘어나는 것 방지
            dgvHistoryList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvHistoryList.ColumnHeadersHeight = 35; // 원하는 높이 (예: 30~40 사이)로 지정

            // 2. 헤더 텍스트 줄 바꿈 방지 (가능한 경우 한 줄로 표시)
            foreach (DataGridViewColumn col in dgvHistoryList.Columns)
            {
                col.HeaderCell.Style.WrapMode = DataGridViewTriState.False;
            }
        }
    }
}