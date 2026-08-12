using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SushiKioskAdmin.Views
{
    public partial class UcUserManagement : UserControl
    {
        // 회원 데이터를 관리할 전역 DataTable
        private DataTable userTable;

        public UcUserManagement()
        {
            InitializeComponent();
            InitUserData();
        }

        // ==========================================
        // 1. 초기화 및 샘플 데이터 로드
        // ==========================================

        /// <summary>
        /// 회원 목록 구조 생성 및 더미 데이터 로드
        /// </summary>
        private void InitUserData()
        {
            userTable = new DataTable();
            userTable.Columns.Add("회원번호", typeof(int));
            userTable.Columns.Add("회원명", typeof(string));
            userTable.Columns.Add("연락처", typeof(string));
            userTable.Columns.Add("포인트", typeof(int));
            userTable.Columns.Add("주소", typeof(string));
            userTable.Columns.Add("가입일자", typeof(string));

            // CSV 파일 읽어서 데이터 로드
            LoadUserFromCsv();

            // DataGridView 데이터 바인딩
            dgvUserList.AutoGenerateColumns = true;
            dgvUserList.DataSource = userTable;
            dgvUserList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvUserList.Columns["포인트"].DefaultCellStyle.Format = "N0";

            // 헤더 스타일 회색 고정
            dgvUserList.EnableHeadersVisualStyles = false;
            dgvUserList.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvUserList.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;

            // 열 헤더 높이를 원하는 크기(예: 40픽셀)로 지정
            dgvUserList.ColumnHeadersHeight = 30;

            // 높이를 자동으로 늘어나지 않게 고정
            dgvUserList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        // ==========================================
        // 2. CSV 회원정보 읽기
        // ==========================================

        private void LoadUserFromCsv()
        {
            // 실행 파일이 있는 폴더에서 member.csv 찾기
            string csvPath = Path.Combine(Application.StartupPath,"member.csv");

            // 파일이 없으면 경고
            if (!File.Exists(csvPath))
            {
                MessageBox.Show("member.csv 파일을 찾을 수 없습니다.", "경고" );
                return;
            }

            // CSV 파일 전체 읽기
            string[] lines = File.ReadAllLines( csvPath, Encoding.UTF8);
            foreach (string line in lines)
            {
                // 빈 줄은 무시
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] parts = line.Split(',');

                if (parts.Length >= 7)
                {
                    // 회원번호와 포인트 숫자 변환
                    if (int.TryParse(parts[0].Trim(), out int memberIndex) && int.TryParse(parts[4].Trim(), out int point))
                    {
                        string memberName = parts[1].Trim();
                        string phone = parts[2].Trim();
                        // parts[3]은 비밀번호
                        // 관리자 화면에는 표시하지 않기 때문에 사용하지 않음
                        string address = parts[5].Trim();
                        string joinDate = parts[6].Trim();
                        // DataTable에 추가
                        userTable.Rows.Add(memberIndex, memberName, phone, point, address, joinDate);
                    }
                }
            }
        }

        // ==========================================
        // 3. 신규 회원 CSV 저장
        // ==========================================

        private void SaveNewUserToCsv(string name, string phone, string address)
        {
            try
            {
                // 회원번호 자동 생성
                int newId = 1001;

                if (userTable.Rows.Count > 0)
                {
                    int maxId = 1000;
                    foreach (DataRow row in userTable.Rows)
                    {
                        // 삭제된 데이터는 제외
                        if (row.RowState == DataRowState.Deleted)
                            continue;
                        int id = Convert.ToInt32(row["회원번호"]);
                        if (id > maxId)
                        {
                            maxId = id;
                        }
                    }
                    // 가장 큰 회원번호 + 1
                    newId = maxId + 1;
                }
                // 가입일자 자동 생성
                string regDate = DateTime.Now.ToString("yyyy-MM-dd");

                // 포인트 자동 0
                int point = 0;

                // 현재 관리자 등록화면에는
                // 비밀번호 입력란이 없으므로 빈 값
                string password = "";

                // 화면 DataTable에도 추가
                userTable.Rows.Add(newId, name, phone, point, address, regDate);

                // CSV에도 저장
                string csvPath = Path.Combine(Application.StartupPath, "member.csv");
                string csvLine = newId + "," + name + "," + phone + "," + password + "," + point + "," + address + "," + regDate;
                File.AppendAllText(csvPath, csvLine + Environment.NewLine, new UTF8Encoding(false));
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
                    // 삭제된 행은 CSV에 저장하지 않음
                    if (row.RowState == DataRowState.Deleted)
                        continue;

                    int memberIndex = Convert.ToInt32(row["회원번호"]);
                    string name = row["회원명"].ToString();
                    string phone = row["연락처"].ToString();
                    int point = Convert.ToInt32(row["포인트"]);
                    string address = row["주소"].ToString();
                    string joinDate = row["가입일자"].ToString();

                    // 현재 화면에서는 비밀번호를 보관하지 않으므로 빈 값
                    string password = "";
                    sb.AppendLine(memberIndex + "," + name + "," + phone + "," + password + "," + point + "," + address + "," + joinDate );
                }

                // 기존 CSV 파일 전체 덮어쓰기
                File.WriteAllText(csvPath, sb.ToString(), new UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                MessageBox.Show("CSV 저장 중 오류가 발생했습니다.\n" + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // 4. 조회 및 검색 기능
        // ==========================================

        /// <summary>
        /// [검색] 버튼 클릭 (이름 또는 연락처 키워드로 필터링)
        /// </summary>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            DataView dv = userTable.DefaultView;

            if (string.IsNullOrEmpty(keyword))
            {
                dv.RowFilter = "";
            }
            else
            {
                // 회원명 또는 연락처에 검색어가 포함된 항목만 필터링
                dv.RowFilter = $"회원명 LIKE '%{keyword}%' OR 연락처 LIKE '%{keyword}%'";
            }
        }

        /// <summary>
        /// [전체 보기 / 초기화] 버튼 클릭 (검색조건 초기화)
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            userTable.DefaultView.RowFilter = "";
        }

        // ==========================================
        // 5. 회원 등록 / 수정 / 삭제 기능
        // ==========================================

        /// <summary>
        /// [신규 회원 등록] 버튼 클릭
        /// </summary>
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            string name = txtInputName.Text.Trim();
            string phone = txtInputPhone.Text.Trim();
            string address = txtInputAddress.Text.Trim();

            // 유효성 검사
            SaveNewUserToCsv(name, phone, address);

            MessageBox.Show($"[{name}] 회원이 성공적으로 등록되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearInputs();
        }

        /// <summary>
        /// [회원 정보 수정] 버튼 클릭 (회원명, 연락처, 주소만 수정 가능 / 포인트는 유지)
        /// </summary>
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

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
                {
                    MessageBox.Show("회원명과 연락처를 입력해 주세요.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 정보 수정 (포인트는 변경하지 않고 기존 값 유지)
                rowView["회원명"] = name;
                rowView["연락처"] = phone;
                rowView["주소"] = address;

                // 수정 내용을 CSV에도 저장
                SaveAllUsersToCsv();

                MessageBox.Show($"[{name}] 회원의 정보가 수정되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
            }
        }

        /// <summary>
        /// [회원 삭제] 버튼 클릭
        /// </summary>
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

                DialogResult result = MessageBox.Show(
                    $"정말로 [{name}] 회원을 삭제하시겠습니까?",
                    "회원 삭제 확인",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    rowView.Row.Delete();
                    // CSV에서도 삭제
                    SaveAllUsersToCsv();
                    MessageBox.Show("회원 정보가 삭제되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                }
            }
        }

        // ==========================================
        // 6. 데이터 그리드 선택 연동 및 입력폼 관리
        // ==========================================

        /// <summary>
        /// 그리드 목록에서 항목 선택 시 입력/조회 폼에 자동 데이터 바인딩
        /// </summary>
        private void dgvUserList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUserList.SelectedRows.Count > 0 &&
                dgvUserList.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                // 입력 가능한 폼 채우기
                txtInputName.Text = rowView["회원명"].ToString();
                txtInputPhone.Text = rowView["연락처"].ToString();
                txtInputAddress.Text = rowView["주소"].ToString();

                // 포인트는 조회 전용 라벨(Label)에만 표시 (수정 불가)
                lblPoint.Text = $"{Convert.ToInt32(rowView["포인트"]):N0} P";
            }
            else
            {
                ClearInputs();
            }
        }

        /// <summary>
        /// 입력 텍스트박스 및 포인트 라벨 초기화
        /// </summary>
        private void ClearInputs()
        {
            txtInputName.Clear();
            txtInputPhone.Clear();
            txtInputAddress.Clear();

            // 포인트 표시 라벨 초기화
            lblPoint.Text = "- P";

            // 그리드 선택 해제 (이벤트 중복 실행 방지 처리)
            if (dgvUserList.SelectedRows.Count > 0)
            {
                dgvUserList.ClearSelection();
            }
        }

        private void UcUserManagement_Load(object sender, EventArgs e)
        {
            // 반복되던 컬럼 너비 설정을 배열과 반복문으로 압축
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
        }
    }
}