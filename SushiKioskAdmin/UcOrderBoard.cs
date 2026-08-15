using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SushiKioskAdmin.Views
{
    public partial class UcOrderBoard : UserControl
    {
        private DataTable orderTable;
        private DateTime lastOrdersModifiedTime = DateTime.MinValue;
        private DateTime lastItemsModifiedTime = DateTime.MinValue;

        public UcOrderBoard()
        {
            InitializeComponent();
            InitOrderData();
            InitAutoRefresh();
        }

        private void InitOrderData()
        {
            orderTable = new DataTable();
            orderTable.Columns.Add("주문번호");
            orderTable.Columns.Add("주문출처");
            orderTable.Columns.Add("수령방식");
            orderTable.Columns.Add("주문시간");
            orderTable.Columns.Add("주문내역");
            orderTable.Columns.Add("금액", typeof(int));
            orderTable.Columns.Add("현재상태");

            LoadOrdersFromCsv();

            dgvOrders.DataSource = orderTable;
            dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrders.Columns["금액"].DefaultCellStyle.Format = "N0";
            dgvOrders.EnableHeadersVisualStyles = false;
            dgvOrders.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
            dgvOrders.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
            dgvOrders.ColumnHeadersHeight = 30;
            dgvOrders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        private void InitAutoRefresh()
        {
            string ordersPath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (File.Exists(ordersPath))
                lastOrdersModifiedTime = File.GetLastWriteTime(ordersPath);

            if (File.Exists(itemsPath))
                lastItemsModifiedTime = File.GetLastWriteTime(itemsPath);
        }

        private void LoadOrdersFromCsv()
        {
            orderTable.Clear();

            string ordersPath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");

            if (!File.Exists(ordersPath))
                return;

            string[] lines = File.ReadAllLines(ordersPath, Encoding.UTF8);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 6)
                    continue;

                string identifier = parts[0].Trim();
                string source = parts[1].Trim();
                string orderType = parts[2].Trim();
                string orderTimeStr = parts[3].Trim();
                int totalAmount = int.TryParse(parts[4].Trim(), out int amount) ? amount : 0;
                string orderStatus = parts[5].Trim();

                if (orderStatus == "결제완료" || orderStatus == "픽업완료" || orderStatus == "주문거절")
                    continue;

                string orderTime = orderTimeStr;

                if (DateTime.TryParse(orderTimeStr, out DateTime dt))
                    orderTime = dt.ToString("HH:mm");

                string summaryText = GetOrderItemsSummary(identifier);
                orderTable.Rows.Add(identifier, source, orderType, orderTime, summaryText, totalAmount, orderStatus);
            }

            ApplyCurrentFilter();

            if (FindForm() is MainAdminForm mainForm)
                mainForm.UpdateOrderNotice();
        }

        private string GetOrderItemsSummary(string identifier)
        {
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (!File.Exists(itemsPath))
                return "주문 내역 없음";

            string[] lines = File.ReadAllLines(itemsPath, Encoding.UTF8);
            List<string> itemList = new List<string>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 6)
                    continue;

                string keyId = parts[0].Trim();

                if (!keyId.Equals(identifier, StringComparison.OrdinalIgnoreCase))
                    continue;

                string menuName = parts[1].Trim();
                int qty = int.TryParse(parts[3].Trim(), out int q) ? q : 1;
                int discountQty = int.TryParse(parts[4].Trim(), out int dq) ? dq : 0;

                if (discountQty > 0)
                    itemList.Add($"{menuName} {qty}개 (할인 {discountQty}개)");
                else
                    itemList.Add($"{menuName} {qty}개");
            }

            return itemList.Count > 0 ? string.Join(", ", itemList) : "일반 주문";
        }

        private void UpdateOrderStatus(string identifier, string newStatus)
        {
            string ordersPath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");

            if (!File.Exists(ordersPath))
                return;

            string[] lines = File.ReadAllLines(ordersPath, Encoding.UTF8);

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                string[] parts = lines[i].Split(',');

                if (parts.Length < 6)
                    continue;

                if (parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase))
                {
                    parts[5] = newStatus;
                    lines[i] = string.Join(",", parts);
                    break;
                }
            }

            File.WriteAllLines(ordersPath, lines, new UTF8Encoding(false));
            lastOrdersModifiedTime = File.GetLastWriteTime(ordersPath);
        }

        private void FilterOrders_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rdo && rdo.Checked)
            {
                ApplyCurrentFilter();

                bool isAppOrderFilter = rdoApp.Checked || rdoWaiting.Checked;
                btnAccept.Visible = isAppOrderFilter;
                btnReject.Visible = isAppOrderFilter;
                btnCookDone.Visible = isAppOrderFilter;
                btnPickUpDone.Visible = isAppOrderFilter;
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            ProcessAppOrder("조리 중");
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            ProcessAppOrder("주문 거절");
        }

        private void btnCookDone_Click(object sender, EventArgs e)
        {
            ProcessAppOrder("조리 완료");
        }

        private void btnPickUpDone_Click(object sender, EventArgs e)
        {
            ProcessAppOrder("픽업완료");
        }

        private void ProcessAppOrder(string newStatus)
        {
            if (dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("처리할 주문을 먼저 선택해 주세요.", "안내");
                return;
            }

            if (!(dgvOrders.SelectedRows[0].DataBoundItem is DataRowView rowView))
                return;

            string identifier = rowView["주문번호"].ToString();
            string source = rowView["주문출처"].ToString();
            string currentStatus = rowView["현재상태"].ToString();

            if (source == "키오스크")
            {
                MessageBox.Show("키오스크 주문은 테이블 결제를 통해 처리됩니다.", "안내");
                return;
            }

            if (newStatus == "조리 중")
            {
                if (currentStatus != "접수 대기")
                {
                    MessageBox.Show("접수 대기 상태의 주문만 접수할 수 있습니다.", "안내");
                    return;
                }

                UpdateOrderStatus(identifier, "조리 중");
                LoadOrdersFromCsv();
                MessageBox.Show("앱 주문을 접수했습니다. 조리를 시작합니다.", "알림");
                return;
            }

            if (newStatus == "조리 완료")
            {
                if (currentStatus != "조리 중")
                {
                    MessageBox.Show("조리 중인 주문만 조리 완료 처리할 수 있습니다.", "안내");
                    return;
                }

                UpdateOrderStatus(identifier, "조리 완료");
                LoadOrdersFromCsv();
                MessageBox.Show("조리 완료 처리되었습니다.", "알림");
                return;
            }

            if (newStatus == "주문 거절")
            {
                if (currentStatus != "접수 대기")
                {
                    MessageBox.Show("접수 대기 상태의 주문만 거절할 수 있습니다.", "안내");
                    return;
                }

                DialogResult result = MessageBox.Show("선택한 앱 주문을 거절하시겠습니까?", "주문 거절", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return;

                MainAdminForm mainForm = FindForm() as MainAdminForm;

                if (mainForm == null)
                {
                    MessageBox.Show("메인 관리자 폼을 찾을 수 없습니다.", "오류");
                    return;
                }

                string responseJson = mainForm.RejectAppOrder(identifier);

                try
                {
                    JObject response = JObject.Parse(responseJson);
                    string status = response["Status"]?.ToString();
                    string message = response["Message"]?.ToString();

                    if (status == "SUCCESS")
                    {
                        LoadOrdersFromCsv();
                        MessageBox.Show("앱 주문이 거절되었습니다.", "알림");
                    }
                    else
                    {
                        MessageBox.Show($"주문 거절 처리에 실패했습니다.\n{message}", "오류");
                    }
                }
                catch
                {
                    MessageBox.Show("주문 거절 처리 응답을 확인할 수 없습니다.", "오류");
                }

                return;
            }

            if (newStatus == "픽업완료")
            {
                if (currentStatus != "조리 완료")
                {
                    MessageBox.Show("조리 완료 상태의 주문만 픽업 완료 처리할 수 있습니다.", "안내");
                    return;
                }

                MainAdminForm mainForm = FindForm() as MainAdminForm;

                if (mainForm == null)
                {
                    MessageBox.Show("메인 관리자 폼을 찾을 수 없습니다.", "오류");
                    return;
                }

                string responseJson = mainForm.CompleteAppOrder(identifier);

                try
                {
                    JObject response = JObject.Parse(responseJson);
                    string status = response["Status"]?.ToString();
                    string message = response["Message"]?.ToString();

                    if (status == "SUCCESS")
                    {
                        LoadOrdersFromCsv();
                        MessageBox.Show("픽업 완료 처리되었습니다.\n매출 내역으로 이동되었습니다.", "알림");
                    }
                    else
                    {
                        MessageBox.Show($"픽업 완료 처리에 실패했습니다.\n{message}", "오류");
                    }
                }
                catch
                {
                    MessageBox.Show("픽업 완료 처리 응답을 확인할 수 없습니다.", "오류");
                }

                return;
            }
        }

        private void ApplyCurrentFilter()
        {
            DataView dv = orderTable.DefaultView;

            if (rdoAll.Checked)
                dv.RowFilter = "";
            else if (rdoApp.Checked)
                dv.RowFilter = "주문출처 = '앱'";
            else if (rdoKiosk.Checked)
                dv.RowFilter = "주문출처 = '키오스크'";
            else if (rdoWaiting.Checked)
                dv.RowFilter = "주문출처 = '앱' AND 현재상태 = '접수 대기'";
        }

        private void UcOrderBoard_Load(object sender, EventArgs e)
        {
            var columnWidths = new (string ColumnName, int Width)[]
            {
                ("주문번호", 120),
                ("주문출처", 80),
                ("수령방식", 90),
                ("주문시간", 80),
                ("주문내역", 180),
                ("금액", 90),
                ("현재상태", 90)
            };

            foreach (var col in columnWidths)
            {
                if (dgvOrders.Columns.Contains(col.ColumnName))
                {
                    dgvOrders.Columns[col.ColumnName].Width = col.Width;
                    dgvOrders.Columns[col.ColumnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
            }

            bool isAppOrderFilter = rdoApp.Checked || rdoWaiting.Checked;
            btnAccept.Visible = isAppOrderFilter;
            btnReject.Visible = isAppOrderFilter;
            btnCookDone.Visible = isAppOrderFilter;
            btnPickUpDone.Visible = isAppOrderFilter;
        }

        public void RefreshOrders()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(RefreshOrders));
                return;
            }

            LoadOrdersFromCsv();

            string ordersPath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            lastOrdersModifiedTime = File.Exists(ordersPath)
                ? File.GetLastWriteTime(ordersPath)
                : DateTime.MinValue;

            lastItemsModifiedTime = File.Exists(itemsPath)
                ? File.GetLastWriteTime(itemsPath)
                : DateTime.MinValue;
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            string ordersPath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            DateTime currentOrdersModifiedTime = File.Exists(ordersPath) ? File.GetLastWriteTime(ordersPath) : DateTime.MinValue;
            DateTime currentItemsModifiedTime = File.Exists(itemsPath) ? File.GetLastWriteTime(itemsPath) : DateTime.MinValue;

            if (currentOrdersModifiedTime != lastOrdersModifiedTime || currentItemsModifiedTime != lastItemsModifiedTime)
            {
                lastOrdersModifiedTime = currentOrdersModifiedTime;
                lastItemsModifiedTime = currentItemsModifiedTime;
                LoadOrdersFromCsv();
            }
        }
    }
}