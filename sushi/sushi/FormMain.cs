using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sushi
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            FormSignup signup = new FormSignup();
            signup.ShowDialog();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            using (FormLogin login = new FormLogin())
            {
                if (login.ShowDialog(this) == DialogResult.OK)
                {
                    this.Hide();

                    using (FormHome home = new FormHome())
                    {
                        home.ShowDialog();
                    }

                    this.Close();
                }
            }
        }

        private void btnGuest_Click(object sender, EventArgs e)
        {
            UserSession.MemberId = 0;
            UserSession.Phone = "";
            UserSession.MemberName = "비회원";
            UserSession.DefaultAddress = "";
            UserSession.Point = 0;
            UserSession.JoinDate = "";

            PointStore.Records.Clear();

            this.Hide();

            using (FormHome home = new FormHome())
            {
                home.ShowDialog();
            }

            this.Close();

        }
    }
}
