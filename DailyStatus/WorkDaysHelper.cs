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
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
            {
                throw new ArgumentException($"Last day is before first!\nFirst day: {firstDay}\nLastDay: {lastDay}");
            }

            var span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;

            if (fullWeekCount % 7 != 0)
            {

                int firstDayOfWeek = firstDay.DayOfWeek.GetNumberOfDayOfWeek();
                int lastDayOfWeek = lastDay.DayOfWeek.GetNumberOfDayOfWeek();

                if (lastDayOfWeek < firstDayOfWeek)
                {
                    lastDayOfWeek += 7;
                }

                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount * 2;

            // subtract the number of holidays during the time interval
            foreach (DateTime holiday in holidaysDuringWeek)
            {
                if (firstDay <= holiday && holiday <= lastDay && holiday.DayOfWeek != DayOfWeek.Sunday && holiday.DayOfWeek != DayOfWeek.Saturday)
                {
                    --businessDays;
                }
            }

            return businessDays;
        }

        private static int GetNumberOfDayOfWeek(this DayOfWeek day)
        {
            return day == DayOfWeek.Sunday ? 7 : (int)day;
        }
    }
}
