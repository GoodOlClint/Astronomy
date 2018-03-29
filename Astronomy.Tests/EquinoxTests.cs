using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astronomy.Tests
{
    [TestClass]
    public class EquinoxTests
    {
        [TestMethod]
        public void TestApproximateEquinox()
        {
            double JD = Equinox.Approximate(1962, Season.Summer);
            Assert.AreEqual(JD, 2437837.39245);
        }
    }
}
