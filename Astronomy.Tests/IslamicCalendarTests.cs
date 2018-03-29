using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astronomy.Tests
{
    [TestClass]
    public class IslamicCalendarTests
    {
        [TestMethod]
        public void TestFromJulianDayNumber()
        {
            JulianDay JD = new JulianDay(1991, 8, 13);
            IslamicCalendar MC = IslamicCalendar.FromJulianDay(JD);

            Assert.AreEqual(MC.Year, 1412);
            Assert.AreEqual(MC.Month, 2);
            Assert.AreEqual(MC.Day, 2);
        }

        [TestMethod]
        public void TestToJulianDayNumber()
        {
            IslamicCalendar MC = new IslamicCalendar(1421, 1, 1);
            JulianDay JD = MC.ToJulianDay();

            Assert.AreEqual(JD.Year, 2000);
            Assert.AreEqual(JD.Month, 4);
            Assert.AreEqual(JD.Day, 6);
        }
    }
}
