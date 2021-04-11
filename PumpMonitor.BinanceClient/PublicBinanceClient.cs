using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Spot.MarketData;

namespace PumpMonitor.BinanceClient
{
    public class PublicBinanceClient
    {
        private readonly Binance.Net.BinanceClient _binanceClient = new();

        public async Task<IReadOnlyList<BinancePrice>> GetPricesAsync(CancellationToken cancellationToken)
        {
            var result = await _binanceClient.Spot.Market.GetPricesAsync(cancellationToken);
            
            return result.Data.ToList();
        }
        
        public async Task<IReadOnlyList<BinancePrice>> GetPricesAsync()
        {
            var result = await _binanceClient.Spot.Market.GetPricesAsync();
            
            return result.Data.ToList();
        }

        public async Task<IReadOnlyList<IBinanceTick>> GetPricesStatisticsAsync()
        {
            var result = await _binanceClient.Spot.Market.Get24HPricesAsync();
            
            return result.Data.ToList();
        }
    }
}