using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using DailyStatus.Common.Extensions;

namespace DailyStatus.Common.BLL.Tests
{
    [TestClass()]
    public class WorkDaysCalculatorTests
    {
        [TestMethod()]
        public void Given24OfMarch_ExpWorkDaysShoudReturn17Days_136Hours()
        {
            var day = new DateTime(2018, 03, 24, 5, 5, 5);
            var actual = new WorkDaysCalculator().TimeExpectedHours(day.FirstDayOfMonth(), day, TimeSpan.FromHours(10), 8, new List<DateTime>());
            var expected = TimeSpan.FromHours(136);
            Assert.AreEqual(expected.TotalHours / 8, actual.TotalHours / 8);
        }

        [TestMethod()]
        public void Given23OfMarchEndOfDay_ExpWorkDaysShoudReturn17Days_136Hours()
        {
            var day = new DateTime(2018, 03, 23, 20, 0, 0);
            var actual = new WorkDaysCalculator().TimeExpectedHours(day.FirstDayOfMonth(), day, TimeSpan.FromHours(8), 8, new List<DateTime>());
            var expected = TimeSpan.FromHours(136);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Given23OfMarchStartOdDay_ExpWorkDaysShoudReturn17Days_128Hours()
        {
            var day = new DateTime(2018, 03, 23, 02, 0, 0);
            var actual = new WorkDaysCalculator().TimeExpectedHours(day.FirstDayOfMonth(), day, TimeSpan.FromHours(10), 8, new List<DateTime>());
            var expected = TimeSpan.FromHours(128);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Given23OfMarchStartOdDay_ExpWorkDaysShoudReturn17Days_On26March18()
        {
            var day = new DateTime(2018, 03, 26, 8, 0, 0);
            var actual = new WorkDaysCalculator().TimeExpectedHours(day.FirstDayOfMonth(), day, TimeSpan.FromHours(10), 8, new List<DateTime>());
            var expected = TimeSpan.FromHours(17 * 8);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Given23OfMarchStartOdDay_ExpWorkDaysShoudReturn17AndAHalfDays_On26March18()
        {
            var day = new DateTime(2018, 03, 26, 14, 0, 0);
            var actual = new WorkDaysCalculator().TimeExpectedHours(day.FirstDayOfMonth(), day, TimeSpan.FromHours(10), 8, new List<DateTime>());
            var expected = TimeSpan.FromHours(17 * 8 + 4);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Given23OfMarchStartOdDay_ExpWorkDaysShoudReturn17AndMinutes_On26March18()
        {
            var day = new DateTime(2018, 03, 26, 11, 0, 0);
            var actual = new WorkDaysCalculator().TimeExpectedHours(day.FirstDayOfMonth(), day, TimeSpan.FromHours(10), 8, new List<DateTime>());
            var expected = TimeSpan.FromHours(17 * 8 + 1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Given8OfMarchStartOdDay_ExpWorkDaysShoudReturn5Days5Hours_On8March15()
        {
            var day = new DateTime(2019, 03, 8, 15, 0, 0);
            var actual = new WorkDaysCalculator().TimeExpectedHours(day.FirstDayOfMonth(), day, TimeSpan.FromHours(10), 8, new List<DateTime>());
            var expected = TimeSpan.FromHours(5 * 8 + 5);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Given12_11_2019_ShouldReturn6Days_BecauseThereIsOneHolidayOn11_11()
        {
            var day = new DateTime(2019, 11, 12);
            var actual = new WorkDaysCalculator().TimeExpectedHours(
                since: day.FirstDayOfMonth(),
                todayWithHours: day,
                workDayStartHour: TimeSpan.FromHours(10),
                numberOfWorkingHoursPerDay: 8,
              holidays: new List<DateTime>() { new DateTime(2019, 11, 11) });
            var expected = TimeSpan.FromHours(6 * 8);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Given7_01_2019_ShouldReturn3Days()
        {
            var day = new DateTime(2019, 1, 7);
            var actual = new WorkDaysCalculator().TimeExpectedHours(
                since: day.FirstDayOfMonth(),
                todayWithHours: day,
                workDayStartHour: TimeSpan.FromHours(10),
                numberOfWorkingHoursPerDay: 8,
                holidays: new List<DateTime>() { new DateTime(2019, 1, 1), new DateTime(2019, 1, 6) });
            var expected = TimeSpan.FromHours(3 * 8);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Given3_12_2019_12_00_With6HoursWeek_ShouldReturn8()
        {
            var day = new DateTime(2019, 12, 3, 22, 30, 0);
            var actual = new WorkDaysCalculator().TimeExpectedHours(
                since: day.FirstDayOfMonth(),
             todayWithHours: day,
             workDayStartHour: TimeSpan.FromHours(10),
             numberOfWorkingHoursPerDay: 6,
             holidays: new List<DateTime>()); ;
            var expected = TimeSpan.FromHours(12);
            Assert.AreEqual(expected, actual);
        }
    }
}
