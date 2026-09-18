using System;

namespace SushiMobleWaiting
{
    public class WaitingCustomer
    {
        public int WaitingNumber { get; set; }

        public string PhoneNumber { get; set; }

        public int AheadTeamCount { get; set; }

        // 현재 웨이팅 순서
        public int CurrentPosition { get; set; }

        public DateTime RegisteredTime { get; set; }

        public string Status { get; set; }
    }
}