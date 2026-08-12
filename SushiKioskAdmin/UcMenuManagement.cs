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
            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;

            // 데이터테이블 컬럼 구조 생성
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

            // 헤더 스타일 회색 고정
            dgvMenuList.EnableHeadersVisualStyles = false;
            dgvMenuList.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvMenuList.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;

            // 열 헤더 높이 및 고정 설정
            dgvMenuList.ColumnHeadersHeight = 30;
            dgvMenuList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        /// <summary>
        /// CSV 파일 읽어서 메뉴 정보 로드 (실행 파일 바로 옆 또는 Resources 폴더 대응)
        /// </summary>
        private void LoadMenuFromCsv()
        {
            string csvPath = Path.Combine(Application.StartupPath, "susi_menu.csv");

            if (!File.Exists(csvPath))
            {
                // 없으면 Resources 폴더 경로도 체크
                csvPath = Path.Combine(Application.StartupPath, "Resources", "susi_menu.csv");
            }

            if (!File.Exists(csvPath))
            {
                MessageBox.Show("susi_menu.csv 파일을 찾을 수 없습니다.", "경고");
                return;
            }

            string[] lines = File.ReadAllLines(csvPath, Encoding.UTF8);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // CSV 구조: 0:ID, 1:한글명, 2:일어명, 3:영어명, 4:가격 (, 5:품절여부 등 확장 가능)
                string[] parts = line.Split(',');
                if (parts.Length >= 5)
                {
                    if (int.TryParse(parts[0].Trim(), out int menuId) &&
                        int.TryParse(parts[4].Trim(), out int price))
                    {
                        string menuName = parts[1].Trim();
                        // CSV에 품절여부 필드가 추가되어 있다면 반영, 아니면 기본 "판매중"
                        string status = (parts.Length >= 6 && !string.IsNullOrWhiteSpace(parts[5])) ? parts[5].Trim() : "판매중";

                        menuTable.Rows.Add(menuId, menuName, price, status);
                    }
                }
            }
        }

        /// <summary>
        /// 변경된 전체 메뉴 목록을 CSV 파일에 저장 (동기화)
        /// </summary>
        private void SaveAllMenusToCsv()
        {
            try
            {
                string csvPath = Path.Combine(Application.StartupPath, "susi_menu.csv");
                StringBuilder sb = new StringBuilder();

                foreach (DataRow row in menuTable.Rows)
                {
                    if (row.RowState == DataRowState.Deleted) continue;

                    int id = Convert.ToInt32(row["메뉴ID"]);
                    string name = row["메뉴명"].ToString();
                    int price = Convert.ToInt32(row["가격"]);
                    string status = row["품절여부"].ToString();

                    // 기존 CSV 구조(ID, 한글명, 일어명, 영어명, 가격, 품절여부) 호환 유지
                    // 일어명/영어명이 따로 없다면 빈 값 또는 이름으로 대체
                    sb.AppendLine($"{id},{name},{name},{name},{price},{status}");
                }

                File.WriteAllText(csvPath, sb.ToString(), new UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                MessageBox.Show("CSV 저장 중 오류가 발생했습니다.\n" + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // 2. 헬퍼 메서드 (가격 및 카테고리 매핑)
        // ==========================================

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
        // 3. 디자이너 연결 이벤트 핸들러
        // ==========================================

        // [신규 메뉴 등록] 버튼 클릭
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string name = txtMenuName.Text.Trim();
            string category = cmbCategory.SelectedItem?.ToString() ?? "";
            int price = GetPriceFromCategory(category);

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("메뉴 이름을 입력해 주세요.", "안내");
                return;
            }

            // 자동 ID 채번 (가장 큰 ID + 1)
            int newId = 1;
            if (menuTable.Rows.Count > 0)
            {
                var maxId = menuTable.Compute("MAX(메뉴ID)", "");
                if (maxId != DBNull.Value) newId = Convert.ToInt32(maxId) + 1;
            }

            menuTable.Rows.Add(newId, name, price, "판매중");
            SaveAllMenusToCsv(); // CSV 동기화 저장

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

                SaveAllMenusToCsv(); // CSV 동기화 저장

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
                cmbCategory.SelectedItem = GetCategoryNameByPrice(price, menuName);
            }
        }

        // ==========================================
        // 4. 내부 헬퍼 메서드
        // ==========================================

        private void ChangeStatus(string newStatus)
        {
            if (dgvMenuList.CurrentRow == null || dgvMenuList.CurrentRow.Index < 0)
            {
                MessageBox.Show("상태를 변경할 메뉴를 목록에서 먼저 선택해 주세요.", "안내");
                return;
            }

            if (dgvMenuList.CurrentRow.DataBoundItem is DataRowView rowView)
            {
                rowView["품절여부"] = newStatus;
                SaveAllMenusToCsv(); // CSV 동기화 저장

                MessageBox.Show($"[{rowView["메뉴명"]}] 메뉴가 [{newStatus}] 상태로 변경되었습니다.", "알림");
            }
        }

        private void ClearInputs()
        {
            txtMenuName.Clear();
        }

        private void UcMenuManagement_Load(object sender, EventArgs e)
        {
            // 반복되던 컬럼 너비 설정을 배열과 반복문으로 깔끔하게 압축
            var columnWidths = new (string ColumnName, int Width)[]
            {
                ("메뉴ID", 80),
                ("메뉴명", 150),
                ("가격", 100),
                ("품절여부", 90)
            };

            foreach (var col in columnWidths)
            {
                if (dgvMenuList.Columns.Contains(col.ColumnName))
                {
                    dgvMenuList.Columns[col.ColumnName].Width = col.Width;
                    dgvMenuList.Columns[col.ColumnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
            }
        }
    }
}