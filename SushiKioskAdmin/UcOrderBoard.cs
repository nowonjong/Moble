using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SushiKioskAdmin.Views
{
    public partial class UcOrderBoard : UserControl
    {
        private DataTable orderTable;

        public UcOrderBoard()
        {
            InitializeComponent();
            InitOrderData();
        }

        private void InitOrderData()
        {
            orderTable = new DataTable();
            orderTable.Columns.Add("주문번호"); // Identifier (T02-01 또는 앱 영수증번호)
            orderTable.Columns.Add("주문출처"); // Source (키오스크, 앱)
            orderTable.Columns.Add("수령방식"); // OrderType (매장, 포장, 배달)
            orderTable.Columns.Add("주문시간"); // OrderTime
            orderTable.Columns.Add("주문내역"); // items.csv에서 조합
            orderTable.Columns.Add("금액", typeof(int)); // TotalAmount
            orderTable.Columns.Add("현재상태"); // Status

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

        /// <summary>
        /// susi_orders_realtime.csv를 읽어와서 실시간 탭에 표시 (완료된 주문은 제외)
        /// CSV 구조: Identifier, Source, OrderType, OrderTime, TotalAmount, Status
        /// </summary>
        private void LoadOrdersFromCsv()
        {
            orderTable.Clear();
            string ordersPath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
            if (!File.Exists(ordersPath)) return;

            string[] lines = File.ReadAllLines(ordersPath, Encoding.UTF8);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(',');
                if (parts.Length >= 6)
                {
                    string identifier = parts[0].Trim();
                    string source = parts[1].Trim();
                    string orderType = parts[2].Trim();
                    string orderTimeStr = parts[3].Trim();

                    if (!int.TryParse(parts[4].Trim(), out int totalAmount)) totalAmount = 0;
                    string orderStatus = parts[5].Trim();

                    // 완료된 주문은 실시간 탭에서 제외
                    if (orderStatus == "결제완료" || orderStatus == "픽업완료" || orderStatus == "주문거절")
                    {
                        continue;
                    }

                    string orderTime = orderTimeStr;
                    if (DateTime.TryParse(orderTimeStr, out DateTime dt))
                    {
                        orderTime = dt.ToString("HH:mm");
                    }

                    string summaryText = GetOrderItemsSummary(identifier);

                    orderTable.Rows.Add(identifier, source, orderType, orderTime, summaryText, totalAmount, orderStatus);
                }
            }
            ApplyCurrentFilter();
        }

        private string GetOrderItemsSummary(string identifier)
        {
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");
            if (!File.Exists(itemsPath)) return "주문 내역 없음";

            string[] lines = File.ReadAllLines(itemsPath, Encoding.UTF8);
            var itemList = new List<string>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(',');
                if (parts.Length >= 6)
                {
                    string keyId = parts[0].Trim();
                    if (keyId.Equals(identifier, StringComparison.OrdinalIgnoreCase))
                    {
                        string menuName = parts[1].Trim();
                        int qty = int.TryParse(parts[3].Trim(), out int q) ? q : 1;
                        int discountQty = int.TryParse(parts[4].Trim(), out int dq) ? dq : 0;

                        if (discountQty > 0)
                        {
                            itemList.Add($"{menuName} {qty}개 (할인 {discountQty}개)");
                        }
                        else
                        {
                            itemList.Add($"{menuName} {qty}개");
                        }
                    }
                }
            }

            return itemList.Count > 0 ? string.Join(", ", itemList) : "일반 주문";
        }

        private void SaveOrdersToCsv()
        {
            try
            {
                string ordersPath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
                var allLines = File.Exists(ordersPath) ? new List<string>(File.ReadAllLines(ordersPath, Encoding.UTF8)) : new List<string>();
                var currentDict = new Dictionary<string, string>();

                foreach (var l in allLines)
                {
                    if (string.IsNullOrWhiteSpace(l)) continue;
                    var p = l.Split(',');
                    if (p.Length > 0) currentDict[p[0].Trim()] = l;
                }

                StringBuilder sb = new StringBuilder();

                foreach (var kvp in currentDict)
                {
                    string id = kvp.Key;
                    string originalLine = kvp.Value;

                    DataRow foundRow = null;
                    foreach (DataRow r in orderTable.Rows)
                    {
                        if (r["주문번호"].ToString() == id)
                        {
                            foundRow = r;
                            break;
                        }
                    }

                    if (foundRow != null)
                    {
                        var p = originalLine.Split(',');
                        string identifier = p.Length > 0 ? p[0] : id;
                        string source = p.Length > 1 ? p[1] : foundRow["주문출처"].ToString();
                        string orderType = p.Length > 2 ? p[2] : foundRow["수령방식"].ToString();
                        string orderTime = p.Length > 3 ? p[3] : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string totalAmount = p.Length > 4 ? p[4] : foundRow["금액"].ToString();
                        string orderStatus = foundRow["현재상태"].ToString();

                        sb.AppendLine($"{identifier},{source},{orderType},{orderTime},{totalAmount},{orderStatus}");
                    }
                    else
                    {
                        var p = originalLine.Split(',');
                        if (p.Length >= 6)
                        {
                            p[5] = "픽업완료";
                            sb.AppendLine(string.Join(",", p));
                        }
                        else
                        {
                            sb.AppendLine(originalLine);
                        }
                    }
                }

                File.WriteAllText(ordersPath, sb.ToString(), new UTF8Encoding(false));
            }
            catch (Exception ex)
            {
                MessageBox.Show("주문 저장 오류: " + ex.Message);
            }
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

        private void btnAccept_Click(object sender, EventArgs e) => ProcessAppOrder("조리 중");
        private void btnReject_Click(object sender, EventArgs e) => ProcessAppOrder("주문 거절");
        private void btnCookDone_Click(object sender, EventArgs e) => ProcessAppOrder("조리 완료");
        private void btnPickUpDone_Click(object sender, EventArgs e) => ProcessAppOrder("픽업완료");

        private void ProcessAppOrder(string newStatus)
        {
            if (dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("처리할 주문을 먼저 선택해 주세요.", "안내");
                return;
            }

            if (dgvOrders.SelectedRows[0].DataBoundItem is DataRowView rowView)
            {
                string source = rowView["주문출처"].ToString();

                if (source == "키오스크")
                {
                    MessageBox.Show("키오스크 주문은 테이블 결제를 통해 처리됩니다.", "안내");
                    return;
                }

                rowView["현재상태"] = newStatus;

                if (newStatus == "픽업완료" || newStatus == "주문 거절")
                {
                    rowView.Row.Delete();
                }

                SaveOrdersToCsv();
                LoadOrdersFromCsv();

                MessageBox.Show($"앱 주문 처리 완료: [{newStatus}]", "알림");
            }
        }

        private void ApplyCurrentFilter()
        {
            DataView dv = orderTable.DefaultView;

            if (rdoAll.Checked) dv.RowFilter = "";
            else if (rdoApp.Checked) dv.RowFilter = "주문출처 = '앱'";
            else if (rdoKiosk.Checked) dv.RowFilter = "주문출처 = '키오스크'";
            else if (rdoWaiting.Checked) dv.RowFilter = "주문출처 = '앱' AND 현재상태 = '접수 대기'";
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
    }
}