using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SushiKioskAdmin.Views
{
    public partial class UcStockManagement : UserControl
    {
        private DataTable stockTable;

        public UcStockManagement()
        {
            InitializeComponent();
            InitStockData();
        }

        // ==========================================
        // 1. 초기화 및 더미 데이터 로드
        // ==========================================

        private void InitStockData()
        {
            // 재고 데이터테이블 컬럼 구조 생성
            stockTable = new DataTable();
            stockTable.Columns.Add("품목코드", typeof(string));
            stockTable.Columns.Add("품목명", typeof(string));
            stockTable.Columns.Add("현재재고", typeof(int));
            stockTable.Columns.Add("단위", typeof(string));
            stockTable.Columns.Add("최근주문수량", typeof(int));
            stockTable.Columns.Add("주문상태", typeof(string));

            // CSV 파일에서 데이터 읽기
            LoadStockFromCsv();

            // 그리드뷰 바인딩 및 표시 설정
            dgvStockList.DataSource = stockTable;
            dgvStockList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStockList.Columns["현재재고"].DefaultCellStyle.Format = "N0";
            dgvStockList.Columns["최근주문수량"].DefaultCellStyle.Format = "N0";

            // DataGridView 헤더 회색 고정 (연파란색 스타일 완전히 제거)
            dgvStockList.EnableHeadersVisualStyles = false;
            dgvStockList.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvStockList.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;

            // 열 헤더 높이를 원하는 크기(예: 40픽셀)로 지정
            dgvStockList.ColumnHeadersHeight = 30;

            // 높이를 자동으로 늘어나지 않게 고정
            dgvStockList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }
        private void LoadStockFromCsv()
        {
            // 프로그램 실행 폴더에서 stock_test.csv 찾기
            string csvPath = Path.Combine(Application.StartupPath, "stock.csv" );

            // 파일이 없으면 알림
            if (!File.Exists(csvPath))
            {
                MessageBox.Show("stock_test.csv 파일을 찾을 수 없습니다.", "경고");
                return;
            }

            // CSV 전체 읽기
            string[] lines = File.ReadAllLines(csvPath, Encoding.UTF8);
            foreach (string line in lines)
            {
                // 빈 줄 무시
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                string[] parts = line.Split(',');
                if (parts.Length >= 3)
                {
                    // CSV의 1, 2, 3... 번호를 숫자로 변환
                    if (int.TryParse(parts[0].Trim(), out int stockIndex))
                    {
                        string stockName = parts[1].Trim();
                        string unit = parts[2].Trim();
                        string stockCode = "STK-" + stockIndex.ToString("D3");

                        // CSV에 없는 값들은 초기값 설정
                        int currentStock = 0;
                        int lastOrderQty = 0;
                        string orderStatus = "대기중";
                        stockTable.Rows.Add(stockCode, stockName, currentStock, unit, lastOrderQty,  orderStatus
                        );
                    }
                }
            }
        }

        // ==========================================
        // 2. 디자이너 연결 이벤트 핸들러
        // ==========================================

        // [주문 요청] 버튼 클릭
        private void btnOrder_Click(object sender, EventArgs e)
        {
            if (dgvStockList.CurrentRow == null || dgvStockList.CurrentRow.Index < 0)
            {
                MessageBox.Show("주문할 품목을 목록에서 선택해 주세요.", "안내");
                return;
            }

            int orderQty = (int)numOrderQty.Value;
            if (orderQty <= 0)
            {
                MessageBox.Show("주문 수량을 1개 이상 입력해 주세요.", "안내");
                return;
            }

            if (dgvStockList.CurrentRow.DataBoundItem is DataRowView rowView)
            {
                string itemName = rowView["품목명"].ToString();
                string unit = rowView["단위"].ToString();

                // 1. 기존 현재재고에 주문 수량 즉시 더하기 (원하시는 경우)
                int currentStock = Convert.ToInt32(rowView["현재재고"]);
                rowView["현재재고"] = currentStock + orderQty;

                // 2. 최근 주문 수량 및 상태 업데이트
                rowView["최근주문수량"] = orderQty;
                rowView["주문상태"] = "주문완료";

                // 3. 그리드뷰 강제 새로고침 및 하단 라벨 갱신 연동
                dgvStockList.Refresh();

                // 하단 라벨(lblStock) 즉시 갱신
                lblStock.Text = $"{Convert.ToInt32(rowView["현재재고"]):N0} {unit}";

                MessageBox.Show($"[{itemName}] {orderQty:N0}{unit} 주문 요청 및 재고가 반영되었습니다.", "주문 성공");
                numOrderQty.Value = 1;
            }
        }

        // [주문 취소] 버튼 클릭
        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (dgvStockList.CurrentRow == null || dgvStockList.CurrentRow.Index < 0)
            {
                MessageBox.Show("주문을 취소할 품목을 선택해 주세요.", "안내");
                return;
            }

            if (dgvStockList.CurrentRow.DataBoundItem is DataRowView rowView)
            {
                if (rowView["주문상태"].ToString() == "대기중")
                {
                    MessageBox.Show("선택한 품목은 진행 중인 주문이 없습니다.", "안내");
                    return;
                }

                // 1. 주문했던 수량만큼 현재 재고에서 다시 차감
                int cancelQty = Convert.ToInt32(rowView["최근주문수량"]);
                int currentStock = Convert.ToInt32(rowView["현재재고"]);

                rowView["현재재고"] = currentStock - cancelQty;

                // 2. 주문 정보 초기화
                rowView["최근주문수량"] = 0;
                rowView["주문상태"] = "대기중";

                // 3. 그리드뷰 및 하단 라벨 즉시 갱신
                dgvStockList.Refresh();
                lblStock.Text = $"{Convert.ToInt32(rowView["현재재고"]):N0} {rowView["단위"]}";

                MessageBox.Show($"[{rowView["품목명"]}] 주문이 취소되어 재고가 원복되었습니다.", "알림");
            }
        }

        // 그리드 항목 선택 변경 시 하단 라벨 정보 갱신
        private void dgvStockList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStockList.SelectedRows.Count > 0 &&
                dgvStockList.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                lblItem.Text = rowView["품목명"].ToString();
                lblStock.Text = $"{Convert.ToInt32(rowView["현재재고"]):N0} {rowView["단위"]}";
            }
            else
            {
                lblItem.Text = "-";
                lblStock.Text = "-";
            }
        }
    }
}