namespace Chatbot.Schedule.Service
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Client;
    using Chatbot.Client.Configurations;
    using Chatbot.Schedule.Service.Jobs;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Quartz;
    using Quartz.Impl;
    using Serilog;

    public static class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
             .ConfigureAppConfiguration((hostContext, config) =>
             {
                 config.AddEnvironmentVariables();
                 config.SetBasePath(hostContext.HostingEnvironment.ContentRootPath);
                 config.AddJsonFile("appsettings.json", optional: true);
             })
             .ConfigureServices((hostContext, services) =>
             {
                 Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Debug()
                   .WriteTo.File(
                         $"{hostContext.HostingEnvironment.ContentRootPath}\\Logs\\log.txt",
                         rollingInterval: RollingInterval.Day)
                   .CreateLogger();

                 ChatbotClientManager
                       .UseConfig(new BotClientConfigs(
                            hostContext.Configuration.GetSection("Chatbot:BaseUrl").Value,
                            hostContext.Configuration.GetSection("Chatbot:ApiKey").Value))
                       .UseRestClient(new Client.Transport.RestClient());

                 services.AddLogging();
                 services.AddSingleton(_ => hostContext.Configuration);
                 services.AddSingleton<ISchedulerFactory>(
                    new StdSchedulerFactory(new NameValueCollection
                    {
                        { "quartz.serializer.type", "binary" }
                    }));
                 services.AddTransient<IJobListener, JobFailureHandler>();
                 services.AddHostedService<ScheduleNotificationService>();
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