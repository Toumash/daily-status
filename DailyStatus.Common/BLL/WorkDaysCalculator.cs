using System;
using System.Collections.Generic;
using System.Linq;
using DailyStatus.Common.Extensions;

namespace DailyStatus.Common.BLL
{
    public class WorkDaysCalculator
    {
        public TimeSpan MonthExpectedHours(DateTime todayWithHours, TimeSpan workDayStartHour, double numberOfWorkingHoursPerDay, params DateTime[] holidays)
        {
            return TimeExpectedHours(new DateTime(todayWithHours.Year, todayWithHours.Month, 1), todayWithHours, workDayStartHour, numberOfWorkingHoursPerDay, holidays);
        }
        public TimeSpan TimeExpectedHours(DateTime since, DateTime todayWithHours, TimeSpan workDayStartHour, double numberOfWorkingHoursPerDay, params DateTime[] holidays)
        {
            var today = todayWithHours.Date;
            var first = since;
            var workDayStart = today + workDayStartHour;

            var worktime = TimeSpan.FromHours(first.BusinessDaysUntil(today, holidays) * numberOfWorkingHoursPerDay);

            if (today.DayOfWeek != DayOfWeek.Saturday && today.DayOfWeek != DayOfWeek.Sunday && !holidays.Contains(today))
            {
                worktime -= TimeSpan.FromHours(numberOfWorkingHoursPerDay);
                var diff = (todayWithHours - workDayStart).TotalHours;

                diff = Math.Min(numberOfWorkingHoursPerDay, Math.Max(0d, diff));

                worktime += TimeSpan.FromHours(diff);
            }

            return worktime;
        }

        public TimeSpan ExpectedWorkedDays(TimeSpan workDayStartHour, double numberOfWorkingHoursPerDay, params DateTime[] holidaysDuringWeek)
        {
            return MonthExpectedHours(DateTime.Now, workDayStartHour, numberOfWorkingHoursPerDay, holidaysDuringWeek);
        }

        internal TimeSpan ExpectedWorkedDaysSince(DateTime since, TimeSpan workDayStartHour, double numberOfWorkingHoursPerDay, params DateTime[] holidaysDuringWeek)
        {
            return TimeExpectedHours(since, DateTime.Now, workDayStartHour, numberOfWorkingHoursPerDay, holidaysDuringWeek);
        }
    }

}
