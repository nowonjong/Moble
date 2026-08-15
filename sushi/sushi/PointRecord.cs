using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sushi
{
    public class PointRecord
    {
        public DateTime PointDate { get; set; }
        public string Reason { get; set; } = "";
        public int PointChange { get; set; }
    }

    public static class PointStore
    {
        public static List<PointRecord> Records { get; } = new List<PointRecord>();
        public static int TotalPoints
        {
            get { return UserSession.Point + Records.Sum(record => record.PointChange); }
        }
    }
}
