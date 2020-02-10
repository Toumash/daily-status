using DailyStatus.Common.BLL;
using DailyStatus.Common.Exceptions;
using DailyStatus.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;

namespace DailyStatus.Common.Services
{
    public partial class TogglProxy
    {
        ITogglApi _togglApi;
        Workspace _workspace;
        List<Workspace> _workspaces;
        private TogglReportApi _togglReportApi;

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

        public TimeSpan GetExpectedWorkingTime(WorkDay dayConfig, DateTime since, List<DateTime> holidays)
        {
            return new WorkDaysCalculator()
                 .TimeExpectedHours(since, DateTime.Now, TimeSpan.FromHours(dayConfig.WorkDayStartHour),
                                     dayConfig.NumberOfWorkingHoursPerDay, holidays);
        }

        public TimeSpan GetDifference(TimeSpan expected, TimeSpan sum)
        {
            var diff = sum - expected;
            return diff;
        }

        public async Task<TogglStatus> GetStatus(DateTime since)
        {
            try
            {
                var workspace = await GetWorkspace();

                var userId = await _togglReportApi.GetUserId();
                var sum = await _togglReportApi.GetHoursSum(since, DateTime.Now.Date.AddDays(-1), userId, workspace.Id);


                var todayEntries = await _togglApi.TimeEntries.GetAllSince(DateTime.Now.Date)
                    .SelectMany(e => e)
                    .Where(e => !e.ServerDeletedAt.HasValue)
                    .Where(e => e.WorkspaceId == workspace.Id)
                    .ToList();

                var currentTaskElement = todayEntries
                    .FirstOrDefault(e => !e.Duration.HasValue);

                var currentTaskDuration = TimeSpan.FromSeconds(0);
                if (currentTaskElement != null)
                {
                    currentTaskDuration = (DateTime.UtcNow - currentTaskElement.Start);
                }
                sum += currentTaskDuration;

                var todayHoursSum = TimeSpan.FromSeconds(todayEntries
                    .Where(e => e.Start > DateTime.Today && e.Duration.HasValue)
                    .Sum(e => e.Duration.Value));

                sum += todayHoursSum;

                var todaysHours = todayHoursSum + currentTaskDuration;

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
            _togglReportApi = new TogglReportApi(key);
            var credentials = Credentials.WithApiToken(key);
            _togglApi = TogglApiWith(credentials);
        }

        public async Task<List<Workspace>> GetAllWorkspaces()
        {
            _workspaces = (await _togglApi.Workspaces.GetAll()).Select(w => new Workspace { Name = w.Name, Id = w.Id }).ToList();
            return _workspaces;
        }
    }
}
