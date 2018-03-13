namespace Toumash.DailyStatus
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Toggl.Ultrawave;
    using Toggl.Ultrawave.Network;

    public class Program
    {
        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var credentials = Credentials.WithApiToken("enter-your-api-key");
            var togglApi = TogglApiWith(credentials);
            var user = togglApi.User.Get();

            while (true)
            {
                var sum = await togglApi.TimeEntries.GetAll()
                    .SelectMany(e => e)
                    .Where(e => e.Duration.HasValue)
                    .Sum(e => e.Duration.Value)
                    .Select(e => TimeSpan.FromSeconds(e));
                Console.Write($"\bSum is:{sum.ToString().PadLeft(15)}");


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

        private static ITogglApi TogglApiWith(Credentials credentials)
            => new TogglApi(ConfigurationFor(credentials));

        private static ApiConfiguration ConfigurationFor(Credentials credentials)
            => new ApiConfiguration(ApiEnvironment.Production, credentials, new UserAgent("Toumash console", "1"));
    }
}
