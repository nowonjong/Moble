using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SushiKioskAdmin.Views
{
    public partial class UcTableMonitor : UserControl
    {
        public UcTableMonitor()
        {
            InitializeComponent();
            LoadTableCards();
        }

        // ==========================================
        // 1. 테이블 카드 동적 생성 및 화면 배치 (CSV 연동)
        // ==========================================

        private void LoadTableCards()
        {
            flpTables.Controls.Clear();

            // susi_order_items.csv에서 테이블별 총 금액 및 착석 상태 계산
            Dictionary<string, int> tableAmounts = GetTableAmountsFromCsv();

            // 총 34개 테이블 동적 생성
            for (int i = 1; i <= 34; i++)
            {
                string tableKey = $"Table {i:D2}";

                // 해당 테이블 번호로 등록된 주문 금액이 있는지 확인
                bool isOccupied = tableAmounts.ContainsKey(tableKey) && tableAmounts[tableKey] > 0;
                int amountValue = isOccupied ? tableAmounts[tableKey] : 0;

                string amountStr = $"{amountValue:N0}원";
                string statusText = isOccupied ? "식사 중" : "빈 테이블";

                // 테이블 UI 버튼 카드 생성
                Button btnTable = new Button
                {
                    Width = 160,
                    Height = 130,
                    Margin = new Padding(10),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("맑은 고딕", 10, FontStyle.Bold),
                    TextAlign = ContentAlignment.TopLeft,
                    Text = $" Table {i:D2}\n\n [{statusText}]\n 금액: {amountStr}",
                    Tag = i, // 테이블 번호 저장
                    BackColor = isOccupied ? Color.FromArgb(231, 76, 60) : Color.FromArgb(46, 204, 113), // 빨강(사용중), 초록(빈테이블)
                    ForeColor = Color.White
                };

                btnTable.FlatAppearance.BorderSize = 0;
                btnTable.Click += TableCard_Click;

                flpTables.Controls.Add(btnTable);
            }
        }

        /// <summary>
        /// susi_order_items.csv 파일을 읽어 테이블별 총 주문 금액을 계산하여 딕셔너리로 반환
        /// CSV 구조: KeyId, MenuName, Price, Quantity, DiscountQty, SubTotal
        /// </summary>
        private Dictionary<string, int> GetTableAmountsFromCsv()
        {
            var amounts = new Dictionary<string, int>();
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (!File.Exists(itemsPath)) return amounts;

            string[] lines = File.ReadAllLines(itemsPath, Encoding.UTF8);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(',');
                if (parts.Length >= 6) // 컬럼 6개 구조 반영
                {
                    string key = parts[0].Trim(); // 예: "Table 02" 또는 영수증 번호

                    // "Table" 로 시작하는 항목만 테이블 현황 대상으로 집계
                    if (key.StartsWith("Table", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(parts[5].Trim(), out int subTotal)) // SubTotal은 index 5
                        {
                            if (amounts.ContainsKey(key))
                            {
                                amounts[key] += subTotal;
                            }
                            else
                            {
                                amounts[key] = subTotal;
                            }
                        }
                    }
                }
            }

            return amounts;
        }

        // ==========================================
        // 2. 디자이너 연결 이벤트 핸들러 (테이블 클릭)
        // ==========================================

        private void TableCard_Click(object sender, EventArgs e)
        {
            if (sender is Button btnTable && btnTable.Tag is int tableNo)
            {
                // 이미 빈 테이블인 경우 처리 제외
                if (btnTable.Text.Contains("빈 테이블"))
                {
                    MessageBox.Show($"Table {tableNo:D2}번은 현재 빈 테이블입니다.", "안내");
                    return;
                }

                // 퇴장 및 테이블 비우기 확인 창
                DialogResult result = MessageBox.Show(
                    $"Table {tableNo:D2}번의 퇴장 처리(테이블 비우기 및 정산)를 진행하시겠습니까?",
                    "테이블 정산 관리",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    string tableKey = $"Table {tableNo:D2}";

                    // 1. 해당 테이블의 총 금액 계산
                    int totalAmount = GetTableTotalAmount(tableKey);

                    // 2. susi_sales_history.csv에 매출 완료 기록 추가 (영수증 번호 생성)
                    string receiptNo = $"ORD-{DateTime.Now:yyyyMMdd}-{tableNo:D2}";
                    SaveToSalesHistory(receiptNo, totalAmount);

                    // 3. susi_order_items.csv에서 해당 테이블의 키를 영수증 번호로 변경하거나 정리 (여기서는 정산 완료 후 삭제 및 이동 처리)
                    ProcessTableCheckout(tableKey, receiptNo);

                    // 4. 초록색 빈 테이블 상태로 UI 즉시 업데이트
                    btnTable.BackColor = Color.FromArgb(46, 204, 113);
                    btnTable.Text = $" Table {tableNo:D2}\n\n [빈 테이블]\n 금액: 0원";

                    MessageBox.Show($"Table {tableNo:D2}번 정산이 완료되었습니다.\n(영수증 번호: {receiptNo})", "알림");
                }
            }
        }

        private int GetTableTotalAmount(string tableKey)
        {
            var amounts = GetTableAmountsFromCsv();
            return amounts.ContainsKey(tableKey) ? amounts[tableKey] : 0;
        }

        private void SaveToSalesHistory(string receiptNo, int totalAmount)
        {
            try
            {
                string historyPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");
                string paymentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string source = "키오스크";
                string orderType = "매장";
                string paymentMethod = "신용카드";

                // CSV 구조: ReceiptNo, PaymentDate, Source, OrderType, TotalAmount, PaymentMethod
                string newLine = $"{receiptNo},{paymentDate},{source},{orderType},{totalAmount},{paymentMethod}";

                File.AppendAllText(historyPath, newLine + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("매출 기록 저장 중 오류 발생: " + ex.Message);
            }
        }

        /// <summary>
        /// 정산 시 susi_orders_realtime 및 susi_order_items 정리
        /// </summary>
        private void ProcessTableCheckout(string tableKey, string receiptNo)
        {
            try
            {
                // 1. susi_orders_realtime.csv에서 해당 테이블 항목 제거
                string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
                if (File.Exists(realtimePath))
                {
                    var lines = File.ReadAllLines(realtimePath, Encoding.UTF8);
                    var sbRealtime = new StringBuilder();
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var parts = line.Split(',');
                        // Identifier가 tableKey로 시작하거나 포함되어 있으면 실시간에서 제외
                        if (parts.Length > 0 && !parts[0].Trim().StartsWith(tableKey, StringComparison.OrdinalIgnoreCase))
                        {
                            sbRealtime.AppendLine(line);
                        }
                    }
                    File.WriteAllText(realtimePath, sbRealtime.ToString(), new UTF8Encoding(false));
                }

                // 2. susi_order_items.csv에서 해당 테이블 키(KeyId)를 영수증 번호로 변경 (또는 유지)
                string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");
                if (File.Exists(itemsPath))
                {
                    var lines = File.ReadAllLines(itemsPath, Encoding.UTF8);
                    var sbItems = new StringBuilder();
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var parts = line.Split(',');
                        if (parts.Length >= 6)
                        {
                            string key = parts[0].Trim();
                            if (key.Equals(tableKey, StringComparison.OrdinalIgnoreCase))
                            {
                                // KeyId를 영수증 번호로 변경하여 과거 내역(4번 탭)에서 조회 가능하게 함
                                parts[0] = receiptNo;
                                sbItems.AppendLine(string.Join(",", parts));
                            }
                            else
                            {
                                sbItems.AppendLine(line);
                            }
                        }
                    }
                    File.WriteAllText(itemsPath, sbItems.ToString(), new UTF8Encoding(false));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("테이블 정산 데이터 처리 중 오류가 발생했습니다.\n" + ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}