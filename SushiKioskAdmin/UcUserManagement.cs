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

            string[] lines = File.ReadAllLines(csvPath, Encoding.UTF8);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 7)
                    continue;

                if (!int.TryParse(parts[0].Trim(), out int memberIndex))
                    continue;

                int.TryParse(parts[4].Trim(), out int point);

                string memberName = parts[1].Trim();
                string phone = parts[2].Trim();
                string password = parts[3].Trim();
                string address = parts[5].Trim();
                string joinDate = parts[6].Trim();

                userTable.Rows.Add(memberIndex, memberName, phone, password, point, address, joinDate);
            }
        }

        private void SaveNewUserToCsv(string name, string phone, string address)
        {
            try
            {
                int newId = 1001;

                if (userTable.Rows.Count > 0)
                {
                    int maxId = 1000;

                    foreach (DataRow row in userTable.Rows)
                    {
                        if (row.RowState == DataRowState.Deleted)
                            continue;

                        int id = Convert.ToInt32(row["회원번호"]);

                        if (id > maxId)
                            maxId = id;
                    }

                    newId = maxId + 1;
                }

                string regDate = DateTime.Now.ToString("yyyy-MM-dd");
                int point = 0;
                string password = "";

                userTable.Rows.Add(newId, name, phone, password, point, address, regDate);

                string csvPath = Path.Combine(Application.StartupPath, "member.csv");
                string csvLine = $"{newId},{name},{phone},{password},{point},{address},{regDate}";

                File.AppendAllText(csvPath, csvLine + Environment.NewLine, new UTF8Encoding(false));
                lastMemberModifiedTime = File.GetLastWriteTime(csvPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("회원 저장 중 오류가 발생했습니다.\n" + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveAllUsersToCsv()
        {
            try
            {
                string csvPath = Path.Combine(Application.StartupPath, "member.csv");
                StringBuilder sb = new StringBuilder();

                foreach (DataRow row in userTable.Rows)
                {
                    if (row.RowState == DataRowState.Deleted)
                        continue;

                    int memberIndex = Convert.ToInt32(row["회원번호"]);
                    string name = row["회원명"].ToString();
                    string phone = row["연락처"].ToString();
                    string password = row["비밀번호"].ToString();
                    int point = Convert.ToInt32(row["포인트"]);
                    string address = row["주소"].ToString();
                    string joinDate = row["가입일자"].ToString();

                    sb.AppendLine($"{memberIndex},{name},{phone},{password},{point},{address},{joinDate}");
                }

                File.WriteAllText(csvPath, sb.ToString(), new UTF8Encoding(false));
                lastMemberModifiedTime = File.GetLastWriteTime(csvPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CSV 저장 중 오류가 발생했습니다.\n" + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            DataView dv = userTable.DefaultView;

            if (string.IsNullOrEmpty(keyword))
                dv.RowFilter = "";
            else
                dv.RowFilter = $"회원명 LIKE '%{keyword.Replace("'", "''")}%' OR 연락처 LIKE '%{keyword.Replace("'", "''")}%'";
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            userTable.DefaultView.RowFilter = "";
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            string name = txtInputName.Text.Trim();
            string phone = txtInputPhone.Text.Trim();
            string address = txtInputAddress.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("회원명과 연락처를 입력해 주세요.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveNewUserToCsv(name, phone, address);
            MessageBox.Show($"[{name}] 회원이 성공적으로 등록되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearInputs();
        }

        private void btnUpdateUser_Click(object sender, EventArgs e)
        {
            if (dgvUserList.SelectedRows.Count == 0)
            {
                MessageBox.Show("수정할 회원을 목록에서 선택해 주세요.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dgvUserList.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                string name = txtInputName.Text.Trim();
                string phone = txtInputPhone.Text.Trim();
                string address = txtInputAddress.Text.Trim();

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
                {
                    MessageBox.Show("회원명과 연락처를 입력해 주세요.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                rowView["회원명"] = name;
                rowView["연락처"] = phone;
                rowView["주소"] = address;

                SaveAllUsersToCsv();
                MessageBox.Show($"[{name}] 회원의 정보가 수정되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
            }
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUserList.SelectedRows.Count == 0)
            {
                MessageBox.Show("삭제할 회원을 목록에서 선택해 주세요.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dgvUserList.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                string name = rowView["회원명"].ToString();

                DialogResult result = MessageBox.Show($"정말로 [{name}] 회원을 삭제하시겠습니까?", "회원 삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    rowView.Row.Delete();
                    SaveAllUsersToCsv();
                    MessageBox.Show("회원 정보가 삭제되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                }
            }
        }

        private void dgvUserList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUserList.SelectedRows.Count > 0 && dgvUserList.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                txtInputName.Text = rowView["회원명"].ToString();
                txtInputPhone.Text = rowView["연락처"].ToString();
                txtInputAddress.Text = rowView["주소"].ToString();
                lblPoint.Text = $"{Convert.ToInt32(rowView["포인트"]):N0} P";
            }
            else
            {
                ClearInputs();
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