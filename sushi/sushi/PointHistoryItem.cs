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
    public partial class PointHistoryItem : UserControl
    {
        public PointHistoryItem()
        {
            InitializeComponent();
        }

        public void SetPointData(PointRecord record)
        {
            lbDate.Text = record.PointDate.ToString("yyyy-MM-dd HH:mm");
            lbReason.Text = record.Reason;
            
            if(record.PointChange > 0)
            {
                lbPoints.Text = "+" + record.PointChange.ToString("N0") + "P";
                lbPoints.ForeColor = Color.Blue;
            }
            else if (record.PointChange < 0)
            {
                lbPoints.Text = record.PointChange.ToString("N0") + "P";
                lbPoints.ForeColor = Color.Red;
            }
            else
            {
                lbPoints.Text = "0P";
                lbPoints.ForeColor = Color.Black;
            }
        }
    }
}
