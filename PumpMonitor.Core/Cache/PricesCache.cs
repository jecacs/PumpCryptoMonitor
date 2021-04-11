using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Binance.Net.Objects.Spot.MarketData;
using PumpMonitor.Core.SystemUtils;
using PumpMonitor.Domains;

namespace PumpMonitor.Core.Cache
{
    public class PricesCache
    {
        private readonly ConcurrentDictionary<string, SortedList<DateTime, BinancePrice>> _instruments = new();

        public void Add(IReadOnlyList<BinancePrice> prices)
        {
            foreach (var price in prices)
            {
                Add(price);
            }
        }
        
        public void Add(BinancePrice price)
        {
            if (!_instruments.ContainsKey(price.Symbol))
                _instruments.TryAdd(price.Symbol, new SortedList<DateTime, BinancePrice>());

            var dt = price.Timestamp ?? DateTime.UtcNow;

            _instruments[price.Symbol].Add(dt, price);
        }

        public (decimal price, decimal lastPrice) GetFirstLastPrice(string instrument, TimeFrame timeFrame, DateTime currentDt)
        {
            if (!_instruments.ContainsKey(instrument))
                return (0, 0);

            var ts = timeFrame.GetTimeSpan();

            var date = currentDt - ts;

            var index = _instruments[instrument].FindFirstIndexGreaterThanOrEqualTo(date);

            var lastPrice = _instruments[instrument].Values.Last().Price;

            var change = _instruments[instrument].Values[index];

            return (change.Price, lastPrice);
        }

        public List<string> Instruments => _instruments.Keys.ToList();

        public void ClearCache(DateTime dt)
        {
            foreach (var instrument in _instruments.Keys)
            {
                var oldValues = _instruments[instrument]
                    .Where(x => x.Key <= dt)
                    .ToList();

                foreach (var (key, _) in oldValues)
                {
                    _instruments[instrument].Remove(key);
                }
            }
        }

        public decimal GetLasPrice(string instrument)
        {
            if (!_instruments.ContainsKey(instrument))
                return 0;

            return _instruments[instrument].Values.Last().Price;
        }
        
        public int GetTotalInCache()
        {
            var count = 0;
            
            foreach (var instrument in _instruments.Keys)
            {
                count += _instruments[instrument].Count;
            }
            
            return count;
        }
    }
}