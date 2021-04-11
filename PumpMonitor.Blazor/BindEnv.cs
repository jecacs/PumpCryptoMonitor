using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PumpMonitor.Blazor
{
    public static class BindEnvironmentVariables
    {
        public static AppSettings BindEnv(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = new AppSettings();
            configuration.GetSection(AppSettings.KeyName).Bind(appSettings);
            services.AddSingleton(appSettings);

            return appSettings;
        }
    }
}