using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

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
        // 1. 초기화 및 CSV 데이터 로드
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
            historyTable.Columns.Add("결제일시", typeof(DateTime));
            historyTable.Columns.Add("출처", typeof(string));
            historyTable.Columns.Add("수령방식", typeof(string));
            historyTable.Columns.Add("결제금액", typeof(int));
            historyTable.Columns.Add("결제수단", typeof(string));

            // susi_sales_history.csv 파일에서 데이터 로드
            LoadHistoryFromCsv();

            // 그리드뷰 바인딩 및 표시 설정
            dgvHistoryList.DataSource = historyTable;
            dgvHistoryList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistoryList.Columns["결제금액"].DefaultCellStyle.Format = "N0";
            dgvHistoryList.Columns["결제일시"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";

            // 헤더 회색 고정
            dgvHistoryList.EnableHeadersVisualStyles = false;
            dgvHistoryList.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvHistoryList.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
            dgvHistoryList.ColumnHeadersHeight = 35;
            dgvHistoryList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        /// <summary>
        /// susi_sales_history.csv 파일을 읽어와서 historyTable에 채움
        /// CSV 구조: ReceiptNo, PaymentDate, Source, OrderType, TotalAmount, PaymentMethod
        /// </summary>
        private void LoadHistoryFromCsv()
        {
            historyTable.Clear();
            string historyPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");
            if (!File.Exists(historyPath)) return;

            string[] lines = File.ReadAllLines(historyPath, Encoding.UTF8);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(',');

                // 구조: 영수증번호(0), 결제일시(1), 주문출처(2), 수령방식(3), 결제금액(4), 결제수단(5)
                if (parts.Length >= 6)
                {
                    string receiptNo = parts[0].Trim();
                    if (DateTime.TryParse(parts[1].Trim(), out DateTime paymentDate))
                    {
                        string source = parts[2].Trim();
                        string orderType = parts[3].Trim();
                        int totalAmount = int.TryParse(parts[4].Trim(), out int amt) ? amt : 0;
                        string payMethod = parts[5].Trim();

                        historyTable.Rows.Add(receiptNo, paymentDate, source, orderType, totalAmount, payMethod);
                    }
                }
            }
        }

        // ==========================================
        // 2. 디자이너 연결 이벤트 핸들러
        // ==========================================

        // [조회] 버튼 클릭 시 출처 및 날짜 조건 필터링
        private void btnSearch_Click(object sender, EventArgs e)
        {
            // 조회 버튼 누를 때 최신 CSV 파일 내용을 다시 불러와서 검색하면 더욱 정확합니다.
            LoadHistoryFromCsv();

            DataView dv = historyTable.DefaultView;
            string selectedType = cmbOrderType.SelectedItem?.ToString() ?? "전체";

            DateTime startDate = dtpStart.Value.Date;
            DateTime endDate = dtpEnd.Value.Date.AddDays(1).AddSeconds(-1); // 당일 23:59:59까지 포함

            // 출처 조건 생성
            string typeFilter = selectedType == "전체" ? "" : $"출처 = '{selectedType}'";

            // 날짜 범위 조건 생성
            string dateFilter = $"결제일시 >= #{startDate:yyyy-MM-dd HH:mm:ss}# AND 결제일시 <= #{endDate:yyyy-MM-dd HH:mm:ss}#";

            // 최종 필터 결합
            if (string.IsNullOrEmpty(typeFilter))
                dv.RowFilter = dateFilter;
            else
                dv.RowFilter = $"{typeFilter} AND {dateFilter}";

            MessageBox.Show("조회가 완료되었습니다.", "안내");
        }

        // 그리드 항목 선택 시 susi_order_items.csv와 연동하여 실제 영수증 템플릿 생성
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
                sb.AppendLine("            [ 초밥 키오스크 영수증 ]          ");
                sb.AppendLine("==========================================");
                sb.AppendLine($"영수증번호 : {orderNo}");
                sb.AppendLine($"결제일시 : {orderDate}");
                sb.AppendLine($"주문유형 : [{source}] - {type}");
                sb.AppendLine("------------------------------------------");
                sb.AppendLine(" 상품명                수량     금액(SubTotal)");
                sb.AppendLine("------------------------------------------");

                // susi_order_items.csv에서 해당 영수증 번호와 일치하는 품목들을 동적으로 로드
                LoadReceiptItems(orderNo, sb);

                sb.AppendLine("------------------------------------------");
                sb.AppendLine($" 합계금액 :                     {totalAmount:N0}원");
                sb.AppendLine($" 결제수단 :                     {payMethod}");
                sb.AppendLine("==========================================");
                sb.AppendLine("           이용해 주셔서 감사합니다!          ");
                sb.AppendLine("==========================================");

                txtReceipt.Text = sb.ToString();
            }
        }

        /// <summary>
        /// susi_order_items.csv에서 영수증 번호(KeyId)에 해당하는 상세 품목을 읽어와서 영수증 텍스트에 추가
        /// CSV 구조: KeyId, MenuName, Price, Quantity, DiscountQty, SubTotal
        /// </summary>
        private void LoadReceiptItems(string receiptNo, StringBuilder sb)
        {
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");
            if (!File.Exists(itemsPath))
            {
                sb.AppendLine(" 상세 주문 내역이 없습니다.");
                return;
            }

            string[] lines = File.ReadAllLines(itemsPath, Encoding.UTF8);
            bool hasItem = false;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(',');
                if (parts.Length >= 6)
                {
                    string keyId = parts[0].Trim();
                    if (keyId.Equals(receiptNo, StringComparison.OrdinalIgnoreCase))
                    {
                        string menuName = parts[1].Trim();
                        int qty = int.TryParse(parts[3].Trim(), out int q) ? q : 1;
                        int discountQty = int.TryParse(parts[4].Trim(), out int dq) ? dq : 0;
                        int subTotal = int.TryParse(parts[5].Trim(), out int st) ? st : 0;

                        // 할인 수량이 있는 경우 표시 방식 개선
                        if (discountQty > 0)
                        {
                            sb.AppendLine($" {menuName} (할인{discountQty}개 포함)  {qty}개    {subTotal:N0}원");
                        }
                        else
                        {
                            sb.AppendLine($" {menuName}                    {qty}개    {subTotal:N0}원");
                        }
                        hasItem = true;
                    }
                }
            }

            if (!hasItem)
            {
                sb.AppendLine(" 해당 주문의 상세 품목을 찾을 수 없습니다.");
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
            dgvHistoryList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvHistoryList.ColumnHeadersHeight = 35;

            foreach (DataGridViewColumn col in dgvHistoryList.Columns)
            {
                col.HeaderCell.Style.WrapMode = DataGridViewTriState.False;
            }
        }
    }
}