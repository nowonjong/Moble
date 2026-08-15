using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;

namespace sushi
{
    public partial class FormPayment : Form
    {
        private string orderType;

        private List<CartItem> orderItems = new List<CartItem>();

        private int paymentAmount = 0;
        private int usedPoint = 0;
        private int finalPaymentAmount = 0;

        private bool pickupLayoutReduced = false;
        private readonly string orderIdentifier = CreateOrderIdentifier();

        public FormPayment()
        {
            InitializeComponent();

            tbaddress.PlaceholderText = "주소를 입력해주세요.";

            cbPay.SelectedIndex = 0;
            radioButton2.Checked = true;
        }

        public FormPayment(string orderType, List<CartItem> orderItems) : this()
        {
            this.orderType=orderType;
            this.orderItems=orderItems;

            lbOrder.Text = orderType + "주문";

            ShowOrderItems();
            SetOrdertype();
            ShowPaymentAmount();
            SetPointInfo();
        }

        private static string CreateOrderIdentifier()
        {
            DateTime now = DateTime.Now;
            return $"ORD-{now:yyyyMMdd}-APP{now:HHmmssfff}";
        }

        private void ShowOrderItems()
        {
            tbOrderMenu.Clear();

            foreach (CartItem item in orderItems)
            {
                string orderLine = item.MenuName +
                    " " +
                    item.Quantity +
                    "개 " +
                    item.TotalPrice.ToString("N0") +
                    "원";

                tbOrderMenu.AppendText(orderLine+Environment.NewLine);
            }
        }

        private void SetOrdertype()
        {
            bool isDelivery = orderType == "배달";

            lbadress.Visible = isDelivery;
            tbaddress.Visible = isDelivery;

            tbaddress.ReadOnly = false;

            if (isDelivery)
            {
                tbaddress.Text = "";
            }
            else
            {
                tbaddress.Text = "";

                ReducePickupLayout();
            }
        }

        private void ShowPaymentAmount()
        {
            paymentAmount = orderItems.Sum(item => item.TotalPrice);

            lbTotal.Text = paymentAmount.ToString("N0") + "원";
        }

        private void ReducePickupLayout()
        {
            if (pickupLayoutReduced)
            {
                return;
            }

            pickupLayoutReduced = true;

            int moveup = 75;

            Control[] controlsToMove =
            {
                 label3, tbRequest, label5, cbPay, lbPayInfo, lbPoint, groupBox1, label6, label4, label12, lbpay, btnPay
            };

            foreach (Control control in controlsToMove)
            {
                control.Top -= moveup;
            }

            ClientSize = new Size(ClientSize.Width, ClientSize.Height - moveup);
        }

        private void SetPointInfo()
        {
            int availablePoint = Math.Max(0, PointStore.TotalPoints);

            lbTotalPoint.Text = "총 포인트 : " + availablePoint.ToString("N0") + "P";

            UpdatePointPayment();
        }

        private void UpdatePointPayment()
        {
            int avilablePoint = Math.Max(0, PointStore.TotalPoints);

            if (radioButton1.Checked)
            {
                usedPoint = Math.Min(avilablePoint, paymentAmount);
            }
            else
            {
                usedPoint = 0;
            }

            finalPaymentAmount = paymentAmount - usedPoint;

            if (usedPoint > 0)
            {
                lbPoint.Text = "-" + usedPoint.ToString("N0") + "원";
            }
            else
            {
                lbPoint.Text = "0원";
            }

            lbpay.Text = finalPaymentAmount.ToString("N0") + "원";
        }
        private void cbPay_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbPayInfo.Items.Clear();

            if (cbPay.SelectedIndex == 0)
            {
                lbPayInfo.Items.Add("카카오페이");
                lbPayInfo.Items.Add("삼성페이");
                lbPayInfo.Items.Add("애플페이");
                lbPayInfo.Items.Add("토스페이");
                lbPayInfo.Items.Add("네이버페이");
            }
            else if (cbPay.SelectedIndex == 1)
            {
                lbPayInfo.Items.Add("국민은행");
                lbPayInfo.Items.Add("우리은행");
                lbPayInfo.Items.Add("신한은행");
                lbPayInfo.Items.Add("기업은행");
                lbPayInfo.Items.Add("농협은행");

                lbPayInfo.SelectedIndex = -1;
            }
        }

