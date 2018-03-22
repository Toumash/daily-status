using System;
using System.Collections.Generic;
using System.Linq;
using DailyStatus.Common.Extensions;

namespace DailyStatus.Common.BLL
{
    public class WorkDaysCalculator
    {
        public TimeSpan ExpectedWorkedDays(TimeSpan workDayStartHour, double numberOfWorkingHoursPerDay, params DateTime[] holidaysDuringWeek)
        {
            var today = DateTime.Today;
            var first = new DateTime(today.Year, today.Month, 1);
            var workDayStart = today + workDayStartHour;

            var worktime = TimeSpan.FromHours(first.BusinessDaysUntil(today, holidaysDuringWeek) * numberOfWorkingHoursPerDay);

            if (today.DayOfWeek != DayOfWeek.Saturday || today.DayOfWeek != DayOfWeek.Sunday || !holidaysDuringWeek.Contains(today))
            {
                worktime -= TimeSpan.FromHours(numberOfWorkingHoursPerDay);
                var diff = (DateTime.Now - workDayStart).TotalHours;

                diff = Math.Min(8d, Math.Max(0d, diff));

                worktime += TimeSpan.FromHours(diff);
            }

            return worktime;
        }
    }

}
