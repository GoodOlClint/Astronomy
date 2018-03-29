using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Astronomy.Tests
{
    [TestClass]
    public class InstantTests
    {
        [TestMethod]
        public void TestMeanSiderealDate()
        {
            //Midnight, April 10th 1987:
            double JD = 2446895.5;
            double T = (JD - 2451545.0) / 36525;

            Instant MeanSiderealDate = new Instant(6, 41, 50.54841);
            MeanSiderealDate += 8640184.812866 * T;
            MeanSiderealDate += 0.093104 * Math.Pow(T, 2);
            MeanSiderealDate += 0.0000062 * Math.Pow(T, 3);

            Assert.AreEqual(MeanSiderealDate.Hour, 13);
            Assert.AreEqual(MeanSiderealDate.Minute, 10);
            Assert.AreEqual(Math.Round(MeanSiderealDate.Second, 4), 46.3668);
        }

        [TestMethod]
        public void TestMeanSiderealInstant()
        {
            //Midnight, April 10th 1987:
            double JD = 2446895.5;
            double T = (JD - 2451545.0) / 36525;

            Instant MeanSiderealDate = new Instant(6, 41, 50.54841);
            MeanSiderealDate += 8640184.812866 * T;
            MeanSiderealDate += 0.093104 * Math.Pow(T, 2);
            MeanSiderealDate += 0.0000062 * Math.Pow(T, 3);

            Instant MeanSiderealInstant = new Instant(19, 21, 00);
            MeanSiderealInstant = MeanSiderealInstant * 1.00273790935;
            MeanSiderealInstant += MeanSiderealDate;

            Assert.AreEqual(MeanSiderealInstant.Hour, 8);
            Assert.AreEqual(MeanSiderealInstant.Minute, 34);
            Assert.AreEqual(Math.Round(MeanSiderealInstant.Second, 4), 57.0896);
        }
    }
}
