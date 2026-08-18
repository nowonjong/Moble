using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SushiKioskAdmin.Views
{
    public partial class UcOrderHistory : UserControl
    {
        private DataTable historyTable;
        private DateTime lastHistoryModifiedTime = DateTime.MinValue;
        private DateTime lastItemsModifiedTime = DateTime.MinValue;

        public UcOrderHistory()
        {
            InitializeComponent();
            InitHistoryData();
            InitAutoRefresh();
        }

        private void InitHistoryData()
        {
            cmbOrderType.Items.Clear();
            cmbOrderType.Items.AddRange(new string[] { "전체", "앱", "키오스크" });
            cmbOrderType.SelectedIndex = 0;
            dtpStart.Value = DateTime.Now.AddDays(-7);
            dtpEnd.Value = DateTime.Now;

            historyTable = new DataTable();
            historyTable.Columns.Add("영수증번호", typeof(string));
            historyTable.Columns.Add("결제일시", typeof(DateTime));
            historyTable.Columns.Add("출처", typeof(string));
            historyTable.Columns.Add("수령방식", typeof(string));
            historyTable.Columns.Add("결제금액", typeof(int));
            historyTable.Columns.Add("결제수단", typeof(string));
            historyTable.Columns.Add("원주문금액", typeof(int));
            historyTable.Columns.Add("사용포인트", typeof(int));
            historyTable.Columns.Add("적립포인트", typeof(int));
            historyTable.Columns.Add("회원번호", typeof(int));

            LoadHistoryFromCsv();

            dgvHistoryList.DataSource = historyTable;
            dgvHistoryList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistoryList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistoryList.MultiSelect = false;
            dgvHistoryList.ReadOnly = true;
            dgvHistoryList.AllowUserToAddRows = false;

            dgvHistoryList.Columns["결제금액"].DefaultCellStyle.Format = "N0";
            dgvHistoryList.Columns["결제일시"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";

            dgvHistoryList.Columns["원주문금액"].Visible = false;
            dgvHistoryList.Columns["사용포인트"].Visible = false;
            dgvHistoryList.Columns["적립포인트"].Visible = false;
            dgvHistoryList.Columns["회원번호"].Visible = false;

            dgvHistoryList.EnableHeadersVisualStyles = false;
            dgvHistoryList.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvHistoryList.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
            dgvHistoryList.ColumnHeadersHeight = 35;
            dgvHistoryList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        private void InitAutoRefresh()
        {
            string historyPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (File.Exists(historyPath))
                lastHistoryModifiedTime = File.GetLastWriteTime(historyPath);

            if (File.Exists(itemsPath))
                lastItemsModifiedTime = File.GetLastWriteTime(itemsPath);
        }

        private void LoadHistoryFromCsv()
        {
            historyTable.Clear();

            string historyPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");

            if (!File.Exists(historyPath))
                return;

            foreach (string line in File.ReadAllLines(historyPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 10)
                    continue;

                string receiptNo = parts[0].Trim();

                if (!DateTime.TryParse(parts[1].Trim(), out DateTime paymentDate))
                    continue;

                string source = parts[2].Trim();
                string orderType = parts[3].Trim();

                int originalAmount = int.TryParse(parts[4].Trim(), out int original) ? original : 0;
                int usedPoint = int.TryParse(parts[5].Trim(), out int used) ? used : 0;
                int totalAmount = int.TryParse(parts[6].Trim(), out int total) ? total : 0;
                int earnedPoint = int.TryParse(parts[7].Trim(), out int earned) ? earned : 0;
                int memberId = int.TryParse(parts[8].Trim(), out int member) ? member : 0;

                string paymentMethod = parts[9].Trim();

                historyTable.Rows.Add(
                    receiptNo,
                    paymentDate,
                    source,
                    orderType,
                    totalAmount,
                    paymentMethod,
                    originalAmount,
                    usedPoint,
                    earnedPoint,
                    memberId);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ApplyFilter();
            MessageBox.Show("조회가 완료되었습니다.", "안내");
        }

        private void ApplyFilter()
        {
            DataView dv = historyTable.DefaultView;

            string selectedType = cmbOrderType.SelectedItem?.ToString() ?? "전체";

            DateTime startDate = dtpStart.Value.Date;
            DateTime endDate = dtpEnd.Value.Date.AddDays(1).AddSeconds(-1);

            if (startDate > endDate)
            {
                MessageBox.Show("시작일이 종료일보다 뒤일 수 없습니다.", "안내");
                return;
            }

            string dateFilter =
                $"결제일시 >= #{startDate:MM/dd/yyyy HH:mm:ss}# " +
                $"AND 결제일시 <= #{endDate:MM/dd/yyyy HH:mm:ss}#";

            if (selectedType == "전체")
                dv.RowFilter = dateFilter;
            else
                dv.RowFilter = $"출처 = '{selectedType.Replace("'", "''")}' AND {dateFilter}";
        }

        private void dgvHistoryList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHistoryList.SelectedRows.Count == 0)
            {
                txtReceipt.Clear();
                return;
            }

            if (!(dgvHistoryList.SelectedRows[0].DataBoundItem is DataRowView rowView))
                return;

            string orderNo = rowView["영수증번호"].ToString();
            string orderDate = Convert.ToDateTime(rowView["결제일시"]).ToString("yyyy-MM-dd HH:mm:ss");
            string source = rowView["출처"].ToString();
            string type = rowView["수령방식"].ToString();

            int originalAmount = Convert.ToInt32(rowView["원주문금액"]);
            int usedPoint = Convert.ToInt32(rowView["사용포인트"]);
            int totalAmount = Convert.ToInt32(rowView["결제금액"]);
            int earnedPoint = Convert.ToInt32(rowView["적립포인트"]);
            int memberId = Convert.ToInt32(rowView["회원번호"]);

            string payMethod = rowView["결제수단"].ToString();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("==========================================");
            sb.AppendLine("            [ 초밥 키오스크 영수증 ]");
            sb.AppendLine("==========================================");
            sb.AppendLine($"영수증번호 : {orderNo}");
            sb.AppendLine($"결제일시 : {orderDate}");
            sb.AppendLine($"주문유형 : [{source}] - {type}");

            if (memberId > 0)
                sb.AppendLine($"회원번호 : {memberId}");

            sb.AppendLine("------------------------------------------");
            sb.AppendLine(" 상품명                수량     금액(SubTotal)");
            sb.AppendLine("------------------------------------------");

            LoadReceiptItems(orderNo, sb);

            sb.AppendLine("------------------------------------------");
            sb.AppendLine($" 주문금액 :                     {originalAmount:N0}원");

            if (usedPoint > 0)
                sb.AppendLine($" 포인트사용 :                  -{usedPoint:N0}P");

            sb.AppendLine($" 결제금액 :                     {totalAmount:N0}원");
            sb.AppendLine($" 결제수단 :                     {payMethod}");

            if (memberId > 0)
                sb.AppendLine($" 적립포인트 :                   +{earnedPoint:N0}P");

            sb.AppendLine("==========================================");
            sb.AppendLine("           이용해 주셔서 감사합니다!");
            sb.AppendLine("==========================================");

            txtReceipt.Text = sb.ToString();
        }

        private void LoadReceiptItems(string receiptNo, StringBuilder sb)
        {
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (!File.Exists(itemsPath))
            {
                sb.AppendLine(" 상세 주문 내역이 없습니다.");
                return;
            }

            bool hasItem = false;

            foreach (string line in File.ReadAllLines(itemsPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 6)
                    continue;

                if (!parts[0].Trim().Equals(receiptNo, StringComparison.OrdinalIgnoreCase))
                    continue;

                string menuName = parts[1].Trim();

                int qty = int.TryParse(parts[3].Trim(), out int q) ? q : 1;
                int discountQty = int.TryParse(parts[4].Trim(), out int dq) ? dq : 0;
                int subTotal = int.TryParse(parts[5].Trim(), out int st) ? st : 0;

                if (discountQty > 0)
                    sb.AppendLine($" {menuName} (할인 {discountQty}개 포함)  {qty}개    {subTotal:N0}원");
                else
                    sb.AppendLine($" {menuName}                    {qty}개    {subTotal:N0}원");

                hasItem = true;
            }

            if (!hasItem)
                sb.AppendLine(" 해당 주문의 상세 품목을 찾을 수 없습니다.");
        }

        private void btnPrintReceipt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReceipt.Text))
            {
                MessageBox.Show("출력할 영수증을 목록에서 먼저 선택해 주세요.", "안내");
                return;
            }

            MessageBox.Show(
                "영수증 프린터로 인쇄 명령을 전송했습니다.",
                "영수증 재발행",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            string historyPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            DateTime currentHistoryModifiedTime =
                File.Exists(historyPath)
                ? File.GetLastWriteTime(historyPath)
                : DateTime.MinValue;

            DateTime currentItemsModifiedTime =
                File.Exists(itemsPath)
                ? File.GetLastWriteTime(itemsPath)
                : DateTime.MinValue;

            if (currentHistoryModifiedTime != lastHistoryModifiedTime ||
                currentItemsModifiedTime != lastItemsModifiedTime)
            {
                lastHistoryModifiedTime = currentHistoryModifiedTime;
                lastItemsModifiedTime = currentItemsModifiedTime;

                string selectedReceiptNo = null;

                if (dgvHistoryList.SelectedRows.Count > 0 &&
                    dgvHistoryList.SelectedRows[0].DataBoundItem is DataRowView selectedRow)
                {
                    selectedReceiptNo = selectedRow["영수증번호"].ToString();
                }

                LoadHistoryFromCsv();
                ApplyFilter();

                if (!string.IsNullOrWhiteSpace(selectedReceiptNo))
                    RestoreSelection(selectedReceiptNo);
            }
        }

        private void RestoreSelection(string receiptNo)
        {
            foreach (DataGridViewRow row in dgvHistoryList.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Cells["영수증번호"].Value?.ToString()
                    .Equals(receiptNo, StringComparison.OrdinalIgnoreCase) == true)
                {
                    row.Selected = true;

                    if (row.Cells.Count > 0)
                        dgvHistoryList.CurrentCell = row.Cells[0];

                    return;
                }
            }

            txtReceipt.Clear();
        }

        public void RefreshHistory()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(RefreshHistory));
                return;
            }

            LoadHistoryFromCsv();
            ApplyFilter();

            string historyPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            lastHistoryModifiedTime =
                File.Exists(historyPath)
                ? File.GetLastWriteTime(historyPath)
                : DateTime.MinValue;

            lastItemsModifiedTime =
                File.Exists(itemsPath)
                ? File.GetLastWriteTime(itemsPath)
                : DateTime.MinValue;
        }

        private void UcOrderHistory_Load(object sender, EventArgs e)
        {
            dgvHistoryList.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvHistoryList.ColumnHeadersHeight = 35;

            foreach (DataGridViewColumn col in dgvHistoryList.Columns)
                col.HeaderCell.Style.WrapMode = DataGridViewTriState.False;
        }
    }
}