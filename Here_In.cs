using sushikiosk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kiosk
{
    /// <summary>
    /// 매장 내 테이블 선택 및 상태 관리를 제공하는 화면입니다.
    /// 다국어 선택 상태를 지원합니다.
    /// </summary>
    public partial class Here_In : BaseLanguageForm
    {
        /// <summary>
        /// Here_In 클래스의 새 인스턴스를 생성하고 구성 요소를 초기화합니다.
        /// </summary>
        public Here_In()
        {
            InitializeComponent();
            buttons_arrayform();
        }

        /// <summary>
        /// 매장 내 배치된 테이블 버튼 목록을 저장하는 리스트입니다.
        /// </summary>
        List<Button> buttons = new List<Button>();

        /// 각 테이블(1~34번)의 선택 상태를 기록하는 배열입니다.
        bool[] table_state = new bool[35];
        private int? selectedTableNumber;

        /// <summary>
        /// 테이블 선택 알림 메시지의 다국어 템플릿입니다.
        /// Index 0: 영어, 1: 일본어, 2: 한국어
        /// </summary>
        private readonly string[] tableSelectFormats = { "Table {0} has been selected. State: {1}", "テーブル {0} が選択されました。 状態: {1}", "{0}번 테이블이 선택되었습니다. 상태: {1}" };

        // 버튼(btn_back, btn_choice) 다국어 텍스트 적용 (0: 영어, 1: 일본어, 2: 한국어)
        private readonly string[] backBtnTexts = { "← Back", "← 戻る", "← 이전으로" };
        private readonly string[] choiceBtnTexts = { "Select", "選択", "선택" };

        /// <summary>
        /// 폼 내부에서 button1부터 button34까지의 컨트롤을 찾아 이벤트를 연결하고 리스트에 등록합니다.
        /// </summary>
        private void buttons_arrayform()
        {

            for (int i = 1; i <= 34; i++)
            {
                // 이름으로 컨트롤 검색 (예: "button1", "button2"...)
                Control[] found = this.Controls.Find($"button{i}", true);

                if (found.Length > 0 && found[0] is Button btn)
                {
                    btn.Tag = i; // 버튼 고유 번호(1~34)를 Tag에 저장 (클릭 시 식별용)

                    // 34개의 공용 클릭 이벤트 메서드를 연결합니다.
                    btn.Click += Here_In_Button_Click;

                    buttons.Add(btn); // 리스트에 버튼 추가
                }
            }
        }







        /// <summary>
        /// 임의의 테이블 버튼을 클릭했을 때 작동하며, 테이블의 상태를 변경하고 다국어 안내 메시지를 출력합니다.
        /// </summary>
        /// <param name="sender">클릭 이벤트를 발생시킨 Button 객체입니다.</param>
        /// <param name="e">이벤트 데이터가 포함된 객체입니다.</param>
        private void Here_In_Button_Click(object? sender, EventArgs e)
        {
            Button? clicked_Button = sender as Button;
            if (clicked_Button == null) return;

            // 버튼의 Tag에 저장해둔 고유 인덱스 번호(1~34) 가져오기
            int btn_Index = (int)clicked_Button.Tag;

            foreach (Button tableButton in buttons)
                tableButton.BackColor = SystemColors.Control;

            // 해당 버튼의 상태를 토글(클릭할 때마다 true/false 전환) 또는 true로 고정
            // 여기서는 클릭 시 true로 변경하고 이미지를 바꾸는 예시입니다.
            table_state[btn_Index] = true;
            clicked_Button.BackColor = Color.LightSteelBlue;
            btn_choice.BackColor = Color.LightSteelBlue;
            selectedTableNumber = btn_Index;
            // 현재 언어 인덱스에 맞는 포맷을 가져와 바인딩
            int lang = LanguageManager.CurrentLanguageIndex;
            string message = string.Format(tableSelectFormats[lang], btn_Index, table_state[btn_Index]);
            int btn_Index1 = btn_Index;
            MessageBox.Show(message);


        }


        private void btn_choice_Click(object sender, EventArgs e)
        {
            // 테이블 선택 완료 후 메뉴 주문 화면(MenuForm) 표시 및 현재 창 숨김
            //MenuForm menuForm = new MenuForm();
            //menuForm.Show();
            //this.Hide();
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            Firstform fr = new Firstform();
            fr.Show();
            this.Hide();
        }
        private void Here_In_Load(object sender, EventArgs e)
        {

        }



        protected override void ApplyLanguage()
        {
            base.ApplyLanguage();
            int lang = LanguageManager.CurrentLanguageIndex;

            if (btn_back != null) btn_back.Text = backBtnTexts[lang];
            if (btn_choice != null) btn_choice.Text = choiceBtnTexts[lang];
        }

        private void btn_choice_Click_1(object sender, EventArgs e)
        {
            // 테이블 선택 완료 후 메뉴 주문 화면(MenuForm) 표시 및 현재 창 숨김
            if (selectedTableNumber is null)
            {
                MessageBox.Show("테이블을 먼저 선택해주세요.");
                return;
            }

            KioskSession.BeginTableOrder(selectedTableNumber.Value);
            MenuForm menuForm = new MenuForm();
            menuForm.Show();
            this.Hide();
        }
    }
}