        private async void btnPay_Click(object sender, EventArgs e)
        {
            if (orderType == "배달" && string.IsNullOrWhiteSpace(tbaddress.Text))
            {
                MessageBox.Show("배달주소를 입력해주세요.");

                tbaddress.Focus();
                return;
            }

            if (finalPaymentAmount > 0 && lbPayInfo.SelectedItem == null)
            {
                MessageBox.Show("결제수단을 선택해주세요.");

                lbPayInfo.Focus();
                return;
            }

            string paymentMethod;

            if(finalPaymentAmount == 0)
            {
                paymentMethod = "포인트 전액결제";
            }
            else
            {
                paymentMethod = lbPayInfo.SelectedItem?.ToString()??"";

                if(usedPoint > 0)
                {
                    paymentMethod += " + 포인트";
                }
            }

            DateTime paymentDate = DateTime.Now;
            int memberId = UserSession.IsLoggedIn ? UserSession.MemberId : 0;
            int pointToUse = memberId > 0 ? usedPoint : 0;

            btnPay.Enabled = false;

            try
            {
                var requestData = new
                {
                    Action = "NEW_APP_ORDER",
                    Identifier = orderIdentifier,
                    Source = "앱",
                    OrderType = orderType,
                    OrderTime = paymentDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    TotalAmount = paymentAmount,
                    MemberId = memberId,
                    UsedPoint = pointToUse,
                    PaymentMethod = "앱선결제",
                    Status = "접수 대기",
                    Items = orderItems.Select(item => new
                    {
                        MenuName = item.MenuName,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        DiscountQty = 0,
                        SubTotal = item.TotalPrice
                    }).ToList()
                };

                string requestJson = JsonSerializer.Serialize(requestData);
                string responseJson = await TcpClient.SendJsonAsync(requestJson);

                using JsonDocument document = JsonDocument.Parse(responseJson);
                JsonElement response = document.RootElement;

                string status = response.GetProperty("Status").GetString() ?? "";
                string message = response.TryGetProperty("Message", out JsonElement messageElement)
                    ? messageElement.GetString() ?? ""
                    : "";

                if (status != "SUCCESS")
                {
                    MessageBox.Show($"주문 등록 실패\n{message}");
                    return;
                }

                OrderRecord newOrder = new OrderRecord
                {
                    Identifier = orderIdentifier,
                    OrderStatus = "접수 대기",
                    OrderDate = paymentDate,
                    OrderType = orderType,
                    PaymentAmount = paymentAmount,
                    Request = tbRequest.Text.Trim()
                };

                foreach (CartItem item in orderItems)
                {
                    newOrder.Items.Add(new CartItem
                    {
                        MenuId = item.MenuId,
                        MenuName = item.MenuName,
                        Price = item.Price,
                        Quantity = item.Quantity
                    });
                }

                if (pointToUse > 0)
                {
                    PointStore.Records.Insert(0, new PointRecord
                    {
                        PointDate = paymentDate,
                        Reason = orderType + " 주문 포인트 사용",
                        PointChange = -pointToUse
                    });
                }

                int earnedPoint = UserSession.IsLoggedIn ? finalPaymentAmount / 100 : 0;

                if (earnedPoint > 0)
                {
                    PointStore.Records.Insert(0, new PointRecord
                    {
                        PointDate = paymentDate,
                        Reason = orderType + " 주문 적립",
                        PointChange = earnedPoint
                    });
                }

                OrderStore.Orders.Insert(0, newOrder);

                MessageBox.Show(
                    "결제가 완료됐습니다.\n" +
                    "주문번호: " + orderIdentifier + "\n" +
                    "결제수단: " + paymentMethod + "\n" +
                    "결제금액: " + finalPaymentAmount.ToString("N0") + "원\n" +
                    "적립포인트: " + earnedPoint.ToString("N0") + "P");

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"관리자 서버 주문 전송 실패\n{ex.Message}");
            }
            finally
            {
                if (!IsDisposed)
                    btnPay.Enabled = true;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked && PointStore.TotalPoints <= 0)
            {
                MessageBox.Show("사용 가능한 포인트가 없습니다.");

                radioButton2.Checked = true;
                return;
            }

            UpdatePointPayment();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
