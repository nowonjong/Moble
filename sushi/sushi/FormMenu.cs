using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sushi
{
    public partial class FormMenu : Form
    {
        private string orderType;

        private int cartCount = 0;
        private int totalPrice = 0;
        private const int MinimumOrderAmount = 12000;

        private readonly Dictionary<string, CartItem> cartItems = new Dictionary<string, CartItem>();

        // 카테고리 제목의 위치를 저장
        private readonly Dictionary<string, Control> categoryTargets = new Dictionary<string, Control>();

        // 카테고리 출력 순서
        private readonly string[] categoryOrder =
        {
            "활어/참치",
            "해산물",
            "롤/마끼",
            "단품/기타초밥",
            "사이드/면/디저트",
            "음료"
        };

        private readonly HashSet<string> fishMenus = new HashSet<string>
        {
            "점성어초밥",
            "숭어초밥",
            "묵은지숭어초밥",
            "연어파인초밥",
            "광어초밥",
            "묵은지광어초밥",
            "광어지느러미초밥",
            "연어초밥",
            "연어뱃살초밥",
            "토핑연어초밥",
            "구운연어초밥",
            "홍민어초밥",
            "묵은지활어초밥",
            "눈다랑어초밥",
            "구운참치초밥",
            "참치대뱃살초밥",
            "황새치뱃살초밥",
            "도미뱃살조림초밥"
        };

        private readonly HashSet<string> seafoodMenus = new HashSet<string>
        {
            "오징어초밥",
            "게살초밥",
            "소라초밥",
            "날치알군함",
            "초새우초밥",
            "갑오징어초밥",
            "치즈소라초밥",
            "한치초밥",
            "생새우초밥",
            "계란새우초밥",
            "구운소라초밥",
            "가지소라초밥",
            "타코와사비초밥",
            "간장새우초밥",
            "가리비치즈초밥",
            "가리비초밥",
            "마늘가리비초밥",
            "생새우마늘구이초밥",
            "계란장어초밥",
            "아귀간군함",
            "성게알군함",
            "구운관자초밥"
        };

        private readonly HashSet<string> rollMenus = new HashSet<string>
        {
            "후톳마끼",
            "치즈새우롤",
            "구운연어롤",
            "고구마롤",
            "새우튀김롤",
            "김마끼"
        };

        private readonly HashSet<string> singleMenus = new HashSet<string>
        {
            "유부초밥",
            "계란초밥",
            "우삼겹초밥",
            "육사시미초밥",
            "스테이크초밥",
            "육회초밥"
        };

        private readonly HashSet<string> sideMenus = new HashSet<string>
        {
            "파인애플",
            "가라아게",
            "새우튀김",
            "미니 모밀",
            "미니 우동"
        };

        private readonly HashSet<string> drinkMenus = new HashSet<string>
        {
            "사이다",
            "콜라",
            "제로콜라"
        };

        public FormMenu(string orderType)
        {
            InitializeComponent();

            this.orderType = orderType;
            lbOrder.Text = orderType + "주문";
            Shown += FormMenu_Shown;

            // 카테고리 버튼 이벤트 연결
            button1.Click += (sender, e) => ScrollToCategory("활어/참치");
            button2.Click += (sender, e) => ScrollToCategory("해산물");
            button3.Click += (sender, e) => ScrollToCategory("롤/마끼");
            button4.Click += (sender, e) => ScrollToCategory("단품/기타초밥");
            btnSide.Click += (sender, e) => ScrollToCategory("사이드/면/디저트");
            btnDrink.Click += (sender, e) => ScrollToCategory("음료");
        }

        private async void FormMenu_Shown(object sender, EventArgs e)
        {
            await LoadMenuFromServerAsync();
        }

        private async Task LoadMenuFromServerAsync()
        {
            UseWaitCursor = true;

            try
            {
                string requestJson = JsonSerializer.Serialize(new { Action = "GET_MENU" });
                string responseJson = await TcpClient.SendJsonAsync(requestJson);

                using JsonDocument document = JsonDocument.Parse(responseJson);
                JsonElement response = document.RootElement;

                string status = response.GetProperty("Status").GetString() ?? "";

                if (status != "SUCCESS")
                {
                    string message = response.GetProperty("Message").GetString() ?? "";
                    MessageBox.Show($"메뉴 조회 실패\n{message}");
                    return;
                }

                flpMenu.Controls.Clear();
                categoryTargets.Clear();
                cartItems.Clear();

                cartCount = 0;
                totalPrice = 0;
                UpdateCartSummary();

                List<MenuInfo> menuList = new List<MenuInfo>();

                foreach (JsonElement menuData in response.GetProperty("Menus").EnumerateArray())
                {
                    string menuId = menuData.GetProperty("MenuId").GetInt32().ToString();
                    string menuName = menuData.GetProperty("KoreanName").GetString() ?? "";
                    int price = menuData.GetProperty("Price").GetInt32();
                    string saleStatus = menuData.GetProperty("SaleStatus").GetString() ?? "";
                    string imageFile = menuData.GetProperty("ImageFile").GetString() ?? "";

                    menuList.Add(new MenuInfo
                    {
                        Id = menuId,
                        Name = menuName,
                        Price = price,
                        Category = GetCategory(menuName),
                        SaleStatus = saleStatus,
                        ImageFile = imageFile
                    });
                }

                foreach (string category in categoryOrder)
                {
                    List<MenuInfo> categoryMenus = menuList
                        .Where(menu => menu.Category == category)
                        .OrderBy(menu => menu.Price)
                        .ToList();

                    if (categoryMenus.Count == 0)
                        continue;

                    Label categoryHeader = AddCategoryHeader(category);
                    categoryTargets[category] = categoryHeader;

                    foreach (MenuInfo menu in categoryMenus)
                        AddMenuRow(menu.Id, menu.Name, menu.Price, menu.SaleStatus, menu.ImageFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"관리자 서버 메뉴 조회 실패\n{ex.Message}");
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private Label AddCategoryHeader(string category)
        {
            Label categoryHeader = new Label();

            int headerWidth = flpMenu.Width - SystemInformation.VerticalScrollBarWidth - 25;

            categoryHeader.Width = Math.Max(280, headerWidth);
            categoryHeader.Height = 42;
            categoryHeader.Margin = new Padding(5, 12, 5, 3);
            categoryHeader.Text = category;
            categoryHeader.Font = new Font("맑은 고딕", 13, FontStyle.Bold);
            categoryHeader.TextAlign = ContentAlignment.MiddleLeft;
            categoryHeader.Padding = new Padding(10, 0, 0, 0);
            categoryHeader.BackColor = Color.MistyRose;

            flpMenu.Controls.Add(categoryHeader);

            return categoryHeader;
        }

        private Panel AddMenuRow(string menuId, string menuName, int price, string saleStatus, string imageFile)
        {
            Panel menuRow = new Panel();

            int rowWidth = flpMenu.Width - SystemInformation.VerticalScrollBarWidth - 25;

            menuRow.Width = Math.Max(280, rowWidth);
            menuRow.Height = 120;
            menuRow.Margin = new Padding(5);
            menuRow.BorderStyle = BorderStyle.FixedSingle;
            menuRow.BackColor = Color.White;

            // 메뉴 사진
            PictureBox pbMenu = new PictureBox();

            pbMenu.Location = new Point(10, 15);
            pbMenu.Size = new Size(90, 90);
            pbMenu.SizeMode = PictureBoxSizeMode.Zoom;
            pbMenu.BackColor = Color.Gainsboro;

            string imagePath = FindMenuImage(imageFile);

            if (!string.IsNullOrEmpty(imagePath))
            {
                pbMenu.Image = Image.FromFile(imagePath);
            }
            else
            {
                Label lblNoImage = new Label();

                lblNoImage.Text = "사진 없음";
                lblNoImage.Dock = DockStyle.Fill;
                lblNoImage.TextAlign = ContentAlignment.MiddleCenter;
                lblNoImage.ForeColor = Color.Gray;

                pbMenu.Controls.Add(lblNoImage);
            }

            // 메뉴 이름
            Label lblMenuName = new Label();

            lblMenuName.Text = menuName;
            lblMenuName.Font = new Font("맑은 고딕", 11, FontStyle.Bold);
            lblMenuName.Location = new Point(110, 10);
            lblMenuName.Size = new Size(menuRow.Width - 125, 35);
            lblMenuName.TextAlign = ContentAlignment.MiddleLeft;

            // 가격
            Label lblPrice = new Label();

            lblPrice.Text = price.ToString("N0") + "원";
            lblPrice.Font = new Font("맑은 고딕", 10);
            lblPrice.Location = new Point(110, 45);
            lblPrice.Size = new Size(menuRow.Width - 125, 25);
            lblPrice.TextAlign = ContentAlignment.MiddleLeft;

            // 수량 빼기 버튼
            Button btnMinus = new Button();

            btnMinus.Text = "−";
            btnMinus.Location = new Point(110, 78);
            btnMinus.Size = new Size(32, 28);
            btnMinus.Enabled = false;

            // 수량 표시
            Label lblQuantity = new Label();

            lblQuantity.Text = "0";
            lblQuantity.Location = new Point(145, 78);
            lblQuantity.Size = new Size(32, 28);
            lblQuantity.TextAlign = ContentAlignment.MiddleCenter;

            // 수량 더하기 버튼
            Button btnPlus = new Button();

            btnPlus.Text = "+";
            btnPlus.Location = new Point(180, 78);
            btnPlus.Size = new Size(32, 28);

            bool isOnSale = saleStatus == "판매중";
            btnPlus.Enabled = isOnSale;

            if (!isOnSale)
            {
                lblPrice.Text = price.ToString("N0") + "원 (품절)";
                lblPrice.ForeColor = Color.Red;
                menuRow.BackColor = Color.Gainsboro;
            }

            int quantity = 0;

            // 더하기
            btnPlus.Click += (sender, e) =>
            {
                quantity++;
                cartCount++;
                totalPrice += price;

                lblQuantity.Text = quantity.ToString();
                btnMinus.Enabled = true;

                if (!cartItems.ContainsKey(menuId))
                {
                    cartItems.Add(menuId, new CartItem
                    {
                        MenuId = menuId,
                        MenuName = menuName,
                        Price = price,
                        Quantity = 0
                    });
                }

                cartItems[menuId].Quantity = quantity;
                UpdateCartSummary();
            };

            // 빼기
            btnMinus.Click += (sender, e) =>
            {
                if (quantity <= 0)
                    return;

                quantity--;
                cartCount--;
                totalPrice -= price;

                lblQuantity.Text = quantity.ToString();
                btnMinus.Enabled = quantity > 0;

                if (quantity == 0)
                    cartItems.Remove(menuId);
                else
                    cartItems[menuId].Quantity = quantity;

                UpdateCartSummary();
            };

            menuRow.Controls.Add(pbMenu);
            menuRow.Controls.Add(lblMenuName);
            menuRow.Controls.Add(lblPrice);
            menuRow.Controls.Add(btnMinus);
            menuRow.Controls.Add(lblQuantity);
            menuRow.Controls.Add(btnPlus);

            flpMenu.Controls.Add(menuRow);

            return menuRow;
        }

        private void UpdateCartSummary()
        {
            label2.Text = "장바구니 " + cartCount + "개 / 총 " + totalPrice.ToString("N0") + "원";
        }

        private void ScrollToCategory(string category)
        {
            if (!categoryTargets.TryGetValue(category, out Control target))
                return;

            // 현재 스크롤 위치를 고려한 실제 위치
            int targetY = target.Top - flpMenu.AutoScrollPosition.Y;

            // 카테고리 제목 위치로 이동
            flpMenu.AutoScrollPosition = new Point(0, targetY);
        }

        private string GetCategory(string menuName)
        {
            if (fishMenus.Contains(menuName))
                return "활어/참치";

            if (seafoodMenus.Contains(menuName))
                return "해산물";

            if (rollMenus.Contains(menuName))
                return "롤/마끼";

            if (singleMenus.Contains(menuName))
                return "단품/기타초밥";

            if (sideMenus.Contains(menuName))
                return "사이드/면/디저트";

            if (drinkMenus.Contains(menuName))
                return "음료";

            return "단품/기타초밥";
        }

        private string FindMenuImage(string imageFile)
        {
            string imageFolder = Path.Combine(Application.StartupPath, "MenuImages");

            if (string.IsNullOrWhiteSpace(imageFile))
                return "";

            string serverImagePath = Path.Combine(imageFolder, Path.GetFileName(imageFile));

            if (File.Exists(serverImagePath))
                return serverImagePath;

            string csvPath = Path.Combine(Application.StartupPath, "susi_menu.csv");

            if (!File.Exists(csvPath))
                return "";

            string englishName = Path.GetFileNameWithoutExtension(imageFile);
            string[] extensions = { ".jpg", ".jpeg", ".png" };

            foreach (string line in File.ReadAllLines(csvPath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] values = line.Split(',');

                if (values.Length < 4)
                    continue;

                string localMenuId = values[0].Trim();
                string localEnglishName = values[3].Trim();

                if (!localEnglishName.Equals(englishName, StringComparison.OrdinalIgnoreCase))
                    continue;

                foreach (string extension in extensions)
                {
                    string imagePath = Path.Combine(imageFolder, localMenuId + extension);

                    if (File.Exists(imagePath))
                        return imagePath;
                }
            }

            return "";
        }

        private class MenuInfo
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public int Price { get; set; }
            public string Category { get; set; } = "";
            public string SaleStatus { get; set; } = "";
            public string ImageFile { get; set; } = "";
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("메뉴를 선택해주세요");
                return;
            }

            if (totalPrice < MinimumOrderAmount)
            {
                int shortage = MinimumOrderAmount - totalPrice;

                MessageBox.Show("최소 주문금액은 12000원입니다.\n" + shortage.ToString("N0") + "원 부족합니다.");
                return;
            }

            List<CartItem> selectedItems = cartItems.Values.ToList();

            FormPayment payment = new FormPayment(orderType, selectedItems);

            if (payment.ShowDialog() == DialogResult.OK)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}