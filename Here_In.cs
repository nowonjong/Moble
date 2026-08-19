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

        private int? selectedTableIndex = null;

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

            for (int i = 1; i <= 10; i++)
            {
                Control[] found = this.Controls.Find($"button{i}", true);

                if (found.Length > 0 && found[0] is Button btn)
                {
                    btn.Tag = i;

                    btn.Click += Here_In_Button_Click;

                    buttons.Add(btn);
                }
            }
        }







        /// <summary>
        /// 임의의 테이블 버튼을 클릭했을 때 작동하며, 테이블의 상태를 변경하고 다국어 안내 메시지를 출력합니다.
        /// </summary>
        /// <param name="sender">클릭 이벤트를 발생시킨 Button 객체입니다.</param>
        /// <param name="e">이벤트 데이터가 포함된 객체입니다.</param>
        private void Here_In_Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton == null)
                return;

            int tableNumber = (int)clickedButton.Tag;

            string tableCode = $"T{tableNumber:D2}";

            TableStateData tableData = TableStateStore.Load();

            // 이미 사용 중인 테이블은 선택 불가능
            if (tableData.Tables.ContainsKey(tableCode) &&
                tableData.Tables[tableCode] == "OCCUPIED")
            {
                MessageBox.Show(
                    $"{tableNumber}번 테이블은 현재 사용 중입니다.",
                    "테이블 선택",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            // 기존 선택 색상 초기화
            foreach (Button button in buttons)
            {
                button.BackColor = SystemColors.Control;
            }

            // 현재 선택한 테이블 저장
            selectedTableIndex = tableNumber;

            clickedButton.BackColor = Color.LightSteelBlue;
            btn_choice.BackColor = Color.LightSteelBlue;

            int lang = LanguageManager.CurrentLanguageIndex;

            string message = string.Format(
                tableSelectFormats[lang],
                tableNumber,
                true
            );

            MessageBox.Show(message);

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
            // 테이블을 선택하지 않은 경우
            if (selectedTableIndex == null)
            {
                MessageBox.Show(
                    "테이블을 먼저 선택해주세요.",
                    "테이블 선택",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            // 선택한 테이블 번호
            int tableNumber = selectedTableIndex.Value;

            // T01, T02 ... 형식으로 테이블 코드 생성
            string tableCode = $"T{tableNumber:D2}";


            // 현재 테이블 상태 확인
            TableStateData tableData = TableStateStore.Load();

            if (tableData.Tables.ContainsKey(tableCode) &&
                tableData.Tables[tableCode] == "OCCUPIED")
            {
                MessageBox.Show(
                    $"{tableNumber}번 테이블은 이미 사용 중입니다.",
                    "테이블 선택",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }


            // =====================================
            // 기존 키오스크의 매장 주문 세션 시작
            // =====================================
            KioskSession.BeginTableOrder(tableNumber);


            // =====================================
            // 웨이팅 시스템용 테이블 상태
            // 해당 테이블을 사용 중으로 변경
            // =====================================
            TableStateStore.Occupy(tableCode);


            // 메뉴 화면으로 이동
            MenuForm menuForm = new MenuForm();

            menuForm.Show();

            this.Hide();
        }
    }
}
