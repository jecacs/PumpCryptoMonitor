using System;
using System.Linq;
using Binance.Net.Objects.Spot.MarketData;
using NUnit.Framework;
using PumpMonitor.Core.Cache;
using PumpMonitor.Domains;

namespace PumpMonitor.Tests
{
    public class PricesCacheTest
    {
        private const string Btcusd = "BTCUSD";
        
        [Test]
        public void TestAdd()
        {
            var prices = new PricesCache();
            
            prices.Add(new BinancePrice
            {
                Price = 10,
                Symbol = Btcusd,
                Timestamp = DateTime.Parse("2021-01-01T01:30:01")
            });
            
            var btcUsd = prices.Instruments.FirstOrDefault(x => x == Btcusd);
            
            Assert.AreEqual(Btcusd, btcUsd);
        }

        [Test]
        public void TestGetFirstLastPrice()
        {
            var prices = new PricesCache();

            prices.Add(new BinancePrice
            {
                Price = 10,
                Symbol = Btcusd,
                Timestamp = DateTime.Parse("2021-01-01T01:30:01")
            });
            
            prices.Add(new BinancePrice
            {
                Price = 11,
                Symbol = Btcusd,
                Timestamp = DateTime.Parse("2021-01-01T01:30:02")
            });
            
            prices.Add(new BinancePrice
            {
                Price = 12,
                Symbol = Btcusd,
                Timestamp = DateTime.Parse("2021-01-01T01:30:03")
            });
            
            prices.Add(new BinancePrice
            {
                Price = 13,
                Symbol = Btcusd,
                Timestamp = DateTime.Parse("2021-01-01T01:33:01")
            });
            
            prices.Add(new BinancePrice
            {
                Price = 14,
                Symbol = Btcusd,
                Timestamp = DateTime.Parse("2021-01-01T01:33:05")
            });

            var (price, lastPrice) = prices
                .GetFirstLastPrice(Btcusd, TimeFrame.Min, DateTime.Parse("2021-01-01T01:33:10"));

            Assert.AreEqual(13, price);
            Assert.AreEqual(14, lastPrice);
        }

        [Test]
        public void TestClearCache()
        {
            var prices = new PricesCache();

            var ts = TimeSpan.Parse("00:05:00");

            prices.Add(new BinancePrice
            {
                Price = 10,
                Symbol = Btcusd,
                Timestamp = DateTime.Parse("2021-01-01T01:31:01")
            });
            
            prices.Add(new BinancePrice
            {
                Price = 11,
                Symbol = Btcusd,
                Timestamp = DateTime.Parse("2021-01-01T01:32:02")
            });
            
            prices.Add(new BinancePrice
            {
                Price = 12,
                Symbol = Btcusd,
                Timestamp = DateTime.Parse("2021-01-01T01:33:03")
            });
            
            prices.Add(new BinancePrice
            {
                Price = 13,
                Symbol = Btcusd,
                Timestamp = DateTime.Parse("2021-01-01T01:34:01")
            });
            
            prices.Add(new BinancePrice
            {
                Price = 14,
                Symbol = Btcusd,
                Timestamp = DateTime.Parse("2021-01-01T02:00:00")
            });

            var dt = DateTime.Parse("2021-01-01T02:00:00") - ts;
            
            prices.ClearCache(dt);
            
            Assert.AreEqual(1, prices.GetTotalInCache());
        }
    }
}