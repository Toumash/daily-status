namespace DailyStatus
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
                return new WorkDay(8, 8d);
            }
        }
    }
}
