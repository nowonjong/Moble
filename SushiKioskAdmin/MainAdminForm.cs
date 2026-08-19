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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SushiKioskAdmin
{
    public partial class MainAdminForm : Form
    {
        private Button currentSelectedButton;
        private TcpListener server;
        private bool isServerRunning = false;
        private bool noticeBlinkState = true;
        private int pendingOrderCount = 0;

        private const int SERVER_PORT = 9000;
        private const int MAX_MESSAGE_SIZE = 1024 * 1024;

        private readonly object memberFileLock = new object();
        private readonly object orderFileLock = new object();

        private class FileSnapshot
        {
            public bool Exists { get; set; }
            public byte[] Data { get; set; }
        }

        public MainAdminForm()
        {
            InitializeComponent();
            Size = new Size(1024, 768);
            MinimumSize = new Size(1024, 768);
            StartPosition = FormStartPosition.CenterScreen;
            SetupSidebarStyle();
        }

        // =========================================================
        // 기본 화면
        // =========================================================

        private void SetupSidebarStyle()
        {
            pnlSidebar.BackColor = Color.FromArgb(45, 45, 48);

            Button[] navButtons =
            {
                btnNavOrder, btnNavTable, btnNavMenu, btnNavHistory,
                btnNavUser, btnNavStock, btnNavReport
            };

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

        // =========================================================
        // TCP 서버
        // =========================================================

        private void StartSocketServer()
        {
            isServerRunning = true;

            Task.Run(() =>
            {
                try
                {
                    server = new TcpListener(IPAddress.Any, SERVER_PORT);
                    server.Start();
                    System.Diagnostics.Debug.WriteLine($"[소켓 서버] 포트 {SERVER_PORT}에서 서버 시작");

                    while (isServerRunning)
                    {
                        TcpClient client = server.AcceptTcpClient();
                        System.Diagnostics.Debug.WriteLine($"[클라이언트 연결] {client.Client.RemoteEndPoint}");
                        Task.Run(() => HandleClientCommunication(client));
                    }
                }
                catch (SocketException ex)
                {
                    if (isServerRunning)
                        System.Diagnostics.Debug.WriteLine($"[소켓 서버 오류] {ex.Message}");
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
                client.ReceiveTimeout = 10000;
                client.SendTimeout = 10000;

                using (NetworkStream stream = client.GetStream())
                {
                    string jsonMessage = ReadJsonMessage(stream);
                    System.Diagnostics.Debug.WriteLine($"[수신 원문] {jsonMessage}");

                    if (string.IsNullOrWhiteSpace(jsonMessage))
                        return;

                    JObject packet = JObject.Parse(jsonMessage);
                    System.Diagnostics.Debug.WriteLine($"[수신 파싱] {packet.ToString(Newtonsoft.Json.Formatting.None)}");

                    string action = packet["Action"]?.ToString()?.Trim();
                    string responseJson;

                    switch (action)
                    {
                        case "NEW_ORDER":
                        case "NEW_APP_ORDER":
                            responseJson = ProcessNewOrder(packet);
                            break;

                        case "PAYMENT_COMPLETE":
                            responseJson = ProcessPaymentComplete(packet);
                            break;

                        case "APP_PICKUP_COMPLETE":
                            responseJson = ProcessAppPickupComplete(packet);
                            break;

                        case "APP_REJECT_ORDER":
                            responseJson = ProcessAppReject(packet);
                            break;

                        case "GET_ORDER_STATUS":
                            responseJson = ProcessGetOrderStatus(packet);
                            break;

                        case "REGISTER_MEMBER":
                            responseJson = ProcessRegisterMember(packet);
                            break;

                        case "LOGIN_MEMBER":
                            responseJson = ProcessLoginMember(packet);
                            break;

                        case "GET_MEMBER":
                            responseJson = ProcessGetMember(packet);
                            break;

                        case "GET_MENU":
                            responseJson = ProcessGetMenu();
                            break;

                        default:
                            responseJson = Fail("Unknown action.");
                            break;
                    }

                    System.Diagnostics.Debug.WriteLine($"[처리 결과] {responseJson}");

                    byte[] responseBytes = Encoding.UTF8.GetBytes(responseJson);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                    stream.Flush();
                    System.Diagnostics.Debug.WriteLine($"[응답 전송 완료] {client.Client.RemoteEndPoint}");
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

        private string ReadJsonMessage(NetworkStream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[4096];

                while (ms.Length < MAX_MESSAGE_SIZE)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead <= 0)
                        break;

                    ms.Write(buffer, 0, bytesRead);
                    string json = Encoding.UTF8.GetString(ms.ToArray());

                    try
                    {
                        JObject.Parse(json);
                        return json;
                    }
                    catch (Newtonsoft.Json.JsonReaderException)
                    {
                    }
                }

                if (ms.Length >= MAX_MESSAGE_SIZE)
                    throw new InvalidOperationException("JSON message is too large.");

                if (ms.Length == 0)
                    return null;

                string finalJson = Encoding.UTF8.GetString(ms.ToArray());
                JObject.Parse(finalJson);
                return finalJson;
            }
        }

        // =========================================================
        // 주문 등록
        // =========================================================

        private string ProcessNewOrder(JObject packet)
        {
            try
            {
                string action = packet["Action"]?.ToString()?.Trim();
                string identifier = packet["Identifier"]?.ToString()?.Trim();
                string source = packet["Source"]?.ToString()?.Trim();
                string orderType = packet["OrderType"]?.ToString()?.Trim();
                string orderTime = packet["OrderTime"]?.ToString()?.Trim();
                decimal totalAmount = packet["TotalAmount"]?.Value<decimal>() ?? 0;
                string status = packet["Status"]?.ToString()?.Trim() ?? "접수 대기";

                if (string.IsNullOrWhiteSpace(identifier))
                    return Fail("Identifier is required.");

                if (string.IsNullOrWhiteSpace(source))
                    return Fail("Source is required.");

                if (string.IsNullOrWhiteSpace(orderType))
                    return Fail("OrderType is required.");

                if (totalAmount < 0)
                    return Fail("Invalid order amount.");

                if (action == "NEW_ORDER" && source != "키오스크")
                    return Fail("NEW_ORDER source must be kiosk.");

                if (action == "NEW_APP_ORDER" && source != "앱")
                    return Fail("NEW_APP_ORDER source must be app.");

                if (source == "키오스크")
                {
                    if (orderType != "매장" && orderType != "포장")
                        return Fail("Invalid kiosk order type.");

                    if (orderType == "매장" && !IsValidKioskTableOrderIdentifier(identifier))
                        return Fail("Kiosk table Identifier must use Tnn-nn format.");

                    if (orderType == "포장" && !IsValidKioskTakeoutIdentifier(identifier))
                        return Fail("Kiosk takeout Identifier must use K-yyyyMMdd-nnn format.");
                }

                if (source == "앱" && orderType != "포장" && orderType != "배달")
                    return Fail("Invalid app order type.");

                if (source == "앱" && !IsValidAppOrderIdentifier(identifier))
                    return Fail("Invalid app order identifier.");

                JArray items = packet["Items"] as JArray;

                if (items == null || items.Count == 0)
                    return Fail("Order items are required.");

                decimal calculatedTotal = 0;

                foreach (JToken item in items)
                {
                    string menuName = item["MenuName"]?.ToString()?.Trim();
                    decimal price = item["Price"]?.Value<decimal>() ?? 0;
                    int quantity = item["Quantity"]?.Value<int>() ?? 0;
                    int discountQty = item["DiscountQty"]?.Value<int>() ?? 0;
                    decimal subTotal = item["SubTotal"]?.Value<decimal>() ?? 0;

                    if (string.IsNullOrWhiteSpace(menuName))
                        return Fail("MenuName is required.");

                    if (menuName.Contains(","))
                        return Fail("Comma cannot be used in MenuName.");

                    if (price < 0 || quantity <= 0 || discountQty < 0 || discountQty > quantity)
                        return Fail("Invalid order item.");

                    decimal expectedSubTotal = (quantity - discountQty) * price;

                    if (subTotal != expectedSubTotal)
                        return Fail("Invalid item subtotal.");

                    calculatedTotal += subTotal;
                }

                if (calculatedTotal != totalAmount)
                    return Fail("Order total does not match item total.");

                int memberId = 0;
                int usedPoint = 0;
                int earnedPoint = 0;
                string paymentMethod = "앱선결제";

                if (source == "앱")
                {
                    memberId = packet["MemberId"]?.Value<int>() ?? 0;
                    usedPoint = packet["UsedPoint"]?.Value<int>() ?? 0;
                    paymentMethod = packet["PaymentMethod"]?.ToString()?.Trim() ?? "앱선결제";

                    if (usedPoint < 0 || usedPoint > totalAmount)
                        return Fail("Invalid point usage.");

                    if (memberId == 0 && usedPoint > 0)
                        return Fail("Non-members cannot use points.");

                    if (memberId > 0 && !MemberExists(memberId))
                        return Fail("Member not found.");

                    decimal paidAmount = totalAmount - usedPoint;
                    earnedPoint = memberId > 0 ? (int)(paidAmount * 0.01m) : 0;
                }

                lock (orderFileLock)
                {
                    if (OrderIdentifierExists(identifier))
                    {
                        return new JObject
                        {
                            ["Status"] = "FAIL",
                            ["Identifier"] = identifier,
                            ["Message"] = "Duplicate order identifier."
                        }.ToString(Newtonsoft.Json.Formatting.None);
                    }

                    // 앱 포인트 예약 검사는 반드시 저장과 같은 lock 안에서 처리
                    if (source == "앱" && memberId > 0)
                    {
                        int availablePoint = GetAvailableMemberPoint(memberId);

                        if (usedPoint > availablePoint)
                            return Fail("Not enough available points.");
                    }

                    string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
                    string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");
                    string realtimeLine = $"{identifier},{source},{orderType},{orderTime},{totalAmount},{status}";
                    AppendCsvLinesSafe(realtimePath, new[] { realtimeLine });

                    List<string> itemLines = new List<string>();

                    foreach (JToken item in items)
                    {
                        string menuName = item["MenuName"]?.ToString()?.Trim();
                        decimal price = item["Price"]?.Value<decimal>() ?? 0;
                        int quantity = item["Quantity"]?.Value<int>() ?? 0;
                        int discountQty = item["DiscountQty"]?.Value<int>() ?? 0;
                        decimal subTotal = item["SubTotal"]?.Value<decimal>() ?? 0;

                        itemLines.Add($"{identifier},{menuName},{price},{quantity},{discountQty},{subTotal}");
                    }

                    AppendCsvLinesSafe(itemsPath, itemLines);

                    if (source == "앱")
                        SaveAppPayment(identifier, memberId, usedPoint, earnedPoint, paymentMethod);
                }

                UpdateOrderNotice();
                RefreshOrderBoard();

                return Success("Order registered successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[주문 접수 오류] {ex.Message}");
                return Fail("Order registration failed.");
            }
        }

        // =========================================================
        // 키오스크 결제
        // =========================================================

        private string ProcessPaymentComplete(JObject packet)
        {
            string salesPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");
            string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");
            string memberPath = Path.Combine(Application.StartupPath, "member.csv");

            FileSnapshot salesBackup = null;
            FileSnapshot realtimeBackup = null;
            FileSnapshot itemsBackup = null;
            FileSnapshot memberBackup = null;

            try
            {
                string identifier = packet["Identifier"]?.ToString()?.Trim();
                string paymentMethod = packet["PaymentMethod"]?.ToString()?.Trim() ?? "신용카드";
                int memberId = packet["MemberId"]?.Value<int>() ?? 0;
                int usedPoint = packet["UsedPoint"]?.Value<int>() ?? 0;
                decimal originalAmount = packet["OriginalAmount"]?.Value<decimal>() ?? 0;
                decimal totalAmount = packet["TotalAmount"]?.Value<decimal>() ?? 0;

                if (string.IsNullOrWhiteSpace(identifier))
                    return Fail("Identifier is required.");

                if (originalAmount < 0 || totalAmount < 0 || usedPoint < 0)
                    return Fail("Invalid payment amount.");

                if (originalAmount - usedPoint != totalAmount)
                    return Fail("Payment amount does not match point usage.");

                if (memberId == 0 && usedPoint > 0)
                    return Fail("Non-members cannot use points.");

                if (memberId > 0 && !MemberExists(memberId))
                    return Fail("Member not found.");

                int earnedPoint = memberId > 0 ? (int)(totalAmount * 0.01m) : 0;
                string tablePrefix = GetTablePrefix(identifier);
                string orderType;
                decimal serverOrderAmount;

                lock (orderFileLock)
                {
                    if (tablePrefix != null)
                    {
                        orderType = "매장";

                        if (!TryGetTableOrderTotal(tablePrefix, out serverOrderAmount))
                            return Fail("Realtime order not found.");
                    }
                    else
                    {
                        if (!GetRealtimeOrderInfo(identifier, out orderType, out serverOrderAmount))
                            return Fail("Realtime order not found.");

                        if (orderType != "포장")
                            return Fail("Invalid kiosk payment identifier.");

                        if (!IsValidKioskTakeoutIdentifier(identifier))
                            return Fail("Invalid kiosk takeout identifier.");
                    }

                    if (serverOrderAmount != originalAmount)
                    {
                        return new JObject
                        {
                            ["Status"] = "FAIL",
                            ["ServerAmount"] = serverOrderAmount,
                            ["ReceivedAmount"] = originalAmount,
                            ["Message"] = "OriginalAmount does not match server order total."
                        }.ToString(Newtonsoft.Json.Formatting.None);
                    }

                    // 앱에서 예약 중인 포인트까지 고려
                    if (memberId > 0)
                    {
                        int availablePoint = GetAvailableMemberPoint(memberId);

                        if (usedPoint > availablePoint)
                            return Fail("Not enough available points.");
                    }

                    // 실제 파일 변경 직전에 한 번 더 확인
                    if (tablePrefix != null)
                    {
                        if (!TryGetTableOrderTotal(tablePrefix, out decimal checkAmount))
                            return Fail("Order already processed.");

                        if (checkAmount != originalAmount)
                            return Fail("Order amount changed.");
                    }
                    else
                    {
                        if (!GetRealtimeOrderInfo(identifier, out _, out decimal checkAmount))
                            return Fail("Order already processed.");

                        if (checkAmount != originalAmount)
                            return Fail("Order amount changed.");
                    }

                    salesBackup = CreateSnapshot(salesPath);
                    realtimeBackup = CreateSnapshot(realtimePath);
                    itemsBackup = CreateSnapshot(itemsPath);

                    if (memberId > 0)
                        memberBackup = CreateSnapshot(memberPath);

                    string newReceiptNo = GenerateNewReceiptNumber();
                    string paymentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    // 포인트 먼저 처리하고 이후 작업 실패 시 백업으로 모두 복구
                    if (memberId > 0)
                    {
                        bool pointUpdated = UpdateMemberPoint(memberId, usedPoint, earnedPoint);

                        if (!pointUpdated)
                            return Fail("Failed to update member points.");
                    }

                    try
                    {
                        string salesLine = $"{newReceiptNo},{paymentDate},키오스크,{orderType},{originalAmount},{usedPoint},{totalAmount},{earnedPoint},{memberId},{paymentMethod}";
                        AppendCsvLinesSafe(salesPath, new[] { salesLine });

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
                    }
                    catch
                    {
                        RestoreSnapshot(salesPath, salesBackup);
                        RestoreSnapshot(realtimePath, realtimeBackup);
                        RestoreSnapshot(itemsPath, itemsBackup);

                        if (memberId > 0)
                        {
                            lock (memberFileLock)
                                RestoreSnapshot(memberPath, memberBackup);
                        }

                        throw;
                    }

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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[결제 처리 오류] {ex.Message}");
                return Fail("Payment processing failed.");
            }
        }

        // =========================================================
        // 앱 픽업 완료
        // =========================================================

        private string ProcessAppPickupComplete(JObject packet)
        {
            string salesPath = Path.Combine(Application.StartupPath, "susi_sales_history.csv");
            string realtimePath = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
            string itemsPath = Path.Combine(Application.StartupPath, "susi_order_items.csv");
            string paymentPath = Path.Combine(Application.StartupPath, "susi_order_payments.csv");
            string memberPath = Path.Combine(Application.StartupPath, "member.csv");

            try
            {
                string identifier = packet["Identifier"]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(identifier))
                    return Fail("Identifier is required.");

                lock (orderFileLock)
                {
                    if (!GetRealtimeOrderInfo(identifier, out string orderType, out decimal originalAmount))
                        return Fail("Realtime order not found.");

                    if (!GetAppPayment(identifier, out int memberId, out int usedPoint, out int earnedPoint, out string paymentMethod))
                        return Fail("Payment information not found.");

                    decimal totalAmount = originalAmount - usedPoint;

                    if (totalAmount < 0)
                        return Fail("Invalid payment amount.");

                    if (SalesIdentifierExists(identifier))
                        return Fail("Order already completed.");

                    if (memberId > 0)
                    {
                        if (!MemberExists(memberId))
                            return Fail("Member not found.");

                        int currentPoint = GetMemberPoint(memberId);

                        if (usedPoint > currentPoint)
                            return Fail("Not enough points.");
                    }

                    FileSnapshot salesBackup = CreateSnapshot(salesPath);
                    FileSnapshot realtimeBackup = CreateSnapshot(realtimePath);
                    FileSnapshot itemsBackup = CreateSnapshot(itemsPath);
                    FileSnapshot paymentBackup = CreateSnapshot(paymentPath);
                    FileSnapshot memberBackup = memberId > 0 ? CreateSnapshot(memberPath) : null;

                    string paymentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string receiptNo = identifier;

                    if (memberId > 0)
                    {
                        bool pointUpdated = UpdateMemberPoint(memberId, usedPoint, earnedPoint);

                        if (!pointUpdated)
                            return Fail("Failed to update member points.");
                    }

                    try
                    {
                        string salesLine = $"{receiptNo},{paymentDate},앱,{orderType},{originalAmount},{usedPoint},{totalAmount},{earnedPoint},{memberId},{paymentMethod}";
                        AppendCsvLinesSafe(salesPath, new[] { salesLine });

                        RemoveRealtimeOrder(identifier);
                        RemoveAppPayment(identifier);

                        // 앱은 Identifier와 ReceiptNo가 같으므로 실질적으로 값은 그대로지만 유지
                        UpdateItemKeyId(identifier, receiptNo);
                    }
                    catch
                    {
                        RestoreSnapshot(salesPath, salesBackup);
                        RestoreSnapshot(realtimePath, realtimeBackup);
                        RestoreSnapshot(itemsPath, itemsBackup);
                        RestoreSnapshot(paymentPath, paymentBackup);

                        if (memberId > 0)
                        {
                            lock (memberFileLock)
                                RestoreSnapshot(memberPath, memberBackup);
                        }

                        throw;
                    }

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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[앱 주문 완료 오류] {ex.Message}");
                return Fail("App pickup processing failed.");
            }
        }

        // =========================================================
        // 앱 주문 거절
        // =========================================================

        private string ProcessAppReject(JObject packet)
        {
            try
            {
                string identifier = packet["Identifier"]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(identifier))
                    return Fail("Identifier is required.");

                lock (orderFileLock)
                {
                    if (!GetRealtimeOrderInfo(identifier, out string orderType, out decimal originalAmount))
                        return Fail("Realtime order not found.");

                    int memberId = 0;
                    int usedPoint = 0;
                    int earnedPoint = 0;
                    string paymentMethod = "앱선결제";

                    GetAppPayment(identifier, out memberId, out usedPoint, out earnedPoint, out paymentMethod);

                    decimal refundAmount = originalAmount - usedPoint;

                    if (refundAmount < 0)
                        refundAmount = 0;

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
                        ["RefundRequired"] = true,
                        ["RefundAmount"] = refundAmount,
                        ["PaymentMethod"] = paymentMethod,
                        ["Message"] = "App order rejected. Prepayment cancellation is required."
                    }.ToString(Newtonsoft.Json.Formatting.None);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[앱 주문 거절 오류] {ex.Message}");
                return Fail("App order rejection failed.");
            }
        }

        // =========================================================
        // 주문 상태 조회
        // =========================================================

        private string ProcessGetOrderStatus(JObject packet)
        {
            try
            {
                string identifier = packet["Identifier"]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(identifier))
                    return Fail("Identifier is required.");

                lock (orderFileLock)
                {
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

                    if (SalesIdentifierExists(identifier))
                    {
                        return new JObject
                        {
                            ["Status"] = "SUCCESS",
                            ["Identifier"] = identifier,
                            ["OrderStatus"] = "픽업 완료"
                        }.ToString(Newtonsoft.Json.Formatting.None);
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
                                    ["OrderStatus"] = "주문 거절",
                                    ["RefundRequired"] = true,
                                    ["Message"] = "Order rejected. Check prepayment cancellation."
                                }.ToString(Newtonsoft.Json.Formatting.None);
                            }
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
                return Fail("Failed to get order status.");
            }
        }

        // =========================================================
        // 회원가입
        // =========================================================

        private string ProcessRegisterMember(JObject packet)
        {
            try
            {
                string memberName = packet["MemberName"]?.ToString()?.Trim();
                string phone = packet["Phone"]?.ToString()?.Trim();
                string password = packet["Password"]?.ToString()?.Trim();
                string address = packet["Address"]?.ToString()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(memberName))
                    return Fail("MemberName is required.");

                if (string.IsNullOrWhiteSpace(phone))
                    return Fail("Phone is required.");

                if (string.IsNullOrWhiteSpace(password))
                    return Fail("Password is required.");

                if (memberName.Contains(",") || phone.Contains(",") || password.Contains(",") || address.Contains(","))
                    return Fail("Comma cannot be used in member information.");

                lock (memberFileLock)
                {
                    string memberPath = Path.Combine(Application.StartupPath, "member.csv");

                    if (!File.Exists(memberPath))
                        File.WriteAllText(memberPath, "", new UTF8Encoding(false));

                    string[] lines = File.ReadAllLines(memberPath, Encoding.UTF8);

                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length < 7)
                            continue;

                        if (parts[2].Trim().Equals(phone, StringComparison.OrdinalIgnoreCase))
                            return Fail("Phone number already registered.");
                    }

                    int memberId = GenerateNextMemberId(lines);
                    int point = 0;
                    string joinDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string memberLine = $"{memberId},{memberName},{phone},{password},{point},{address},{joinDate}";

                    AppendCsvLinesSafe(memberPath, new[] { memberLine });

                    return new JObject
                    {
                        ["Status"] = "SUCCESS",
                        ["MemberId"] = memberId,
                        ["Point"] = point,
                        ["JoinDate"] = joinDate,
                        ["Message"] = "Member registered successfully."
                    }.ToString(Newtonsoft.Json.Formatting.None);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[회원가입 오류] {ex.Message}");
                return Fail("Member registration failed.");
            }
        }

        // =========================================================
        // 로그인
        // =========================================================

        private string ProcessLoginMember(JObject packet)
        {
            try
            {
                string phone = packet["Phone"]?.ToString()?.Trim();
                string password = packet["Password"]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(phone))
                    return Fail("Phone is required.");

                if (string.IsNullOrWhiteSpace(password))
                    return Fail("Password is required.");

                lock (memberFileLock)
                {
                    string memberPath = Path.Combine(Application.StartupPath, "member.csv");

                    if (!File.Exists(memberPath))
                        return Fail("Member data not found.");

                    foreach (string line in File.ReadAllLines(memberPath, Encoding.UTF8))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length < 7)
                            continue;

                        string savedPhone = parts[2].Trim();

                        if (!savedPhone.Equals(phone, StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (parts[3].Trim() != password)
                            return Fail("Invalid phone or password.");

                        int.TryParse(parts[0].Trim(), out int memberId);
                        int.TryParse(parts[4].Trim(), out int point);

                        return new JObject
                        {
                            ["Status"] = "SUCCESS",
                            ["MemberId"] = memberId,
                            ["MemberName"] = parts[1].Trim(),
                            ["Phone"] = savedPhone,
                            ["Point"] = point,
                            ["Address"] = parts[5].Trim(),
                            ["JoinDate"] = parts[6].Trim(),
                            ["Message"] = "Login successful."
                        }.ToString(Newtonsoft.Json.Formatting.None);
                    }
                }

                return Fail("Invalid phone or password.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[로그인 오류] {ex.Message}");
                return Fail("Login failed.");
            }
        }

        // =========================================================
        // 회원 조회
        // =========================================================

        private string ProcessGetMember(JObject packet)
        {
            try
            {
                string phone = packet["Phone"]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(phone))
                    return Fail("Phone is required.");

                lock (memberFileLock)
                {
                    string memberPath = Path.Combine(Application.StartupPath, "member.csv");

                    if (!File.Exists(memberPath))
                        return Fail("Member data not found.");

                    foreach (string line in File.ReadAllLines(memberPath, Encoding.UTF8))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length < 7)
                            continue;

                        if (!parts[2].Trim().Equals(phone, StringComparison.OrdinalIgnoreCase))
                            continue;

                        int.TryParse(parts[0].Trim(), out int memberId);
                        int.TryParse(parts[4].Trim(), out int point);

                        return new JObject
                        {
                            ["Status"] = "SUCCESS",
                            ["MemberId"] = memberId,
                            ["MemberName"] = parts[1].Trim(),
                            ["Phone"] = parts[2].Trim(),
                            ["Point"] = point,
                            ["Message"] = "Member found."
                        }.ToString(Newtonsoft.Json.Formatting.None);
                    }
                }

                return Fail("Member not found.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[회원 조회 오류] {ex.Message}");
                return Fail("Failed to get member information.");
            }
        }

        // =========================================================
        // 메뉴 조회
        // =========================================================

        private string ProcessGetMenu()
        {
            try
            {
                string menuPath = Path.Combine(Application.StartupPath, "susi_menu.csv");

                if (!File.Exists(menuPath))
                    return Fail("Menu data not found.");

                JArray menus = new JArray();

                foreach (string line in File.ReadAllLines(menuPath, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 7)
                        continue;

                    if (!int.TryParse(parts[0].Trim(), out int menuId))
                        continue;

                    int.TryParse(parts[4].Trim(), out int price);

                    menus.Add(new JObject
                    {
                        ["MenuId"] = menuId,
                        ["KoreanName"] = parts[1].Trim(),
                        ["JapaneseName"] = parts[2].Trim(),
                        ["EnglishName"] = parts[3].Trim(),
                        ["Price"] = price,
                        ["SaleStatus"] = parts[5].Trim(),
                        ["ImageFile"] = parts[6].Trim()
                    });
                }

                return new JObject
                {
                    ["Status"] = "SUCCESS",
                    ["Menus"] = menus
                }.ToString(Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[메뉴 조회 오류] {ex.Message}");
                return Fail("Failed to get menu information.");
            }
        }

        // =========================================================
        // 회원 / 포인트
        // =========================================================

        private bool MemberExists(int memberId)
        {
            lock (memberFileLock)
            {
                string path = Path.Combine(Application.StartupPath, "member.csv");

                if (!File.Exists(path))
                    return false;

                foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 7)
                        continue;

                    if (int.TryParse(parts[0].Trim(), out int id) && id == memberId)
                        return true;
                }

                return false;
            }
        }

        private int GetMemberPoint(int memberId)
        {
            lock (memberFileLock)
            {
                string path = Path.Combine(Application.StartupPath, "member.csv");

                if (!File.Exists(path))
                    return 0;

                foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 7)
                        continue;

                    if (int.TryParse(parts[0].Trim(), out int id) && id == memberId)
                    {
                        int.TryParse(parts[4].Trim(), out int point);
                        return point;
                    }
                }

                return 0;
            }
        }

        private int GetReservedPoint(int memberId)
        {
            string path = Path.Combine(Application.StartupPath, "susi_order_payments.csv");

            if (!File.Exists(path))
                return 0;

            int reserved = 0;

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 5)
                    continue;

                if (!int.TryParse(parts[1].Trim(), out int savedMemberId) || savedMemberId != memberId)
                    continue;

                if (int.TryParse(parts[2].Trim(), out int usedPoint))
                    reserved += usedPoint;
            }

            return reserved;
        }

        private int GetAvailableMemberPoint(int memberId)
        {
            int available = GetMemberPoint(memberId) - GetReservedPoint(memberId);
            return Math.Max(0, available);
        }

        private bool UpdateMemberPoint(int memberId, int usedPoint, int earnedPoint)
        {
            lock (memberFileLock)
            {
                string path = Path.Combine(Application.StartupPath, "member.csv");

                if (!File.Exists(path))
                    return false;

                string[] lines = File.ReadAllLines(path, Encoding.UTF8);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] parts = lines[i].Split(',');

                    if (parts.Length < 7)
                        continue;

                    if (!int.TryParse(parts[0].Trim(), out int id) || id != memberId)
                        continue;

                    int.TryParse(parts[4].Trim(), out int currentPoint);

                    if (usedPoint < 0 || usedPoint > currentPoint)
                        return false;

                    parts[4] = (currentPoint - usedPoint + earnedPoint).ToString();
                    lines[i] = string.Join(",", parts);

                    File.WriteAllLines(path, lines, new UTF8Encoding(false));
                    return true;
                }

                return false;
            }
        }

        public bool UpdateMemberInfo(int memberId, string name, string phone, string address, out string message)
        {
            lock (memberFileLock)
            {
                try
                {
                    string path = Path.Combine(Application.StartupPath, "member.csv");

                    if (!File.Exists(path))
                    {
                        message = "Member data not found.";
                        return false;
                    }

                    string[] lines = File.ReadAllLines(path, Encoding.UTF8);
                    int targetIndex = -1;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i]))
                            continue;

                        string[] parts = lines[i].Split(',');

                        if (parts.Length < 7)
                            continue;

                        if (int.TryParse(parts[0].Trim(), out int id) && id == memberId)
                            targetIndex = i;

                        if (int.TryParse(parts[0].Trim(), out int otherId) &&
                            otherId != memberId &&
                            parts[2].Trim().Equals(phone, StringComparison.OrdinalIgnoreCase))
                        {
                            message = "이미 등록된 연락처입니다.";
                            return false;
                        }
                    }

                    if (targetIndex < 0)
                    {
                        message = "회원 정보를 찾을 수 없습니다.";
                        return false;
                    }

                    string[] target = lines[targetIndex].Split(',');

                    target[1] = name;
                    target[2] = phone;
                    target[5] = address;

                    lines[targetIndex] = string.Join(",", target);

                    File.WriteAllLines(path, lines, new UTF8Encoding(false));

                    message = "SUCCESS";
                    return true;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return false;
                }
            }
        }

        public bool DeleteMember(int memberId, out string message)
        {
            lock (memberFileLock)
            {
                try
                {
                    if (HasPendingAppOrder(memberId))
                    {
                        message = "진행 중인 앱 주문이 있는 회원은 삭제할 수 없습니다.";
                        return false;
                    }

                    string path = Path.Combine(Application.StartupPath, "member.csv");

                    if (!File.Exists(path))
                    {
                        message = "Member data not found.";
                        return false;
                    }

                    List<string> lines = File.ReadAllLines(path, Encoding.UTF8).ToList();

                    int removedCount = lines.RemoveAll(line =>
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            return false;

                        string[] parts = line.Split(',');

                        return parts.Length >= 7 &&
                               int.TryParse(parts[0].Trim(), out int id) &&
                               id == memberId;
                    });

                    if (removedCount == 0)
                    {
                        message = "회원 정보를 찾을 수 없습니다.";
                        return false;
                    }

                    File.WriteAllLines(path, lines, new UTF8Encoding(false));

                    message = "SUCCESS";
                    return true;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return false;
                }
            }
        }

        private bool HasPendingAppOrder(int memberId)
        {
            string path = Path.Combine(Application.StartupPath, "susi_order_payments.csv");

            if (!File.Exists(path))
                return false;

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 5)
                    continue;

                if (int.TryParse(parts[1].Trim(), out int savedMemberId) &&
                    savedMemberId == memberId)
                    return true;
            }

            return false;
        }

        // =========================================================
        // 주문 조회 / Identifier
        // =========================================================

        private bool TryGetTableOrderTotal(string tablePrefix, out decimal total)
        {
            total = 0;

            string path = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");

            if (!File.Exists(path))
                return false;

            bool found = false;

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 6)
                    continue;

                if (!parts[0].Trim().StartsWith(tablePrefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (parts[1].Trim() != "키오스크" || parts[2].Trim() != "매장")
                    continue;

                if (decimal.TryParse(parts[4].Trim(), out decimal amount))
                {
                    total += amount;
                    found = true;
                }
            }

            return found;
        }

        private bool GetRealtimeOrderInfo(string identifier, out string orderType, out decimal totalAmount)
        {
            orderType = "";
            totalAmount = 0;

            string path = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");

            if (!File.Exists(path))
                return false;

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
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

        private bool OrderIdentifierExists(string identifier)
        {
            string[] files =
            {
                "susi_orders_realtime.csv",
                "susi_sales_history.csv",
                "susi_order_rejections.csv"
            };

            foreach (string file in files)
            {
                string path = Path.Combine(Application.StartupPath, file);

                if (!File.Exists(path))
                    continue;

                foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length > 0 && parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        private bool SalesIdentifierExists(string identifier)
        {
            string path = Path.Combine(Application.StartupPath, "susi_sales_history.csv");

            if (!File.Exists(path))
                return false;

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length > 0 && parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private bool IsValidKioskTableOrderIdentifier(string identifier)
        {
            return Regex.IsMatch(identifier ?? "", @"^T\d{2}-\d{2,}$", RegexOptions.IgnoreCase);
        }

        private bool IsValidKioskTakeoutIdentifier(string identifier)
        {
            return Regex.IsMatch(identifier ?? "", @"^K-\d{8}-\d{3,}$", RegexOptions.IgnoreCase);
        }

        private bool IsValidAppOrderIdentifier(string identifier)
        {
            return Regex.IsMatch(identifier ?? "", @"^ORD-\d{8}-APP\d+$", RegexOptions.IgnoreCase);
        }

        private string GetTablePrefix(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return null;

            Match match = Regex.Match(identifier, @"^(T\d{2})$", RegexOptions.IgnoreCase);

            if (!match.Success)
                return null;

            return match.Groups[1].Value.ToUpper() + "-";
        }

        // =========================================================
        // CSV 처리
        // =========================================================

        private void AppendCsvLinesSafe(string path, IEnumerable<string> lines)
        {
            if (lines == null)
                return;

            List<string> lineList = lines.Where(line => line != null).ToList();

            if (lineList.Count == 0)
                return;

            bool needsNewLine = false;

            if (File.Exists(path))
            {
                using (FileStream readStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (readStream.Length > 0)
                    {
                        readStream.Seek(-1, SeekOrigin.End);
                        int lastByte = readStream.ReadByte();
                        needsNewLine = lastByte != '\n' && lastByte != '\r';
                    }
                }
            }

            using (FileStream writeStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(writeStream, new UTF8Encoding(false)))
            {
                if (needsNewLine)
                    writer.WriteLine();

                foreach (string line in lineList)
                    writer.WriteLine(line);
            }
        }

        private int GenerateNextMemberId(string[] lines)
        {
            int max = 1000;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length > 0 && int.TryParse(parts[0].Trim(), out int id) && id > max)
                    max = id;
            }

            return max + 1;
        }

        private string GenerateNewReceiptNumber()
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            string prefix = $"ORD-{today}-";
            string path = Path.Combine(Application.StartupPath, "susi_sales_history.csv");
            int max = 0;

            if (File.Exists(path))
            {
                foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 1)
                        continue;

                    string receipt = parts[0].Trim();

                    if (!receipt.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        continue;

                    string sequence = receipt.Substring(prefix.Length);

                    if (int.TryParse(sequence, out int number) && number > max)
                        max = number;
                }
            }

            return $"{prefix}{max + 1:D3}";
        }

        private void SaveRejectedOrder(string identifier, string orderType)
        {
            string path = Path.Combine(Application.StartupPath, "susi_order_rejections.csv");
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            AppendCsvLinesSafe(path, new[] { $"{identifier},{date},앱,{orderType}" });
        }

        private void SaveAppPayment(string identifier, int memberId, int usedPoint, int earnedPoint, string paymentMethod)
        {
            string path = Path.Combine(Application.StartupPath, "susi_order_payments.csv");
            AppendCsvLinesSafe(path, new[] { $"{identifier},{memberId},{usedPoint},{earnedPoint},{paymentMethod}" });
        }

        private bool GetAppPayment(string identifier, out int memberId, out int usedPoint, out int earnedPoint, out string paymentMethod)
        {
            memberId = 0;
            usedPoint = 0;
            earnedPoint = 0;
            paymentMethod = "앱선결제";

            string path = Path.Combine(Application.StartupPath, "susi_order_payments.csv");

            if (!File.Exists(path))
                return false;

            foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
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
            RemoveCsvRows("susi_order_payments.csv", identifier);
        }

        private void RemoveOrderItems(string identifier)
        {
            RemoveCsvRows("susi_order_items.csv", identifier);
        }

        private void RemoveRealtimeOrder(string identifier)
        {
            RemoveCsvRows("susi_orders_realtime.csv", identifier);
        }

        private void RemoveCsvRows(string fileName, string identifier)
        {
            string path = Path.Combine(Application.StartupPath, fileName);

            if (!File.Exists(path))
                return;

            List<string> lines = File.ReadAllLines(path, Encoding.UTF8).ToList();

            lines.RemoveAll(line =>
            {
                string[] parts = line.Split(',');
                return parts.Length > 0 && parts[0].Trim().Equals(identifier, StringComparison.OrdinalIgnoreCase);
            });

            File.WriteAllLines(path, lines, new UTF8Encoding(false));
        }

        private void UpdateTableItemKeyIds(string tablePrefix, string newReceiptNo)
        {
            string path = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (!File.Exists(path))
                return;

            List<string> lines = File.ReadAllLines(path, Encoding.UTF8).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split(',');

                if (parts.Length < 1)
                    continue;

                if (parts[0].Trim().StartsWith(tablePrefix, StringComparison.OrdinalIgnoreCase))
                {
                    parts[0] = newReceiptNo;
                    lines[i] = string.Join(",", parts);
                }
            }

            File.WriteAllLines(path, lines, new UTF8Encoding(false));
        }

        private void UpdateItemKeyId(string oldKeyId, string newReceiptNo)
        {
            string path = Path.Combine(Application.StartupPath, "susi_order_items.csv");

            if (!File.Exists(path))
                return;

            List<string> lines = File.ReadAllLines(path, Encoding.UTF8).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split(',');

                if (parts.Length < 1)
                    continue;

                if (parts[0].Trim().Equals(oldKeyId, StringComparison.OrdinalIgnoreCase))
                {
                    parts[0] = newReceiptNo;
                    lines[i] = string.Join(",", parts);
                }
            }

            File.WriteAllLines(path, lines, new UTF8Encoding(false));
        }

        // =========================================================
        // 파일 백업 / 복구
        // =========================================================

        private FileSnapshot CreateSnapshot(string path)
        {
            return new FileSnapshot
            {
                Exists = File.Exists(path),
                Data = File.Exists(path) ? File.ReadAllBytes(path) : null
            };
        }

        private void RestoreSnapshot(string path, FileSnapshot snapshot)
        {
            if (snapshot == null)
                return;

            if (snapshot.Exists)
                File.WriteAllBytes(path, snapshot.Data ?? new byte[0]);
            else if (File.Exists(path))
                File.Delete(path);
        }

        // =========================================================
        // UserControl에서 호출
        // =========================================================

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

        // =========================================================
        // 신규 주문 알림
        // =========================================================

        public void UpdateOrderNotice()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateOrderNotice));
                return;
            }

            string path = Path.Combine(Application.StartupPath, "susi_orders_realtime.csv");
            int waitingCount = 0;

            if (File.Exists(path))
            {
                foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 6)
                        continue;

                    if (parts[1].Trim() == "앱" && parts[5].Trim() == "접수 대기")
                        waitingCount++;
                }
            }

            UpdateNotice(waitingCount);
        }

        public void UpdateNotice(int waitingCount)
        {
            lblNotice.Text = $"신규 주문 [{waitingCount}건] 대기 중";

            if (waitingCount > 0)
            {
                if (!noticeBlinkTimer.Enabled)
                {
                    noticeBlinkState = false;
                    lblNotice.ForeColor = Color.Yellow;
                    noticeBlinkTimer.Start();
                }
            }
            else
            {
                noticeBlinkTimer.Stop();
                noticeBlinkState = false;
                lblNotice.ForeColor = Color.Yellow;
            }
        }

        private void RefreshOrderBoard()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(RefreshOrderBoard));
                return;
            }

            foreach (Control control in pnlMainContainer.Controls)
            {
                if (control is UcOrderBoard orderBoard)
                {
                    orderBoard.RefreshOrders();
                    break;
                }
            }
        }

        private void noticeBlinkTimer_Tick(object sender, EventArgs e)
        {
            noticeBlinkState = !noticeBlinkState;

            if (noticeBlinkState)
                lblNotice.ForeColor = Color.Red;
            else
                lblNotice.ForeColor = Color.Yellow;
        }

        // =========================================================
        // JSON 공통 응답
        // =========================================================

        private string Fail(string message)
        {
            return new JObject
            {
                ["Status"] = "FAIL",
                ["Message"] = message
            }.ToString(Newtonsoft.Json.Formatting.None);
        }

        private string Success(string message)
        {
            return new JObject
            {
                ["Status"] = "SUCCESS",
                ["Message"] = message
            }.ToString(Newtonsoft.Json.Formatting.None);
        }

        // =========================================================
        // Form
        // =========================================================

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