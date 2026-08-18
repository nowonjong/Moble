using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SushiKioskAdmin.Views
{
    public partial class UcUserManagement : UserControl
    {
        private DataTable userTable;
        private DateTime lastMemberModifiedTime = DateTime.MinValue;

        public UcUserManagement()
        {
            InitializeComponent();
            InitUserData();
            InitAutoRefresh();
        }

        private void InitUserData()
        {
            userTable = new DataTable();
            userTable.Columns.Add("회원번호", typeof(int));
            userTable.Columns.Add("회원명", typeof(string));
            userTable.Columns.Add("연락처", typeof(string));
            userTable.Columns.Add("비밀번호", typeof(string));
            userTable.Columns.Add("포인트", typeof(int));
            userTable.Columns.Add("주소", typeof(string));
            userTable.Columns.Add("가입일자", typeof(string));

            LoadUserFromCsv();

            dgvUserList.AutoGenerateColumns = true;
            dgvUserList.DataSource = userTable;
            dgvUserList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dgvUserList.Columns.Contains("비밀번호"))
                dgvUserList.Columns["비밀번호"].Visible = false;

            if (dgvUserList.Columns.Contains("포인트"))
                dgvUserList.Columns["포인트"].DefaultCellStyle.Format = "N0";

            dgvUserList.EnableHeadersVisualStyles = false;
            dgvUserList.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvUserList.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
            dgvUserList.ColumnHeadersHeight = 30;
            dgvUserList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        private void InitAutoRefresh()
        {
            string csvPath = Path.Combine(Application.StartupPath, "member.csv");

            if (File.Exists(csvPath))
                lastMemberModifiedTime = File.GetLastWriteTime(csvPath);
        }

        private void LoadUserFromCsv()
        {
            string csvPath = Path.Combine(Application.StartupPath, "member.csv");

            if (!File.Exists(csvPath))
            {
                MessageBox.Show("member.csv 파일을 찾을 수 없습니다.", "경고");
                return;
            }

            userTable.Clear();

            foreach (string line in File.ReadAllLines(csvPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 7)
                    continue;

                if (!int.TryParse(parts[0].Trim(), out int memberId))
                    continue;

                int.TryParse(parts[4].Trim(), out int point);

                userTable.Rows.Add(
                    memberId,
                    parts[1].Trim(),
                    parts[2].Trim(),
                    parts[3].Trim(),
                    point,
                    parts[5].Trim(),
                    parts[6].Trim());
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            DataView dv = userTable.DefaultView;

            if (string.IsNullOrEmpty(keyword))
            {
                dv.RowFilter = "";
                return;
            }

            string safeKeyword = keyword.Replace("'", "''");
            dv.RowFilter = $"회원명 LIKE '%{safeKeyword}%' OR 연락처 LIKE '%{safeKeyword}%'";
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            userTable.DefaultView.RowFilter = "";
        }

        private void btnUpdateUser_Click(object sender, EventArgs e)
        {
            if (dgvUserList.SelectedRows.Count == 0)
            {
                MessageBox.Show("수정할 회원을 목록에서 선택해 주세요.", "안내");
                return;
            }

            if (!(dgvUserList.SelectedRows[0].DataBoundItem is DataRowView rowView))
                return;

            string name = txtInputName.Text.Trim();
            string phone = txtInputPhone.Text.Trim();
            string address = txtInputAddress.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("회원명과 연락처를 입력해 주세요.", "안내");
                return;
            }

            if (name.Contains(",") || phone.Contains(",") || address.Contains(","))
            {
                MessageBox.Show("회원명, 연락처, 주소에는 쉼표(,)를 사용할 수 없습니다.", "안내");
                return;
            }

            MainAdminForm mainForm = FindForm() as MainAdminForm;

            if (mainForm == null)
            {
                MessageBox.Show("메인 관리자 폼을 찾을 수 없습니다.", "오류");
                return;
            }

            int memberId = Convert.ToInt32(rowView["회원번호"]);

            bool success = mainForm.UpdateMemberInfo(
                memberId,
                name,
                phone,
                address,
                out string message);

            if (!success)
            {
                MessageBox.Show($"회원 정보 수정에 실패했습니다.\n{message}", "오류");
                return;
            }

            LoadUserFromCsv();

            string csvPath = Path.Combine(Application.StartupPath, "member.csv");

            if (File.Exists(csvPath))
                lastMemberModifiedTime = File.GetLastWriteTime(csvPath);

            MessageBox.Show($"[{name}] 회원의 정보가 수정되었습니다.", "알림");
            ClearInputs();
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUserList.SelectedRows.Count == 0)
            {
                MessageBox.Show("삭제할 회원을 목록에서 선택해 주세요.", "안내");
                return;
            }

            if (!(dgvUserList.SelectedRows[0].DataBoundItem is DataRowView rowView))
                return;

            int memberId = Convert.ToInt32(rowView["회원번호"]);
            string name = rowView["회원명"].ToString();

            DialogResult result = MessageBox.Show(
                $"정말로 [{name}] 회원을 삭제하시겠습니까?",
                "회원 삭제 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            MainAdminForm mainForm = FindForm() as MainAdminForm;

            if (mainForm == null)
            {
                MessageBox.Show("메인 관리자 폼을 찾을 수 없습니다.", "오류");
                return;
            }

            bool success = mainForm.DeleteMember(memberId, out string message);

            if (!success)
            {
                MessageBox.Show(message, "삭제 불가", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadUserFromCsv();

            string csvPath = Path.Combine(Application.StartupPath, "member.csv");

            if (File.Exists(csvPath))
                lastMemberModifiedTime = File.GetLastWriteTime(csvPath);

            MessageBox.Show("회원 정보가 삭제되었습니다.", "알림");
            ClearInputs();
        }

        private void dgvUserList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUserList.SelectedRows.Count > 0 &&
                dgvUserList.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                txtInputName.Text = rowView["회원명"].ToString();
                txtInputPhone.Text = rowView["연락처"].ToString();
                txtInputAddress.Text = rowView["주소"].ToString();
                lblPoint.Text = $"{Convert.ToInt32(rowView["포인트"]):N0} P";
            }
            else
            {
                txtInputName.Clear();
                txtInputPhone.Clear();
                txtInputAddress.Clear();
                lblPoint.Text = "- P";
            }
        }

        private void ClearInputs()
        {
            txtInputName.Clear();
            txtInputPhone.Clear();
            txtInputAddress.Clear();
            lblPoint.Text = "- P";

            if (dgvUserList.SelectedRows.Count > 0)
                dgvUserList.ClearSelection();
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            string csvPath = Path.Combine(Application.StartupPath, "member.csv");

            if (!File.Exists(csvPath))
                return;

            DateTime currentModifiedTime = File.GetLastWriteTime(csvPath);

            if (currentModifiedTime != lastMemberModifiedTime)
            {
                lastMemberModifiedTime = currentModifiedTime;
                LoadUserFromCsv();
            }
        }

        private void UcUserManagement_Load(object sender, EventArgs e)
        {
            var columnWidths = new (string ColumnName, int Width)[]
            {
                ("회원번호", 80),
                ("회원명", 70),
                ("연락처", 100),
                ("포인트", 70),
                ("가입일자", 80)
            };

            foreach (var col in columnWidths)
            {
                if (dgvUserList.Columns.Contains(col.ColumnName))
                {
                    dgvUserList.Columns[col.ColumnName].Width = col.Width;
                    dgvUserList.Columns[col.ColumnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
            }

            if (dgvUserList.Columns.Contains("비밀번호"))
                dgvUserList.Columns["비밀번호"].Visible = false;
        }
    }
}