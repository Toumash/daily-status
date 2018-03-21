using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Toumash.DailyStatus
{
    public class WorkDay
    {
        public int NumberOfWorkingHoursPerDay { get; set; }
        public double WorkDayStartHour { get; set; }

        public WorkDay(int numberOfWorkingHoursPerDay, double workDayStartHour)
        {
            NumberOfWorkingHoursPerDay = numberOfWorkingHoursPerDay;
            WorkDayStartHour = workDayStartHour;
        }

        public WorkDay() : this(StandardFullTime.NumberOfWorkingHoursPerDay, StandardFullTime.WorkDayStartHour)
        { }

        public static WorkDay StandardFullTime
        {
            get
            {
                return new WorkDay(8, 10d);
            }
        }
    }
}
