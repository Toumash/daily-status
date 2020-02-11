using System;
using System.Threading.Tasks;

namespace DailyStatus.Common.Services
{
    public interface ITogglReportApi
    {
        Task<TimeSpan> GetHoursSum(DateTime since, DateTime until, long userId, long workspaceId);
        Task<long> GetUserId();
    }
}