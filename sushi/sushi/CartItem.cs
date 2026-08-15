using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sushi
{
    public class CartItem
    {
        public string MenuId { get; set; } = "";
        public string MenuName { get; set; } = "";
        public int Price { get; set; } 
        public int Quantity { get; set; } 

        public int TotalPrice
        {
            get
            {
                return Price * Quantity;
            }
        }
    }
}
