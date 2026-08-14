using Newtonsoft.Json.Linq;
using SushiKioskAdmin.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SushiKioskAdmin
{
    public partial class MainAdminForm : Form
    {
        private Button currentSelectedButton;
        private TcpListener server;
        private bool isServerRunning = false;
        private const int SERVER_PORT = 9000;

        public MainAdminForm()
        {
            InitializeComponent();
            this.Size = new Size(1024, 768);
            this.MinimumSize = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            SetupSidebarStyle();
        }

        private void SetupSidebarStyle()
        {
            pnlSidebar.BackColor = Color.FromArgb(45, 45, 48);
            Button[] navButtons = { btnNavOrder, btnNavTable, btnNavMenu, btnNavHistory, btnNavUser, btnNavStock, btnNavReport };
            foreach (var btn in navButtons)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Margin = new Padding(0);
                btn.Padding = new Padding(0);
                btn.Height = 50;
                btn.Dock = DockStyle.Top;
                btn.BackColor = Color.FromArgb(45, 45, 48);
                btn.ForeColor = Color.White;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 65);
            }
        }

        private void MainAdminForm_Load(object sender, EventArgs e)
        {
            ShowView(new UcOrderBoard(), btnNavOrder);
            UpdateOrderNotice();
            StartSocketServer();
        }

        private void StartSocketServer()
        {
            isServerRunning = true;
            Task.Run(() =>
            {
                try
                {
                    server = new TcpListener(IPAddress.Any, SERVER_PORT);
                    server.Start();
                    System.Diagnostics.Debug.WriteLine($"[소켓 서버] 포트 {SERVER_PORT}에서 서버가 시작되었습니다.");
                    while (isServerRunning)
                    {
                        TcpClient client = server.AcceptTcpClient();
                        Task.Run(() => HandleClientCommunication(client));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[소켓 서버 오류] {ex.Message}");
                }
            });
        }

        private void HandleClientCommunication(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        return;

                    string jsonMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    JObject packet = JObject.Parse(jsonMessage);
                    string action = packet["Action"]?.ToString();
                    string responseJson;

                    if (action == "NEW_ORDER" || action == "NEW_APP_ORDER")
                        responseJson = ProcessNewOrder(packet);
                    else if (action == "PAYMENT_COMPLETE")
                        responseJson = ProcessPaymentComplete(packet);
                    else if (action == "APP_PICKUP_COMPLETE")
                        responseJson = ProcessAppPickupComplete(packet);
                    else if (action == "APP_REJECT_ORDER")
                        responseJson = ProcessAppReject(packet);
                    else if (action == "GET_ORDER_STATUS")
                        responseJson = ProcessGetOrderStatus(packet);
                    else
                        responseJson = "{\"Status\":\"FAIL\",\"Message\":\"Unknown action.\"}";

                    byte[] responseBytes = Encoding.UTF8.GetBytes(responseJson);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[통신 오류] {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        private string ProcessNewOrder(JObject packet)
        {
            try
            {
                string identifier = packet["Identifier"]?.ToString();
                string source = packet["Source"]?.ToString();
                string orderType = packet["OrderType"]?.ToString();
                string orderTime = packet["OrderTime"]?.ToString();
                decimal totalAmount = packet["TotalAmount"]?.Value<decimal>() ?? 0;
                string status = packet["Status"]?.ToString() ?? "접수 대기";

                if (string.IsNullOrWhiteSpace(identifier))
                    return "{\"Status\":\"FAIL\",\"Message\":\"Identifier is required.\"}";

                if (totalAmount < 0)
                    return "{\"Status\":\"FAIL\",\"Message\":\"Invalid order amount.\"}";

                int memberId = 0;
                int usedPoint = 0;
                int earnedPoint = 0;
                string paymentMethod = "앱선결제";

                if (source == "앱")
                {
                    memberId = packet["MemberId"]?.Value<int>() ?? 0;
                    usedPoint = packet["UsedPoint"]?.Value<int>() ?? 0;
                    paymentMethod = packet["PaymentMethod"]?.ToString() ?? "앱선결제";

                    if (usedPoint < 0 || usedPoint > totalAmount)
                        return "{\"Status\":\"FAIL\",\"Message\":\"Invalid point usage.\"}";

                    if (memberId == 0 && usedPoint > 0)
                        return "{\"Status\":\"FAIL\",\"Message\":\"Non-members cannot use points.\"}";

                    if (memberId > 0)
                    {
                        int currentPoint = GetMemberPoint(memberId);
                        if (usedPoint > currentPoint)
                            return "{\"Status\":\"FAIL\",\"Message\":\"Not enough points.\"}";
                    }

                    decimal paidAmount = totalAmount - usedPoint;
                    earnedPoint = memberId > 0 ? (int)(paidAmount * 0.01m) : 0;
                }

                string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
                string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");
                string realtimeLine = $"{identifier},{source},{orderType},{orderTime},{totalAmount},{status}";
                File.AppendAllLines(realtimePath, new[] { realtimeLine }, new UTF8Encoding(false));

                JArray items = packet["Items"] as JArray;
                if (items != null)
                {
                    List<string> itemLines = new List<string>();
                    foreach (var item in items)
                    {
                        string menuName = item["MenuName"]?.ToString();
                        decimal price = item["Price"]?.Value<decimal>() ?? 0;
                        int quantity = item["Quantity"]?.Value<int>() ?? 0;
                        int discountQty = item["DiscountQty"]?.Value<int>() ?? 0;
                        decimal subTotal = item["SubTotal"]?.Value<decimal>() ?? 0;
                        itemLines.Add($"{identifier},{menuName},{price},{quantity},{discountQty},{subTotal}");
                    }

                    if (itemLines.Count > 0)
                        File.AppendAllLines(itemsPath, itemLines, new UTF8Encoding(false));
                }

                if (source == "앱")
                    SaveAppPayment(identifier, memberId, usedPoint, earnedPoint, paymentMethod);

                UpdateOrderNotice();
                return "{\"Status\":\"SUCCESS\",\"Message\":\"Order registered successfully.\"}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[주문 접수 오류] {ex.Message}");
                return "{\"Status\":\"FAIL\",\"Message\":\"Order registration failed.\"}";
            }
        }

        private string ProcessPaymentComplete(JObject packet)
        {
            try
            {
                string identifier = packet["Identifier"]?.ToString();
                string paymentMethod = packet["PaymentMethod"]?.ToString() ?? "신용카드";
                int memberId = packet["MemberId"]?.Value<int>() ?? 0;
                int usedPoint = packet["UsedPoint"]?.Value<int>() ?? 0;
                decimal originalAmount = packet["OriginalAmount"]?.Value<decimal>() ?? 0;
                decimal totalAmount = packet["TotalAmount"]?.Value<decimal>() ?? 0;

                if (string.IsNullOrWhiteSpace(identifier))
                    return "{\"Status\":\"FAIL\",\"Message\":\"Identifier is required.\"}";

                if (originalAmount < 0 || totalAmount < 0 || usedPoint < 0)
                    return "{\"Status\":\"FAIL\",\"Message\":\"Invalid payment amount.\"}";

                if (originalAmount - usedPoint != totalAmount)
                    return "{\"Status\":\"FAIL\",\"Message\":\"Payment amount does not match point usage.\"}";

                int earnedPoint = 0;

                if (memberId > 0)
                {
                    int currentPoint = GetMemberPoint(memberId);

                    if (usedPoint > currentPoint)
                        return "{\"Status\":\"FAIL\",\"Message\":\"Not enough points.\"}";

                    earnedPoint = (int)(totalAmount * 0.01m);
                }
                else if (usedPoint > 0)
                {
                    return "{\"Status\":\"FAIL\",\"Message\":\"Non-members cannot use points.\"}";
                }

                string tablePrefix = GetTablePrefix(identifier);
                string paymentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string newReceiptNo = GenerateNewReceiptNumber();
                string source = "키오스크";
                string orderType = tablePrefix != null ? "매장" : "포장";
                string salesPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");
                string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");

                string salesLine = $"{newReceiptNo},{paymentDate},{source},{orderType},{originalAmount},{usedPoint},{totalAmount},{earnedPoint},{memberId},{paymentMethod}";
                File.AppendAllLines(salesPath, new[] { salesLine }, new UTF8Encoding(false));

                if (memberId > 0)
                {
                    bool pointUpdated = UpdateMemberPoint(memberId, usedPoint, earnedPoint);

                    if (!pointUpdated)
                    {
                        RemoveSalesHistory(newReceiptNo);
                        return "{\"Status\":\"FAIL\",\"Message\":\"Failed to update member points.\"}";
                    }
                }

                if (File.Exists(realtimePath))
                {
                    List<string> lines = File.ReadAllLines(realtimePath, Encoding.UTF8).ToList();

                    if (tablePrefix != null)
                    {
                        lines.RemoveAll(line =>
                        {
                            string[] parts = line.Split(',');
                            return parts.Length > 0 && parts[0].Trim().StartsWith(tablePrefix, StringComparison.OrdinalIgnoreCase);
                        });
                    }
                    else
                    {
                        lines.RemoveAll(line =>
                        {
                            string[] parts = line.Split(',');
                            return parts.Length > 0 && parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase);
                        });
                    }

                    File.WriteAllLines(realtimePath, lines, new UTF8Encoding(false));
                }

                if (tablePrefix != null)
                    UpdateTableItemKeyIds(tablePrefix, newReceiptNo);
                else
                    UpdateItemKeyId(identifier, newReceiptNo);

                UpdateOrderNotice();

                return new JObject
                {
                    ["Status"] = "SUCCESS",
                    ["ReceiptNo"] = newReceiptNo,
                    ["OriginalAmount"] = originalAmount,
                    ["UsedPoint"] = usedPoint,
                    ["TotalAmount"] = totalAmount,
                    ["EarnedPoint"] = earnedPoint,
                    ["Message"] = "Payment processed successfully."
                }.ToString(Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[결제 처리 오류] {ex.Message}");
                return "{\"Status\":\"FAIL\",\"Message\":\"Payment processing failed.\"}";
            }
        }

        private string ProcessAppPickupComplete(JObject packet)
        {
            try
            {
                string identifier = packet["Identifier"]?.ToString();

                if (string.IsNullOrWhiteSpace(identifier))
                    return "{\"Status\":\"FAIL\",\"Message\":\"Identifier is required.\"}";

                if (!GetRealtimeOrderInfo(identifier, out string orderType, out decimal originalAmount))
                    return "{\"Status\":\"FAIL\",\"Message\":\"Realtime order not found.\"}";

                if (!GetAppPayment(identifier, out int memberId, out int usedPoint, out int earnedPoint, out string paymentMethod))
                    return "{\"Status\":\"FAIL\",\"Message\":\"Payment information not found.\"}";

                decimal totalAmount = originalAmount - usedPoint;

                if (totalAmount < 0)
                    return "{\"Status\":\"FAIL\",\"Message\":\"Invalid payment amount.\"}";

                if (memberId > 0)
                {
                    int currentPoint = GetMemberPoint(memberId);

                    if (usedPoint > currentPoint)
                        return "{\"Status\":\"FAIL\",\"Message\":\"Not enough points.\"}";
                }

                string paymentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string receiptNo = identifier;
                string salesPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");

                string salesLine = $"{receiptNo},{paymentDate},앱,{orderType},{originalAmount},{usedPoint},{totalAmount},{earnedPoint},{memberId},{paymentMethod}";
                File.AppendAllLines(salesPath, new[] { salesLine }, new UTF8Encoding(false));

                if (memberId > 0)
                {
                    bool pointUpdated = UpdateMemberPoint(memberId, usedPoint, earnedPoint);

                    if (!pointUpdated)
                    {
                        RemoveSalesHistory(receiptNo);
                        return "{\"Status\":\"FAIL\",\"Message\":\"Failed to update member points.\"}";
                    }
                }

                RemoveRealtimeOrder(identifier);
                RemoveAppPayment(identifier);
                UpdateItemKeyId(identifier, receiptNo);
                UpdateOrderNotice();

                return new JObject
                {
                    ["Status"] = "SUCCESS",
                    ["ReceiptNo"] = receiptNo,
                    ["OriginalAmount"] = originalAmount,
                    ["UsedPoint"] = usedPoint,
                    ["TotalAmount"] = totalAmount,
                    ["EarnedPoint"] = earnedPoint,
                    ["Message"] = "App order pickup completed."
                }.ToString(Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[앱 주문 완료 오류] {ex.Message}");
                return "{\"Status\":\"FAIL\",\"Message\":\"App pickup processing failed.\"}";
            }
        }

        private string ProcessAppReject(JObject packet)
        {
            try
            {
                string identifier = packet["Identifier"]?.ToString();

                if (string.IsNullOrWhiteSpace(identifier))
                    return "{\"Status\":\"FAIL\",\"Message\":\"Identifier is required.\"}";

                if (!GetRealtimeOrderInfo(identifier, out string orderType, out decimal totalAmount))
                    return "{\"Status\":\"FAIL\",\"Message\":\"Realtime order not found.\"}";

                SaveRejectedOrder(identifier, orderType);
                RemoveRealtimeOrder(identifier);
                RemoveAppPayment(identifier);
                RemoveOrderItems(identifier);
                UpdateOrderNotice();

                return new JObject
                {
                    ["Status"] = "SUCCESS",
                    ["Identifier"] = identifier,
                    ["OrderStatus"] = "주문 거절",
                    ["Message"] = "App order rejected."
                }.ToString(Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[앱 주문 거절 오류] {ex.Message}");
                return "{\"Status\":\"FAIL\",\"Message\":\"App order rejection failed.\"}";
            }
        }

        private string ProcessGetOrderStatus(JObject packet)
        {
            try
            {
                string identifier = packet["Identifier"]?.ToString();

                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return new JObject
                    {
                        ["Status"] = "FAIL",
                        ["Message"] = "Identifier is required."
                    }.ToString(Newtonsoft.Json.Formatting.None);
                }

                string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");

                if (File.Exists(realtimePath))
                {
                    foreach (string line in File.ReadAllLines(realtimePath, Encoding.UTF8))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length < 6)
                            continue;

                        if (parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase))
                        {
                            return new JObject
                            {
                                ["Status"] = "SUCCESS",
                                ["Identifier"] = identifier,
                                ["OrderStatus"] = parts[5].Trim()
                            }.ToString(Newtonsoft.Json.Formatting.None);
                        }
                    }
                }

                string salesPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");

                if (File.Exists(salesPath))
                {
                    foreach (string line in File.ReadAllLines(salesPath, Encoding.UTF8))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length < 10)
                            continue;

                        if (parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase))
                        {
                            return new JObject
                            {
                                ["Status"] = "SUCCESS",
                                ["Identifier"] = identifier,
                                ["OrderStatus"] = "픽업 완료"
                            }.ToString(Newtonsoft.Json.Formatting.None);
                        }
                    }
                }

                string rejectionPath = Path.Combine(Application.StartupPath, "susi_order_rejections.csv");

                if (File.Exists(rejectionPath))
                {
                    foreach (string line in File.ReadAllLines(rejectionPath, Encoding.UTF8))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length < 4)
                            continue;

                        if (parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase))
                        {
                            return new JObject
                            {
                                ["Status"] = "SUCCESS",
                                ["Identifier"] = identifier,
                                ["OrderStatus"] = "주문 거절"
                            }.ToString(Newtonsoft.Json.Formatting.None);
                        }
                    }
                }

                return new JObject
                {
                    ["Status"] = "FAIL",
                    ["Identifier"] = identifier,
                    ["Message"] = "Order not found."
                }.ToString(Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[주문 상태 조회 오류] {ex.Message}");

                return new JObject
                {
                    ["Status"] = "FAIL",
                    ["Message"] = "Failed to get order status."
                }.ToString(Newtonsoft.Json.Formatting.None);
            }
        }

        private void SaveRejectedOrder(string identifier, string orderType)
        {
            string rejectionPath = Path.Combine(Application.StartupPath, "susi_order_rejections.csv");
            string rejectDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string line = $"{identifier},{rejectDate},앱,{orderType}";
            File.AppendAllLines(rejectionPath, new[] { line }, new UTF8Encoding(false));
        }

        private void SaveAppPayment(string identifier, int memberId, int usedPoint, int earnedPoint, string paymentMethod)
        {
            string paymentPath = Path.Combine(Application.StartupPath, "susi_order_payments.csv");
            string paymentLine = $"{identifier},{memberId},{usedPoint},{earnedPoint},{paymentMethod}";
            File.AppendAllLines(paymentPath, new[] { paymentLine }, new UTF8Encoding(false));
        }

        private bool GetAppPayment(string identifier, out int memberId, out int usedPoint, out int earnedPoint, out string paymentMethod)
        {
            memberId = 0;
            usedPoint = 0;
            earnedPoint = 0;
            paymentMethod = "앱선결제";

            string paymentPath = Path.Combine(Application.StartupPath, "susi_order_payments.csv");

            if (!File.Exists(paymentPath))
                return false;

            foreach (string line in File.ReadAllLines(paymentPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 5)
                    continue;

                if (!parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase))
                    continue;

                int.TryParse(parts[1].Trim(), out memberId);
                int.TryParse(parts[2].Trim(), out usedPoint);
                int.TryParse(parts[3].Trim(), out earnedPoint);
                paymentMethod = parts[4].Trim();

                return true;
            }

            return false;
        }

        private void RemoveAppPayment(string identifier)
        {
            string paymentPath = Path.Combine(Application.StartupPath, "susi_order_payments.csv");

            if (!File.Exists(paymentPath))
                return;

            List<string> lines = File.ReadAllLines(paymentPath, Encoding.UTF8).ToList();

            lines.RemoveAll(line =>
            {
                string[] parts = line.Split(',');
                return parts.Length > 0 && parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase);
            });

            File.WriteAllLines(paymentPath, lines, new UTF8Encoding(false));
        }

        private void RemoveOrderItems(string identifier)
        {
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (!File.Exists(itemsPath))
                return;

            List<string> lines = File.ReadAllLines(itemsPath, Encoding.UTF8).ToList();

            lines.RemoveAll(line =>
            {
                string[] parts = line.Split(',');
                return parts.Length > 0 && parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase);
            });

            File.WriteAllLines(itemsPath, lines, new UTF8Encoding(false));
        }

        private bool GetRealtimeOrderInfo(string identifier, out string orderType, out decimal totalAmount)
        {
            orderType = "";
            totalAmount = 0;

            string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");

            if (!File.Exists(realtimePath))
                return false;

            foreach (string line in File.ReadAllLines(realtimePath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 6)
                    continue;

                if (!parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase))
                    continue;

                orderType = parts[2].Trim();
                decimal.TryParse(parts[4].Trim(), out totalAmount);

                return true;
            }

            return false;
        }

        private void RemoveRealtimeOrder(string identifier)
        {
            string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");

            if (!File.Exists(realtimePath))
                return;

            List<string> lines = File.ReadAllLines(realtimePath, Encoding.UTF8).ToList();

            lines.RemoveAll(line =>
            {
                string[] parts = line.Split(',');
                return parts.Length > 0 && parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase);
            });

            File.WriteAllLines(realtimePath, lines, new UTF8Encoding(false));
        }

        private string GetTablePrefix(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return null;

            if (!identifier.StartsWith("T", StringComparison.OrdinalIgnoreCase))
                return null;

            int dashIndex = identifier.IndexOf('-');

            if (dashIndex >= 0)
                return identifier.Substring(0, dashIndex) + "-";

            return identifier + "-";
        }

        private void UpdateTableItemKeyIds(string tablePrefix, string newReceiptNo)
        {
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (!File.Exists(itemsPath))
                return;

            List<string> itemLines = File.ReadAllLines(itemsPath, Encoding.UTF8).ToList();

            for (int i = 0; i < itemLines.Count; i++)
            {
                string[] parts = itemLines[i].Split(',');

                if (parts.Length < 1)
                    continue;

                string keyId = parts[0].Trim();

                if (keyId.StartsWith(tablePrefix, StringComparison.OrdinalIgnoreCase))
                {
                    parts[0] = newReceiptNo;
                    itemLines[i] = string.Join(",", parts);
                }
            }

            File.WriteAllLines(itemsPath, itemLines, new UTF8Encoding(false));
        }

        private void UpdateItemKeyId(string oldKeyId, string newReceiptNo)
        {
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (!File.Exists(itemsPath))
                return;

            List<string> itemLines = File.ReadAllLines(itemsPath, Encoding.UTF8).ToList();

            for (int i = 0; i < itemLines.Count; i++)
            {
                string[] parts = itemLines[i].Split(',');

                if (parts.Length < 1)
                    continue;

                if (parts[0].Trim().Equals(oldKeyId, StringComparison.OrdinalIgnoreCase))
                {
                    parts[0] = newReceiptNo;
                    itemLines[i] = string.Join(",", parts);
                }
            }

            File.WriteAllLines(itemsPath, itemLines, new UTF8Encoding(false));
        }

        private void RemoveSalesHistory(string receiptNo)
        {
            string salesPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");

            if (!File.Exists(salesPath))
                return;

            List<string> lines = File.ReadAllLines(salesPath, Encoding.UTF8).ToList();

            lines.RemoveAll(line =>
            {
                string[] parts = line.Split(',');
                return parts.Length > 0 && parts[0].Trim().Equals(receiptNo, StringComparison.OrdinalIgnoreCase);
            });

            File.WriteAllLines(salesPath, lines, new UTF8Encoding(false));
        }

        private string GenerateNewReceiptNumber()
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            string prefix = $"ORD-{today}-";
            string salesPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");
            int maxSequence = 0;

            if (File.Exists(salesPath))
            {
                foreach (string line in File.ReadAllLines(salesPath, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] columns = line.Split(',');

                    if (columns.Length < 1)
                        continue;

                    string receiptNo = columns[0].Trim();

                    if (!receiptNo.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    string sequenceText = receiptNo.Substring(prefix.Length);

                    if (int.TryParse(sequenceText, out int sequence) && sequence > maxSequence)
                        maxSequence = sequence;
                }
            }

            return $"{prefix}{maxSequence + 1:D3}";
        }

        private int GetMemberPoint(int memberId)
        {
            string csvPath = Path.Combine(Application.StartupPath, "member.csv");

            if (!File.Exists(csvPath))
                return 0;

            foreach (string line in File.ReadAllLines(csvPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 7)
                    continue;

                if (int.TryParse(parts[0].Trim(), out int id) && id == memberId)
                {
                    if (int.TryParse(parts[4].Trim(), out int point))
                        return point;
                }
            }

            return 0;
        }

        private bool UpdateMemberPoint(int memberId, int usedPoint, int earnedPoint)
        {
            string csvPath = Path.Combine(Application.StartupPath, "member.csv");

            if (!File.Exists(csvPath))
                return false;

            string[] lines = File.ReadAllLines(csvPath, Encoding.UTF8);
            bool found = false;

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                string[] parts = lines[i].Split(',');

                if (parts.Length < 7)
                    continue;

                if (int.TryParse(parts[0].Trim(), out int id) && id == memberId)
                {
                    int currentPoint = int.TryParse(parts[4].Trim(), out int point) ? point : 0;

                    if (usedPoint < 0 || usedPoint > currentPoint)
                        return false;

                    int newPoint = currentPoint - usedPoint + earnedPoint;
                    parts[4] = newPoint.ToString();
                    lines[i] = string.Join(",", parts);
                    found = true;
                    break;
                }
            }

            if (!found)
                return false;

            File.WriteAllLines(csvPath, lines, new UTF8Encoding(false));

            return true;
        }

        public string CompleteAppOrder(string identifier)
        {
            JObject packet = new JObject
            {
                ["Action"] = "APP_PICKUP_COMPLETE",
                ["Identifier"] = identifier
            };

            return ProcessAppPickupComplete(packet);
        }

        public string RejectAppOrder(string identifier)
        {
            JObject packet = new JObject
            {
                ["Action"] = "APP_REJECT_ORDER",
                ["Identifier"] = identifier
            };

            return ProcessAppReject(packet);
        }

        public string CompleteKioskPayment(string identifier, int memberId, decimal originalAmount, int usedPoint, decimal totalAmount, string paymentMethod)
        {
            JObject packet = new JObject
            {
                ["Action"] = "PAYMENT_COMPLETE",
                ["Identifier"] = identifier,
                ["MemberId"] = memberId,
                ["OriginalAmount"] = originalAmount,
                ["UsedPoint"] = usedPoint,
                ["TotalAmount"] = totalAmount,
                ["PaymentMethod"] = paymentMethod
            };

            return ProcessPaymentComplete(packet);
        }

        public void UpdateOrderNotice()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateOrderNotice));
                return;
            }

            string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
            int waitingCount = 0;

            if (File.Exists(realtimePath))
            {
                foreach (string line in File.ReadAllLines(realtimePath, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 6)
                        continue;

                    string source = parts[1].Trim();
                    string status = parts[5].Trim();

                    if (source == "앱" && status == "접수 대기")
                        waitingCount++;
                }
            }

            UpdateNotice(waitingCount);
        }

        public void UpdateNotice(int waitingCount)
        {
            lblNotice.Text = $"신규 주문 [{waitingCount}건] 대기 중";
            lblNotice.ForeColor = Color.Yellow;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            isServerRunning = false;
            server?.Stop();
            base.OnFormClosed(e);
        }

        private void ShowView(UserControl view, Button clickedButton)
        {
            pnlMainContainer.Controls.Clear();
            view.Dock = DockStyle.Fill;
            pnlMainContainer.Controls.Add(view);
            view.BringToFront();
            HighlightButton(clickedButton);
        }

        private void HighlightButton(Button btn)
        {
            if (currentSelectedButton != null)
            {
                currentSelectedButton.BackColor = Color.FromArgb(45, 45, 48);
                currentSelectedButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 65);
            }

            currentSelectedButton = btn;

            if (currentSelectedButton != null)
            {
                Color activeColor = Color.FromArgb(0, 122, 204);
                currentSelectedButton.BackColor = activeColor;
                currentSelectedButton.FlatAppearance.MouseOverBackColor = activeColor;
            }
        }

        private void btnNavOrder_Click(object sender, EventArgs e) => ShowView(new UcOrderBoard(), (Button)sender);
        private void btnNavTable_Click(object sender, EventArgs e) => ShowView(new UcTableMonitor(), (Button)sender);
        private void btnNavMenu_Click(object sender, EventArgs e) => ShowView(new UcMenuManagement(), (Button)sender);
        private void btnNavHistory_Click(object sender, EventArgs e) => ShowView(new UcOrderHistory(), (Button)sender);
        private void btnNavUser_Click(object sender, EventArgs e) => ShowView(new UcUserManagement(), (Button)sender);
        private void btnNavStock_Click(object sender, EventArgs e) => ShowView(new UcStockManagement(), (Button)sender);
        private void btnNavReport_Click(object sender, EventArgs e) => ShowView(new UcSalesReport(), (Button)sender);

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("관리자 시스템을 종료하시겠습니까?", "시스템 종료", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                Application.Exit();
        }
    }
}