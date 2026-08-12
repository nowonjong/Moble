using System;
using System.Data;
using System.Drawing;
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

            userTable.Rows.Add(1001, "김철수", "010-1234-5678", 2500, "서울시 강남구", "2026-01-15");
            userTable.Rows.Add(1002, "이영희", "010-9876-5432", 800, "부산시 해운대구", "2026-03-20");
            userTable.Rows.Add(1003, "박민수", "010-5555-4444", 5100, "대구시 수성구", "2025-11-02");
            userTable.Rows.Add(1004, "정수진", "010-3333-2222", 1200, "인천시 남동구", "2026-06-10");

            // 컬럼 자동 생성 활성화 (디자이너와 충돌 방지)
            dgvUserList.AutoGenerateColumns = true;

            // DataGridView 데이터 바인딩
            dgvUserList.AutoGenerateColumns = true;
            dgvUserList.DataSource = userTable;
            dgvUserList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // ★ 포인트 열 너비를 코드로 강제 조절하는 구문은 에러를 유발하므로 제거하고, 
            // 천단위 콤마 서식(Format)만 안전하게 지정합니다.
            if (dgvUserList.Columns.Contains("포인트"))
            {
                dgvUserList.Columns["포인트"].DefaultCellStyle.Format = "N0";
            }

            // 헤더 스타일 회색 고정
            dgvUserList.EnableHeadersVisualStyles = false;
            dgvUserList.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvUserList.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
        }

        // ==========================================
        // 2. 조회 및 검색 기능
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
        // 3. 회원 등록 / 수정 / 삭제 기능
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
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("회원명과 연락처는 필수 입력 항목입니다.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 채번 (마지막 회원번호 + 1, 목록이 비어있으면 1001부터 시작)
            int newId = 1001;
            if (userTable.Rows.Count > 0)
            {
                newId = Convert.ToInt32(userTable.Rows[userTable.Rows.Count - 1]["회원번호"]) + 1;
            }

            string regDate = DateTime.Now.ToString("yyyy-MM-dd");

            // 신규 회원은 포인트 0으로 자동 저장 (포인트는 수정/입력 불가)
            userTable.Rows.Add(newId, name, phone, 0, address, regDate);

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
                    MessageBox.Show("회원 정보가 삭제되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                }
            }
        }

        // ==========================================
        // 4. 데이터 그리드 선택 연동 및 입력폼 관리
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
            // 화면에 완전히 로드된 이후 안전하게 열 너비 설정
            if (dgvUserList.Columns.Contains("회원번호"))
            {
                dgvUserList.Columns["회원번호"].Width = 80;
                dgvUserList.Columns["회원번호"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            if (dgvUserList.Columns.Contains("회원명"))
            {
                dgvUserList.Columns["회원명"].Width = 70;
                dgvUserList.Columns["회원명"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            if (dgvUserList.Columns.Contains("연락처"))
            {
                dgvUserList.Columns["연락처"].Width = 100;
                dgvUserList.Columns["연락처"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            if (dgvUserList.Columns.Contains("포인트"))
            {
                dgvUserList.Columns["포인트"].Width = 70;
                dgvUserList.Columns["포인트"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            if (dgvUserList.Columns.Contains("가입일자"))
            {
                dgvUserList.Columns["가입일자"].Width = 80;
                dgvUserList.Columns["가입일자"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }

        }
    }
}