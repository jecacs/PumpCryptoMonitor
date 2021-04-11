using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PumpMonitor.BinanceClient;
using PumpMonitor.Core.Cache;
using PumpMonitor.Domains;

namespace PumpMonitor.Core.Services
{
    public class PricesCacheService
    {
        private readonly PublicBinanceClient _binanceClient;
        private readonly PricesChanges _pricesChanges;
        private readonly PricesCache _pricesCache;
        private readonly BlackListInstruments _blackListInstruments;

        public PricesCacheService(
            PricesCache pricesCache, 
            PricesChanges pricesChanges, 
            PublicBinanceClient binanceClient,
            BlackListInstruments blackListInstruments
        )
        {
            _pricesCache = pricesCache;
            _pricesChanges = pricesChanges;
            _binanceClient = binanceClient;
            _blackListInstruments = blackListInstruments;
        }

        public async Task InitialAsync()
        {
            var ticks = await _binanceClient.GetPricesStatisticsAsync();
            
            _blackListInstruments.Initial(ticks);
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var binancePrices = await _binanceClient
                .GetPricesAsync(stoppingToken);

            foreach (var price in binancePrices)
            {
                if (!_blackListInstruments.IsExist(price.Symbol))
                {
                    _pricesCache.Add(price);
                }
            }

            var dt = DateTime.UtcNow;

            var calculateTasks = new List<Task>();
            
            foreach (var instrument in _pricesCache.Instruments)
            {
                calculateTasks.Add(Task.Run(() => CalculateChanges(instrument, dt), stoppingToken));
            }
            
            await Task.WhenAll(calculateTasks);
        }

        private void CalculateChanges(string instrument, DateTime dt)
        {
            foreach (var timeout in TimeFrameMapper.GetTimeFrames())
            {
                var (price, lastPrice) = _pricesCache.GetFirstLastPrice(instrument, timeout, dt);

                _pricesChanges.CalculateMinChanges(instrument, timeout,  price, lastPrice);
            }
        }

        public void CleanCache(DateTime dt)
        {
            _pricesCache.ClearCache(dt);
        }
    }
}