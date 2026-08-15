using System;
using System.Drawing;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sushi
{
    public partial class FormOrder : Form
    {
        public FormOrder()
        {
            InitializeComponent();

            ShowOrderHistory();
            Shown += FormOrder_Shown;
        }

        private async void FormOrder_Shown(object sender, EventArgs e)
        {
            await RefreshOrderStatusesAsync();
            ShowOrderHistory();
        }

        private async Task RefreshOrderStatusesAsync()
        {
            foreach (OrderRecord order in OrderStore.Orders)
            {
                if (string.IsNullOrWhiteSpace(order.Identifier))
                    continue;

                try
                {
                    var requestData = new
                    {
                        Action = "GET_ORDER_STATUS",
                        Identifier = order.Identifier
                    };

                    string requestJson = JsonSerializer.Serialize(requestData);
                    string responseJson = await TcpClient.SendJsonAsync(requestJson);

                    using JsonDocument document = JsonDocument.Parse(responseJson);
                    JsonElement response = document.RootElement;

                    string status = response.GetProperty("Status").GetString() ?? "";

                    if (status == "SUCCESS")
                        order.OrderStatus = response.GetProperty("OrderStatus").GetString() ?? order.OrderStatus;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"주문 상태 조회 실패\n{ex.Message}");
                    return;
                }
            }
        }

        private void ShowOrderHistory()
        {
            flpOrders.Controls.Clear();

            if (OrderStore.Orders.Count == 0)
            {
                Label lbEmpty = new Label
                {
                    Text = "주문내역이 없습니다.",
                    AutoSize = true,
                    Margin = new Padding(20)
                };

                flpOrders.Controls.Add(lbEmpty);
                return;
            }

            foreach (OrderRecord order in OrderStore.Orders)
            {
                OrderHistoryItem item = new OrderHistoryItem();
                item.SetOrderData(order);
                flpOrders.Controls.Add(item);
            }
        }
    }
}