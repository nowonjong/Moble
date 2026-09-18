using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SushiMobleWaiting
{
    public partial class WaitingForm : Form
    {
        private string phoneInput = "";
        private int nextWaitingNumber = 1;
        private int waitingTeamCount = 0;

        private List<WaitingCustomer> waitingCustomers = new List<WaitingCustomer>();

        private SmsService smsService = new SmsService();

        private long lastReleaseSequence = 0;

        // 대기열 변경 처리 중 중복 실행 방지
        private bool queueAdvanceRunning = false;

        // 테이블 상태를 주기적으로 확인할 타이머
        private System.Windows.Forms.Timer tableStateTimer;

        public WaitingForm()
        {
            InitializeComponent();

            btn0.Click += NumberButton_Click;
            btn1.Click += NumberButton_Click;
            btn2.Click += NumberButton_Click;
            btn3.Click += NumberButton_Click;
            btn4.Click += NumberButton_Click;
            btn5.Click += NumberButton_Click;
            btn6.Click += NumberButton_Click;
            btn7.Click += NumberButton_Click;
            btn8.Click += NumberButton_Click;
            btn9.Click += NumberButton_Click;

            // 지우기 버튼
            btnClear.Click += BtnClear_Click;

            // 한 글자 삭제 버튼
            btnBack.Click += BtnBack_Click;

            btnRegisterWaiting.Click += BtnRegisterWaiting_Click;

            // 웨이팅 프로그램을 켠 시점의 ReleaseSequence를 기준값으로 저장
            lastReleaseSequence = TableStateReader.GetReleaseSequence();

            // 1초마다 테이블 상태 확인
            tableStateTimer = new System.Windows.Forms.Timer();

            tableStateTimer.Interval = 1000;

            tableStateTimer.Tick += TableStateTimer_Tick;

            tableStateTimer.Start();

            // 처음 전화번호 표시
            UpdatePhoneDisplay();

            // 처음 테이블 상태 표시
            UpdateTableStatus();
        }

        private async void TableStateTimer_Tick(object sender, EventArgs e)
        {
            // 현재 빈 테이블 수를 화면에 갱신
            UpdateTableStatus();

            // 이미 대기열 처리 중이면 중복 실행하지 않음
            if (queueAdvanceRunning)
                return;

            long currentReleaseSequence =
                TableStateReader.GetReleaseSequence();

            // 새로 비워진 테이블이 없으면 종료
            if (currentReleaseSequence <= lastReleaseSequence)
                return;

            long releasedCount =
                currentReleaseSequence - lastReleaseSequence;

            // 동일한 ReleaseSequence 중복 처리 방지
            lastReleaseSequence =
                currentReleaseSequence;

            queueAdvanceRunning = true;

            try
            {
                // 새로 비워진 테이블 수만큼 대기열 진행
                for (long i = 0; i < releasedCount; i++)
                {
                    await AdvanceWaitingQueueAsync();
                }
            }
            finally
            {
                queueAdvanceRunning = false;
            }
        }

        private async Task AdvanceWaitingQueueAsync()
        {
            // 현재 Waiting 상태인 고객을 순서대로 가져옴
            List<WaitingCustomer> activeCustomers =
                waitingCustomers
                .Where(c => c.Status == "Waiting")
                .OrderBy(c => c.CurrentPosition)
                .ToList();

            // 대기 중인 고객이 없으면 아무것도 하지 않음
            if (activeCustomers.Count == 0)
                return;


            // =====================================
            // 현재 1번째 고객 → 입장 대상
            // =====================================
            WaitingCustomer enteringCustomer =
                activeCustomers[0];

            string entranceMessage =
                $"[스시 모블] 대기 {enteringCustomer.WaitingNumber}번 고객님, " +
                $"입장 순서입니다. 매장으로 입장해주세요.";

            try
            {
                bool entranceSmsSuccess = await smsService.SendSmsAsync(
                    enteringCustomer.PhoneNumber, 
                    entranceMessage);

                if (!entranceSmsSuccess)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[입장 문자 실패] 대기번호 {enteringCustomer.WaitingNumber}"
                    );

                    return;
                }

                // 문자 발송 성공 후 입장 처리
                enteringCustomer.Status = "Entered";
                enteringCustomer.CurrentPosition = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "[입장 문자 오류] " + ex.Message
                );

                return;
            }


            // =====================================
            // 나머지 고객 순서 1칸씩 앞으로 이동
            // =====================================
            List<WaitingCustomer> remainingCustomers =
                waitingCustomers
                .Where(c => c.Status == "Waiting")
                .OrderBy(c => c.CurrentPosition)
                .ToList();

            foreach (WaitingCustomer customer in remainingCustomers)
            {
                customer.CurrentPosition--;

                customer.AheadTeamCount =
                    customer.CurrentPosition - 1;


                // 실제로 순서가 바뀐 경우만 SMS
                    string positionMessage =
                        $"[스시 모블] 대기 순서가 변경되었습니다. " +
                        $"현재 {customer.CurrentPosition}번째 순서입니다.";

                    try
                    {
                        await smsService.SendSmsAsync(
                            customer.PhoneNumber,
                            positionMessage
                        );
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[순서 변경 문자 오류] " +
                            $"{customer.WaitingNumber}번 - {ex.Message}"
                        );
                    }
                }


            // =====================================
            // 현재 웨이팅 팀 수 화면 갱신
            // =====================================
            waitingTeamCount =
                waitingCustomers.Count(c => c.Status == "Waiting");

            lblWaitingCount.Text =
                waitingTeamCount + "팀";
        }

        private void NumberButton_Click(object sender, EventArgs e)
        {
            // 이미 8자리를 입력했다면 더 이상 입력하지 않음
            if (phoneInput.Length >= 8)
                return;

            Button? button = sender as Button;

            if (button == null)
                return;

            // 누른 버튼의 숫자를 저장
            phoneInput += button.Text;

            // 화면 갱신
            UpdatePhoneDisplay();
        }

        private void UpdatePhoneDisplay()
        {
            // 아직 입력하지 않은 부분은 _ 로 표시
            string numbers = phoneInput.PadRight(8, '_');

            string firstPart = numbers.Substring(0, 4);
            string secondPart = numbers.Substring(4, 4);

            lblPhoneNumber.Text = $"010 - {firstPart} - {secondPart}";
        }

        private void UpdateTableStatus()
        {
            int availableCount =
                TableStateReader.GetAvailableCount();

            if (availableCount == 0)
            {
                lblTableStatus.Text = "현재 좌석 : 만석";
            }
            else
            {
                lblTableStatus.Text =
                    $"빈 테이블 : {availableCount}개";
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            // 입력한 숫자를 전부 삭제
            phoneInput = "";

            UpdatePhoneDisplay();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            // 입력된 숫자가 없으면 아무것도 하지 않음
            if (phoneInput.Length == 0)
                return;

            // 마지막 숫자 하나 삭제
            phoneInput = phoneInput.Substring(0, phoneInput.Length - 1);

            UpdatePhoneDisplay();
        }

        private async void BtnRegisterWaiting_Click(object sender, EventArgs e)
        {
            // 매장이 만석인지 먼저 확인
            if (!TableStateReader.IsFull())
            {
                int availableCount =
                    TableStateReader.GetAvailableCount();

                MessageBox.Show(
                    $"현재 이용 가능한 테이블이 {availableCount}개 있습니다.\n\n" +
                    $"웨이팅 등록 없이 매장을 이용해주세요.",
                    "빈 테이블 있음",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }


            // 휴대폰 뒤 8자리를 모두 입력했는지 확인
            if (phoneInput.Length != 8)
            {
                MessageBox.Show(
                    "휴대폰 번호를 정확히 입력해주세요.",
                    "입력 확인",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            // 010 + 사용자가 입력한 뒤 8자리
            string phoneNumber = "010" + phoneInput;

            // 이미 웨이팅 중인 전화번호인지 확인
            bool alreadyWaiting =
                waitingCustomers.Any(c =>
                    c.Status == "Waiting" &&
                    c.PhoneNumber == phoneNumber);

            if (alreadyWaiting)
            {
                MessageBox.Show(
                    "이미 웨이팅 등록된 휴대폰 번호입니다.",
                    "중복 등록",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            // 현재 앞에 대기하고 있는 팀 수
            int aheadTeamCount = waitingCustomers.Count(c => c.Status == "Waiting");

            // 새로운 고객 정보 생성
            WaitingCustomer customer = new WaitingCustomer();

            customer.WaitingNumber = nextWaitingNumber;
            customer.PhoneNumber = phoneNumber;
            customer.AheadTeamCount = aheadTeamCount;
            customer.CurrentPosition = aheadTeamCount + 1;
            customer.RegisteredTime = DateTime.Now;
            customer.Status = "Waiting";

            // 대기 목록에 저장
            waitingCustomers.Add(customer);

            // 현재 웨이팅 팀 수 갱신
            waitingTeamCount = waitingCustomers.Count(c => c.Status == "Waiting");

            // 왼쪽 화면의 웨이팅 팀 수 변경
            lblWaitingCount.Text = waitingTeamCount + "팀";


            // ---------------------------------
            // 고객에게 발송할 문자 내용
            // ---------------------------------
            string smsMessage =
                $"[스시 모블] 대기 {customer.WaitingNumber}번 접수완료. " +
                $"앞 대기 {customer.AheadTeamCount}팀, " +
                $"현재 {customer.CurrentPosition}번째 순서입니다.";


            bool smsSuccess = false;

            try
            {
                // 중복 클릭 방지
                btnRegisterWaiting.Enabled = false;

                // 실제 문자 발송
                smsSuccess = await smsService.SendSmsAsync(
                    customer.PhoneNumber,
                    smsMessage
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "문자 발송 중 오류가 발생했습니다.\n\n" +
                    ex.Message,
                    "SMS 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnRegisterWaiting.Enabled = true;
            }


            // 문자 발송 성공
            if (smsSuccess)
            {
                MessageBox.Show(
                    $"웨이팅 등록이 완료되었습니다.\n\n" +
                    $"대기번호 : {customer.WaitingNumber}번\n" +
                    $"앞 대기팀 : {customer.AheadTeamCount}팀\n" +
                    $"현재 순서 : {customer.CurrentPosition}번째\n\n" +
                    $"등록한 휴대폰으로 문자를 발송했습니다.",
                    "웨이팅 등록 완료",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            // 문자 발송 실패
            else
            {
                MessageBox.Show(
                    $"웨이팅은 등록되었지만\n문자 발송에 실패했습니다.\n\n" +
                    $"대기번호 : {customer.WaitingNumber}번",
                    "문자 발송 실패",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }


            // 다음 고객을 위해 대기번호 증가
            nextWaitingNumber++;

            // 전화번호 입력창 초기화
            phoneInput = "";

            UpdatePhoneDisplay();
        }
    }
}
