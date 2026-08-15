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
    public partial class FormSignup : Form
    {
        public FormSignup()
        {
            InitializeComponent();

            tbPassword.UseSystemPasswordChar = true;
            tbPasswordConfirm.UseSystemPasswordChar = true;
        }

        private void chbShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            bool hidePassword = !chbShowPassword.Checked;

            tbPassword.UseSystemPasswordChar = hidePassword;
            tbPasswordConfirm.UseSystemPasswordChar = hidePassword;
        }

        private async void btnSignup_Click(object sender, EventArgs e)
        {
            string name = tbName.Text.Trim();
            string password = tbPassword.Text;
            string passWordConfirm = tbPasswordConfirm.Text;
            string phone = tbPhone.Text.Trim();
            string address = tbAddress.Text.Trim();

            if(string. IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("회원명을 입력해주세요.");

                tbName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("비밀번호를 입력해주세요.");
                return;
            }

            if (string.IsNullOrWhiteSpace(passWordConfirm))
            {
                MessageBox.Show("비밀번호 확인을 입력해주세요.");
                return;
            }

            if (password != passWordConfirm)
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다.");
                return;
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("연락처를 입력해주세요.");
                return;
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("주소를 입력해주세요.");
                return;
            }

            if (!chbPrivacyAgree.Checked)
            {
                MessageBox.Show("개인정보 수집•이용에 동의해주세요.");
                chbPrivacyAgree.Focus();
                return;
            }

            btnSignup.Enabled = false;

            try
            {
                var requestData = new
                {
                    Action = "REGISTER_MEMBER",
                    MemberName = name,
                    Phone = phone,
                    Password = password,
                    Address = address
                };

                string requestJson = JsonSerializer.Serialize(requestData);
                string responseJson = await TcpClient.SendJsonAsync(requestJson);

                using JsonDocument document = JsonDocument.Parse(responseJson);
                JsonElement response = document.RootElement;

                string status = response.GetProperty("Status").GetString() ?? "";
                string message = response.GetProperty("Message").GetString() ?? "";

                if (status == "SUCCESS")
                {
                    int memberId = response.GetProperty("MemberId").GetInt32();

                    MessageBox.Show($"회원가입이 완료됐습니다!\n회원번호: {memberId}");
                    Close();
                }
                else
                {
                    MessageBox.Show($"회원가입 실패\n{message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"관리자 서버 통신 실패\n{ex.Message}");
            }
            finally
            {
                if (!IsDisposed)
                    btnSignup.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
