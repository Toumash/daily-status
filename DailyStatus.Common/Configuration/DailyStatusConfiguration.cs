using System;
using System.Collections.Generic;

namespace DailyStatus.Common.Configuration
{
    public enum DisplayType
    {
        Time,
        Money
    }

    public class DailyStatusConfiguration
    {
        public DisplayType DisplayType { get; set; }
        public decimal HourRate { get; set; }

        public int WorkDayStartHour { get; set; }
        public int HoursADay { get; set; }
        public long WorkspaceId { get; set; }
        public List<DateTime> Holidays { get; set; }
        public DateTime SumSince { get; set; }
    }
}
