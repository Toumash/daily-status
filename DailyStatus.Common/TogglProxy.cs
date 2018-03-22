using DailyStatus.Common.BLL;
using DailyStatus.Common.Model;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;

namespace DailyStatus.Common
{
    public class TogglProxy
    {
        ITogglApi _togglApi = null;

        public async Task<TimeSpan> GetDifference(WorkDay dayConfig)
        {
            var expected = new WorkDaysCalculator()
                 .ExpectedWorkedDays(TimeSpan.FromHours(dayConfig.WorkDayStartHour),
                                     dayConfig.NumberOfWorkingHoursPerDay);
            var sum = await GetWorkingTime();
            var diff = sum - expected;
            return diff;
        }

        public async Task<TimeSpan> GetWorkingTime()
        {
            var today = DateTime.Today;
            var offset = new DateTimeOffset(new DateTime(today.Year, today.Month, 1));

            var sum = await _togglApi.TimeEntries.GetAllSince(offset)
                .SelectMany(e => e)
                .Where(e => e.Duration.HasValue && !e.ServerDeletedAt.HasValue && e.Start > offset)
                .Sum(e => e.Duration.Value)
                .Select(e => TimeSpan.FromSeconds(e));

            var currentTaskElement = await _togglApi.TimeEntries.GetAllSince(offset)
                .SelectMany(e => e)
                .Where(e => !e.Duration.HasValue)
                .FirstOrDefaultAsync();

            if (currentTaskElement != null)
            {
                var currentTaskDuration = (DateTime.UtcNow - currentTaskElement.Start);
                sum += currentTaskDuration;
            }
            return sum;
        }

        public bool TestConnection()
        {
            try
            {
                var user = _togglApi.User.Get().GetAwaiter().Wait();
                return true;
            }
            catch (Toggl.Ultrawave.Exceptions.UnauthorizedException)
            {
                return false;
            }
        }

        public ITogglApi TogglApiWith(Credentials credentials)
            => new TogglApi(ConfigurationFor(credentials));

        public ApiConfiguration ConfigurationFor(Credentials credentials)
            => new ApiConfiguration(ApiEnvironment.Production, credentials, new UserAgent("toumash.dailystatus", "1"));

        public void Configure(string key)
        {
            var credentials = Credentials.WithApiToken(key);
            _togglApi = TogglApiWith(credentials);
        }
    }
}
