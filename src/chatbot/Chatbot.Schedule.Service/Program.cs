namespace Chatbot.Schedule.Service
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System.IO;
    using System.Threading.Tasks;
    using Quartz;
    using Quartz.Impl;
    using System.Collections.Specialized;
    using Chatbot.Client;
    using Chatbot.Client.Configurations;
    using Chatbot.Schedule.Service.Jobs;

    public static class Program
    {
        private static async Task Main(string[] args)
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