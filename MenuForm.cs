using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Kiosk;

namespace sushikiosk
{
    public partial class MenuForm : BaseLanguageForm
    {
        public class SushiMenu
        {
            public string Name { get; set; } = "";
            public int Price { get; set; }
            public string Category { get; set; } = "";
            public string ImageFile { get; set; } = "";
            public string EnglishName { get; set; } = "";
            public string JapaneseName { get; set; } = "";
            public string SaleStatus { get; set; } = "판매중";
            public bool CanOrder => SaleStatus == "판매중";

            public string GetDisplayName(int language) => language switch
            {
                0 when !string.IsNullOrWhiteSpace(EnglishName) => EnglishName,
                1 when !string.IsNullOrWhiteSpace(JapaneseName) => JapaneseName,
                _ => Name
            };
        }

        public class OrderItem          // 나중에 관리자랑 연동할 때 OrderItem.cs파일로 빼는
        {
            public string Name { get; set; } = "";
            public int Price { get; set; }
            public int Quantity { get; set; }
            public string Category { get; set; } = "";
            public bool IsFree { get; set; }   // 이벤트 당첨 여부
        }

        List<SushiMenu> menuList = new List<SushiMenu>();   // 메뉴 목록

        List<OrderItem> orderList = new List<OrderItem>();          // 전체 주문 목록
        List<OrderItem> currentOrderList = new List<OrderItem>();   // 이번에 새로 담은 주문 목록

        Random random = new Random();
        private bool serverMenuLoaded;
        private bool orderRequestRunning;

        string currentCategory = "활어/참치";
        int currentPage = 0;        // 현재 페이지
        int pageSize = 8;           // 한 페이지에 8개를 띄워라

        Button[] addButtons;

        Label[] nameLabels;
        Label[] priceLabels;
        Panel[] menuPanels;
        PictureBox[] menuPictures;

        public MenuForm()
        {
            InitializeComponent();

            nameLabels = new[]
            {
                lblMenuName1, lblMenuName2, lblMenuName3, lblMenuName4,
                lblMenuName5, lblMenuName6, lblMenuName7, lblMenuName8};

            addButtons = new[]
            {
                btnAdd1, btnAdd2, btnAdd3, btnAdd4,
                btnAdd5, btnAdd6, btnAdd7, btnAdd8};

            priceLabels = new[]
            {
                lblMenuPrice1, lblMenuPrice2, lblMenuPrice3, lblMenuPrice4,
                lblMenuPrice5, lblMenuPrice6, lblMenuPrice7, lblMenuPrice8};

            menuPanels = new[]
            {
                panelMenu1, panelMenu2, panelMenu3, panelMenu4,
                panelMenu5, panelMenu6, panelMenu7, panelMenu8};

            menuPictures = new[]
            {
                picMenu1, picMenu2, picMenu3, picMenu4,
                picMenu5, picMenu6, picMenu7, picMenu8};
        }

