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

            string imageFolderPath = Path.Combine(Application.StartupPath, "MenuImages");

            if (!Directory.Exists(imageFolderPath))
                Directory.CreateDirectory(imageFolderPath);

            menuTable = new DataTable();
            menuTable.Columns.Add("메뉴ID", typeof(int));
            menuTable.Columns.Add("메뉴명", typeof(string));
            menuTable.Columns.Add("일어명", typeof(string));
            menuTable.Columns.Add("영어명", typeof(string));
            menuTable.Columns.Add("가격", typeof(int));
            menuTable.Columns.Add("품절여부", typeof(string));
            menuTable.Columns.Add("이미지파일명", typeof(string));

            LoadMenuFromCsv();

            dgvMenuList.DataSource = menuTable;
            dgvMenuList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMenuList.Columns["가격"].DefaultCellStyle.Format = "N0";

            if (dgvMenuList.Columns.Contains("일어명"))
                dgvMenuList.Columns["일어명"].Visible = false;

            if (dgvMenuList.Columns.Contains("영어명"))
                dgvMenuList.Columns["영어명"].Visible = false;

            if (dgvMenuList.Columns.Contains("이미지파일명"))
                dgvMenuList.Columns["이미지파일명"].Visible = false;

            dgvMenuList.EnableHeadersVisualStyles = false;
            dgvMenuList.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvMenuList.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;

            dgvMenuList.ColumnHeadersHeight = 30;
            dgvMenuList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        private void LoadMenuFromCsv()
        {
            string csvPath = Path.Combine(Application.StartupPath, "susi_menu.csv");

            if (!File.Exists(csvPath))
                csvPath = Path.Combine(Application.StartupPath, "Resources", "susi_menu.csv");

            if (!File.Exists(csvPath))
                return;

            string[] lines = File.ReadAllLines(csvPath, Encoding.UTF8);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 5)
                    continue;

                if (!int.TryParse(parts[0].Trim(), out int menuId))
                    continue;

                if (!int.TryParse(parts[4].Trim(), out int price))
                    continue;

                string menuName = parts[1].Trim();
                string jpName = parts.Length > 2 ? parts[2].Trim() : "";
                string enName = parts.Length > 3 ? parts[3].Trim() : "";

                string status = "판매중";
                string imgName = "basic.png";

                if (parts.Length >= 7)
                {
                    status = string.IsNullOrWhiteSpace(parts[5])
                        ? "판매중"
                        : parts[5].Trim();

                    imgName = string.IsNullOrWhiteSpace(parts[6])
                        ? "basic.png"
                        : parts[6].Trim();
                }
                else if (parts.Length == 6)
                {
                    imgName = string.IsNullOrWhiteSpace(parts[5])
                        ? "basic.png"
                        : parts[5].Trim();
                }

                menuTable.Rows.Add(
                    menuId,
                    menuName,
                    jpName,
                    enName,
                    price,
                    status,
                    imgName);
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
                    if (row.RowState == DataRowState.Deleted)
                        continue;

                    int id = Convert.ToInt32(row["메뉴ID"]);
                    string name = row["메뉴명"].ToString();
                    string jp = row["일어명"].ToString();
                    string en = row["영어명"].ToString();
                    int price = Convert.ToInt32(row["가격"]);
                    string status = row["품절여부"].ToString();
                    string img = row["이미지파일명"].ToString();

                    if (string.IsNullOrWhiteSpace(img))
                        img = "basic.png";

                    sb.AppendLine($"{id},{name},{jp},{en},{price},{status},{img}");
                }

                File.WriteAllText(csvPath, sb.ToString(), new UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "CSV 저장 중 오류가 발생했습니다.\n" + ex.Message,
                    "오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // 2. 가격 / 카테고리
        // ==========================================

        private int GetPriceFromCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                return 1000;

            if (category.Contains("1,000"))
                return 1000;

            if (category.Contains("1,500"))
                return 1500;

            if (category.Contains("2,000"))
                return 2000;

            if (category.Contains("3,000"))
                return 3000;

            if (category.Contains("5,000"))
                return 5000;

            if (category.Contains("6,000"))
                return 6000;

            return 1000;
        }

        private string GetCategoryNameByPrice(int price, string menuName)
        {
            if (menuName.Contains("사이다") ||
                menuName.Contains("콜라") ||
                menuName.Contains("음료"))
            {
                return "🥤 1,000원 음료";
            }

            switch (price)
            {
                case 1000:
                    return "🔴 1,000원 메뉴";

                case 1500:
                    return "🔴 1,500원 메뉴";

                case 2000:
                    return "🔴 2,000원 사이드/디저트";

                case 3000:
                    return "🔴 3,000원 메뉴";

                case 5000:
                    return "🔴 5,000원 면류";

                case 6000:
                    return "🔴 6,000원 프리미엄";

                default:
                    return "🔴 1,000원 메뉴";
            }
        }

        // ==========================================
        // 3. 신규 메뉴 등록
        // ==========================================

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string name = txtMenuName.Text.Trim();
            string jpName = txtJapanese.Text.Trim();
            string enName = txtEnglish.Text.Trim();

            string category = cmbCategory.SelectedItem?.ToString() ?? "";
            int price = GetPriceFromCategory(category);

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("메뉴 이름을 입력해 주세요.", "안내");
                return;
            }

            if (name.Contains(",") ||
                jpName.Contains(",") ||
                enName.Contains(","))
            {
                MessageBox.Show(
                    "메뉴 이름에는 쉼표(,)를 사용할 수 없습니다.",
                    "안내");
                return;
            }

            int newId = 1;

            if (menuTable.Rows.Count > 0)
            {
                object maxId = menuTable.Compute("MAX(메뉴ID)", "");

                if (maxId != DBNull.Value)
                    newId = Convert.ToInt32(maxId) + 1;
            }

            // 신규 메뉴 이미지는 항상 basic.png
            menuTable.Rows.Add(
                newId,
                name,
                jpName,
                enName,
                price,
                "판매중",
                "basic.png");

            SaveAllMenusToCsv();

            MessageBox.Show(
                $"[{name}] 메뉴가 추가되었습니다. (가격: {price:N0}원)",
                "알림");

            ClearInputs();
        }

        // ==========================================
        // 4. 메뉴 정보 수정
        // ==========================================

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvMenuList.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "수정할 메뉴를 우측 목록에서 선택해 주세요.",
                    "안내");

                return;
            }

            if (dgvMenuList.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                string name = txtMenuName.Text.Trim();
                string jpName = txtJapanese.Text.Trim();
                string enName = txtEnglish.Text.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show(
                        "메뉴 이름을 입력해 주세요.",
                        "안내");

                    return;
                }

                if (name.Contains(",") ||
                    jpName.Contains(",") ||
                    enName.Contains(","))
                {
                    MessageBox.Show(
                        "메뉴 이름에는 쉼표(,)를 사용할 수 없습니다.",
                        "안내");

                    return;
                }

                string category = cmbCategory.SelectedItem?.ToString() ?? "";
                int price = GetPriceFromCategory(category);

                rowView["메뉴명"] = name;
                rowView["일어명"] = jpName;
                rowView["영어명"] = enName;
                rowView["가격"] = price;

                // 이미지파일명은 수정하지 않음.
                // 기존 이미지 그대로 유지.

                SaveAllMenusToCsv();

                CurrencyManager cm =
                    (CurrencyManager)dgvMenuList.BindingContext[dgvMenuList.DataSource];

                cm.Refresh();

                MessageBox.Show(
                    $"[{rowView["메뉴명"]}] 정보가 수정되었습니다.",
                    "알림");
            }
        }

        // ==========================================
        // 5. 품절 / 판매 재개
        // ==========================================

        private void btnSoldOut_Click(object sender, EventArgs e)
        {
            ChangeStatus("품절");
        }

        private void btnSalesResume_Click(object sender, EventArgs e)
        {
            ChangeStatus("판매중");
        }

        private void ChangeStatus(string newStatus)
        {
            if (dgvMenuList.CurrentRow == null ||
                dgvMenuList.CurrentRow.Index < 0)
            {
                MessageBox.Show(
                    "상태를 변경할 메뉴를 목록에서 먼저 선택해 주세요.",
                    "안내");

                return;
            }

            if (dgvMenuList.CurrentRow.DataBoundItem is DataRowView rowView)
            {
                rowView["품절여부"] = newStatus;

                SaveAllMenusToCsv();

                CurrencyManager cm =
                    (CurrencyManager)dgvMenuList.BindingContext[dgvMenuList.DataSource];

                cm.Refresh();

                MessageBox.Show(
                    $"[{rowView["메뉴명"]}] 메뉴가 [{newStatus}] 상태로 변경되었습니다.",
                    "알림");
            }
        }

        // ==========================================
        // 6. 메뉴 선택 시 이미지 보기
        // ==========================================

        private void DgvMenuList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMenuList.SelectedRows.Count == 0)
                return;

            if (!(dgvMenuList.SelectedRows[0].DataBoundItem is DataRowView rowView))
                return;

            txtMenuName.Text = rowView["메뉴명"].ToString();
            txtJapanese.Text = rowView["일어명"].ToString();
            txtEnglish.Text = rowView["영어명"].ToString();

            int price = Convert.ToInt32(rowView["가격"]);

            cmbCategory.SelectedItem =
                GetCategoryNameByPrice(price, txtMenuName.Text);

            string imgFile =
                rowView["이미지파일명"]?.ToString()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(imgFile))
                imgFile = "basic.png";

            string imageFolderPath =
                Path.Combine(Application.StartupPath, "MenuImages");

            string fullPath =
                Path.Combine(imageFolderPath, imgFile);

            // CSV에 등록된 이미지가 실제로 없으면 basic.png 사용
            if (!File.Exists(fullPath))
            {
                imgFile = "basic.png";
                fullPath = Path.Combine(imageFolderPath, imgFile);
            }

            LoadMenuImage(fullPath);
        }

        private void LoadMenuImage(string imagePath)
        {
            if (picMenuImage.Image != null)
            {
                picMenuImage.Image.Dispose();
                picMenuImage.Image = null;
            }

            if (!File.Exists(imagePath))
                return;

            try
            {
                using (Bitmap bmp = new Bitmap(imagePath))
                {
                    picMenuImage.Image = new Bitmap(bmp);
                }

                picMenuImage.SizeMode =
                    PictureBoxSizeMode.StretchImage;
            }
            catch
            {
                picMenuImage.Image = null;
            }
        }

        // ==========================================
        // 7. 입력값 초기화
        // ==========================================

        private void ClearInputs()
        {
            txtMenuName.Clear();
            txtJapanese.Clear();
            txtEnglish.Clear();

            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;

            if (picMenuImage.Image != null)
            {
                picMenuImage.Image.Dispose();
                picMenuImage.Image = null;
            }
        }

        // ==========================================
        // 8. 화면 설정
        // ==========================================

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

                    dgvMenuList.Columns[col.ColumnName].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.None;
                }
            }
        }
    }
}