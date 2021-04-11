using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PumpMonitor.Core.Services;
using Serilog;

namespace PumpMonitor.Blazor
{
    public class PricesBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly PricesCacheService _pricesCacheService;

        public PricesBackgroundService(ILogger logger, PricesCacheService pricesCacheService)
        {
            _logger = logger;
            _pricesCacheService = pricesCacheService;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("PricesBackgroundService is starting");

            await _pricesCacheService.InitialAsync();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _pricesCacheService.ExecuteAsync(stoppingToken);
                    
                    await Task.Delay(100, stoppingToken);
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