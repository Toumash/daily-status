using DailyStatus.Common.BLL;
using DailyStatus.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Multivac.Models;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;

namespace DailyStatus.Common
{
    public class TogglProxy
    {
        ITogglApi _togglApi = null;


        public TimeSpan GetExpectedWorkingTime(WorkDay dayConfig)
        {
            return new WorkDaysCalculator()
                 .ExpectedWorkedDays(TimeSpan.FromHours(dayConfig.WorkDayStartHour),
                                     dayConfig.NumberOfWorkingHoursPerDay);
        }

        public TimeSpan GetDifference(TimeSpan expected, TimeSpan sum)
        {
            var diff = sum - expected;
            return diff;
        }

        public async Task<TimeSpan> GetDifference(WorkDay dayConfig)
        {
            var expected = GetExpectedWorkingTime(dayConfig);
            var sum = await GetStatus();
            return GetDifference(expected, sum.TimeInMonth);
        }
        public class TogglStatus
        {
            public TimeSpan TimeInMonth { get; set; }

            public bool IsTimerActive { get; set; }
            public TimeSpan TodaysHours { get; set; }
        }

        public async Task<TogglStatus> GetStatus()
        {
            try
            {
                var today = DateTime.Today;
                var offset = new DateTimeOffset(new DateTime(today.Year, today.Month, 1));
                var entries = await _togglApi.TimeEntries.GetAllSince(offset)
                    .SelectMany(e => e)
                    .Where(e => !e.ServerDeletedAt.HasValue && e.Start > offset)
                    .ToList();
                var sumSeconds = entries.Where(e => e.Duration.HasValue)
                    .Sum(e => e.Duration.Value);

                var sum = TimeSpan.FromSeconds(sumSeconds);

                var currentTaskElement = entries
                    .Where(e => !e.Duration.HasValue)
                    .FirstOrDefault();

                TimeSpan currentTaskDuration = TimeSpan.FromSeconds(0);
                if (currentTaskElement != null)
                {
                    currentTaskDuration = (DateTime.UtcNow - currentTaskElement.Start);
                }
                sum += currentTaskDuration;

                var todayHoursSum = entries
                    .Where(e => e.Start > DateTime.Today && e.Duration.HasValue)
                    .Sum(e => e.Duration.Value);
                var todaysHours = TimeSpan.FromSeconds(todayHoursSum) + currentTaskDuration;

                return new TogglStatus()
                {
                    TimeInMonth = sum,
                    IsTimerActive = currentTaskElement != null,
                    TodaysHours = todaysHours
                };
            }
            catch (Toggl.Ultrawave.Exceptions.OfflineException e)
            {
                throw new OfflineException("No internet connection", e);
            }
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
            catch (Toggl.Ultrawave.Exceptions.OfflineException)
            {
                return false;
            }
        }

        public async Task StartTimer()
        {
            var workspaces = await _togglApi.Workspaces.GetAll();

            var user = await _togglApi.User.Get();
            var timeEntry = await _togglApi.TimeEntries.Create(new TimeEntry()
            {
                At = DateTime.UtcNow,
                Description = Guid.NewGuid().ToString(),
                Start = DateTime.UtcNow,
                UserId = user.Id,
                TagIds = new List<long>(),
                WorkspaceId = user.DefaultWorkspaceId,
            });
        }

        public async Task StopTimer()
        {
            var workspaces = await _togglApi.Workspaces.GetAll();

            var user = await _togglApi.User.Get();
            var data = await _togglApi.TimeEntries.GetAllSince(DateTime.Now.AddDays(-1)).SelectMany(e => e).ToList();
            var lastTask = new TimeEntry(await _togglApi.TimeEntries.GetAllSince(DateTime.Now.AddDays(-1)).SelectMany(e => e).Where(e => e.Duration == null).FirstOrDefaultAsync());
            if (lastTask != null)
            {
                // FIXME: Bad Request
                lastTask.Duration = (long)(DateTime.Now - lastTask.Start).TotalSeconds;
                await _togglApi.TimeEntries.Update(lastTask);
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


    [Serializable]
    public class OfflineException : Exception
    {
        public OfflineException() { }
        public OfflineException(string message) : base(message) { }
        public OfflineException(string message, Exception inner) : base(message, inner) { }
        protected OfflineException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
