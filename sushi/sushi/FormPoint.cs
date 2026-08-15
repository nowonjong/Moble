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
    public partial class FormPoint : Form
    {
        public FormPoint()
        {
            InitializeComponent();

            ShowPointHistory();
        }

        private void ShowPointHistory()
        {
            lbTotalPoint.Text = "총 포인트 : " + PointStore.TotalPoints.ToString("N0") + "P";

            flpPoints.Controls.Clear();

            if(PointStore.Records.Count == 0 )
            {
                Label lbEmpty = new Label();

                lbEmpty.Text = "포인트 내역이 없습니다.";
                lbEmpty.AutoSize = true;
                lbEmpty.Margin = new Padding(20);

                flpPoints.Controls.Add(lbEmpty);

                return;
            }
            
            foreach (PointRecord record in PointStore.Records.OrderByDescending(record => record.PointDate))
            {
                PointHistoryItem item = new PointHistoryItem();
                item.SetPointData(record);
                flpPoints.Controls.Add(item);
            }
        }
    }
}
