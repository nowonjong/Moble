using AxWMPLib;
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
    public partial class Payment : Form
    {
        private readonly Pop_MemberNum? memberForm;
        private bool paymentRunning;
        // 다국어 텍스트 정의 (0: 영어, 1: 일본어, 2: 한국어)
        private readonly string[] label4Texts = { "Please select a payment method!", "お支払い方法を選択してください！", "결제 방식을 선택해주세요 !" };
        private readonly string[] label2Texts = { "Payment Method", "お支払い方法", "결제 방식" };
        private readonly string[] label3Texts = { "Voucher Payment", "商品券決済", "상품권 결제" };
        private readonly string[] cardTexts = { "Card Payment", "カード決済", "카드 결제" };
        private readonly string[] naverTexts = { "Naver Pay", "ネイバーペイ", "네이버 페이" };
        private readonly string[] kakaoTexts = { "Kakao Pay", "カカオペイ", "카카오 페이" };
        private readonly string[] samsungTexts = { "Samsung Pay", "サムスンペイ", "삼성페이" };
        private readonly string[] couponTexts = { "Coupon / Voucher", "クーポン・商品券", "쿠폰 / 상품권 복합 결제" };
        private readonly string[] allDeleteTexts = { "Cancel All", "すべて取消", "전체 취소" };
        private readonly string[] backTexts = { "Back", "戻る", "이전" };

        // 회원 및 적립 관련 다국어 텍스트 정의 (0: 영어, 1: 일본어, 2: 한국어)
        private readonly string[] cusIdTexts = { "Member ID", "会員番号", "회원번호" };
        private readonly string[] phonenumTexts = { "Phone Number", "携帯電話番号", "휴대폰 번호" };
        private readonly string[] sumTexts = { "Total Amount", "対象金額", "대상금액" };
        private readonly string[] cusNameTexts = { "Member Name", "会員名", "회원명" };
        private readonly string[] savePointTexts = { "Remaining Points", "残高ポイント", "잔여 포인트" };
        private readonly string[] del2Texts = { "Delete", "削除", "삭제" };
        private readonly string[] button32Texts = { "Cancel", "キャンセル", "취소" };
        private readonly string[] savepointTexts = { "Earn Points", "ポイント積立", "포인트 적립" };
        private readonly string[] receiveTexts = { "Earn Complete", "積立完了", "적립 완료" };

        // 동적으로 추가될 수 있는 회원/적립 관련 예비 컨트롤 필드 선언 (컴파일 오류 방지)
        private Label lb_cusId;
        private Label lb_phonenum;
        private Label lb_sum;
        private Label lb_cusName;
        private Label lb_savePoint;
        private Button btn_del2;
        private Button button32;
        private Button btn_savepoint;
        private Button btn_receive;

        public Payment() : this(null)
        {
        }

        public Payment(Pop_MemberNum? memberForm)
        {
            this.memberForm = memberForm;
            InitializeComponent();
            

            axWindowsMediaPlayer1.URL = System.IO.Path.Combine(Application.StartupPath, "Images", "스시결제.mp4");
            // 2. 아래 하단 바(컨트롤 레이아웃) 숨기기 ("none"으로 설정 시 영상만 출력)
            axWindowsMediaPlayer1.uiMode = "none";

            // 3. 무한 반복 재생 설정 ("loop" 모드를 true로 지정)
            axWindowsMediaPlayer1.settings.setMode("loop", true);

            btn_card.Click += async (s, e) => await ProcessPaymentAsync("신용카드");
            btn_naverPay.Click += async (s, e) => await ProcessPaymentAsync("네이버페이");
            btn_KakaoPay.Click += async (s, e) => await ProcessPaymentAsync("카카오페이");
            btn_SamsungPay.Click += async (s, e) => await ProcessPaymentAsync("삼성페이");
            btn_coupon.Click += async (s, e) => await ProcessPaymentAsync("쿠폰/상품권");
            btn_allDelete.Click += CancelAll_Click;

            // 언어 변경 이벤트 구독
            LanguageManager.LanguageChanged += ApplyLanguage;

            // 폼이 닫힐 때 이벤트 구독 해제 (메모리 누수 방지)
            this.FormClosed += (s, e) =>
            {
                LanguageManager.LanguageChanged -= ApplyLanguage;
            };

            // 최초 1회 현재 언어 적용
            ApplyLanguage();
            label1.Text = $"결제 대상 금액: {KioskSession.OriginalAmount:N0}원";
        }

        private void ApplyLanguage()
        {
            int langIndex = LanguageManager.CurrentLanguageIndex;

            if (label4 != null) label4.Text = label4Texts[langIndex];
            if (label2 != null) label2.Text = label2Texts[langIndex];
            if (label3 != null) label3.Text = label3Texts[langIndex];
            if (btn_card != null) btn_card.Text = cardTexts[langIndex];
            if (btn_naverPay != null) btn_naverPay.Text = naverTexts[langIndex];
            if (btn_KakaoPay != null) btn_KakaoPay.Text = kakaoTexts[langIndex];
            if (btn_SamsungPay != null) btn_SamsungPay.Text = samsungTexts[langIndex];
            if (btn_coupon != null) btn_coupon.Text = couponTexts[langIndex];
            if (btn_allDelete != null) btn_allDelete.Text = allDeleteTexts[langIndex];
            if (btn_back != null) btn_back.Text = backTexts[langIndex];

            // 회원 및 적립 관련 컨트롤 다국어 바인딩 (직접 조건문 검사)
            if (lb_cusId != null) lb_cusId.Text = cusIdTexts[langIndex];
            if (lb_phonenum != null) lb_phonenum.Text = phonenumTexts[langIndex];
            if (lb_sum != null) lb_sum.Text = sumTexts[langIndex];
            if (lb_cusName != null) lb_cusName.Text = cusNameTexts[langIndex];
            if (lb_savePoint != null) lb_savePoint.Text = savePointTexts[langIndex];
            if (btn_del2 != null) btn_del2.Text = del2Texts[langIndex];
            if (button32 != null) button32.Text = button32Texts[langIndex];
            if (btn_savepoint != null) btn_savepoint.Text = savepointTexts[langIndex];
            if (btn_receive != null) btn_receive.Text = receiveTexts[langIndex];
        }

        private void Payment_Load(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            Pop_MemberNum memeber = memberForm ?? new Pop_MemberNum();
            memeber.Show();
            this.Hide();
        }

        private async Task ProcessPaymentAsync(string paymentMethod)
        {
            if (paymentRunning)
                return;
            if (!KioskSession.HasOrders)
            {
                MessageBox.Show("결제할 주문이 없습니다.");
                return;
            }

            int usedPoint = 0;
            MemberResponse? member = KioskSession.Member;
            if (member != null && member.Point > 0)
            {
                using PointUseDialog dialog = new(member.Point, KioskSession.OriginalAmount);
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                usedPoint = dialog.UsedPoint;
            }

            int finalAmount = KioskSession.OriginalAmount - usedPoint;
            DialogResult confirmed = MessageBox.Show(
                $"{paymentMethod} 결제를 진행하시겠습니까?\n\n결제 금액: {finalAmount:N0}원",
                "결제 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirmed != DialogResult.Yes)
                return;

            paymentRunning = true;
            SetPaymentButtonsEnabled(false);
            try
            {
                PaymentResponse response = await KioskSession.Server.CompletePaymentAsync(
                    KioskSession.GetPaymentIdentifier(),
                    member?.MemberId ?? 0,
                    KioskSession.OriginalAmount,
                    usedPoint,
                    paymentMethod);

                if (!response.IsSuccess)
                {
                    MessageBox.Show(
                        response.Message,
                        "결제 처리 실패",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }


                // ==========================================
                // 매장 주문이면 현재 테이블을 빈자리로 변경
                // ==========================================
                if (!KioskSession.IsTakeout &&
                    KioskSession.TableNumber.HasValue)
                {
                    int tableNumber = KioskSession.TableNumber.Value;

                    string tableCode = $"T{tableNumber:00}";

                    TableStateStore.Release(tableCode);
                }


                MessageBox.Show(
                    $"결제가 완료되었습니다.\n\n" +
                    $"영수증 번호: {response.ReceiptNo}\n" +
                    $"결제 금액: {response.TotalAmount:N0}원\n" +
                    $"적립 포인트: {response.EarnedPoint:N0}P",
                    "결제 완료",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);


                // ★ 반드시 테이블 해제보다 나중에 실행
                KioskSession.Reset();

                ReturnToStart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "결제 결과 전송 중 오류가 발생했습니다. 관리자에서 결제 처리 여부를 반드시 확인해주세요.\n\n" + ex.Message,
                    "결제 통신 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                paymentRunning = false;
                if (!IsDisposed)
                    SetPaymentButtonsEnabled(true);
            }
        }

        private void ReleaseCurrentTable()
        {
        
            if (MessageBox.Show(
                "현재 주문을 모두 취소하고 처음 화면으로 이동할까요?",
                "전체 취소",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            // 매장 주문이었다면 테이블도 해제
            ReleaseCurrentTable();

            KioskSession.Reset();

            ReturnToStart();
        
        }

        private void CancelAll_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("현재 주문을 모두 취소하고 처음 화면으로 이동할까요?", "전체 취소",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            KioskSession.Reset();
            ReturnToStart();
        }

        private void SetPaymentButtonsEnabled(bool enabled)
        {
            btn_card.Enabled = enabled;
            btn_naverPay.Enabled = enabled;
            btn_KakaoPay.Enabled = enabled;
            btn_SamsungPay.Enabled = enabled;
            btn_coupon.Enabled = enabled;
            btn_allDelete.Enabled = enabled;
            btn_back.Enabled = enabled;
        }

        private void ReturnToStart()
        {
            Firstform start = Application.OpenForms.OfType<Firstform>().FirstOrDefault() ?? new Firstform();
            start.Show();
            start.BringToFront();

            foreach (Form form in Application.OpenForms.Cast<Form>().ToArray())
            {
                if (!ReferenceEquals(form, start))
                    form.Close();
            }
        }
    }
}
