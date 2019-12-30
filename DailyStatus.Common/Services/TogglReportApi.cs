using DailyStatus.Common.Model;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStatus.Common.Services
{
    public class TogglReportApi
    {
        private readonly string apiToken;

        public TogglReportApi(string apiToken)
        {
            this.apiToken = apiToken;
        }

        public async Task<TimeSpan> GetHoursSum(DateTime since, DateTime until, long userId, long workspaceId)
        {
            var client = new RestClient("https://toggl.com/reports/api/v2/summary");
            client.Authenticator = new HttpBasicAuthenticator(apiToken, "api_token");
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("user_agent", "toumash.dailystatus");
            request.AddQueryParameter("workspace_id", workspaceId.ToString());
            request.AddQueryParameter("since", since.ToString("yyyy-MM-dd"));
            request.AddQueryParameter("until", until.ToString("yyyy-MM-dd"));
            request.AddQueryParameter("user_ids", userId.ToString());
            request.AddQueryParameter("grouping", "users");
            request.AddQueryParameter("subgrouping", "clients");
            var response = await client.ExecuteGetTaskAsync<TogglSummaryReportDto>(request);

            var timeMs = response.Data?.Data?.FirstOrDefault()?.Time;
            return TimeSpan.FromMilliseconds(timeMs.HasValue ? timeMs.Value : 0);
        }

        public async Task<long> GetUserId()
        {
            var client = new RestClient("https://www.toggl.com/api/v8/me");
            client.Authenticator = new HttpBasicAuthenticator(apiToken, "api_token");
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("user_agent", "toumash.dailystatus");
            var response = await client.ExecuteGetTaskAsync<TogglMeDto>(request);

            return response.Data.Data.Id;
        }
    }
}
