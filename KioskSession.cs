using System.IO;

namespace Kiosk;

public static class KioskSession
{
    public static AdminServerClient Server { get; } = new();
    public static string OrderType { get; private set; } = "";
    public static int? TableNumber { get; private set; }
    public static int TableOrderSequence { get; private set; }
    public static string? TakeoutIdentifier { get; private set; }
    public static int OriginalAmount { get; private set; }
    public static MemberResponse? Member { get; set; }

    public static bool HasOrders => OriginalAmount > 0;
    public static bool IsTakeout => OrderType == "포장";

    public static void BeginTableOrder(int tableNumber)
    {
        Reset();
        OrderType = "매장";
        TableNumber = tableNumber;
    }

    public static void BeginTakeout()
    {
        Reset();
        OrderType = "포장";
        TakeoutIdentifier = TakeoutSequence.ReserveIdentifier();
    }

    public static string GetNextOrderIdentifier()
    {
        if (IsTakeout)
            return TakeoutIdentifier ?? throw new InvalidOperationException("포장 주문번호가 없습니다.");
        if (TableNumber is null)
            throw new InvalidOperationException("테이블을 먼저 선택해야 합니다.");
        return $"T{TableNumber.Value:00}-{TableOrderSequence + 1:00}";
    }

    public static string GetPaymentIdentifier()
    {
        if (IsTakeout)
            return TakeoutIdentifier ?? throw new InvalidOperationException("포장 주문번호가 없습니다.");
        if (TableNumber is null)
            throw new InvalidOperationException("테이블을 먼저 선택해야 합니다.");
        return $"T{TableNumber.Value:00}";
    }

    public static void ConfirmOrder(int amount)
    {
        if (!IsTakeout)
            TableOrderSequence++;
        OriginalAmount += amount;
    }

    public static void Reset()
    {
        OrderType = "";
        TableNumber = null;
        TableOrderSequence = 0;
        TakeoutIdentifier = null;
        OriginalAmount = 0;
        Member = null;
    }
}

internal static class TakeoutSequence
{
    private static readonly object Sync = new();

    public static string ReserveIdentifier()
    {
        lock (Sync)
        {
            string directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Kiosk");
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, "takeout-sequence.txt");
            string today = DateTime.Now.ToString("yyyyMMdd");
            int sequence = 1;

            if (File.Exists(path))
            {
                string[] saved = File.ReadAllText(path).Split('|');
                if (saved.Length == 2 && saved[0] == today && int.TryParse(saved[1], out int last))
                    sequence = last + 1;
            }

            File.WriteAllText(path, $"{today}|{sequence}");
            return $"K-{today}-{sequence:000}";
        }
    }
}
