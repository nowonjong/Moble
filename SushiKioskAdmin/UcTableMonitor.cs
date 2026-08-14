using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SushiKioskAdmin.Views
{
    public partial class UcTableMonitor : UserControl
    {
        private DateTime lastRealtimeModifiedTime = DateTime.MinValue;
        private DateTime lastItemsModifiedTime = DateTime.MinValue;

        public UcTableMonitor()
        {
            InitializeComponent();
            LoadTableCards();
            InitAutoRefresh();
        }

        private void LoadTableCards()
        {
            flpTables.Controls.Clear();
            Dictionary<string, int> tableAmounts = GetTableAmountsFromCsv();

            int totalTables = 10;
            int occupiedTables = 0;

            for (int i = 1; i <= totalTables; i++)
            {
                string tableKey = $"Table {i:D2}";
                bool isOccupied = tableAmounts.ContainsKey(tableKey) && tableAmounts[tableKey] > 0;
                int amountValue = isOccupied ? tableAmounts[tableKey] : 0;

                if (isOccupied)
                    occupiedTables++;

                string amountStr = $"{amountValue:N0}원";
                string statusText = isOccupied ? "식사 중" : "빈 테이블";

                Button btnTable = new Button
                {
                    Width = 160,
                    Height = 130,
                    Margin = new Padding(10),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                    TextAlign = ContentAlignment.TopLeft,
                    Text = $" Table {i:D2}\n\n [{statusText}]\n 금액: {amountStr}",
                    Tag = i,
                    BackColor = isOccupied ? Color.FromArgb(231, 76, 60) : Color.FromArgb(46, 204, 113),
                    ForeColor = Color.White
                };

                btnTable.FlatAppearance.BorderSize = 0;
                btnTable.Click += TableCard_Click;
                flpTables.Controls.Add(btnTable);
            }

            int emptyTables = totalTables - occupiedTables;

            lblTotalTables.Text = $"{totalTables}개";
            lblOccupiedTables.Text = $"{occupiedTables}개";
            lblEmptyTables.Text = $"{emptyTables}개";
        }

        private void InitAutoRefresh()
        {
            string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (File.Exists(realtimePath))
                lastRealtimeModifiedTime = File.GetLastWriteTime(realtimePath);

            if (File.Exists(itemsPath))
                lastItemsModifiedTime = File.GetLastWriteTime(itemsPath);
        }

        private Dictionary<string, int> GetTableAmountsFromCsv()
        {
            Dictionary<string, int> amounts = new Dictionary<string, int>();
            string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");

            if (!File.Exists(realtimePath))
                return amounts;

            string[] lines = File.ReadAllLines(realtimePath, Encoding.UTF8);

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

                if (source != "키오스크" || orderType != "매장")
                    continue;

                if (!identifier.StartsWith("T", StringComparison.OrdinalIgnoreCase))
                    continue;

                string tableKey = ConvertIdentifierToTableKey(identifier);

                if (tableKey == null)
                    continue;

                int totalAmount = int.TryParse(parts[4].Trim(), out int amount) ? amount : 0;

                if (amounts.ContainsKey(tableKey))
                    amounts[tableKey] += totalAmount;
                else
                    amounts[tableKey] = totalAmount;
            }

            return amounts;
        }

        private void TableCard_Click(object sender, EventArgs e)
        {
            if (!(sender is Button btnTable) || !(btnTable.Tag is int tableNo))
                return;

            if (btnTable.Text.Contains("빈 테이블"))
            {
                MessageBox.Show($"Table {tableNo:D2}번은 현재 빈 테이블입니다.", "안내");
                return;
            }

            string tableKey = $"Table {tableNo:D2}";
            int totalAmount = GetTableTotalAmount(tableKey);

            ShowTableDetail(tableNo, totalAmount);
        }

        private void ShowTableDetail(int tableNo, int totalAmount)
        {
            string tablePrefix = $"T{tableNo:D2}-";
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Table {tableNo:D2} 주문 내역");
            sb.AppendLine("----------------------------------------");

            bool hasItem = false;

            if (File.Exists(itemsPath))
            {
                string[] lines = File.ReadAllLines(itemsPath, Encoding.UTF8);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 6)
                        continue;

                    string keyId = parts[0].Trim();

                    if (!keyId.StartsWith(tablePrefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    string menuName = parts[1].Trim();
                    int price = int.TryParse(parts[2].Trim(), out int p) ? p : 0;
                    int quantity = int.TryParse(parts[3].Trim(), out int q) ? q : 0;
                    int discountQty = int.TryParse(parts[4].Trim(), out int dq) ? dq : 0;
                    int subTotal = int.TryParse(parts[5].Trim(), out int st) ? st : 0;

                    sb.AppendLine($"{menuName}  {quantity}개  {subTotal:N0}원");

                    if (discountQty > 0)
                        sb.AppendLine($"  └ 할인 적용: {discountQty}개 / 단가 {price:N0}원");

                    hasItem = true;
                }
            }

            if (!hasItem)
                sb.AppendLine("주문 내역이 없습니다.");

            sb.AppendLine("----------------------------------------");
            sb.AppendLine($"총 금액: {totalAmount:N0}원");

            MessageBox.Show(sb.ToString(), $"Table {tableNo:D2} 상세", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string ConvertIdentifierToTableKey(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return null;

            string[] parts = identifier.Split('-');

            if (parts.Length < 2)
                return null;

            string tablePart = parts[0];

            if (!tablePart.StartsWith("T", StringComparison.OrdinalIgnoreCase))
                return null;

            string tableNumber = tablePart.Substring(1);

            if (!int.TryParse(tableNumber, out int number))
                return null;

            return $"Table {number:D2}";
        }

        private int GetTableTotalAmount(string tableKey)
        {
            Dictionary<string, int> amounts = GetTableAmountsFromCsv();
            return amounts.ContainsKey(tableKey) ? amounts[tableKey] : 0;
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            DateTime currentRealtimeModifiedTime = File.Exists(realtimePath) ? File.GetLastWriteTime(realtimePath) : DateTime.MinValue;
            DateTime currentItemsModifiedTime = File.Exists(itemsPath) ? File.GetLastWriteTime(itemsPath) : DateTime.MinValue;

            if (currentRealtimeModifiedTime != lastRealtimeModifiedTime || currentItemsModifiedTime != lastItemsModifiedTime)
            {
                lastRealtimeModifiedTime = currentRealtimeModifiedTime;
                lastItemsModifiedTime = currentItemsModifiedTime;
                LoadTableCards();
            }
        }
    }
}