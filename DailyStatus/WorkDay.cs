using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toumash.DailyStatus
{
    struct WorkDay
    {
        public int NumberOfWorkingHoursPerDay { get; set; }
        public double WorkDayStartHour { get; set; }

        public WorkDay(int numberOfWorkingHoursPerDay, double workDayStartHour)
        {
            NumberOfWorkingHoursPerDay = numberOfWorkingHoursPerDay;
            WorkDayStartHour = workDayStartHour;
        }

        public static WorkDay StandardFullTime
        {
            get
            {
                return new WorkDay(8, 10d);
            }
        }
    }
}
