using NUnit.Framework;
using PumpMonitor.Core.Cache;
using PumpMonitor.Domains;

namespace PumpMonitor.Tests
{
    public class PricesChangesTest
    {
        private const string Btcusd = "BTCUSD";

        [Test]
        public void TestAdd()
        {
            var pricesChanges = new PricesChanges();

            pricesChanges.CalculateMinChanges(Btcusd, TimeFrame.Min, 10, 20);
            
            Assert.AreEqual(100, pricesChanges.GetChangesDictionary(TimeFrame.Min)[Btcusd]);
            
            pricesChanges.CalculateMinChanges(Btcusd, TimeFrame.Min, 10, 15);

            Assert.AreEqual(50, pricesChanges.GetChangesDictionary(TimeFrame.Min)[Btcusd]);
            
            pricesChanges.CalculateMinChanges(Btcusd, TimeFrame.Min, 10, 5);

            Assert.AreEqual(-50, pricesChanges.GetChangesDictionary(TimeFrame.Min)[Btcusd]);
        }
    }
}