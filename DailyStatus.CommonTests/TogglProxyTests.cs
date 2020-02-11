using DailyStatus.Common.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Toggl.Multivac.Models;
using Toggl.Ultrawave;

namespace DailyStatus.CommonTests
{
    [TestClass()]
    public class TogglProxyTests
    {
        [TestMethod]
        public async Task Given3hYesterdayAnd5hToday_WhenGettingStatus_ShouldIncludeTodayHours()
        {
            const int HoursWithoutToday = 3;
            const int HoursToday = 5;

            const long WorkSpaceId = -1L;
            var yesterdayEntry = new Mock<ITimeEntry>();
            yesterdayEntry.Setup(e => e.ServerDeletedAt).Returns(() => null);
            yesterdayEntry.Setup(e => e.WorkspaceId).Returns(() => WorkSpaceId);
            yesterdayEntry.Setup(e => e.Duration).Returns(() => (long)TimeSpan.FromHours(HoursToday).TotalSeconds);
            yesterdayEntry.Setup(e => e.Start).Returns(() => DateTime.Today.AddHours(1));

            var observable = Observable.Create<List<ITimeEntry>>((obs) =>
            {
                obs.OnNext(new List<ITimeEntry>() { yesterdayEntry.Object });
                obs.OnCompleted();
                return () => { };
            });

            var togglMock = new Mock<ITogglApi>();
            togglMock.Setup(t => t.TimeEntries.GetAllSince(It.IsAny<DateTimeOffset>()))
            .Returns(observable);

            var workspaceMock = new Mock<IWorkspace>();
            workspaceMock.Setup(w => w.Name).Returns("test");
            workspaceMock.Setup(w => w.Id).Returns(WorkSpaceId);

            var observableWorkspace = Observable.Create<List<IWorkspace>>((obs) =>
            {
                obs.OnNext(new List<IWorkspace>() { workspaceMock.Object });
                obs.OnCompleted();
                return () => { };
            });

            togglMock.Setup(t => t.Workspaces.GetAll())
                .Returns(observableWorkspace);
          
            var reportMock = new Mock<ITogglReportApi>();
            reportMock.Setup(r => r.GetUserId()).Returns(Task.FromResult(-1L));
            reportMock.Setup(r => r.GetHoursSum(It.IsAny<DateTime>(), It.IsAny<DateTime>(),
                It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult(TimeSpan.FromHours(HoursWithoutToday)));


            var sut = new TogglProxy(reportMock.Object, togglMock.Object);
            var status = await sut.GetStatus(new DateTime(2020, 02, 04));
            status.TodaysHours.Should().Be(TimeSpan.FromHours(HoursToday));
            status.TimeInMonth.Should().BeGreaterThan(TimeSpan.FromHours(HoursToday));
        }
    }
}
