using System.Net.Sockets;
using System.Text.Json;

namespace Kiosk;

public abstract class AdminResponse
{
    public string Status { get; set; } = "";
    public string Message { get; set; } = "";
    public bool IsSuccess => string.Equals(Status, "SUCCESS", StringComparison.OrdinalIgnoreCase);
}

public sealed class MenuResponse : AdminResponse
{
    public List<ServerMenu> Menus { get; set; } = new();
}

public sealed class ServerMenu
{
    public int MenuId { get; set; }
    public string KoreanName { get; set; } = "";
    public string JapaneseName { get; set; } = "";
    public string EnglishName { get; set; } = "";
    public int Price { get; set; }
    public string SaleStatus { get; set; } = "";
    public string ImageFile { get; set; } = "";
}

public sealed class MemberResponse : AdminResponse
{
    public int MemberId { get; set; }
    public string MemberName { get; set; } = "";
    public string Phone { get; set; } = "";
    public int Point { get; set; }
}

public sealed class NewOrderItem
{
    public string MenuName { get; set; } = "";
    public int Price { get; set; }
    public int Quantity { get; set; }
    public int DiscountQty { get; set; }
    public int SubTotal => (Quantity - DiscountQty) * Price;
}

public sealed class OrderResponse : AdminResponse
{
    public string Identifier { get; set; } = "";
}

public sealed class PaymentResponse : AdminResponse
{
    public string ReceiptNo { get; set; } = "";
    public double OriginalAmount { get; set; }
    public double UsedPoint { get; set; }
    public double TotalAmount { get; set; }
    public double EarnedPoint { get; set; }
}

public sealed class AdminServerClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = null,
        PropertyNameCaseInsensitive = true
    };

    public string Host { get; }
    public int Port { get; }
    public TimeSpan Timeout { get; }

    public AdminServerClient(string host = "192.168.0.62", int port = 9000, TimeSpan? timeout = null)
    {
        Host = host;
        Port = port;
        Timeout = timeout ?? TimeSpan.FromSeconds(10);
    }

    public Task<MenuResponse> GetMenuAsync(CancellationToken cancellationToken = default) =>
        SendAsync<MenuResponse>(new { Action = "GET_MENU" }, cancellationToken);

    public Task<MemberResponse> GetMemberAsync(string phone, CancellationToken cancellationToken = default) =>
        SendAsync<MemberResponse>(new { Action = "GET_MEMBER", Phone = phone }, cancellationToken);

    public Task<OrderResponse> NewOrderAsync(
        string identifier,
        string orderType,
        IReadOnlyCollection<NewOrderItem> items,
        CancellationToken cancellationToken = default)
    {
        if (items.Count == 0)
            throw new ArgumentException("주문 항목이 없습니다.", nameof(items));

        foreach (NewOrderItem item in items)
        {
            if (item.Quantity <= 0 || item.DiscountQty < 0 || item.DiscountQty > item.Quantity)
                throw new ArgumentException("주문 수량 또는 무료 수량이 올바르지 않습니다.", nameof(items));
        }

        return SendAsync<OrderResponse>(new
        {
            Action = "NEW_ORDER",
            Identifier = identifier,
            Source = "키오스크",
            OrderType = orderType,
            OrderTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            TotalAmount = items.Sum(item => item.SubTotal),
            Status = "조리 중",
            Items = items
        }, cancellationToken);
    }

    public Task<PaymentResponse> CompletePaymentAsync(
        string identifier,
        int memberId,
        double originalAmount,
        double usedPoint,
        string paymentMethod,
        CancellationToken cancellationToken = default)
    {
        if (originalAmount < 0 || usedPoint < 0 || usedPoint > originalAmount)
            throw new ArgumentException("결제 금액 또는 사용 포인트가 올바르지 않습니다.");
        if (memberId == 0 && usedPoint != 0)
            throw new ArgumentException("비회원은 포인트를 사용할 수 없습니다.");

        return SendAsync<PaymentResponse>(new
        {
            Action = "PAYMENT_COMPLETE",
            Identifier = identifier,
            MemberId = memberId,
            OriginalAmount = originalAmount,
            UsedPoint = usedPoint,
            TotalAmount = originalAmount - usedPoint,
            PaymentMethod = paymentMethod
        }, cancellationToken);
    }

    private async Task<TResponse> SendAsync<TResponse>(object request, CancellationToken cancellationToken)
        where TResponse : AdminResponse
    {
        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(Timeout);

        try
        {
            using TcpClient client = new();
            await client.ConnectAsync(Host, Port).WaitAsync(timeout.Token);

            await using NetworkStream stream = client.GetStream();
            byte[] requestBytes = JsonSerializer.SerializeToUtf8Bytes(request, JsonOptions);
            await stream.WriteAsync(requestBytes, 0, requestBytes.Length, timeout.Token);
            await stream.FlushAsync(timeout.Token);
            client.Client.Shutdown(SocketShutdown.Send);

            using MemoryStream responseBytes = new();
            byte[] buffer = new byte[8192];
            while (true)
            {
                int read = await stream.ReadAsync(buffer, 0, buffer.Length, timeout.Token);
                if (read == 0)
                    break;
                responseBytes.Write(buffer, 0, read);
                if (responseBytes.Length > 10 * 1024 * 1024)
                    throw new InvalidDataException("서버 응답 크기가 너무 큽니다.");
            }

            if (responseBytes.Length == 0)
                throw new InvalidDataException("관리자 서버가 빈 응답을 반환했습니다.");

            return JsonSerializer.Deserialize<TResponse>(responseBytes.ToArray(), JsonOptions)
                ?? throw new InvalidDataException("관리자 서버 응답을 해석할 수 없습니다.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"관리자 서버({Host}:{Port}) 응답 시간이 초과되었습니다.");
        }
        catch (SocketException ex)
        {
            throw new IOException($"관리자 서버({Host}:{Port})에 연결할 수 없습니다.", ex);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException("관리자 서버가 잘못된 JSON을 반환했습니다.", ex);
        }
    }
}
