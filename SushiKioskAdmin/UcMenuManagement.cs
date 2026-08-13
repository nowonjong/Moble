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
        private string selectedImageFileName = ""; // 선택된 이미지 파일명 보관

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

            // MenuImages 폴더가 없으면 자동으로 생성
            string imageFolderPath = Path.Combine(Application.StartupPath, "MenuImages");
            if (!Directory.Exists(imageFolderPath))
            {
                Directory.CreateDirectory(imageFolderPath);
            }

            // 데이터테이블 컬럼 구조 생성 (7개 컬럼: ID, 한글명, 일어명, 영어명, 가격, 품절여부, 이미지파일명)
            menuTable = new DataTable();
            menuTable.Columns.Add("메뉴ID", typeof(int));
            menuTable.Columns.Add("메뉴명", typeof(string));
            menuTable.Columns.Add("일어명", typeof(string));
            menuTable.Columns.Add("영어명", typeof(string));
            menuTable.Columns.Add("가격", typeof(int));
            menuTable.Columns.Add("품절여부", typeof(string));
            menuTable.Columns.Add("이미지파일명", typeof(string));

            // CSV 파일 읽어서 데이터 로드
            LoadMenuFromCsv();

            // 그리드뷰 바인딩 및 표시 설정
            dgvMenuList.DataSource = menuTable;
            dgvMenuList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMenuList.Columns["가격"].DefaultCellStyle.Format = "N0";

            // ★ 일어명, 영어명, 이미지파일명 열은 그리드뷰에서 숨기기
            if (dgvMenuList.Columns.Contains("일어명"))
                dgvMenuList.Columns["일어명"].Visible = false;
            if (dgvMenuList.Columns.Contains("영어명"))
                dgvMenuList.Columns["영어명"].Visible = false;
            if (dgvMenuList.Columns.Contains("이미지파일명"))
                dgvMenuList.Columns["이미지파일명"].Visible = false;

            // 헤더 스타일 회색 고정
            dgvMenuList.EnableHeadersVisualStyles = false;
            dgvMenuList.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvMenuList.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;

            // 열 헤더 높이 및 고정 설정
            dgvMenuList.ColumnHeadersHeight = 30;
            dgvMenuList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        private void LoadMenuFromCsv()
        {
            string csvPath = Path.Combine(Application.StartupPath, "susi_menu.csv");

            if (!File.Exists(csvPath))
            {
                csvPath = Path.Combine(Application.StartupPath, "Resources", "susi_menu.csv");
            }

            if (!File.Exists(csvPath))
            {
                return; // 파일이 없으면 빈 상태로 시작
            }

            string[] lines = File.ReadAllLines(csvPath, Encoding.UTF8);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // CSV 구조: 0:ID, 1:한글명, 2:일어명, 3:영어명, 4:가격, 5:품절여부(선택), 6:이미지파일명
                string[] parts = line.Split(',');
                if (parts.Length >= 5)
                {
                    if (int.TryParse(parts[0].Trim(), out int menuId) &&
                        int.TryParse(parts[4].Trim(), out int price))
                    {
                        string menuName = parts[1].Trim();
                        string jpName = parts.Length > 2 ? parts[2].Trim() : "";
                        string enName = parts.Length > 3 ? parts[3].Trim() : "";

                        // 기존 6개 컬럼 구조(마지막이 이미지)인지, 7개 컬럼(품절여부 포함)인지 판별
                        string status = "판매중";
                        string imgName = "";

                        if (parts.Length >= 7)
                        {
                            status = string.IsNullOrWhiteSpace(parts[5]) ? "판매중" : parts[5].Trim();
                            imgName = parts[6].Trim();
                        }
                        else if (parts.Length == 6)
                        {
                            // 6개 컬럼일 때 5번째는 기존 이미지 파일명이었음
                            imgName = parts[5].Trim();
                        }

                        menuTable.Rows.Add(menuId, menuName, jpName, enName, price, status, imgName);
                    }
                }
            }
        }

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
                    string jp = row["일어명"].ToString();
                    string en = row["영어명"].ToString();
                    int price = Convert.ToInt32(row["가격"]);
                    string status = row["품절여부"].ToString();
                    string img = row["이미지파일명"].ToString();

                    // 7개 컬럼 구조로 저장: ID, 한글명, 일어명, 영어명, 가격, 품절여부, 이미지파일명
                    sb.AppendLine($"{id},{name},{jp},{en},{price},{status},{img}");
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
            string jpName = txtJapanese.Text.Trim();
            string enName = txtEnglish.Text.Trim();
            string category = cmbCategory.SelectedItem?.ToString() ?? "";
            int price = GetPriceFromCategory(category);

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("메뉴 이름을 입력해 주세요.", "안내");
                return;
            }

            int newId = 1;
            if (menuTable.Rows.Count > 0)
            {
                var maxId = menuTable.Compute("MAX(메뉴ID)", "");
                if (maxId != DBNull.Value) newId = Convert.ToInt32(maxId) + 1;
            }

            menuTable.Rows.Add(newId, name, jpName, enName, price, "판매중", selectedImageFileName);
            SaveAllMenusToCsv();

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
                rowView["일어명"] = txtJapanese.Text.Trim();
                rowView["영어명"] = txtEnglish.Text.Trim();
                rowView["가격"] = price;
                rowView["이미지파일명"] = selectedImageFileName;

                SaveAllMenusToCsv();

                MessageBox.Show($"[{rowView["메뉴명"]}] 정보가 수정되었습니다.", "알림");
            }
        }

        // [품절 처리] 버튼 클릭
        private void btnSoldOut_Click(object sender, EventArgs e) => ChangeStatus("품절");

        // [판매 재개] 버튼 클릭
        private void btnSalesResume_Click(object sender, EventArgs e) => ChangeStatus("판매중");

        // [이미지 선택] 버튼 클릭 - 파일을 MenuImages 폴더로 복사하고 픽처박스에 띄우기
        private void btnBrowseImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "이미지 파일 (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedImageFileName = Path.GetFileName(ofd.FileName);

                    string imageFolderPath = Path.Combine(Application.StartupPath, "MenuImages");
                    if (!Directory.Exists(imageFolderPath))
                    {
                        Directory.CreateDirectory(imageFolderPath);
                    }

                    string targetPath = Path.Combine(imageFolderPath, selectedImageFileName);
                    File.Copy(ofd.FileName, targetPath, true); // 덮어쓰기 허용

                    using (var bmp = new Bitmap(targetPath))
                    {
                        picMenuImage.Image = new Bitmap(bmp);
                    }
                    picMenuImage.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }

        // 그리드 항목 선택 변경 시 좌측 입력폼 및 이미지 자동 연동
        private void DgvMenuList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMenuList.SelectedRows.Count > 0 &&
                dgvMenuList.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                txtMenuName.Text = rowView["메뉴명"].ToString();
                txtJapanese.Text = rowView["일어명"].ToString();
                txtEnglish.Text = rowView["영어명"].ToString();
                int price = Convert.ToInt32(rowView["가격"]);
                cmbCategory.SelectedItem = GetCategoryNameByPrice(price, txtMenuName.Text);

                // MenuImages 폴더 안에서 이미지 파일 로드
                string imgFile = rowView["이미지파일명"]?.ToString() ?? "";
                string imageFolderPath = Path.Combine(Application.StartupPath, "MenuImages");
                string fullPath = Path.Combine(imageFolderPath, imgFile);

                // 만약 CSV에 등록된 이미지명이 없거나 파일이 없다면, "한글메뉴명.png" 등으로 자동 대체 탐색 시도
                if (string.IsNullOrEmpty(imgFile) || !File.Exists(fullPath))
                {
                    string menuName = txtMenuName.Text.Trim();
                    string[] extensions = { ".png", ".jpg", ".jpeg" };
                    foreach (var ext in extensions)
                    {
                        string tentativePath = Path.Combine(imageFolderPath, menuName + ext);
                        if (File.Exists(tentativePath))
                        {
                            fullPath = tentativePath;
                            imgFile = menuName + ext;
                            break;
                        }
                    }
                }

                // 최종 이미지 로드
                if (!string.IsNullOrEmpty(imgFile) && File.Exists(fullPath))
                {
                    try
                    {
                        using (var bmp = new Bitmap(fullPath))
                        {
                            picMenuImage.Image = new Bitmap(bmp);
                        }
                        picMenuImage.SizeMode = PictureBoxSizeMode.StretchImage;
                        selectedImageFileName = imgFile;
                    }
                    catch
                    {
                        picMenuImage.Image = null;
                        selectedImageFileName = "";
                    }
                }
                else
                {
                    picMenuImage.Image = null;
                    selectedImageFileName = "";
                }
            }
        }

        // ==========================================
        // 4. 내부 헬퍼 메서드
        // ==========================================

        // [품절/판매재개 상태 변경 공통 메서드]
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
                SaveAllMenusToCsv();

                // ★ 바인딩 컨텍스트를 강제로 갱신하여 화면 글씨를 즉시 다시 그림
                CurrencyManager cm = (CurrencyManager)dgvMenuList.BindingContext[dgvMenuList.DataSource];
                cm.Refresh();

                MessageBox.Show($"[{rowView["메뉴명"]}] 메뉴가 [{newStatus}] 상태로 변경되었습니다.", "알림");
            }
        }

        private void ClearInputs()
        {
            txtMenuName.Clear();
            txtJapanese.Clear();
            txtEnglish.Clear();
            picMenuImage.Image = null;
            selectedImageFileName = "";
        }

        private void UcMenuManagement_Load(object sender, EventArgs e)
        {
            var columnWidths = new (string ColumnName, int Width)[]
            {
                ("메뉴ID", 70),
                ("메뉴명", 160),
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