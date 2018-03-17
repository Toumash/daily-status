namespace Toumash.DailyStatus
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using Toggl.Ultrawave;
    using Toggl.Ultrawave.Network;
    using System.Reactive.Linq;

    public class Program
    {
        const int NumberOfWorkingHoursPerDay = 8;

        public static void Main(string[] args)
        {
            string key = string.Empty;
            bool authorized = false;
            ITogglApi togglApi = null;

            while (!authorized)
            {
                var repo = new ApiKeyRepository();
                if (repo.Get() != string.Empty)
                {
                    key = repo.Get();
                }
                else
                {
                    Console.WriteLine("Please supply your password");
                    key = SecureStringToString(Password);
                }

                var credentials = Credentials.WithApiToken(key);
                togglApi = TogglApiWith(credentials);
                try
                {
                    var user = togglApi.User.Get().GetAwaiter().Wait();
                    repo.Save(key);
                    authorized = true;
                }
                catch (Toggl.Ultrawave.Exceptions.UnauthorizedException)
                {
                    Console.WriteLine("Unauthorized");
                    repo.Save("");
                }
            }

            var expected = ExpectedWorkedDays();
            Console.WriteLine($"You should worked:\t{WorkingTimeToString(expected)}");

            while (true)
            {
                var sum = GetWorkingTime(togglApi);
                char sign = '-';
                
                if(sum < expected)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    sign = '+';
                }

                var diff = expected - sum;

                Console.Write($"\rYou worked:\t\t{WorkingTimeToString(sum)}\tDiff: {sign}{WorkingTimeToString(diff.Duration()).PadRight(20)}");

                using (var progress = new ProgressBar())
                {
                    const int max = 5;
                    for (int i = 0; i <= 5; i++)
                    {
                        progress.Report((double)i / max);
                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private static TimeSpan GetWorkingTime(ITogglApi togglApi)
        {
            var today = DateTime.Today;
            var offset = new DateTimeOffset(new DateTime(today.Year, today.Month, 1));
            var sum = togglApi.TimeEntries.GetAllSince(offset)
                .SelectMany(e => e)
                .Where(e => e.Duration.HasValue && !e.ServerDeletedAt.HasValue && e.Start > offset)
                .Sum(e => e.Duration.Value)
                .Select(e => TimeSpan.FromSeconds(e)).GetAwaiter().Wait();

            return sum;
        }

        private static string WorkingTimeToString(TimeSpan workTime, int workingHoursPerDay = NumberOfWorkingHoursPerDay)
        {
            return $"{Math.Truncate(workTime.TotalHours / workingHoursPerDay)}.{workTime.Hours % workingHoursPerDay}:{workTime.Minutes}:{workTime.Seconds}";
        }

        private static TimeSpan ExpectedWorkedDays()
        {
            var today = DateTime.Today;
            var first = new DateTime(today.Year, today.Month, 1);

            return TimeSpan.FromHours(first.BusinessDaysUntil(today)*NumberOfWorkingHoursPerDay);
        }

        private static ITogglApi TogglApiWith(Credentials credentials)
            => new TogglApi(ConfigurationFor(credentials));

        private static ApiConfiguration ConfigurationFor(Credentials credentials)
            => new ApiConfiguration(ApiEnvironment.Production, credentials, new UserAgent("toumash.dailystatus", "1"));

        public static SecureString Password
        {
            get
            {
                var pwd = new SecureString();
                while (true)
                {
                    ConsoleKeyInfo i = Console.ReadKey(true);
                    if (i.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    else if (i.Key == ConsoleKey.Backspace)
                    {
                        if (pwd.Length > 0)
                        {
                            pwd.RemoveAt(pwd.Length - 1);
                            Console.Write("\b \b");
                        }
                    }
                    else
                    {
                        pwd.AppendChar(i.KeyChar);
                        Console.Write("*");
                    }
                }
                return pwd;
            }
        }

        private static string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}
