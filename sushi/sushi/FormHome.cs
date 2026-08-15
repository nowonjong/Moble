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
    public partial class FormHome : Form
    {
        private Form? currentPage;
        public FormHome()
        {
            InitializeComponent();

            if(UserSession.IsLoggedIn)
            {
                lbMember.Text = UserSession.MemberName;
            }
            else
            {
                lbMember.Text = "비회원";
            }

                ShowHomePage();
        }

        private void ShowPage(Form page, Button selectedButton)
        {
            RemoveCurrentPage();

            pnlHome.Visible = false;

            currentPage = page;

            page.TopLevel = false;
            page.FormBorderStyle = FormBorderStyle.None;
            page.Dock = DockStyle.Fill;

            pnlContent.Controls.Add(page);

            page.BringToFront();
            page.Show();

            SetSelectedButton(selectedButton);
        }

        private void ShowHomePage()
        {
            RemoveCurrentPage();
            pnlHome.Visible = true;
            pnlHome.BringToFront();

            SetSelectedButton(btnHome);
        }

        private void RemoveCurrentPage()
        {
            if (currentPage == null)
            {
                return;
            }
            pnlContent.Controls.Remove(currentPage);
            currentPage.Dispose();
            currentPage = null;
        }

        private void SetSelectedButton(Button selectedButton)
        {
            btnHome.Enabled = true;
            btnOrder.Enabled = true;

            btnPoint.Enabled = UserSession.IsLoggedIn;

            selectedButton.Enabled = false;
        }
        private void btnDelivery_Click(object sender, EventArgs e)
        {
            FormMenu formMenu = new FormMenu("배달");
            formMenu.ShowDialog();
        }

        private void btnPickup_Click(object sender, EventArgs e)
        {
            FormMenu formMenu = new FormMenu("포장");
            formMenu.ShowDialog();
        }

        private void btnPoint_Click(object sender, EventArgs e)
        {
            FormPoint formPoint = new FormPoint();
            ShowPage(formPoint, btnPoint);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            ShowHomePage();
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            FormOrder formOrder = new FormOrder();

            ShowPage(formOrder, btnOrder);
        }
    }
}
