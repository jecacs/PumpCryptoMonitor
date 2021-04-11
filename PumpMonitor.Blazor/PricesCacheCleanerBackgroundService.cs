using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PumpMonitor.Core.Services;
using Serilog;

namespace PumpMonitor.Blazor
{
    public class PricesCacheCleanerBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly PricesCacheService _pricesCacheService;

        private readonly TimeSpan _pricesCacheCleanTs;

        private readonly int _pricesCacheWorker;

        public PricesCacheCleanerBackgroundService(
            ILogger logger, 
            PricesCacheService pricesCacheService, 
            AppSettings settings
            )
        {
            _logger = logger;
            _pricesCacheService = pricesCacheService;
            
            _pricesCacheCleanTs = TimeSpan.Parse(settings.PricesCacheCleanTs);
            _pricesCacheWorker = (int) TimeSpan.Parse(settings.PricesCacheWorkerTs).TotalMilliseconds;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("PricesCacheCleanerBackgroundService is starting");

            while (!stoppingToken.IsCancellationRequested) 
            {
                try
                {
                    await Task.Delay(_pricesCacheWorker, stoppingToken);

                    var dt = DateTime.UtcNow - _pricesCacheCleanTs;
                    _pricesCacheService.CleanCache(dt);
                }
                catch (Exception e)
                {
                    _logger.Fatal(e, e.Message);
                    
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}