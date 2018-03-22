using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DailyStatus.Common.Extensions;

namespace DailyStatus.CommonTests
{
    [TestClass]
    public class WorkDaysHelperTests
    {
        [TestMethod()]
        public void GivenFullWeekFromMondayToSunday_ShouldReturn5()
        {
            var monday = new DateTime(2018, 03, 19);
            var sunday = new DateTime(2018, 03, 25);
            var actual = monday.BusinessDaysUntil(sunday);
            Assert.AreEqual(5, actual);
        }

        [TestMethod()]
        public void GivenMarchOf2018From1stOfTheMonth_ShouldReturn14OnDay21()
        {
            var monday = new DateTime(2018, 03, 1);
            var sunday = new DateTime(2018, 03, 21);
            var actual = monday.BusinessDaysUntil(sunday);
            Assert.AreEqual(15, actual);
        }
    }
}
