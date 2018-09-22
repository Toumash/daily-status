using DailyStatus.Common.BLL;
using DailyStatus.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;

namespace DailyStatus.Common
{
    public class TogglProxy
    {
        ITogglApi _togglApi;
        Workspace _workspace;
        List<Workspace> _workspaces;

        public void SetWorkspace(Workspace workspace)
        {
            if (_workspaces.Any(w => w.Id == workspace.Id))
                _workspace = workspace;
            else
                throw new ArgumentException("given workspace does not exist on current toggl account");
        }
        public async Task<Workspace> GetWorkspace()
        {
            if (_workspace == null)
            {
                _workspaces = await GetAllWorkspaces();
                _workspace = _workspaces.First();
            }
            return _workspace;
        }

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

        public Workspace GetWorkspaceCached()
        {
            return _workspace;
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
                var workspace = await GetWorkspace();
                var today = DateTime.Today;
                var offset = new DateTimeOffset(new DateTime(today.Year, today.Month, 1));
                var entries = await _togglApi.TimeEntries.GetAllSince(offset)
                    .SelectMany(e => e)
                    .Where(e => !e.ServerDeletedAt.HasValue && e.Start > offset)
                    .Where(e => e.WorkspaceId == workspace.Id)
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
            catch (Toggl.Ultrawave.Exceptions.BadRequestException e)
            {
                throw new BadRequestException("bad request", e);
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
            catch (Toggl.Ultrawave.Exceptions.BadRequestException)
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

        public async Task<List<Workspace>> GetAllWorkspaces()
        {
            _workspaces = (await _togglApi.Workspaces.GetAll()).Select(w => new Workspace { Name = w.Name, Id = w.Id }).ToList();
            return _workspaces;
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


    [Serializable]
    public class BadRequestException : Exception
    {
        public BadRequestException() { }
        public BadRequestException(string message) : base(message) { }
        public BadRequestException(string message, Exception inner) : base(message, inner) { }
        protected BadRequestException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
