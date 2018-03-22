namespace DailyStatus
{
    using System;
    using System.Threading;
    using Toggl.Ultrawave;
    using Toggl.Ultrawave.Network;
    using System.Reactive.Linq;
    using System.Linq;
    using System.Xml.Serialization;
    using System.IO;
    using DailyStatus.Security;
    using DailyStatus.ConsoleUtils;
    using DailyStatus.Common;
    using DailyStatus.Common.BLL;
    using DailyStatus.Configuration;
    using DailyStatus.Common.Configuration;
    using DailyStatus.Common.Model;
    using DailyStatus.Common.Extensions;

    public class Program
    {
        private static AppConfig appConfig = null;

        public static void Main(string[] args)
        {
            string key = string.Empty;
            bool authorized = false;
            var togglClient = new TogglProxy();

            var workDay = new DailyStatusConfiguration().GetWorkDayConfig();

            while (!authorized)
            {
                var repo = new WindowsCredentialManager();
                if (repo.Get() != string.Empty)
                {
                    key = repo.Get();
                }
                else
                {
                    Console.WriteLine("Please supply your password");
                    key = ConsolePasswordReader.ReadPassword().ToNormalString();
                }

                togglClient.Configure(key);

                if (togglClient.TestConnection())
                {
                    repo.Save(key);
                    authorized = true;
                }
                else
                {
                    Console.WriteLine("Unauthorized");
                    repo.Save("");
                }
            }
            Console.ResetColor();

            while (true)
            {
                var expected = new WorkDaysCalculator()
                    .ExpectedWorkedDays(TimeSpan.FromHours(workDay.WorkDayStartHour),
                                        workDay.NumberOfWorkingHoursPerDay);

                var sum = togglClient.GetWorkingTime().Result;
                char sign = '-';

                if (sum < expected)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    sign = '+';
                }

                var diff = expected - sum;

                Console.Clear();
                Console.WriteLine($"You should worked:\t{expected.ToWorkingTimeString(workDay.NumberOfWorkingHoursPerDay)}");
                Console.Write($"\rYou worked:\t\t{sum.ToWorkingTimeString(workDay.NumberOfWorkingHoursPerDay)}\tDiff: {sign}{diff.Duration().ToWorkingTimeString(workDay.NumberOfWorkingHoursPerDay).PadRight(20)}");

                using (var progress = new ConsoleProgressBar())
                {
                    const int max = 2;
                    for (int i = 0; i <= 2; i++)
                    {
                        progress.Report((double)i / max);
                        Thread.Sleep(1000);
                    }
                }
            }
        }


    }
}
