using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Astronomy.Tests
{
    [TestClass]
    public class EquinoxTests
    {
        [TestMethod]
        public void TestMeanEquinox()
        {
            double JD = Equinox.Mean(1962, Season.Summer);
            Assert.AreEqual(JD, 2437837.38589);
        }

        [TestMethod]
        public void TestApproximateEquinox()
        {
            double JD = Equinox.Approximate(1962, Season.Summer);
            Assert.AreEqual(JD, 2437837.39245);
        }

        [TestMethod]
        public void TestExactEquinox()
        {
            double JD = Equinox.Exact(2010, Season.Spring);
            JulianDay JD1 = new JulianDay(2010, 3, 20, 17, 31);

            double T = (JD1.JulianDayNumber - 2451545.0) / 36525;

            Instant MeanSiderealDate = new Instant(JD1.Hour, JD1.Minute, JD1.Second);
            MeanSiderealDate += 8640184.812866 * T;
            MeanSiderealDate += 0.093104 * Math.Pow(T, 2);
            MeanSiderealDate += 0.0000062 * Math.Pow(T, 3);

            Instant MeanSiderealInstant = new Instant(19, 21, 00);
            MeanSiderealInstant = MeanSiderealInstant * 1.00273790935;
            MeanSiderealInstant += MeanSiderealDate;
            Assert.AreEqual(JD, JD1);
        }
    }
}
