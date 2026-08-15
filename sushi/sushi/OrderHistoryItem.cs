using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sushi
{
    public partial class OrderHistoryItem : UserControl
    {
        public OrderHistoryItem()
        {
            InitializeComponent();
        }

        public void SetOrderData(OrderRecord order)
        {
            lbOrderDate.Text = order.OrderDate.ToString("yyyy-MM-dd HH:mm");

            lbOrderType.Text = order.OrderType + " 주문 / " + order.OrderStatus;

            tbMenu.Text = string.Join(Environment.NewLine, order.Items.Select(item => item.MenuName + " " + item.Quantity + "개"));

            lbPayAmount.Text = order.PaymentAmount.ToString("N0") + "원";

            if (string.IsNullOrWhiteSpace(order.Request))
            {
                tbRequest.Text = "요청사항 없음";
            }
            else
            {
                tbRequest.Text = order.Request;
            }
        }
    }
}
