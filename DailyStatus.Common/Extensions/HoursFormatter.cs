﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStatus.Common.Extensions
{
    public static class HoursFormatter
    {
        public static string ToWorkingTimeString(this TimeSpan workTime, int workingHoursPerDay)
        {
            return $"{Math.Truncate(workTime.TotalHours / workingHoursPerDay)}.{workTime.Hours % workingHoursPerDay}:{workTime.Minutes}:{workTime.Seconds}";
        }
    }
}