        private async void MenuForm_Load(object sender, EventArgs e)      // 메뉴폼이 실행될 때 메뉴를 등록하고 담기 버튼 이벤트를 연결
        {
            AddMenu("점성어초밥", 1500, "활어/참치", "Red Drum Sushi.png");           // 활어,참치 카테고리
            AddMenu("숭어초밥", 1500, "활어/참치", "Mullet Sushi.png");
            AddMenu("묵은지숭어초밥", 1500, "활어/참치", "Aged Kimchi Mullet Sushi.png");
            AddMenu("연어파인초밥", 1500, "활어/참치", "Salmon Pineapple Sushi.png");
            AddMenu("광어초밥", 3000, "활어/참치", "Flatfish Sushi.png");
            AddMenu("묵은지광어초밥", 3000, "활어/참치", "Aged Kimchi Flatfish Sushi.png");
            AddMenu("광어지느러미초밥", 3000, "활어/참치", "Flatfish Fin Sushi.png");
            AddMenu("연어초밥", 3000, "활어/참치", "Salmon Sushi.png");
            AddMenu("연어뱃살초밥", 3000, "활어/참치", "Salmon Belly Sushi.png");
            AddMenu("토핑연어초밥", 3000, "활어/참치", "Topped Salmon Sushi.png");
            AddMenu("구운연어초밥", 3000, "활어/참치", "Seared Salmon Sushi.png");
            AddMenu("묵은지활어초밥", 3000, "활어/참치", "Aged Kimchi Fresh Fish Sushi.png");
            AddMenu("눈다랑어초밥", 3000, "활어/참치", "Bigeye Tuna Sushi.png");
            AddMenu("구운참치초밥", 3000, "활어/참치", "Seared Tuna Sushi.png");
            AddMenu("참치대뱃살초밥", 6000, "활어/참치", "Fatty Tuna Sushi.png");
            AddMenu("황새치뱃살초밥", 6000, "활어/참치", "Swordfish Belly Sushi.png");
            AddMenu("도미뱃살조림초밥", 6000, "활어/참치", "Simmered Sea Bream Belly Sushi.png");

            AddMenu("오징어초밥", 1000, "해산물", "Squid Sushi.png");             // 해산물 카테고리
            AddMenu("게살초밥", 1000, "해산물", "Crab Meat Sushi.png");
            AddMenu("소라초밥", 1000, "해산물", "Whelk Sushi.png");
            AddMenu("날치알군함", 1000, "해산물", "Flying Fish Roe Gunkan.png");
            AddMenu("초새우초밥", 1500, "해산물", "Cooked Shrimp Sushi.png");
            AddMenu("갑오징어초밥", 1500, "해산물", "Cuttlefish Sushi.png");
            AddMenu("치즈소라초밥", 1500, "해산물", "Cheese Whelk Sushi.png");
            AddMenu("한치초밥", 1500, "해산물", "Spear Squid Sushi.png");
            AddMenu("생새우초밥", 1500, "해산물", "Raw Shrimp Sushi.png");
            AddMenu("계란새우초밥", 1500, "해산물", "Egg Shrimp Sushi.png");
            AddMenu("구운소라초밥", 1500, "해산물", "Seared Whelk Sushi.png");
            AddMenu("가지소라초밥", 1500, "해산물", "Eggplant Whelk Sushi.png");
            AddMenu("타코와사비초밥", 1500, "해산물", "Octopus Wasabi Sushi.png");
            AddMenu("간장새우초밥", 3000, "해산물", "Soy-marinated Shrimp Sushi.png");
            AddMenu("가리비치즈초밥", 3000, "해산물", "Cheese Scallop Sushi.png");
            AddMenu("가리비초밥", 3000, "해산물", "Scallop Sushi.png");
            AddMenu("마늘가리비초밥", 3000, "해산물", "Garlic Scallop Sushi.png");
            AddMenu("생새우마늘구이초밥", 3000, "해산물", "Garlic Grilled Shrimp Sushi.png");
            AddMenu("계란장어초밥", 3000, "해산물", "Egg Eel Sushi.png");
            AddMenu("아귀간군함", 6000, "해산물", "Monkfish Liver Gunkan.png");
            AddMenu("성게알군함", 6000, "해산물", "Sea Urchin Gunkan.png");
            AddMenu("구운관자초밥", 6000, "해산물", "Seared Scallop Sushi.png");

            AddMenu("후톳마끼", 6000, "롤/마끼", "Futomaki.png");                  // 롤,마끼
            AddMenu("치즈새우롤", 6000, "롤/마끼", "Cheese Shrimp Roll.png");
            AddMenu("구운연어롤", 6000, "롤/마끼", "Seared Salmon Roll.png");
            AddMenu("고구마롤", 6000, "롤/마끼", "Sweet Potato Roll.png");
            AddMenu("새우튀김롤", 6000, "롤/마끼", "Shrimp Tempura Roll.png");
            AddMenu("김마끼", 6000, "롤/마끼", "Seaweed Roll.png");

            AddMenu("유부초밥", 1000, "단품/기타초밥", "Inari Sushi.png");        // 단품,기타초밥
            AddMenu("계란초밥", 1000, "단품/기타초밥", "Egg Sushi.png");
            AddMenu("우삼겹초밥", 1500, "단품/기타초밥", "Beef Belly Sushi.png");
            AddMenu("육사시미초밥", 3000, "단품/기타초밥", "Raw Beef Sushi.png");
            AddMenu("스테이크초밥", 3000, "단품/기타초밥", "Steak Sushi.png");
            AddMenu("육회초밥", 3000, "단품/기타초밥", "Beef Tartare Sushi.png");

            AddMenu("파인애플", 8000, "사이드/면/디저트", "Pineapple.png");        // 사이드,면,디저트
            AddMenu("가라아게", 12000, "사이드/면/디저트", "Karaage.png");
            AddMenu("새우튀김", 3000, "사이드/면/디저트", "Fried Shrimp.png");
            AddMenu("미니 모밀", 5000, "사이드/면/디저트", "Mini Soba.png");
            AddMenu("미니 우동", 5000, "사이드/면/디저트", "Mini Udon.png");

            AddMenu("사이다", 1000, "음료", "Sprite.jpg");                       // 음료
            AddMenu("콜라", 1000, "음료", "Coke.png");
            AddMenu("제로콜라", 1000, "음료", "Coke Zero.png");

            AddMenu("물 리필해주세요", 0, "직원 호출", "Water Refill.jpg");        // 직원 호출
            AddMenu("컵 주세요", 0, "직원 호출", "Cup Request.jpg");
            AddMenu("직원만 호출", 0, "직원 호출", "Staff Call.jpg");

            foreach (Button button in addButtons)
            {
                button.Click += AddButton_Click;
                button.Enabled = false;
            }

            ShowPage();
            await LoadLatestMenuAsync();
        }

