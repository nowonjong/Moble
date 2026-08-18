using sushikiosk;
using System.Linq.Expressions;

namespace Kiosk
{
    public partial class Firstform : BaseLanguageForm
    {
        private readonly string[] hereinTexts = { "Eat In", "店内飲食", "매장 식사" };
        private readonly string[] togoTexts = { "Take Out", "持ち帰り", "포장 주문" };

        public Firstform()
        {
            InitializeComponent();

            // 첫 화면에서, 언어 선택 버튼 이벤트 바인딩
            btn_EngCh.Click += (s, e) => LanguageManager.SetLanguage(0); // English
            btn_JapCh.Click += (s, e) => LanguageManager.SetLanguage(1); // Japanese
            btn_KorCh.Click += (s, e) => LanguageManager.SetLanguage(2); // Korean
        }


        // BaseLanguageForm 클래스에서 상속 
        protected override void ApplyLanguage()
        {
            int langIndex = LanguageManager.CurrentLanguageIndex;
            btn_herein.Text = hereinTexts[langIndex];
            btn_togo.Text = togoTexts[langIndex];
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            Here_In hereinform = new Here_In();
            hereinform.Show();
            this.Hide();
        }

        private void lb_herein_Click(object sender, EventArgs e)
        {

        }

        private void btn_JapCh_Click(object sender, EventArgs e)
        {

        }

        private void btn_togo_Click(object sender, EventArgs e)
        {
            KioskSession.BeginTakeout();
            MenuForm mf = new MenuForm();
            mf.Show();
            this.Hide();
        }
    }
}
