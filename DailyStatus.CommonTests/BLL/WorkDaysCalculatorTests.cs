using Microsoft.VisualStudio.TestTools.UnitTesting;
using DailyStatus.Common.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyStatus.Common.BLL.Tests
{
    [TestClass()]
    public class WorkDaysCalculatorTests
    {
        [TestMethod()]
        public void Given24OfMarch_ExpWorkDaysShoudReturn17Days_136Hours()
        {
            var actual = new WorkDaysCalculator().ExpectedWorkedDays(new DateTime(2018, 03, 24, 5, 5, 5), TimeSpan.FromHours(10), 8);
            var expected = TimeSpan.FromHours(136);
            Assert.AreEqual(expected.TotalHours / 8, actual.TotalHours / 8);
        }

        [TestMethod()]
        public void Given23OfMarchEndOfDay_ExpWorkDaysShoudReturn17Days_136Hours()
        {
            var actual = new WorkDaysCalculator().ExpectedWorkedDays(new DateTime(2018, 03, 23, 20, 0, 0), TimeSpan.FromHours(8), 8);
            var expected = TimeSpan.FromHours(136);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Given23OfMarchStartOdDay_ExpWorkDaysShoudReturn17Days_128Hours()
        {
            var actual = new WorkDaysCalculator().ExpectedWorkedDays(new DateTime(2018, 03, 23, 02, 0, 0), TimeSpan.FromHours(10), 8);
            var expected = TimeSpan.FromHours(128);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Given23OfMarchStartOdDay_ExpWorkDaysShoudReturn17Days_On26March18()
        {
            var actual = new WorkDaysCalculator().ExpectedWorkedDays(new DateTime(2018, 03, 26, 8, 0, 0), TimeSpan.FromHours(10), 8);
            var expected = TimeSpan.FromHours(17 * 8);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void Given23OfMarchStartOdDay_ExpWorkDaysShoudReturn17AndAHalfDays_On26March18()
        {
            var actual = new WorkDaysCalculator().ExpectedWorkedDays(new DateTime(2018, 03, 26, 14, 0, 0), TimeSpan.FromHours(10), 8);
            var expected = TimeSpan.FromHours(17 * 8 + 4);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Given23OfMarchStartOdDay_ExpWorkDaysShoudReturn17AndMinutes_On26March18()
        {
            var actual = new WorkDaysCalculator().ExpectedWorkedDays(new DateTime(2018, 03, 26, 11, 0, 0), TimeSpan.FromHours(10), 8);
            var expected = TimeSpan.FromHours(17 * 8 + 1);
            Assert.AreEqual(expected, actual);
        }
    }
}