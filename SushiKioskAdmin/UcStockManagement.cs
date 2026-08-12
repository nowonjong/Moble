using System;
using System.Data;
using System.Drawing;
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

            // 샘플 더미 데이터 추가
            stockTable.Rows.Add("STK-001", "생연어 (kg)", 5, "kg", 0, "대기중");
            stockTable.Rows.Add("STK-002", "광어필렛 (kg)", 3, "kg", 0, "대기중");
            stockTable.Rows.Add("STK-003", "초밥용 쌀 (kg)", 20, "kg", 0, "대기중");
            stockTable.Rows.Add("STK-004", "초새우 (팩)", 12, "팩", 10, "주문완료");
            stockTable.Rows.Add("STK-005", "김가루/구이김 (속)", 8, "속", 0, "대기중");
            stockTable.Rows.Add("STK-006", "와사비 (kg)", 2, "kg", 5, "주문완료");

            // 그리드뷰 바인딩 및 표시 설정
            dgvStockList.DataSource = stockTable;
            dgvStockList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStockList.Columns["현재재고"].DefaultCellStyle.Format = "N0";
            dgvStockList.Columns["최근주문수량"].DefaultCellStyle.Format = "N0";

            // DataGridView 헤더 회색 고정 (연파란색 스타일 완전히 제거)
            dgvStockList.EnableHeadersVisualStyles = false;
            dgvStockList.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvStockList.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
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

                rowView["최근주문수량"] = orderQty;
                rowView["주문상태"] = "주문완료";

                MessageBox.Show($"[{itemName}] {orderQty:N0}{unit} 주문 요청이 완료되었습니다.", "주문 성공");
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

                rowView["최근주문수량"] = 0;
                rowView["주문상태"] = "대기중";
                MessageBox.Show($"[{rowView["품목명"]}] 주문이 취소되었습니다.", "알림");
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