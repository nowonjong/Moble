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
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();

            tbLoginPassword.UseSystemPasswordChar = true;
            tbLoginPhone.Focus();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string phone = tbLoginPhone.Text.Trim();
            string password = tbLoginPassword.Text;

            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("전화번호를 입력해주세요.");
                tbLoginPhone.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("비밀번호를 입력해주세요.");
                tbLoginPassword.Focus();
                return;
            }

            btnLogin.Enabled = false;

            try
            {
                var requestData = new
                {
                    Action = "LOGIN_MEMBER",
                    Phone = phone,
                    Password = password
                };

                string requestJson = JsonSerializer.Serialize(requestData);
                string responseJson = await TcpClient.SendJsonAsync(requestJson);

                using JsonDocument document = JsonDocument.Parse(responseJson);
                JsonElement response = document.RootElement;

                string status = response.GetProperty("Status").GetString() ?? "";
                string message = response.GetProperty("Message").GetString() ?? "";

                if (status != "SUCCESS")
                {
                    MessageBox.Show($"로그인 실패\n{message}");
                    return;
                }

                UserSession.MemberId = response.GetProperty("MemberId").GetInt32();
                UserSession.MemberName = response.GetProperty("MemberName").GetString() ?? "";
                UserSession.Phone = response.GetProperty("Phone").GetString() ?? "";
                UserSession.Point = response.GetProperty("Point").GetInt32();
                UserSession.DefaultAddress = response.GetProperty("Address").GetString() ?? "";
                UserSession.JoinDate = response.GetProperty("JoinDate").GetString() ?? "";

                PointStore.Records.Clear();

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"관리자 서버 통신 실패\n{ex.Message}");
            }
            finally
            {
                if (!IsDisposed)
                    btnLogin.Enabled = true;
            }
        }

        private void bthCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bool hidePassword = !chbShowPassword.Checked;
            tbLoginPassword.UseSystemPasswordChar = hidePassword;
        }
    }
}
