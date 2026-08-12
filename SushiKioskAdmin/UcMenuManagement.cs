using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SushiKioskAdmin.Views
{
    public partial class UcMenuManagement : UserControl
    {
        private DataTable menuTable;

        public UcMenuManagement()
        {
            InitializeComponent();
            InitMenuData();
        }

        // ==========================================
        // 1. 초기화 및 CSV 데이터 로드
        // ==========================================

        private void InitMenuData()

        {
            cmbCategory.SelectedIndex = 0;

            // 데이터테이블 컬럼 구조 생성 (접시등급 열 제외)
            menuTable = new DataTable();
            menuTable.Columns.Add("메뉴ID", typeof(int));
            menuTable.Columns.Add("메뉴명", typeof(string));
            menuTable.Columns.Add("가격", typeof(int));
            menuTable.Columns.Add("품절여부", typeof(string));

            // CSV 파일 읽어서 데이터 로드
            LoadMenuFromCsv();

            // 그리드뷰 바인딩 및 표시 설정
            dgvMenuList.DataSource = menuTable;
            dgvMenuList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMenuList.Columns["가격"].DefaultCellStyle.Format = "N0";

            // 헤더 회색 고정 (연파란색 하이라이트 제거)
            dgvMenuList.EnableHeadersVisualStyles = false;
            dgvMenuList.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvMenuList.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
        }

        /// <summary>
        /// CSV 파일 읽어서 메뉴 ID, 메뉴명, 가격 정보 로드
        /// </summary>
        private void LoadMenuFromCsv()
        {
            string csvPath = Path.Combine(Application.StartupPath, "susi_menu.csv");

            if (!File.Exists(csvPath))
            {
                MessageBox.Show("susi_menu.csv 파일을 찾을 수 없습니다.", "경고");
                return;
            }

            string[] lines = File.ReadAllLines(csvPath, Encoding.UTF8);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // CSV 구조: 0:ID, 1:한글명, 2:일어명, 3:영어명, 4:가격
                string[] parts = line.Split(',');
                if (parts.Length >= 5)
                {
                    if (int.TryParse(parts[0].Trim(), out int menuId) &&
                        int.TryParse(parts[4].Trim(), out int price))
                    {
                        string menuName = parts[1].Trim();
                        menuTable.Rows.Add(menuId, menuName, price, "판매중");
                    }
                }
            }
        }

        /// <summary>
        /// 선택한 카테고리 텍스트에서 가격(숫자)을 추출
        /// </summary>
        private int GetPriceFromCategory(string category)
        {
            if (string.IsNullOrEmpty(category)) return 1000;

            if (category.Contains("1,000")) return 1000;
            if (category.Contains("1,500")) return 1500;
            if (category.Contains("2,000")) return 2000;
            if (category.Contains("3,000")) return 3000;
            if (category.Contains("5,000")) return 5000;
            if (category.Contains("6,000")) return 6000;

            return 1000;
        }

        /// <summary>
        /// 가격 및 메뉴명에 따라 카테고리 콤보박스 항목 자동 지정
        /// </summary>
        private string GetCategoryNameByPrice(int price, string menuName)
        {
            if (menuName.Contains("사이다") || menuName.Contains("콜라") || menuName.Contains("음료"))
                return "🥤 1,000원 음료";

            switch (price)
            {
                case 1000: return "🔴 1,000원 메뉴";
                case 1500: return "🔴 1,500원 메뉴";
                case 2000: return "🔴 2,000원 사이드/디저트";
                case 3000: return "🔴 3,000원 메뉴";
                case 5000: return "🔴 5,000원 면류";
                case 6000: return "🔴 6,000원 프리미엄";
                default: return "🔴 1,000원 메뉴";
            }
        }

        // ==========================================
        // 2. 디자이너 연결 이벤트 핸들러
        // ==========================================

        // [신규 메뉴 등록] 버튼 클릭
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string name = txtMenuName.Text.Trim();
            string category = cmbCategory.SelectedItem?.ToString() ?? "";

            // 카테고리 선택 값에서 자동 가격 산정
            int price = GetPriceFromCategory(category);

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("메뉴 이름을 입력해 주세요.", "안내");
                return;
            }

            int newId = menuTable.Rows.Count;
            menuTable.Rows.Add(newId, name, price, "판매중");
            MessageBox.Show($"[{name}] 메뉴가 추가되었습니다. (가격: {price:N0}원)", "알림");
            ClearInputs();
        }

        // [메뉴 정보 수정] 버튼 클릭
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvMenuList.SelectedRows.Count == 0)
            {
                MessageBox.Show("수정할 메뉴를 우측 목록에서 선택해 주세요.", "안내");
                return;
            }

            if (dgvMenuList.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                string category = cmbCategory.SelectedItem?.ToString() ?? "";
                int price = GetPriceFromCategory(category);

                rowView["메뉴명"] = txtMenuName.Text.Trim();
                rowView["가격"] = price;

                MessageBox.Show($"[{rowView["메뉴명"]}] 정보가 수정되었습니다. (가격: {price:N0}원)", "알림");
            }
        }

        // [품절 처리] 버튼 클릭
        private void btnSoldOut_Click(object sender, EventArgs e) => ChangeStatus("품절");

        // [판매 재개] 버튼 클릭
        private void btnSalesResume_Click(object sender, EventArgs e) => ChangeStatus("판매중");

        // 그리드 항목 선택 변경 시 좌측 입력폼 자동 채움
        private void DgvMenuList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMenuList.SelectedRows.Count > 0 &&
                dgvMenuList.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                string menuName = rowView["메뉴명"].ToString();
                int price = Convert.ToInt32(rowView["가격"]);

                txtMenuName.Text = menuName;

                // 가격을 기반으로 카테고리 콤보박스 항목 자동 선택
                cmbCategory.SelectedItem = GetCategoryNameByPrice(price, menuName);
            }
        }

        // ==========================================
        // 3. 내부 헬퍼 메서드
        // ==========================================

        private void ChangeStatus(string newStatus)
        {
            if (dgvMenuList.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "상태를 변경할 메뉴를 목록에서 먼저 선택해 주세요.",
                    "안내"
                );
                return;
            }

            if (dgvMenuList.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                rowView["품절여부"] = newStatus;

                dgvMenuList.Refresh();

                MessageBox.Show(
                    $"[{rowView["메뉴명"]}] 메뉴가 [{newStatus}] 상태로 변경되었습니다.",
                    "알림"
                );
            }
        }

        private void ClearInputs()
        {
            txtMenuName.Clear();
        }
    }
}