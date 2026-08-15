using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private TcpClient client;

        private StreamReader reader;
        private StreamWriter writer;

        private bool isClosing = false;

        // ★ 컴퓨터 1의 IPv4 주소
        private const string SERVER_IP =
            "192.168.0.92";

        private const int SERVER_PORT = 5000;

        public Form1()
        {
            InitializeComponent();

            // 폼 실행 → 자동 연결
            this.Shown += Form1_Shown;

            this.FormClosing += Form1_FormClosing;
        }

        // =========================================
        // 폼이 뜨면 자동 연결
        // =========================================
        private async void Form1_Shown(
            object sender,
            EventArgs e)
        {
            await ConnectLoop();
        }

        // =========================================
        // 컴퓨터 1에 자동 연결
        // =========================================
        private async Task ConnectLoop()
        {
            while (!isClosing)
            {
                try
                {
                    AddLog(
                        "서버 연결 시도 중..."
                    );

                    client = new TcpClient();

                    await client.ConnectAsync(
                        SERVER_IP,
                        SERVER_PORT
                    );

                    AddLog(
                        "컴퓨터 1과 연결되었습니다."
                    );

                    NetworkStream stream =
                        client.GetStream();

                    reader = new StreamReader(
                        stream,
                        Encoding.UTF8
                    );

                    writer = new StreamWriter(
                        stream,
                        Encoding.UTF8
                    );

                    writer.AutoFlush = true;

                    // 관리자 메시지 수신
                    await ReceiveMessage();
                }
                catch (Exception)
                {
                    if (!isClosing)
                    {
                        AddLog(
                            "서버에 연결할 수 없습니다."
                        );

                        AddLog(
                            "2초 후 다시 연결합니다..."
                        );
                    }
                }

                // 연결 정리
                try
                {
                    reader?.Close();
                    writer?.Close();
                    client?.Close();
                }
                catch
                {
                }

                reader = null;
                writer = null;
                client = null;

                // 프로그램 종료 중이 아니라면 재접속
                if (!isClosing)
                {
                    await Task.Delay(2000);
                }
            }
        }

        // =========================================
        // 컴퓨터1 메시지 받기
        // =========================================
        private async Task ReceiveMessage()
        {
            try
            {
                while (!isClosing)
                {
                    string message =
                        await reader.ReadLineAsync();

                    if (message == null)
                    {
                        break;
                    }

                    AddLog(
                        "관리자 : " + message
                    );
                }
            }
            catch
            {
            }

            if (!isClosing)
            {
                AddLog(
                    "서버와 연결이 끊어졌습니다."
                );
            }
        }

        // =========================================
        // 컴퓨터1에 메시지 전송
        // =========================================
        private async void btnSend_Click(
            object sender,
            EventArgs e)
        {
            if (writer == null)
            {
                MessageBox.Show(
                    "서버에 연결되어 있지 않습니다."
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(
                txtMessage.Text))
            {
                return;
            }

            try
            {
                string message =
                    txtMessage.Text;

                await writer.WriteLineAsync(
                    message
                );

                AddLog(
                    "나 : " + message
                );

                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "전송 실패 : " +
                    ex.Message
                );
            }
        }

        // =========================================
        // 로그
        // =========================================
        private void AddLog(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(
                    new Action(() =>
                    {
                        txtLog.AppendText(
                            message +
                            Environment.NewLine
                        );
                    })
                );
            }
            else
            {
                txtLog.AppendText(
                    message +
                    Environment.NewLine
                );
            }
        }

        // =========================================
        // 종료
        // =========================================
        private void Form1_FormClosing(
            object sender,
            FormClosingEventArgs e)
        {
            isClosing = true;

            try
            {
                reader?.Close();
                writer?.Close();
                client?.Close();
            }
            catch
            {
            }
        }
    }
}