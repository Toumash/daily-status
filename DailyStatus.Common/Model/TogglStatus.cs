using System;

namespace DailyStatus.Common.Model
{
    public class TogglStatus
    {
        public TimeSpan TimeInMonth { get; set; }

        public bool IsTimerActive { get; set; }
        public TimeSpan TodaysHours { get; set; }
    }
}
