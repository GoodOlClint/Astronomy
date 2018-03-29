using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astronomy.Tests
{
    [TestClass]
    public class EventsTests
    {
        [TestMethod]
        public void TestEaster()
        {
            JulianDay JD = Events.Easter(179);
            Assert.AreEqual(JD.Month, 04);
            Assert.AreEqual(JD.Day, 12);
        }

        [TestMethod]
        public void TestEaster1()
        {
            JulianDay JD = Events.Easter(1991);
            Assert.AreEqual(JD.Month, 3);
            Assert.AreEqual(JD.Day, 31);
        }

        [TestMethod]
        public void TestPesach()
        {
            JulianDay JD = Events.Pesach(2015);
            Assert.AreEqual(JD.Month, 4);
            Assert.AreEqual(JD.Day, 4);
        }

        [TestMethod]
        public void TestPesach2()
        {
            JulianDay JD = Events.Pesach(2018);
            Assert.AreEqual(JD.Month, 3);
            Assert.AreEqual(JD.Day, 31);
        }
    }
}