        private async Task LoadLatestMenuAsync()
        {
            try
            {
                MenuResponse response = await KioskSession.Server.GetMenuAsync();
                if (!response.IsSuccess)
                    throw new InvalidDataException(response.Message);

                // 직원 호출은 로컬 기능이며, 음식/음료는 서버에 존재하는 메뉴만 표시합니다.
                // 이름에 오타/변경이 있어도 동일 이미지 파일이면 같은 메뉴로 인식합니다.
                foreach (SushiMenu menu in menuList.Where(menu => menu.Category != "직원 호출").ToList())
                {
                    ServerMenu? latest = response.Menus.FirstOrDefault(serverMenu =>
                        serverMenu.KoreanName == menu.Name ||
                        (!string.IsNullOrWhiteSpace(serverMenu.ImageFile) &&
                         string.Equals(serverMenu.ImageFile, menu.ImageFile, StringComparison.OrdinalIgnoreCase)));
                    if (latest is null)
                    {
                        menuList.Remove(menu);
                        continue;
                    }

                    menu.Name = latest.KoreanName;
                    menu.Price = latest.Price;
                    menu.EnglishName = latest.EnglishName;
                    menu.JapaneseName = latest.JapaneseName;
                    menu.SaleStatus = latest.SaleStatus;
                    if (!string.IsNullOrWhiteSpace(latest.ImageFile))
                        menu.ImageFile = latest.ImageFile;
                }

                serverMenuLoaded = true;
                ShowPage();
            }
            catch (Exception ex)
            {
                serverMenuLoaded = false;
                ShowPage();
                MessageBox.Show(
                    "최신 메뉴를 불러오지 못해 주문 기능을 사용할 수 없습니다.\n\n" + ex.Message,
                    "서버 연결 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // 다국어 메뉴 이름 매핑 딕셔너리
        private static readonly Dictionary<string, string[]> menuTranslations = new Dictionary<string, string[]>
                {
                    { "점성어초밥", new[] { "Red Drum Sushi", "ニ베寿司", "점성어초밥" } },
                    { "숭어초밥", new[] { "Mullet Sushi", "ボラ寿司", "숭어초밥" } },
                    { "묵은지숭어초밥", new[] { "Aged Kimchi Mullet Sushi", "古漬けキムチボラ寿司", "묵은지숭어초밥" } },
                    { "연어파인초밥", new[] { "Salmon Pineapple Sushi", "サーモンパイン寿司", "연어파인초밥" } },
                    { "광어초밥", new[] { "Flatfish Sushi", "ヒラメ寿司", "광어초밥" } },
                    { "묵은지광어초밥", new[] { "Aged Kimchi Flatfish Sushi", "古漬けキムチヒラメ寿司", "묵은지광어초밥" } },
                    { "광어지느러미초밥", new[] { "Flatfish Fin Sushi", "エンガワ寿司", "광어지느러미초밥" } },
                    { "연어초밥", new[] { "Salmon Sushi", "サーモン寿司", "연어초밥" } },
                    { "연어뱃살초밥", new[] { "Salmon Belly Sushi", "サーモンハラス寿司", "연어뱃살초밥" } },
                    { "토핑연어초밥", new[] { "Topped Salmon Sushi", "トッピングサーモン寿司", "토핑연어초밥" } },
                    { "구운연어초밥", new[] { "Seared Salmon Sushi", "炙りサーモン寿司", "구운연어초밥" } },
                    { "묵은지활어초밥", new[] { "Aged Kimchi Fresh Fish Sushi", "古漬けキムチ白身魚寿司", "묵은지활어초밥" } },
                    { "눈다랑어초밥", new[] { "Bigeye Tuna Sushi", "メバチマグロ寿司", "눈다랑어초밥" } },
                    { "구운참치초밥", new[] { "Seared Tuna Sushi", "炙りマグロ寿司", "구운참치초밥" } },
                    { "참치대뱃살초밥", new[] { "Fatty Tuna Sushi", "本マグロ大トロ寿司", "참치대뱃살초밥" } },
                    { "황새치뱃살초밥", new[] { "Swordfish Belly Sushi", "メカジキトロ寿司", "황새치뱃살초밥" } },
                    { "도미뱃살조림초밥", new[] { "Simmered Sea Bream Belly Sushi", "鯛腹身煮付け寿司", "도미뱃살조림초밥" } },
                    { "오징어초밥", new[] { "Squid Sushi", "イカ寿司", "오징어초밥" } },
                    { "게살초밥", new[] { "Crab Meat Sushi", "カニカマ寿司", "게살초밥" } },
                    { "소라초밥", new[] { "Whelk Sushi", "ツブ貝寿司", "소라초밥" } },
                    { "날치알군함", new[] { "Flying Fish Roe Gunkan", " tobiko軍艦", "날치알군함" } },
                    { "초새우초밥", new[] { "Cooked Shrimp Sushi", "蒸しエビ寿司", "초새우초밥" } },
                    { "갑오징어초밥", new[] { "Cuttlefish Sushi", "コウイカ寿司", "갑오징어초밥" } },
                    { "치즈소라초밥", new[] { "Cheese Whelk Sushi", "チーズツブ貝寿司", "치즈소라초밥" } },
                    { "한치초밥", new[] { "Spear Squid Sushi", "ヤリイカ寿司", "한치초밥" } },
                    { "생새우초밥", new[] { "Raw Shrimp Sushi", "生エビ寿司", "생새우초밥" } },
                    { "계란새우초밥", new[] { "Egg Shrimp Sushi", "卵エビ寿司", "계란새우초밥" } },
                    { "구운소라초밥", new[] { "Seared Whelk Sushi", "炙りツブ貝寿司", "구운소라초밥" } },
                    { "가지소라초밥", new[] { "Eggplant Whelk Sushi", "ナスツブ貝寿司", "가지소라초밥" } },
                    { "타코와사비초밥", new[] { "Octopus Wasabi Sushi", "たこわさび軍艦", "타코와사비초밥" } },
                    { "간장새우초밥", new[] { "Soy-marinated Shrimp Sushi", "醤油エビ寿司", "간장새우초밥" } },
                    { "가리비치즈초밥", new[] { "Cheese Scallop Sushi", "ホタテチーズ寿司", "가리비치즈초밥" } },
                    { "가리비초밥", new[] { "Scallop Sushi", "ホタテ寿司", "가리비초밥" } },
                    { "마늘가리비초밥", new[] { "Garlic Scallop Sushi", "ガーリックホタテ寿司", "마늘가리비초밥" } },
                    { "생새우마늘구이초밥", new[] { "Garlic Grilled Shrimp Sushi", "生エビニンニク炙り寿司", "생새우마늘구이초밥" } },
                    { "계란장어초밥", new[] { "Egg Eel Sushi", "卵うなぎ寿司", "계란장어초밥" } },
                    { "아귀간군함", new[] { "Monkfish Liver Gunkan", "あんき모軍艦", "아귀간군함" } },
                    { "성게알군함", new[] { "Sea Urchin Gunkan", "ウニ軍艦", "성게알군함" } },
                    { "구운관자초밥", new[] { "Seared Scallop Sushi", "炙りホタテ貝柱寿司", "구운관자초밥" } },
                    { "후톳마끼", new[] { "Futomaki", "太巻き", "후톳마끼" } },
                    { "치즈새우롤", new[] { "Cheese Shrimp Roll", "チーズエビロール", "치즈새우롤" } },
                    { "구운연어롤", new[] { "Seared Salmon Roll", "炙りサーモンロール", "구운연어롤" } },
                    { "고구마롤", new[] { "Sweet Potato Roll", "さつまいもロール", "고구마롤" } },
                    { "새우튀김롤", new[] { "Shrimp Tempura Roll", "エビフライロール", "새우튀김롤" } },
                    { "김마끼", new[] { "Seaweed Roll", "手巻き", "김마끼" } },
                    { "유부초밥", new[] { "Inari Sushi", "いなり寿司", "유부초밥" } },
                    { "계란초밥", new[] { "Egg Sushi", "玉子寿司", "계란초밥" } },
                    { "우삼겹초밥", new[] { "Beef Belly Sushi", "牛バラ寿司", "우삼겹초밥" } },
                    { "육사시미초밥", new[] { "Raw Beef Sushi", "牛刺身寿司", "육사시미초밥" } },
                    { "스테이크초밥", new[] { "Steak Sushi", "ステーキ寿司", "스테이크초밥" } },
                    { "육회초밥", new[] { "Beef Tartare Sushi", "ユッケ寿司", "육회초밥" } },
                    { "파인애플", new[] { "Pineapple", "パイナップル", "파인애플" } },
                    { "가라아게", new[] { "Karaage", "唐揚げ", "가라아게" } },
                    { "새우튀김", new[] { "Fried Shrimp", "エビフライ", "새우튀김" } },
                    { "미니 모밀", new[] { "Mini Soba", "ミニそば", "미니 모밀" } },
                    { "미니 우동", new[] { "Mini Udon", "ミニうどん", "미니 우동" } },
                    { "사이다", new[] { "Sprite", "サイダー", "사이다" } },
                    { "콜라", new[] { "Coke", "コーラ", "콜라" } },
                    { "제로콜라", new[] { "Coke Zero", "ゼロコーラ", "제로콜라" } },
                    { "물 리필해주세요", new[] { "Refill Water", "お水のおかわり", "물 리필해주세요" } },
                    { "컵 주세요", new[] { "Give me a cup", "コップをください", "컵 주세요" } },
                    { "직원만 호출", new[] { "Call Staff", "呼び出し", "직원만 호출" } }
                };








        private void AddMenu(string name, int price, string category, string imageFile)     // 메뉴하나의 이름, 가격, 카테고리,
        {                                                                                   // 이미지 정보를 메뉴 목록에 추가
            menuList.Add(new SushiMenu
            {
                Name = name,
                Price = price,
                Category = category,
                ImageFile = imageFile
            });
        }

        private void ShowPage()         // 현재 선택된 카테고리와 페이지에 맞는 메뉴를 화면에 표시
        {
            List<SushiMenu> filteredMenu =
                menuList.Where(menu => menu.Category == currentCategory).ToList();

            int startIndex = currentPage * pageSize;

            for (int i = 0; i < pageSize; i++)
            {
                int menuIndex = startIndex + i;

                if (menuIndex >= filteredMenu.Count)
                {
                    menuPictures[i].Image?.Dispose();
                    menuPictures[i].Image = null;
                    menuPanels[i].Visible = false;
                    continue;
                }

                SushiMenu menu = filteredMenu[menuIndex];

                int lang = LanguageManager.CurrentLanguageIndex;
                nameLabels[i].Text = menu.GetDisplayName(lang);

                addButtons[i].Tag = menu;

                if (menu.Category == "직원 호출")
                {
                    // 직원 호출에는 가격이 필요 없음
                    priceLabels[i].Visible = false;

                    // 담기 대신 요청
                    string[] requestTexts = { "Request", "要求", "요청" };
                    addButtons[i].Text = requestTexts[lang];
                    addButtons[i].Enabled = true;
                }
                else
                {
                    // 일반 음식 메뉴
                    priceLabels[i].Visible = true;
                    string[] wonTexts = { " KRW", "ウォン", "원" };
                    priceLabels[i].Text = menu.Price.ToString("N0") + wonTexts[lang];

                    if (!menu.CanOrder)
                    {
                        string[] soldOutTexts = { "Sold Out", "売り切れ", "품절" };
                        addButtons[i].Text = soldOutTexts[lang];
                    }
                    else
                    {
                        string[] addTexts = { "Add", "入れる", "담기" };
                        addButtons[i].Text = addTexts[lang];
                    }
                    addButtons[i].Enabled = serverMenuLoaded && menu.CanOrder;
                }

                if (!string.IsNullOrEmpty(menu.ImageFile))
                {
                    string imagePath =
                        Path.Combine(Application.StartupPath, "Images", menu.ImageFile);

                    menuPictures[i].Image?.Dispose();

                    menuPictures[i].Image =
                        File.Exists(imagePath) ? Image.FromFile(imagePath) : null;
                }
                else
                {
                    menuPictures[i].Image?.Dispose();
                    menuPictures[i].Image = null;
                }

                menuPanels[i].Visible = true;
            }

            int totalPages =
                (int)Math.Ceiling((double)filteredMenu.Count / pageSize);

            lblPage.Text = (currentPage + 1) + " / " + totalPages;
        }

        private void btnNext_Click(object sender, EventArgs e)      // 다음 페이지가 있으면 다음 메뉴 페이지로 이동
        {
            List<SushiMenu> filteredMenu =
                menuList.Where(menu => menu.Category == currentCategory).ToList();

            int totalPages =
                (int)Math.Ceiling((double)filteredMenu.Count / pageSize);

            if (currentPage < totalPages - 1)
            {
                currentPage++;
                ShowPage();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)      // 이전 페이지가 있으면 이전 메뉴 페이지로 이동
        {
            if (currentPage > 0)
            {
                currentPage--;
                ShowPage();
            }
        }

        private void AddButton_Click(object? sender, EventArgs e)        // 하나의 이벤트로 담기 8개의 버튼을 처리,
        {
            if (sender is Button button && button.Tag is SushiMenu menu)
            {
                if (menu.Category == "직원 호출")           // 직원 호출 카테고리는 장바구니에 넣지 않음
                {
                    ShowStaffRequest(menu);
                    return;
                }
                if (!serverMenuLoaded || !menu.CanOrder)
                {
                    MessageBox.Show("품절 메뉴이거나 최신 메뉴 정보가 없어 주문할 수 없습니다.");
                    return;
                }
                AddOrder(menu);            // 음식 메뉴만 장바구니에 추가
            }
        }

        private void ShowStaffRequest(SushiMenu menu)
        {
            string message;

            switch (menu.Name)
            {
                case "물 리필해주세요":
                    message = "물 리필을 요청했습니다.";
                    break;

                case "컵 주세요":
                    message = "컵을 요청했습니다.";
                    break;

                case "직원만 호출":
                    message = "직원을 호출했습니다.";
                    break;

                default:
                    return;
            }

            MessageBox.Show(
                message,
                "직원 호출",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void AddOrder(SushiMenu menu)               // 선택한 메뉴를 장바구니에 추가하고 이미 있으면 수량을 증가.
        {
            OrderItem? currentItem =
                currentOrderList.FirstOrDefault(item => item.Name == menu.Name);

            if (currentItem != null)
            {
                currentItem.Quantity++;
            }
            else
            {
                currentOrderList.Add(new OrderItem
                {
                    Name = menu.Name,
                    Price = menu.Price,
                    Quantity = 1,
                    Category = menu.Category,
                    IsFree = false
                });
            }

            ShowOrder();
        }

        private void ShowOrder()        // 현재 장바구니의 메뉴와 수량, 금액, 총 금액을 화면에 표시
        {
            dgvOrder.Rows.Clear();

            int totalPrice = 0;
            int lang = LanguageManager.CurrentLanguageIndex;

            string[] currencySymbols = { "KRW", "円", "원" };
            string[] totalAmountTexts = { "Total Amount : ", "合計金額 : ", "총 금액 : " };

            foreach (OrderItem item in currentOrderList)
            {
                int itemTotal = item.Price * item.Quantity;
                SushiMenu? matchingMenu = menuList.FirstOrDefault(menu => menu.Name == item.Name);
                string displayName = matchingMenu?.GetDisplayName(lang)
                    ?? (menuTranslations.ContainsKey(item.Name) ? menuTranslations[item.Name][lang] : item.Name);

                dgvOrder.Rows.Add(
                    displayName,
                    "-",
                    item.Quantity,
                    "+",
                    itemTotal.ToString("N0") + currencySymbols[lang]
                );

                totalPrice += itemTotal;
            }

            lblTotalPrice.Text =
                totalAmountTexts[lang] + totalPrice.ToString("N0") + currencySymbols[lang];
        }

        private void dgvOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)      // 장바구니의 +, - 버튼을 처리하여 
        {                                                                                       // 주문 수량을 변경.
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            int index = e.RowIndex;

            OrderItem item = currentOrderList[index];

            if (dgvOrder.Columns[e.ColumnIndex].Name == "colPlus")
            {
                item.Quantity++;

                ShowOrder();
            }
            else if (dgvOrder.Columns[e.ColumnIndex].Name == "colMinus")
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                    currentOrderList.Remove(item);

                ShowOrder();
            }
        }

        private Dictionary<string, int> CheckWinningEvent() // 이번에 새로 주문한 메뉴만 대상으로 당첨 이벤트를 확인
        {
<<<<<<< Updated upstream
=======
            Dictionary<string, int> discounts = new();
>>>>>>> Stashed changes
            {
                // 이벤트 대상 초밥 메뉴만 가져옴
                List<OrderItem> eventItems = currentOrderList
                    .Where(item => item.Category == "활어/참치" || item.Category == "해산물" ||
                                   item.Category == "롤/마끼" || item.Category == "단품/기타초밥")
                    .ToList();

<<<<<<< Updated upstream
                List<string> winningMessages = new List<string>();  // 당첨 결과 메시지를 저장

=======
>>>>>>> Stashed changes
                foreach (OrderItem item in eventItems)      // 이벤트 대상 메뉴를 하나씩 검사
                {
                    int winningCount = 0;

                    for (int i = 0; i < item.Quantity; i++)     // 메뉴의 접시 수만큼 각각 5% 확률로 검사
                    {
                        if (random.Next(100) < 5)
                        {
                            winningCount++;
                        }
                    }
                    if (winningCount == 0)          // 당첨된 접시가 없으면 다음 메뉴 검사
                        continue;

<<<<<<< Updated upstream
                    item.Quantity -= winningCount;  // 정상 결제 수량에서 당첨 수량 차감

                    if (item.Quantity <= 0)         // 전부 당첨됐다면 현재 장바구니에서 제거
                    {
                        currentOrderList.Remove(item);
                    }


                    OrderItem freeItem =            // 이전 주문에서 같은 메뉴의 당첨 항목이 있는지 확인
                        orderList.FirstOrDefault(order => order.Name == item.Name && order.IsFree);

                    if (freeItem != null)
                    {
                        freeItem.Quantity += winningCount;      // 기존 당첨 항목에 수량 누적
                    }
                    else
                    {
                        orderList.Add(new OrderItem     // 처음 당첨된 메뉴라면 무료 항목 생성
                        {
                            Name = item.Name,
                            Price = item.Price,
                            Quantity = winningCount,
                            Category = item.Category,
                            IsFree = true
                        });
                    }

                    // MessageBox에 표시할 결과 저장
                    winningMessages.Add(item.Name + " " + winningCount + "접시");
                }
                if (winningMessages.Count > 0)      // 하나라도 당첨됐을 때만 표시
                {
                    MessageBox.Show("당첨!\n\n" + string.Join("\n", winningMessages) + "\n\n무료입니다!", "이벤트 당첨",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
=======
                    discounts[item.Name] = winningCount;

>>>>>>> Stashed changes
                }
            }
            return discounts;
        }

        private void SaveCurrentOrder(IReadOnlyDictionary<string, int> discounts)
        {
            foreach (OrderItem currentItem in currentOrderList)
            {
                int freeQuantity = discounts.TryGetValue(currentItem.Name, out int value) ? value : 0;
                int paidQuantity = currentItem.Quantity - freeQuantity;
                OrderItem? orderedItem =           // 같은 이름의 일반 주문만 찾음
                    orderList.FirstOrDefault(item => item.Name == currentItem.Name && !item.IsFree);

                if (orderedItem != null && paidQuantity > 0)
                {
                    orderedItem.Quantity += paidQuantity;       // 기존 일반 주문에 수량 누적
                }
                else if (paidQuantity > 0)
                {
                    orderList.Add(new OrderItem     // 처음 주문한 메뉴라면 새 항목 추가
                    {
                        Name = currentItem.Name,
                        Price = currentItem.Price,
                        Quantity = paidQuantity,
                        Category = currentItem.Category,
                        IsFree = false
                    });
                }

                if (freeQuantity > 0)
                {
                    OrderItem? freeItem = orderList.FirstOrDefault(
                        item => item.Name == currentItem.Name && item.IsFree);
                    if (freeItem != null)
                    {
                        freeItem.Quantity += freeQuantity;
                    }
                    else
                    {
                        orderList.Add(new OrderItem
                        {
                            Name = currentItem.Name,
                            Price = currentItem.Price,
                            Quantity = freeQuantity,
                            Category = currentItem.Category,
                            IsFree = true
                        });
                    }
                }
            }
            currentOrderList.Clear();       // 주문 처리가 끝났으므로 현재 장바구니 초기화

            ShowOrder();
        }

        private void Category_Click(object sender, EventArgs e)     // 클릭한 메뉴스트립 항목에 맞게 
        {                                                           // 카테고리를 변경하고 첫 페이지를 표시.
            if (!(sender is ToolStripMenuItem item))
                return;

            string txt = item.Text;

            // 다국어 텍스트 매치 -> 한국어 카테고리로 매핑 변환
            if (txt == "Fresh Fish/Tuna" || txt == "活魚/マグロ") currentCategory = "활어/참치";
            else if (txt == "Seafood" || txt == "海鮮") currentCategory = "해산물";
            else if (txt == "Roll/Maki" || txt == "ロール/手巻き") currentCategory = "롤/마끼";
            else if (txt == "Single/Other Sushi" || txt == "単品/その他寿司") currentCategory = "단품/기타초밥";
            else if (txt == "Side/Noodle/Dessert" || txt == "サイド/麺/デザート") currentCategory = "사이드/면/디저트";
            else if (txt == "Beverage" || txt == "飲料") currentCategory = "음료";
            else if (txt == "Staff Call" || txt == "呼び出し") currentCategory = "직원 호출";
            else currentCategory = txt;

            currentPage = 0;
            ShowPage();
        }

        private void btnOrderHistory_Click(object sender, EventArgs e)      // 현재 담긴 주문 내용을 ReceiptForm으로 표시
        {
            if (orderList.Count == 0)
            {
                MessageBox.Show("주문 내역이 없습니다.");
                return;
            }

            new ReceiptForm(orderList).ShowDialog();
        }

        private async void btnOrder_Click(object sender, EventArgs e)             // 주문확인 버튼을 눌렀을 때
        {
            if (orderRequestRunning)
                return;
            if (currentOrderList.Count == 0)
            {
                MessageBox.Show("추가로 주문할 메뉴를 먼저 담아주세요.");
                return;
            }
            if (KioskSession.IsTakeout && KioskSession.HasOrders)
            {
                MessageBox.Show("포장 주문은 한 번만 등록할 수 있습니다. 결제를 진행해주세요.");
                return;
            }

            Dictionary<string, int> discounts = CheckWinningEvent();
            List<NewOrderItem> requestItems = currentOrderList.Select(item => new NewOrderItem
            {
                MenuName = item.Name,
                Price = item.Price,
                Quantity = item.Quantity,
                DiscountQty = discounts.TryGetValue(item.Name, out int count) ? count : 0
            }).ToList();

            string identifier;
            try
            {
                identifier = KioskSession.GetNextOrderIdentifier();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "주문 정보 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            orderRequestRunning = true;
            btnOrder.Enabled = false;
            try
            {
                OrderResponse response = await KioskSession.Server.NewOrderAsync(
                    identifier,
                    KioskSession.OrderType,
                    requestItems);

                if (!response.IsSuccess)
                {
                    MessageBox.Show(response.Message, "주문 등록 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int totalAmount = requestItems.Sum(item => item.SubTotal);
                KioskSession.ConfirmOrder(totalAmount);
                SaveCurrentOrder(discounts);
                if (discounts.Count > 0)
                {
                    string winningMessage = string.Join("\n", discounts.Select(
                        discount => discount.Key + " " + discount.Value + "접시"));
                    MessageBox.Show(
                        "당첨!\n\n" + winningMessage + "\n\n무료입니다!",
                        "이벤트 당첨",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                MessageBox.Show($"주문이 완료되었습니다.\n주문번호: {identifier}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "주문 전송 중 오류가 발생했습니다. 관리자에서 주문번호 저장 여부를 확인한 뒤 다시 시도해주세요.\n\n" + ex.Message,
                    "통신 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                orderRequestRunning = false;
                btnOrder.Enabled = true;
            }
        }

        protected override void ApplyLanguage()
        {
            base.ApplyLanguage();
            int lang = LanguageManager.CurrentLanguageIndex;

            // ToolStripMenuItem은 Control이 아니기 때문에 개별적으로 텍스트를 대입해 줍니다.
            if (toolStripMenuItem1 != null) toolStripMenuItem1.Text = new[] { "Fresh Fish/Tuna", "活魚/マグロ", "활어/참치" }[lang];
            if (toolStripMenuItem2 != null) toolStripMenuItem2.Text = new[] { "Seafood", "海鮮", "해산물" }[lang];
            if (toolStripMenuItem3 != null) toolStripMenuItem3.Text = new[] { "Roll/Maki", "ロール/手巻き", "롤/마끼" }[lang];
            if (toolStripMenuItem5 != null) toolStripMenuItem5.Text = new[] { "Single/Other Sushi", "単品/その他寿司", "단품/기타초밥" }[lang];
            if (toolStripMenuItem4 != null) toolStripMenuItem4.Text = new[] { "Side/Noodle/Dessert", "サイド/麺/デザート", "사이드/면/디저트" }[lang];
            if (toolStripMenuItem6 != null) toolStripMenuItem6.Text = new[] { "Beverage", "飲料", "음료" }[lang];
            if (menuStaff != null) menuStaff.Text = new[] { "Staff Call", "呼び出し", "직원 호출" }[lang];

            // 일반 Control에 속하는 버튼들의 다국어 텍스트 대입
            if (btnPrevious != null) btnPrevious.Text = new[] { "Previous", "以前", "이전" }[lang];
            if (btnNext != null) btnNext.Text = new[] { "Next", "次へ", "다음" }[lang];
            if (btnOrderHistory != null) btnOrderHistory.Text = new[] { "Order history", "注文履歴", "주문 내역" }[lang];
            if (btnOrder != null) btnOrder.Text = new[] { "Order Confirm", "注文確認", "주문 하기" }[lang];
            if (btn_receive != null) btn_receive.Text = new[] { "Pay", "決済する", "결제하기" }[lang];

            // 현재 카테고리 매칭을 위해 매핑 데이터 사용
            var categoryTranslations = new Dictionary<string, string[]>
            {
                { "Fresh Fish/Tuna", new[] { "Fresh Fish/Tuna", "活魚/マグロ", "활어/참치" } },
                { "Seafood", new[] { "Seafood", "海鮮", "해산물" } },
                { "Roll/Maki", new[] { "Roll/Maki", "ロール/手巻き", "롤/마끼" } },
                { "Single/Other Sushi", new[] { "Single/Other Sushi", "単品/その他寿司", "단품/기타초밥" } },
                { "Side/Noodle/Dessert", new[] { "Side/Noodle/Dessert", "サイド/麺/デザート", "사이드/면/디저트" } },
                { "Beverage", new[] { "Beverage", "飲料", "음료" } },
                { "Staff Call", new[] { "Staff Call", "呼び出し", "직원 호출" } }
            };

            var matchedCategory = categoryTranslations
                .FirstOrDefault(kvp => kvp.Value.Contains(currentCategory));

            if (matchedCategory.Key != null)
            {
                // 실제 내부 데이터 처리 기준(한국어)으로 고정하기 위한 매핑 목록
                var rawCategories = new Dictionary<string, string>
                {
                    { "Fresh Fish/Tuna", "활어/참치" },
                    { "Seafood", "해산물" },
                    { "Roll/Maki", "롤/마끼" },
                    { "Single/Other Sushi", "단품/기타초밥" },
                    { "Side/Noodle/Dessert", "사이드/면/디저트" },
                    { "Beverage", "음료" },
                    { "Staff Call", "직원 호출" }
                };
                currentCategory = rawCategories[matchedCategory.Key];
            }

            ShowPage();
            ShowOrder();
        }

        private void btn_receive_Click(object sender, EventArgs e)
        {
            if (!KioskSession.HasOrders)
            {
                MessageBox.Show("결제할 주문이 없습니다. 먼저 주문을 완료해주세요.");
                return;
            }

            Pop_MemberNum member = new Pop_MemberNum(this);
            member.Show();
            this.Hide();
        }
    }
}
