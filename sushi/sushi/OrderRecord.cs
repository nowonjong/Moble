using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sushi
{
    public class OrderRecord
    {
        public DateTime OrderDate { get; set; }
        public string OrderType { get; set; } = "";
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public int PaymentAmount { get; set; }
        public string Request { get; set; } = "";
        public string Identifier { get; set; } = "";
        public string OrderStatus { get; set; } = "접수 대기";
    }

    public static class OrderStore
    {
        public static List<OrderRecord> Orders { get; } = new List<OrderRecord>();
    }
}
