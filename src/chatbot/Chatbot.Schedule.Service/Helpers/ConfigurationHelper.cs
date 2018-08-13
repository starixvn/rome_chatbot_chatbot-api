namespace Chatbot.Schedule.Service.Helpers
{
    using System.IO;
    using Microsoft.Extensions.Configuration;

    public static class ConfigurationHelper
    {
        public static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                  .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}