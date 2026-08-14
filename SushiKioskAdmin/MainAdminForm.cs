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

        private const int SERVER_PORT = 9000;
        private const int MAX_MESSAGE_SIZE = 1024 * 1024;

        private readonly object memberFileLock = new object();
        private readonly object orderFileLock = new object();

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
                btnNavOrder,
                btnNavTable,
                btnNavMenu,
                btnNavHistory,
                btnNavUser,
                btnNavStock,
                btnNavReport
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
                btn.FlatAppearance.MouseOverBackColor =
                    Color.FromArgb(60, 60, 65);
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

                    System.Diagnostics.Debug.WriteLine(
                        $"[소켓 서버] 포트 {SERVER_PORT}에서 서버 시작");

                    while (isServerRunning)
                    {
                        TcpClient client = server.AcceptTcpClient();

                        Task.Run(() =>
                            HandleClientCommunication(client));
                    }
                }
                catch (SocketException ex)
                {
                    if (isServerRunning)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[소켓 서버 오류] {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[소켓 서버 오류] {ex.Message}");
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

                    if (string.IsNullOrWhiteSpace(jsonMessage))
                        return;

                    JObject packet = JObject.Parse(jsonMessage);

                    string action =
                        packet["Action"]?.ToString()?.Trim();

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
                            responseJson =
                                "{\"Status\":\"FAIL\",\"Message\":\"Unknown action.\"}";
                            break;
                    }

                    byte[] responseBytes =
                        Encoding.UTF8.GetBytes(responseJson);

                    stream.Write(
                        responseBytes,
                        0,
                        responseBytes.Length);

                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[통신 오류] {ex.Message}");
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
                    int bytesRead =
                        stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead <= 0)
                        break;

                    ms.Write(buffer, 0, bytesRead);

                    string json =
                        Encoding.UTF8.GetString(ms.ToArray());

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
                {
                    throw new InvalidOperationException(
                        "JSON message is too large.");
                }

                if (ms.Length == 0)
                    return null;

                string finalJson =
                    Encoding.UTF8.GetString(ms.ToArray());

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
                string identifier =
                    packet["Identifier"]?.ToString()?.Trim();

                string source =
                    packet["Source"]?.ToString()?.Trim();

                string orderType =
                    packet["OrderType"]?.ToString()?.Trim();

                string orderTime =
                    packet["OrderTime"]?.ToString()?.Trim();

                decimal totalAmount =
                    packet["TotalAmount"]?.Value<decimal>() ?? 0;

                string status =
                    packet["Status"]?.ToString()?.Trim()
                    ?? "접수 대기";

                if (string.IsNullOrWhiteSpace(identifier))
                    return Fail("Identifier is required.");

                if (string.IsNullOrWhiteSpace(source))
                    return Fail("Source is required.");

                if (string.IsNullOrWhiteSpace(orderType))
                    return Fail("OrderType is required.");

                if (totalAmount < 0)
                    return Fail("Invalid order amount.");

                // 키오스크 포장 주문 번호 검사
                if (source == "키오스크" &&
                    orderType == "포장" &&
                    !IsValidKioskTakeoutIdentifier(identifier))
                {
                    return Fail(
                        "Kiosk takeout Identifier must use K-yyyyMMdd-nnn format.");
                }

                int memberId = 0;
                int usedPoint = 0;
                int earnedPoint = 0;
                string paymentMethod = "앱선결제";

                // -------------------------------------------------
                // 앱 주문
                // -------------------------------------------------
                if (source == "앱")
                {
                    memberId =
                        packet["MemberId"]?.Value<int>() ?? 0;

                    usedPoint =
                        packet["UsedPoint"]?.Value<int>() ?? 0;

                    paymentMethod =
                        packet["PaymentMethod"]?.ToString()?.Trim()
                        ?? "앱선결제";

                    if (usedPoint < 0 ||
                        usedPoint > totalAmount)
                    {
                        return Fail("Invalid point usage.");
                    }

                    if (memberId == 0 && usedPoint > 0)
                    {
                        return Fail(
                            "Non-members cannot use points.");
                    }

                    // [수정 3] 존재하지 않는 회원 차단
                    if (memberId > 0)
                    {
                        if (!MemberExists(memberId))
                            return Fail("Member not found.");

                        int availablePoint =
                            GetAvailableMemberPoint(memberId);

                        // [수정 4]
                        // 다른 앱 주문에서 이미 예약된 포인트 고려
                        if (usedPoint > availablePoint)
                        {
                            return Fail(
                                "Not enough available points.");
                        }
                    }

                    decimal paidAmount =
                        totalAmount - usedPoint;

                    earnedPoint =
                        memberId > 0
                        ? (int)(paidAmount * 0.01m)
                        : 0;
                }

                // Items 검증
                JArray items = packet["Items"] as JArray;

                if (items == null || items.Count == 0)
                    return Fail("Order items are required.");

                decimal calculatedTotal = 0;

                foreach (JToken item in items)
                {
                    decimal price =
                        item["Price"]?.Value<decimal>() ?? 0;

                    int quantity =
                        item["Quantity"]?.Value<int>() ?? 0;

                    int discountQty =
                        item["DiscountQty"]?.Value<int>() ?? 0;

                    decimal subTotal =
                        item["SubTotal"]?.Value<decimal>() ?? 0;

                    if (price < 0 ||
                        quantity <= 0 ||
                        discountQty < 0 ||
                        discountQty > quantity)
                    {
                        return Fail("Invalid order item.");
                    }

                    decimal expectedSubTotal =
                        (quantity - discountQty) * price;

                    if (subTotal != expectedSubTotal)
                    {
                        return Fail(
                            "Invalid item subtotal.");
                    }

                    calculatedTotal += subTotal;
                }

                if (calculatedTotal != totalAmount)
                {
                    return Fail(
                        "Order total does not match item total.");
                }

                lock (orderFileLock)
                {
                    if (OrderIdentifierExists(identifier))
                    {
                        return new JObject
                        {
                            ["Status"] = "FAIL",
                            ["Identifier"] = identifier,
                            ["Message"] =
                                "Duplicate order identifier."
                        }.ToString(
                            Newtonsoft.Json.Formatting.None);
                    }

                    string realtimePath = Path.Combine(
                        Application.StartupPath,
                        "susi_orders_realtime.csv");

                    string itemsPath = Path.Combine(
                        Application.StartupPath,
                        "susi_order_items.csv");

                    string realtimeLine =
                        $"{identifier},{source},{orderType}," +
                        $"{orderTime},{totalAmount},{status}";

                    File.AppendAllLines(
                        realtimePath,
                        new[] { realtimeLine },
                        new UTF8Encoding(false));

                    List<string> itemLines =
                        new List<string>();

                    foreach (JToken item in items)
                    {
                        string menuName =
                            item["MenuName"]?.ToString();

                        decimal price =
                            item["Price"]?.Value<decimal>() ?? 0;

                        int quantity =
                            item["Quantity"]?.Value<int>() ?? 0;

                        int discountQty =
                            item["DiscountQty"]?.Value<int>() ?? 0;

                        decimal subTotal =
                            item["SubTotal"]?.Value<decimal>() ?? 0;

                        itemLines.Add(
                            $"{identifier},{menuName},{price}," +
                            $"{quantity},{discountQty},{subTotal}");
                    }

                    File.AppendAllLines(
                        itemsPath,
                        itemLines,
                        new UTF8Encoding(false));

                    if (source == "앱")
                    {
                        SaveAppPayment(
                            identifier,
                            memberId,
                            usedPoint,
                            earnedPoint,
                            paymentMethod);
                    }
                }

                UpdateOrderNotice();

                return Success(
                    "Order registered successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[주문 접수 오류] {ex.Message}");

                return Fail(
                    "Order registration failed.");
            }
        }

        // =========================================================
        // 키오스크 결제
        // =========================================================

        private string ProcessPaymentComplete(JObject packet)
        {
            try
            {
                string identifier =
                    packet["Identifier"]?.ToString()?.Trim();

                string paymentMethod =
                    packet["PaymentMethod"]?.ToString()?.Trim()
                    ?? "신용카드";

                int memberId =
                    packet["MemberId"]?.Value<int>() ?? 0;

                int usedPoint =
                    packet["UsedPoint"]?.Value<int>() ?? 0;

                decimal originalAmount =
                    packet["OriginalAmount"]?.Value<decimal>() ?? 0;

                decimal totalAmount =
                    packet["TotalAmount"]?.Value<decimal>() ?? 0;

                if (string.IsNullOrWhiteSpace(identifier))
                    return Fail("Identifier is required.");

                if (originalAmount < 0 ||
                    totalAmount < 0 ||
                    usedPoint < 0)
                {
                    return Fail("Invalid payment amount.");
                }

                if (originalAmount - usedPoint != totalAmount)
                {
                    return Fail(
                        "Payment amount does not match point usage.");
                }

                int earnedPoint = 0;

                // [수정 3] 회원 존재 여부 확인
                if (memberId > 0)
                {
                    if (!MemberExists(memberId))
                        return Fail("Member not found.");

                    int currentPoint =
                        GetMemberPoint(memberId);

                    if (usedPoint > currentPoint)
                        return Fail("Not enough points.");

                    earnedPoint =
                        (int)(totalAmount * 0.01m);
                }
                else if (usedPoint > 0)
                {
                    return Fail(
                        "Non-members cannot use points.");
                }

                string tablePrefix =
                    GetTablePrefix(identifier);

                string orderType;
                decimal serverOrderAmount;

                // -------------------------------------------------
                // 매장 주문
                // -------------------------------------------------
                if (tablePrefix != null)
                {
                    orderType = "매장";

                    // [수정 1]
                    // 실제 T02-* 주문이 존재하는지 확인
                    if (!TryGetTableOrderTotal(
                        tablePrefix,
                        out serverOrderAmount))
                    {
                        return Fail(
                            "Realtime order not found.");
                    }

                    // [수정 2]
                    // 서버에 저장된 주문금액과 비교
                    if (serverOrderAmount != originalAmount)
                    {
                        return new JObject
                        {
                            ["Status"] = "FAIL",
                            ["ServerAmount"] =
                                serverOrderAmount,
                            ["ReceivedAmount"] =
                                originalAmount,
                            ["Message"] =
                                "OriginalAmount does not match server order total."
                        }.ToString(
                            Newtonsoft.Json.Formatting.None);
                    }
                }
                // -------------------------------------------------
                // 키오스크 포장
                // -------------------------------------------------
                else
                {
                    if (!GetRealtimeOrderInfo(
                        identifier,
                        out orderType,
                        out serverOrderAmount))
                    {
                        return Fail(
                            "Realtime order not found.");
                    }

                    if (orderType != "포장")
                    {
                        return Fail(
                            "Invalid kiosk payment identifier.");
                    }

                    if (!IsValidKioskTakeoutIdentifier(identifier))
                    {
                        return Fail(
                            "Invalid kiosk takeout identifier.");
                    }

                    // [수정 2]
                    if (serverOrderAmount != originalAmount)
                    {
                        return new JObject
                        {
                            ["Status"] = "FAIL",
                            ["ServerAmount"] =
                                serverOrderAmount,
                            ["ReceivedAmount"] =
                                originalAmount,
                            ["Message"] =
                                "OriginalAmount does not match server order total."
                        }.ToString(
                            Newtonsoft.Json.Formatting.None);
                    }
                }

                string paymentDate =
                    DateTime.Now.ToString(
                        "yyyy-MM-dd HH:mm:ss");

                string newReceiptNo;

                lock (orderFileLock)
                {
                    // 다시 한번 주문 존재 여부 확인
                    // 동시 결제 요청 방지
                    if (tablePrefix != null)
                    {
                        if (!TryGetTableOrderTotal(
                            tablePrefix,
                            out decimal checkAmount))
                        {
                            return Fail(
                                "Order already processed.");
                        }

                        if (checkAmount != originalAmount)
                        {
                            return Fail(
                                "Order amount changed.");
                        }
                    }
                    else
                    {
                        if (!GetRealtimeOrderInfo(
                            identifier,
                            out _,
                            out decimal checkAmount))
                        {
                            return Fail(
                                "Order already processed.");
                        }

                        if (checkAmount != originalAmount)
                        {
                            return Fail(
                                "Order amount changed.");
                        }
                    }

                    newReceiptNo =
                        GenerateNewReceiptNumber();

                    string salesPath = Path.Combine(
                        Application.StartupPath,
                        "susi_sales_history.csv");

                    string salesLine =
                        $"{newReceiptNo},{paymentDate}," +
                        $"키오스크,{orderType}," +
                        $"{originalAmount},{usedPoint}," +
                        $"{totalAmount},{earnedPoint}," +
                        $"{memberId},{paymentMethod}";

                    File.AppendAllLines(
                        salesPath,
                        new[] { salesLine },
                        new UTF8Encoding(false));

                    // 먼저 realtime 제거
                    string realtimePath = Path.Combine(
                        Application.StartupPath,
                        "susi_orders_realtime.csv");

                    if (File.Exists(realtimePath))
                    {
                        List<string> lines =
                            File.ReadAllLines(
                                realtimePath,
                                Encoding.UTF8).ToList();

                        if (tablePrefix != null)
                        {
                            lines.RemoveAll(line =>
                            {
                                string[] parts =
                                    line.Split(',');

                                return parts.Length > 0 &&
                                    parts[0].Trim().StartsWith(
                                        tablePrefix,
                                        StringComparison.OrdinalIgnoreCase);
                            });
                        }
                        else
                        {
                            lines.RemoveAll(line =>
                            {
                                string[] parts =
                                    line.Split(',');

                                return parts.Length > 0 &&
                                    parts[0].Trim().Equals(
                                        identifier,
                                        StringComparison.OrdinalIgnoreCase);
                            });
                        }

                        File.WriteAllLines(
                            realtimePath,
                            lines,
                            new UTF8Encoding(false));
                    }

                    if (tablePrefix != null)
                    {
                        UpdateTableItemKeyIds(
                            tablePrefix,
                            newReceiptNo);
                    }
                    else
                    {
                        UpdateItemKeyId(
                            identifier,
                            newReceiptNo);
                    }
                }

                // 회원 포인트 처리
                if (memberId > 0)
                {
                    bool pointUpdated =
                        UpdateMemberPoint(
                            memberId,
                            usedPoint,
                            earnedPoint);

                    if (!pointUpdated)
                    {
                        return Fail(
                            "Failed to update member points.");
                    }
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
                    ["Message"] =
                        "Payment processed successfully."
                }.ToString(
                    Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[결제 처리 오류] {ex.Message}");

                return Fail(
                    "Payment processing failed.");
            }
        }

        // =========================================================
        // 앱 픽업 완료
        // =========================================================

        private string ProcessAppPickupComplete(JObject packet)
        {
            try
            {
                string identifier =
                    packet["Identifier"]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(identifier))
                    return Fail("Identifier is required.");

                if (!GetRealtimeOrderInfo(
                    identifier,
                    out string orderType,
                    out decimal originalAmount))
                {
                    return Fail(
                        "Realtime order not found.");
                }

                if (!GetAppPayment(
                    identifier,
                    out int memberId,
                    out int usedPoint,
                    out int earnedPoint,
                    out string paymentMethod))
                {
                    return Fail(
                        "Payment information not found.");
                }

                decimal totalAmount =
                    originalAmount - usedPoint;

                if (totalAmount < 0)
                    return Fail("Invalid payment amount.");

                if (memberId > 0)
                {
                    if (!MemberExists(memberId))
                        return Fail("Member not found.");

                    /*
                     * 중요:
                     * 이 주문의 UsedPoint는 이미
                     * susi_order_payments.csv에 예약되어 있음.
                     *
                     * 픽업 완료 시에는 실제 member.csv에서
                     * 차감한다.
                     */
                    int currentPoint =
                        GetMemberPoint(memberId);

                    if (usedPoint > currentPoint)
                        return Fail("Not enough points.");
                }

                string paymentDate =
                    DateTime.Now.ToString(
                        "yyyy-MM-dd HH:mm:ss");

                // 앱 주문번호를 ReceiptNo로 사용
                string receiptNo = identifier;

                lock (orderFileLock)
                {
                    // 중복 완료 방지
                    if (SalesIdentifierExists(receiptNo))
                        return Fail("Order already completed.");

                    string salesPath = Path.Combine(
                        Application.StartupPath,
                        "susi_sales_history.csv");

                    string salesLine =
                        $"{receiptNo},{paymentDate},앱," +
                        $"{orderType},{originalAmount}," +
                        $"{usedPoint},{totalAmount}," +
                        $"{earnedPoint},{memberId}," +
                        $"{paymentMethod}";

                    File.AppendAllLines(
                        salesPath,
                        new[] { salesLine },
                        new UTF8Encoding(false));
                }

                if (memberId > 0)
                {
                    bool pointUpdated =
                        UpdateMemberPoint(
                            memberId,
                            usedPoint,
                            earnedPoint);

                    if (!pointUpdated)
                    {
                        RemoveSalesHistory(receiptNo);

                        return Fail(
                            "Failed to update member points.");
                    }
                }

                lock (orderFileLock)
                {
                    RemoveRealtimeOrder(identifier);
                    RemoveAppPayment(identifier);

                    UpdateItemKeyId(
                        identifier,
                        receiptNo);
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
                    ["Message"] =
                        "App order pickup completed."
                }.ToString(
                    Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[앱 주문 완료 오류] {ex.Message}");

                return Fail(
                    "App pickup processing failed.");
            }
        }

        // =========================================================
        // 앱 주문 거절
        // =========================================================

        private string ProcessAppReject(JObject packet)
        {
            try
            {
                string identifier =
                    packet["Identifier"]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(identifier))
                    return Fail("Identifier is required.");

                if (!GetRealtimeOrderInfo(
                    identifier,
                    out string orderType,
                    out decimal originalAmount))
                {
                    return Fail(
                        "Realtime order not found.");
                }

                int memberId = 0;
                int usedPoint = 0;
                int earnedPoint = 0;
                string paymentMethod = "앱선결제";

                GetAppPayment(
                    identifier,
                    out memberId,
                    out usedPoint,
                    out earnedPoint,
                    out paymentMethod);

                decimal refundAmount =
                    originalAmount - usedPoint;

                if (refundAmount < 0)
                    refundAmount = 0;

                lock (orderFileLock)
                {
                    SaveRejectedOrder(
                        identifier,
                        orderType);

                    RemoveRealtimeOrder(identifier);
                    RemoveAppPayment(identifier);
                    RemoveOrderItems(identifier);
                }

                /*
                 * 예약 방식이므로
                 * member.csv 포인트를 복구할 필요 없음.
                 *
                 * RemoveAppPayment()를 하면
                 * 해당 주문의 예약 포인트가 사라져서
                 * 다시 사용 가능한 포인트가 됨.
                 */

                UpdateOrderNotice();

                return new JObject
                {
                    ["Status"] = "SUCCESS",
                    ["Identifier"] = identifier,
                    ["OrderStatus"] = "주문 거절",
                    ["RefundRequired"] = true,
                    ["RefundAmount"] = refundAmount,
                    ["PaymentMethod"] = paymentMethod,
                    ["Message"] =
                        "App order rejected. Prepayment cancellation is required."
                }.ToString(
                    Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[앱 주문 거절 오류] {ex.Message}");

                return Fail(
                    "App order rejection failed.");
            }
        }

        // =========================================================
        // 주문 상태 조회
        // =========================================================

        private string ProcessGetOrderStatus(JObject packet)
        {
            try
            {
                string identifier =
                    packet["Identifier"]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(identifier))
                    return Fail("Identifier is required.");

                string realtimePath = Path.Combine(
                    Application.StartupPath,
                    "susi_orders_realtime.csv");

                if (File.Exists(realtimePath))
                {
                    foreach (string line in
                        File.ReadAllLines(
                            realtimePath,
                            Encoding.UTF8))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length < 6)
                            continue;

                        if (parts[0].Trim().Equals(
                            identifier,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return new JObject
                            {
                                ["Status"] = "SUCCESS",
                                ["Identifier"] = identifier,
                                ["OrderStatus"] =
                                    parts[5].Trim()
                            }.ToString(
                                Newtonsoft.Json.Formatting.None);
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
                    }.ToString(
                        Newtonsoft.Json.Formatting.None);
                }

                string rejectionPath = Path.Combine(
                    Application.StartupPath,
                    "susi_order_rejections.csv");

                if (File.Exists(rejectionPath))
                {
                    foreach (string line in
                        File.ReadAllLines(
                            rejectionPath,
                            Encoding.UTF8))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length < 4)
                            continue;

                        if (parts[0].Trim().Equals(
                            identifier,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return new JObject
                            {
                                ["Status"] = "SUCCESS",
                                ["Identifier"] = identifier,
                                ["OrderStatus"] = "주문 거절",
                                ["RefundRequired"] = true,
                                ["Message"] =
                                    "Order rejected. Check prepayment cancellation."
                            }.ToString(
                                Newtonsoft.Json.Formatting.None);
                        }
                    }
                }

                return new JObject
                {
                    ["Status"] = "FAIL",
                    ["Identifier"] = identifier,
                    ["Message"] = "Order not found."
                }.ToString(
                    Newtonsoft.Json.Formatting.None);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[주문 상태 조회 오류] {ex.Message}");

                return Fail(
                    "Failed to get order status.");
            }
        }

        // =========================================================
        // 회원가입
        // =========================================================

        private string ProcessRegisterMember(JObject packet)
        {
            try
            {
                string memberName =
                    packet["MemberName"]?.ToString()?.Trim();

                string phone =
                    packet["Phone"]?.ToString()?.Trim();

                string password =
                    packet["Password"]?.ToString()?.Trim();

                string address =
                    packet["Address"]?.ToString()?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(memberName))
                    return Fail("MemberName is required.");

                if (string.IsNullOrWhiteSpace(phone))
                    return Fail("Phone is required.");

                if (string.IsNullOrWhiteSpace(password))
                    return Fail("Password is required.");

                if (memberName.Contains(",") ||
                    phone.Contains(",") ||
                    password.Contains(",") ||
                    address.Contains(","))
                {
                    return Fail(
                        "Comma cannot be used in member information.");
                }

                lock (memberFileLock)
                {
                    string memberPath = Path.Combine(
                        Application.StartupPath,
                        "member.csv");

                    if (!File.Exists(memberPath))
                    {
                        File.WriteAllText(
                            memberPath,
                            "",
                            new UTF8Encoding(false));
                    }

                    string[] lines =
                        File.ReadAllLines(
                            memberPath,
                            Encoding.UTF8);

                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length < 7)
                            continue;

                        if (parts[2].Trim().Equals(
                            phone,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return Fail(
                                "Phone number already registered.");
                        }
                    }

                    int memberId =
                        GenerateNextMemberId(lines);

                    int point = 0;

                    string joinDate =
                        DateTime.Now.ToString("yyyy-MM-dd");

                    string memberLine =
                        $"{memberId},{memberName}," +
                        $"{phone},{password},{point}," +
                        $"{address},{joinDate}";

                    File.AppendAllLines(
                        memberPath,
                        new[] { memberLine },
                        new UTF8Encoding(false));

                    return new JObject
                    {
                        ["Status"] = "SUCCESS",
                        ["MemberId"] = memberId,
                        ["Point"] = point,
                        ["JoinDate"] = joinDate,
                        ["Message"] =
                            "Member registered successfully."
                    }.ToString(
                        Newtonsoft.Json.Formatting.None);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[회원가입 오류] {ex.Message}");

                return Fail(
                    "Member registration failed.");
            }
        }

        // =========================================================
        // 로그인
        // =========================================================

        private string ProcessLoginMember(JObject packet)
        {
            try
            {
                string phone =
                    packet["Phone"]?.ToString()?.Trim();

                string password =
                    packet["Password"]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(phone))
                    return Fail("Phone is required.");

                if (string.IsNullOrWhiteSpace(password))
                    return Fail("Password is required.");

                lock (memberFileLock)
                {
                    string memberPath = Path.Combine(
                        Application.StartupPath,
                        "member.csv");

                    if (!File.Exists(memberPath))
                        return Fail("Member data not found.");

                    foreach (string line in
                        File.ReadAllLines(
                            memberPath,
                            Encoding.UTF8))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length < 7)
                            continue;

                        string savedPhone =
                            parts[2].Trim();

                        if (!savedPhone.Equals(
                            phone,
                            StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (parts[3].Trim() != password)
                        {
                            return Fail(
                                "Invalid phone or password.");
                        }

                        int.TryParse(
                            parts[0].Trim(),
                            out int memberId);

                        int.TryParse(
                            parts[4].Trim(),
                            out int point);

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
                        }.ToString(
                            Newtonsoft.Json.Formatting.None);
                    }
                }

                return Fail(
                    "Invalid phone or password.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[로그인 오류] {ex.Message}");

                return Fail("Login failed.");
            }
        }

        // =========================================================
        // 키오스크 회원 조회
        // =========================================================

        private string ProcessGetMember(JObject packet)
        {
            try
            {
                string phone =
                    packet["Phone"]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(phone))
                    return Fail("Phone is required.");

                lock (memberFileLock)
                {
                    string memberPath = Path.Combine(
                        Application.StartupPath,
                        "member.csv");

                    if (!File.Exists(memberPath))
                        return Fail("Member data not found.");

                    foreach (string line in
                        File.ReadAllLines(
                            memberPath,
                            Encoding.UTF8))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        string[] parts = line.Split(',');

                        if (parts.Length < 7)
                            continue;

                        if (!parts[2].Trim().Equals(
                            phone,
                            StringComparison.OrdinalIgnoreCase))
                            continue;

                        int.TryParse(
                            parts[0].Trim(),
                            out int memberId);

                        int.TryParse(
                            parts[4].Trim(),
                            out int point);

                        return new JObject
                        {
                            ["Status"] = "SUCCESS",
                            ["MemberId"] = memberId,
                            ["MemberName"] = parts[1].Trim(),
                            ["Phone"] = parts[2].Trim(),
                            ["Point"] = point,
                            ["Message"] = "Member found."
                        }.ToString(
                            Newtonsoft.Json.Formatting.None);
                    }
                }

                return Fail("Member not found.");
            }
            catch
            {
                return Fail(
                    "Failed to get member information.");
            }
        }

        // =========================================================
        // 메뉴 조회
        // =========================================================

        private string ProcessGetMenu()
        {
            try
            {
                string menuPath = Path.Combine(
                    Application.StartupPath,
                    "susi_menu.csv");

                if (!File.Exists(menuPath))
                    return Fail("Menu data not found.");

                JArray menus = new JArray();

                foreach (string line in
                    File.ReadAllLines(
                        menuPath,
                        Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 7)
                        continue;

                    if (!int.TryParse(
                        parts[0].Trim(),
                        out int menuId))
                        continue;

                    int.TryParse(
                        parts[4].Trim(),
                        out int price);

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
                }.ToString(
                    Newtonsoft.Json.Formatting.None);
            }
            catch
            {
                return Fail(
                    "Failed to get menu information.");
            }
        }

        // =========================================================
        // [추가] 회원 존재 여부
        // =========================================================

        private bool MemberExists(int memberId)
        {
            lock (memberFileLock)
            {
                string path = Path.Combine(
                    Application.StartupPath,
                    "member.csv");

                if (!File.Exists(path))
                    return false;

                foreach (string line in
                    File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 7)
                        continue;

                    if (int.TryParse(
                        parts[0].Trim(),
                        out int id) &&
                        id == memberId)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        // =========================================================
        // [추가] 앱 예약 포인트 계산
        // =========================================================

        private int GetReservedPoint(int memberId)
        {
            string path = Path.Combine(
                Application.StartupPath,
                "susi_order_payments.csv");

            if (!File.Exists(path))
                return 0;

            int reserved = 0;

            foreach (string line in
                File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 5)
                    continue;

                if (!int.TryParse(
                    parts[1].Trim(),
                    out int savedMemberId))
                    continue;

                if (savedMemberId != memberId)
                    continue;

                if (int.TryParse(
                    parts[2].Trim(),
                    out int usedPoint))
                {
                    reserved += usedPoint;
                }
            }

            return reserved;
        }

        private int GetAvailableMemberPoint(int memberId)
        {
            int currentPoint =
                GetMemberPoint(memberId);

            int reservedPoint =
                GetReservedPoint(memberId);

            int available =
                currentPoint - reservedPoint;

            return Math.Max(0, available);
        }

        // =========================================================
        // [추가] 테이블 주문 총액
        // =========================================================

        private bool TryGetTableOrderTotal(
            string tablePrefix,
            out decimal total)
        {
            total = 0;

            string path = Path.Combine(
                Application.StartupPath,
                "susi_orders_realtime.csv");

            if (!File.Exists(path))
                return false;

            bool found = false;

            foreach (string line in
                File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 6)
                    continue;

                if (!parts[0].Trim().StartsWith(
                    tablePrefix,
                    StringComparison.OrdinalIgnoreCase))
                    continue;

                if (parts[1].Trim() != "키오스크" ||
                    parts[2].Trim() != "매장")
                    continue;

                if (decimal.TryParse(
                    parts[4].Trim(),
                    out decimal amount))
                {
                    total += amount;
                    found = true;
                }
            }

            return found;
        }

        // =========================================================
        // Identifier 검사
        // =========================================================

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
                string path = Path.Combine(
                    Application.StartupPath,
                    file);

                if (!File.Exists(path))
                    continue;

                foreach (string line in
                    File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length > 0 &&
                        parts[0].Trim().Equals(
                            identifier,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool SalesIdentifierExists(string identifier)
        {
            string path = Path.Combine(
                Application.StartupPath,
                "susi_sales_history.csv");

            if (!File.Exists(path))
                return false;

            foreach (string line in
                File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length > 0 &&
                    parts[0].Trim().Equals(
                        identifier,
                        StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private bool IsValidKioskTakeoutIdentifier(
            string identifier)
        {
            return Regex.IsMatch(
                identifier ?? "",
                @"^K-\d{8}-\d{3,}$",
                RegexOptions.IgnoreCase);
        }

        // =========================================================
        // CSV Helper
        // =========================================================

        private int GenerateNextMemberId(string[] lines)
        {
            int max = 1000;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length > 0 &&
                    int.TryParse(
                        parts[0].Trim(),
                        out int id) &&
                    id > max)
                {
                    max = id;
                }
            }

            return max + 1;
        }

        private void SaveRejectedOrder(
            string identifier,
            string orderType)
        {
            string path = Path.Combine(
                Application.StartupPath,
                "susi_order_rejections.csv");

            string date =
                DateTime.Now.ToString(
                    "yyyy-MM-dd HH:mm:ss");

            File.AppendAllLines(
                path,
                new[]
                {
                    $"{identifier},{date},앱,{orderType}"
                },
                new UTF8Encoding(false));
        }

        private void SaveAppPayment(
            string identifier,
            int memberId,
            int usedPoint,
            int earnedPoint,
            string paymentMethod)
        {
            string path = Path.Combine(
                Application.StartupPath,
                "susi_order_payments.csv");

            File.AppendAllLines(
                path,
                new[]
                {
                    $"{identifier},{memberId},{usedPoint}," +
                    $"{earnedPoint},{paymentMethod}"
                },
                new UTF8Encoding(false));
        }

        private bool GetAppPayment(
            string identifier,
            out int memberId,
            out int usedPoint,
            out int earnedPoint,
            out string paymentMethod)
        {
            memberId = 0;
            usedPoint = 0;
            earnedPoint = 0;
            paymentMethod = "앱선결제";

            string path = Path.Combine(
                Application.StartupPath,
                "susi_order_payments.csv");

            if (!File.Exists(path))
                return false;

            foreach (string line in
                File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 5)
                    continue;

                if (!parts[0].Trim().Equals(
                    identifier,
                    StringComparison.OrdinalIgnoreCase))
                    continue;

                int.TryParse(parts[1], out memberId);
                int.TryParse(parts[2], out usedPoint);
                int.TryParse(parts[3], out earnedPoint);

                paymentMethod = parts[4].Trim();

                return true;
            }

            return false;
        }

        private void RemoveAppPayment(string identifier)
        {
            RemoveCsvRows(
                "susi_order_payments.csv",
                identifier);
        }

        private void RemoveOrderItems(string identifier)
        {
            RemoveCsvRows(
                "susi_order_items.csv",
                identifier);
        }

        private void RemoveRealtimeOrder(string identifier)
        {
            RemoveCsvRows(
                "susi_orders_realtime.csv",
                identifier);
        }

        private void RemoveCsvRows(
            string fileName,
            string identifier)
        {
            string path = Path.Combine(
                Application.StartupPath,
                fileName);

            if (!File.Exists(path))
                return;

            List<string> lines =
                File.ReadAllLines(
                    path,
                    Encoding.UTF8).ToList();

            lines.RemoveAll(line =>
            {
                string[] parts = line.Split(',');

                return parts.Length > 0 &&
                    parts[0].Trim().Equals(
                        identifier,
                        StringComparison.OrdinalIgnoreCase);
            });

            File.WriteAllLines(
                path,
                lines,
                new UTF8Encoding(false));
        }

        private bool GetRealtimeOrderInfo(
            string identifier,
            out string orderType,
            out decimal totalAmount)
        {
            orderType = "";
            totalAmount = 0;

            string path = Path.Combine(
                Application.StartupPath,
                "susi_orders_realtime.csv");

            if (!File.Exists(path))
                return false;

            foreach (string line in
                File.ReadAllLines(path, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length < 6)
                    continue;

                if (!parts[0].Trim().Equals(
                    identifier,
                    StringComparison.OrdinalIgnoreCase))
                    continue;

                orderType = parts[2].Trim();

                decimal.TryParse(
                    parts[4].Trim(),
                    out totalAmount);

                return true;
            }

            return false;
        }

        private string GetTablePrefix(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return null;

            Match match = Regex.Match(
                identifier,
                @"^(T\d{2})$",
                RegexOptions.IgnoreCase);

            if (!match.Success)
                return null;

            return match.Groups[1].Value.ToUpper() + "-";
        }

        private void UpdateTableItemKeyIds(
            string tablePrefix,
            string newReceiptNo)
        {
            string path = Path.Combine(
                Application.StartupPath,
                "susi_order_items.csv");

            if (!File.Exists(path))
                return;

            List<string> lines =
                File.ReadAllLines(
                    path,
                    Encoding.UTF8).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split(',');

                if (parts.Length < 1)
                    continue;

                if (parts[0].Trim().StartsWith(
                    tablePrefix,
                    StringComparison.OrdinalIgnoreCase))
                {
                    parts[0] = newReceiptNo;
                    lines[i] = string.Join(",", parts);
                }
            }

            File.WriteAllLines(
                path,
                lines,
                new UTF8Encoding(false));
        }

        private void UpdateItemKeyId(
            string oldKeyId,
            string newReceiptNo)
        {
            string path = Path.Combine(
                Application.StartupPath,
                "susi_order_items.csv");

            if (!File.Exists(path))
                return;

            List<string> lines =
                File.ReadAllLines(
                    path,
                    Encoding.UTF8).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split(',');

                if (parts.Length < 1)
                    continue;

                if (parts[0].Trim().Equals(
                    oldKeyId,
                    StringComparison.OrdinalIgnoreCase))
                {
                    parts[0] = newReceiptNo;
                    lines[i] = string.Join(",", parts);
                }
            }

            File.WriteAllLines(
                path,
                lines,
                new UTF8Encoding(false));
        }

        private void RemoveSalesHistory(string receiptNo)
        {
            lock (orderFileLock)
            {
                RemoveCsvRows(
                    "susi_sales_history.csv",
                    receiptNo);
            }
        }

        private string GenerateNewReceiptNumber()
        {
            string today =
                DateTime.Now.ToString("yyyyMMdd");

            string prefix =
                $"ORD-{today}-";

            string path = Path.Combine(
                Application.StartupPath,
                "susi_sales_history.csv");

            int max = 0;

            if (File.Exists(path))
            {
                foreach (string line in
                    File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 1)
                        continue;

                    string receipt =
                        parts[0].Trim();

                    if (!receipt.StartsWith(
                        prefix,
                        StringComparison.OrdinalIgnoreCase))
                        continue;

                    string sequence =
                        receipt.Substring(prefix.Length);

                    if (int.TryParse(
                        sequence,
                        out int number) &&
                        number > max)
                    {
                        max = number;
                    }
                }
            }

            return $"{prefix}{max + 1:D3}";
        }

        // =========================================================
        // 회원 포인트
        // =========================================================

        private int GetMemberPoint(int memberId)
        {
            lock (memberFileLock)
            {
                string path = Path.Combine(
                    Application.StartupPath,
                    "member.csv");

                if (!File.Exists(path))
                    return 0;

                foreach (string line in
                    File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 7)
                        continue;

                    if (int.TryParse(
                        parts[0].Trim(),
                        out int id) &&
                        id == memberId)
                    {
                        int.TryParse(
                            parts[4].Trim(),
                            out int point);

                        return point;
                    }
                }

                return 0;
            }
        }

        private bool UpdateMemberPoint(
            int memberId,
            int usedPoint,
            int earnedPoint)
        {
            lock (memberFileLock)
            {
                string path = Path.Combine(
                    Application.StartupPath,
                    "member.csv");

                if (!File.Exists(path))
                    return false;

                string[] lines =
                    File.ReadAllLines(
                        path,
                        Encoding.UTF8);

                bool found = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] parts =
                        lines[i].Split(',');

                    if (parts.Length < 7)
                        continue;

                    if (!int.TryParse(
                        parts[0].Trim(),
                        out int id) ||
                        id != memberId)
                        continue;

                    int.TryParse(
                        parts[4].Trim(),
                        out int currentPoint);

                    if (usedPoint < 0 ||
                        usedPoint > currentPoint)
                        return false;

                    int newPoint =
                        currentPoint -
                        usedPoint +
                        earnedPoint;

                    parts[4] =
                        newPoint.ToString();

                    lines[i] =
                        string.Join(",", parts);

                    found = true;
                    break;
                }

                if (!found)
                    return false;

                File.WriteAllLines(
                    path,
                    lines,
                    new UTF8Encoding(false));

                return true;
            }
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

        public string CompleteKioskPayment(
            string identifier,
            int memberId,
            decimal originalAmount,
            int usedPoint,
            decimal totalAmount,
            string paymentMethod)
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
        // 알림
        // =========================================================

        public void UpdateOrderNotice()
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action(UpdateOrderNotice));

                return;
            }

            string path = Path.Combine(
                Application.StartupPath,
                "susi_orders_realtime.csv");

            int waitingCount = 0;

            if (File.Exists(path))
            {
                foreach (string line in
                    File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    if (parts.Length < 6)
                        continue;

                    string source =
                        parts[1].Trim();

                    string status =
                        parts[5].Trim();

                    if (source == "앱" &&
                        status == "접수 대기")
                    {
                        waitingCount++;
                    }
                }
            }

            UpdateNotice(waitingCount);
        }

        public void UpdateNotice(int waitingCount)
        {
            lblNotice.Text =
                $"신규 주문 [{waitingCount}건] 대기 중";

            lblNotice.ForeColor =
                Color.Yellow;
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
            }.ToString(
                Newtonsoft.Json.Formatting.None);
        }

        private string Success(string message)
        {
            return new JObject
            {
                ["Status"] = "SUCCESS",
                ["Message"] = message
            }.ToString(
                Newtonsoft.Json.Formatting.None);
        }

        // =========================================================
        // Form
        // =========================================================

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            isServerRunning = false;
            server?.Stop();

            base.OnFormClosed(e);
        }

        private void ShowView(
            UserControl view,
            Button clickedButton)
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
                currentSelectedButton.BackColor =
                    Color.FromArgb(45, 45, 48);

                currentSelectedButton
                    .FlatAppearance
                    .MouseOverBackColor =
                    Color.FromArgb(60, 60, 65);
            }

            currentSelectedButton = btn;

            if (currentSelectedButton != null)
            {
                Color activeColor =
                    Color.FromArgb(0, 122, 204);

                currentSelectedButton.BackColor =
                    activeColor;

                currentSelectedButton
                    .FlatAppearance
                    .MouseOverBackColor =
                    activeColor;
            }
        }

        private void btnNavOrder_Click(
            object sender,
            EventArgs e)
            => ShowView(
                new UcOrderBoard(),
                (Button)sender);

        private void btnNavTable_Click(
            object sender,
            EventArgs e)
            => ShowView(
                new UcTableMonitor(),
                (Button)sender);

        private void btnNavMenu_Click(
            object sender,
            EventArgs e)
            => ShowView(
                new UcMenuManagement(),
                (Button)sender);

        private void btnNavHistory_Click(
            object sender,
            EventArgs e)
            => ShowView(
                new UcOrderHistory(),
                (Button)sender);

        private void btnNavUser_Click(
            object sender,
            EventArgs e)
            => ShowView(
                new UcUserManagement(),
                (Button)sender);

        private void btnNavStock_Click(
            object sender,
            EventArgs e)
            => ShowView(
                new UcStockManagement(),
                (Button)sender);

        private void btnNavReport_Click(
            object sender,
            EventArgs e)
            => ShowView(
                new UcSalesReport(),
                (Button)sender);

        private void btnExit_Click(
            object sender,
            EventArgs e)
        {
            DialogResult result =
                MessageBox.Show(
                    "관리자 시스템을 종료하시겠습니까?",
                    "시스템 종료",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                Application.Exit();
        }
    }
}