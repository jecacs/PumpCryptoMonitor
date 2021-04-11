using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PumpMonitor.BinanceClient;
using PumpMonitor.Core.Cache;
using PumpMonitor.Core.Services;
using Serilog;

namespace PumpMonitor.Blazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var config = services.BindEnv(Configuration);
            
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSingleton<ILogger>(new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger());
            
            services.AddSingleton<PriceChangesService>();
            services.AddSingleton<PricesCache>();
            services.AddSingleton<PricesChanges>();
            services.AddSingleton<PublicBinanceClient>();

            services.AddSingleton(provider =>
            {
                var settings = provider.GetService<AppSettings>();

                if (settings == null)
                    throw new Exception("not find settings");

                return new BlackListInstruments(settings.CurrencyBlackList, settings.Bot.TradingVolumeToStartTrade, settings.BasicCurrency);
            });
            
            services.AddSingleton(provider =>
            {
                var settings = provider.GetService<AppSettings>();

                if (settings == null)
                    throw new Exception("not find settings");

                return new PricesCacheService(
                    provider.GetService<PricesCache>()!,
                    provider.GetService<PricesChanges>()!,
                    provider.GetService<PublicBinanceClient>()!,
                    provider.GetService<BlackListInstruments>()!
                );
            });

            services.AddHostedService(provider => 
                new PricesBackgroundService(
                    provider.GetService<ILogger>(),
                    provider.GetService<PricesCacheService>()
                ));

            services.AddHostedService(provider =>
                new PricesCacheCleanerBackgroundService(
                    provider.GetService<ILogger>(),
                    provider.GetService<PricesCacheService>(),
                    provider.GetService<AppSettings>()
                ));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}