namespace Chatbot.AutoUM.Service
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using ONEbook.UM.AspNetCore.Services;
    using System.IO;
    using Fanex.Caching;
    using Chatbot.Services;
    using ONEbook.UM.Core.Configurations;
    using Serilog;
    using System.Diagnostics;
    using System.Linq;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddEnvironmentVariables();
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var rootPath = hostContext.HostingEnvironment.ContentRootPath;
                    UMClientConfiguration.InitConfigurationRoot(rootPath);
                    services.AddLogging();
                    services.AddSingleton(_ => hostContext.Configuration);
                    services.AddSingleton<MessengerFactory, MessengerFactoryImpl>();
                    services.AddSingleton<ICacheService, CacheService>();
                    services.AddSingleton<IMaintenanceService, MaintenanceService>();
                    services.AddHostedService<AutoUMService>();

                    Log.Logger = new LoggerConfiguration()
                       .MinimumLevel.Debug()
                       .WriteTo.File($"{rootPath}\\Logs\\log.txt", rollingInterval: RollingInterval.Day)
                       .CreateLogger();
                })
                .UseSerilog();

            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                await builder.RunAsServiceAsync();
            }
            else
            {
                await builder.RunConsoleAsync();
            }
        }
    }
}