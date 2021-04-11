using NUnit.Framework;
using PumpMonitor.Core;

namespace PumpMonitor.Tests
{
    public class TestTradingUtils
    {
        [Test]
        public void TestCalcPercents()
        {
            decimal p = 10;
            decimal p2 = 5;

            var res = p.CalcPercents(p2);
            
            Assert.AreEqual(-50, res);

            decimal a = 50;
            decimal a2 = 100;

            var res1 = a.CalcPercents(a2);
            
            Assert.AreEqual(100, res1);
        }

        [Test]
        public void TestCalcQuantity()
        {
            var percentsOfAmount = 40;
            
            decimal amount = 100;
            decimal price = 1;

            var result = amount.CalcQuantity(price, percentsOfAmount);
            
            Assert.AreEqual(40, result);

            amount = 250;
            price = 26;
            
            var result1 = amount.CalcQuantity(price, percentsOfAmount);

            Assert.AreEqual(3.85m, result1);

            amount = 1488;
            price = 9;
            
            var result2 = amount.CalcQuantity(price, percentsOfAmount);
            
            Assert.AreEqual(66.13m, result2);
        }
        
        [Test]
        public void TestIsClosePositionByProfit()
        {
            int percentsToClose = 5;
            decimal openPrice = 1;
            decimal lastPrice = 1.15m;

            var result = openPrice.IsClosePositionByProfit(lastPrice, percentsToClose);
            
            Assert.AreEqual(true, result);
            
            decimal openPrice1 = 1;
            decimal lastPrice1 = 1.03m;

            var result1 = openPrice1.IsClosePositionByProfit(lastPrice1, percentsToClose);
            
            Assert.AreEqual(false, result1);
        }

        [Test]
        public void TestCalcStopLossPrices()
        {
            decimal openPrice = 100;
            double percentsOfPrice = 0.1;

            var (stopPrice, price) = openPrice.CalcStopLossPrices((decimal) percentsOfPrice);
            
            Assert.AreEqual(99.9, stopPrice);
            Assert.AreEqual(99.9, price);
        }
    }
}