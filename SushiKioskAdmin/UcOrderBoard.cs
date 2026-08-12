using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SushiKioskAdmin.Views
{
    public partial class UcOrderBoard : UserControl
    {
        private DataTable orderTable;

        public UcOrderBoard()
        {
            InitializeComponent();
            InitOrderData();
        }

        // ==========================================
        // 1. 초기화 메서드
        // ==========================================

        private void InitOrderData()
        {
            // 주문 데이터테이블 컬럼 구조 생성
            orderTable = new DataTable();
            orderTable.Columns.Add("주문번호");
            orderTable.Columns.Add("주문출처"); // [앱] 또는 [키오스크]
            orderTable.Columns.Add("수령방식"); // [배달], [포장], [매장]
            orderTable.Columns.Add("주문시간");
            orderTable.Columns.Add("주문내역");
            orderTable.Columns.Add("금액");
            orderTable.Columns.Add("현재상태");

            // 샘플 더미 데이터 생성
            orderTable.Rows.Add("101", "앱", "배달", "12:05", "연어초밥(10p) 1개", "15,000원", "접수 대기");
            orderTable.Rows.Add("102", "키오스크", "매장", "12:08", "Table 03 - 모듬초밥 2개", "32,000원", "조리 중");
            orderTable.Rows.Add("103", "앱", "포장", "12:10", "광어초밥(10p) 1개", "16,000원", "접수 대기");
            orderTable.Rows.Add("104", "키오스크", "포장", "12:12", "참치초밥(10p) 1개", "20,000원", "조리 중");

            // 그리드뷰 데이터 바인딩 및 기본 옵션 설정
            dgvOrders.DataSource = orderTable;
            dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 그리드뷰 헤더 회색 고정 (연파란색 스타일 제거)
            dgvOrders.EnableHeadersVisualStyles = false;
            dgvOrders.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvOrders.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;

            // 열 헤더 높이를 원하는 크기(예: 40픽셀)로 지정
            dgvOrders.ColumnHeadersHeight = 30;

            // 높이를 자동으로 늘어나지 않게 고정
            dgvOrders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        // ==========================================
        // 2. 디자이너 연결 이벤트 핸들러 (버튼 및 라디오)
        // ==========================================

        // [상단 필터] 라디오 버튼 상태 변경 이벤트
        private void FilterOrders_CheckedChanged(object sender, EventArgs e)
        {
            // 체크된 라디오 버튼만 동작
            if (sender is RadioButton rdo && rdo.Checked)
            {
                DataView dv = orderTable.DefaultView;

                if (rdo == rdoAll) dv.RowFilter = "";
                else if (rdo == rdoApp) dv.RowFilter = "주문출처 = '앱'";
                else if (rdo == rdoKiosk) dv.RowFilter = "주문출처 = '키오스크'";
                else if (rdo == rdoWaiting) dv.RowFilter = "주문출처 = '앱' AND 현재상태 = '접수 대기'";
            }
        }

        // [하단 버튼] 앱 주문 수락 (조리 중)
        private void btnAccept_Click(object sender, EventArgs e) => ProcessAppOrder("조리 중");

        // [하단 버튼] 앱 주문 거절
        private void btnReject_Click(object sender, EventArgs e) => ProcessAppOrder("주문 거절");

        // [하단 버튼] 조리 완료
        private void btnCookDone_Click(object sender, EventArgs e) => ProcessAppOrder("조리 완료");

        // ==========================================
        // 3. 비즈니스 로직 (주문 상태 변경 함수)
        // ==========================================

        /// <summary>
        /// 앱 주문에 한해서 상태(조리 중, 주문 거절, 조리 완료) 변경 처리
        /// </summary>
        private void ProcessAppOrder(string newStatus)
        {
            // 선택된 행 유효성 검사
            if (dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("처리할 주문을 먼저 선택해 주세요.", "안내");
                return;
            }

            // 현재 선택된 행의 데이터 가져오기
            if (dgvOrders.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                string source = rowView["주문출처"].ToString();

                // 키오스크 주문 수동 조작 차단 예외 처리
                if (source == "키오스크")
                {
                    MessageBox.Show("키오스크 주문은 자동 처리됩니다.\n수락/거절/조리완료 처리는 [앱 주문]만 가능합니다.", "안내");
                    return;
                }

                // 앱 주문 상태 업데이트
                rowView["현재상태"] = newStatus;

                // ★ [핵심 추가] 상태가 바뀐 후 현재 체크된 라디오 버튼 조건에 따라 그리드 필터 즉시 갱신
                ApplyCurrentFilter();

                MessageBox.Show($"앱 주문 [{rowView["주문번호"]}]번의 상태가 [{newStatus}](으)로 변경되었습니다.", "알림");
            }
        }

        /// <summary>
        /// 현재 체크되어 있는 라디오 버튼의 상태를 기준으로 그리드 필터를 재적용하는 헬퍼 메서드
        /// </summary>
        private void ApplyCurrentFilter()
        {
            DataView dv = orderTable.DefaultView;

            if (rdoAll.Checked) dv.RowFilter = "";
            else if (rdoApp.Checked) dv.RowFilter = "주문출처 = '앱'";
            else if (rdoKiosk.Checked) dv.RowFilter = "주문출처 = '키오스크'";
            else if (rdoWaiting.Checked) dv.RowFilter = "주문출처 = '앱' AND 현재상태 = '접수 대기'";
        }

        private void UcOrderBoard_Load(object sender, EventArgs e)
        {
            // 화면에 완전히 로드된 이후 안전하게 열 너비 설정
            if (dgvOrders.Columns.Contains("주문번호"))
            {
                dgvOrders.Columns["주문번호"].Width = 90;
                dgvOrders.Columns["주문번호"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            if (dgvOrders.Columns.Contains("주문출처"))
            {
                dgvOrders.Columns["주문출처"].Width = 90;
                dgvOrders.Columns["주문출처"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            if (dgvOrders.Columns.Contains("수령방식"))
            {
                dgvOrders.Columns["수령방식"].Width = 90;
                dgvOrders.Columns["수령방식"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            if (dgvOrders.Columns.Contains("주문시간"))
            {
                dgvOrders.Columns["주문시간"].Width = 90;
                dgvOrders.Columns["주문시간"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            if (dgvOrders.Columns.Contains("금액"))
            {
                dgvOrders.Columns["금액"].Width = 100;
                dgvOrders.Columns["금액"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            if (dgvOrders.Columns.Contains("현재상태"))
            {
                dgvOrders.Columns["현재상태"].Width = 100;
                dgvOrders.Columns["현재상태"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }
    }
}