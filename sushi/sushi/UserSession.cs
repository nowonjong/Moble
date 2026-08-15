using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sushi
{
    public static class UserSession
    {
        public static int MemberId { get; set; } = 0;
        public static string Phone { get; set; } = "";
        public static string MemberName { get; set; } = "";
        public static string DefaultAddress { get; set; } = "";
        public static int Point { get; set; } = 0;
        public static string JoinDate { get; set; } = "";
        public static bool IsLoggedIn
        {
            get { return MemberId > 0; }
        }
        public static void Logout()
        {
            MemberId = 0;
            MemberName = "";
            Phone = "";
            DefaultAddress = "";
            Point = 0;
            JoinDate = "";
        }
    }
}
