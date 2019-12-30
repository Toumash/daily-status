using System;
using System.Collections.Generic;
using System.Linq;

namespace DailyStatus.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static int BusinessDaysUntil(this DateTime firstDay, DateTime lastDay, List<DateTime> holidaysDuringWeek)
        {
            var countOfWorkDays = GetDateRange(firstDay, lastDay)
                .Where(day => day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday)
                .Where(day => !holidaysDuringWeek.Any(h => h.Day == day.Day && h.Month == day.Month && h.Year == day.Year))
                .Count();
            return countOfWorkDays;
        }

        public static DateTime FirstDayOfMonth(this DateTime day)
        {
            return new DateTime(day.Year, day.Month, 1);
        }

        public static IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("endDate must be greater than or equal to startDate");

            while (startDate <= endDate)
            {
                yield return startDate;
                startDate = startDate.AddDays(1);
            }
        }
    }
}
