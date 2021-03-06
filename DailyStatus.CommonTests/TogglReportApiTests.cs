﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using DailyStatus.Common.Services;

namespace DailyStatus.Common.Tests
{
    [TestClass()]
    public class TogglReportApiTests
    {
        [TestMethod]
        [Ignore("Needs toggl prod token")]
        public async Task GetHoursSumTest()
        {
            var sum = await new TogglReportApi("<api-token>").GetHoursSum(
                new DateTime(2019, 09, 01),
                new DateTime(2019, 12, 7),
                -1,//user-id>
                -1);// workspace-id
            Assert.IsTrue(sum > TimeSpan.FromSeconds(10));
        }

        [TestMethod()]
        [Ignore("Needs toggl prod token")]
        public async Task GetUserIdTest()
        {
            var id = await new TogglReportApi("<api-token>").GetUserId();
            Assert.IsTrue(id > 0);
        }
    }
}