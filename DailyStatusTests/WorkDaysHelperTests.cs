using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toumash.DailyStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toumash.DailyStatus.Tests
{
    [TestClass()]
    public class WorkDaysHelperTests
    {
        [TestMethod()]
        public void GivenFullWeekFromMondayToSunday_ShouldReturn5()
        {
            var monday = new DateTime(2018, 03, 19);
            var sunday = new DateTime(2018, 03, 25);
            var actual = WorkDaysHelper.BusinessDaysUntil(monday, sunday);
            Assert.AreEqual(5, actual);
        }

        [TestMethod()]
        public void GivenMarchOf2018From1stOfTheMonth_ShouldReturn14OnDay21()
        {
            var monday = new DateTime(2018, 03, 1);
            var sunday = new DateTime(2018, 03, 21);
            var actual = WorkDaysHelper.BusinessDaysUntil(monday, sunday);
            Assert.AreEqual(15, actual);
        }
    }
}