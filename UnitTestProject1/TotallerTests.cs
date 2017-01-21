using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using System.Linq;
using IntradayServices;

namespace UnitTestProject1
{
    [TestClass]
    public class TotallerTests
    {
        [TestMethod]
        public void BasicTest()
        {
            var powerService = new TestPowerService();
            powerService.Trades = new PowerTrade[]
            {
                Utils.Generate(new DateTime(2016,4,1), 
                Enumerable.Range(1, 24)
                .ToDictionary(k => k, v => 100D)),
                Utils.Generate(new DateTime(2016,4,1),
                Enumerable.Range(1, 11)
                .Select(n => new { n, v = 50D })
                .Union(Enumerable.Range(12, 13)
                .Select(n => new { n, v = -20D }))
                .ToDictionary(k => k.n, k => k.v))
            };
            var totaller = new Totaller(powerService);

            var totals = totaller.Totals(DateTime.Now).ToArray();
            Assert.AreEqual(24, totals.Length);
            DateTime eleven = DateTime.Now.Date.AddHours(23);
            for (int i = 0; i < 11; i++)
            {
                Assert.AreEqual(150D, totals[i].Volume);
                Assert.AreEqual(eleven.AddHours(i).TimeOfDay, totals[i].LocalTime);
            }

            for(int i = 11; i < 24; i++)
            {
                Assert.AreEqual(80D, totals[i].Volume);
                Assert.AreEqual(eleven.AddHours(i).TimeOfDay, totals[i].LocalTime);
            }
        }

        [TestMethod]
        public void OctoberTest()
        {
            var powerService = new PowerService();
            var totaller = new Totaller(powerService);

            var testTime = new DateTime(2016, 10, 30, 0, 30, 0);

            var trades = Utils.Sync(powerService, testTime);
            Assert.AreEqual(25, trades.SelectMany(t => t.Periods).Select(p => p.Period).Distinct().Count());
            var totals = totaller.Totals(testTime).ToArray();
            Assert.AreEqual(24, totals.Length);
        }

        [TestMethod]
        public void MarchTest()
        {
            var powerService = new PowerService();
            var totaller = new Totaller(powerService);

            var testTime = new DateTime(2016, 03, 27, 0, 30, 0);

            var trades = Utils.Sync(powerService, testTime);
            Assert.AreEqual(23, trades.SelectMany(t => t.Periods).Select(p => p.Period).Distinct().Count());
            var totals = totaller.Totals(testTime).ToArray();
            Assert.AreEqual(24, totals.Length);
        }
    }
}
