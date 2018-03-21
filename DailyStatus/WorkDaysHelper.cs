//Code from class below based on solution from https://stackoverflow.com/questions/1617049/calculate-the-number-of-business-days-between-two-dates

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toumash.DailyStatus
{
    public static class WorkDaysHelper
    {
        /// <summary>
        /// Calculates number of business days, taking into account:
        ///  - weekends (Saturdays and Sundays)
        ///  - holidays in the middle of the week
        /// </summary>
        /// <param name="firstDay">First day in the time interval</param>
        /// <param name="lastDay">Last day in the time interval</param>
        /// <param name="holidaysDuringWeek">List of holidays</param>
        /// <returns>Number of business days during the 'span'</returns>
        public static int BusinessDaysUntil(this DateTime firstDay, DateTime lastDay, params DateTime[] holidaysDuringWeek)
        {
            var countOfWorkDays = GetDateRange(firstDay, lastDay)
                .Where(day => day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday)
                .Where(day => !holidaysDuringWeek.Any(h => h.Day == day.Day && h.Month == day.Month && h.Year == day.Year))
                .Count();
            return countOfWorkDays;
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
