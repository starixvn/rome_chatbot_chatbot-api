namespace Chatbot.AutoUM.Service
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using ONEbook.UM.AspNetCore.Services;
    using ONEbook.UM.Core.Configurations;
    using System.IO;
    using Fanex.Caching;
    using Chatbot.Services;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureLogging((hostContext, config) =>
                {
                    config.AddConsole();
                    config.AddDebug();
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddEnvironmentVariables();
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddSingleton(_ => hostContext.Configuration);
                    services.AddSingleton<MessengerFactory>(new MessengerFactoryImpl(hostContext.Configuration));
                    services.AddSingleton<ICacheService, CacheService>();
                    services.AddSingleton<IMaintenanceService>(new MaintenanceService(hostContext.Configuration));
                    services.AddHostedService<AutoUMService>();
                })
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                // Start the host
                await host.StartAsync();

                // Wait for the host to shutdown
                await host.WaitForShutdownAsync();
            }
        }
    }
}