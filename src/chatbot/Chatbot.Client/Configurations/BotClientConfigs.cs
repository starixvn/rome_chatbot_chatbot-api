namespace Chatbot.Client.Configurations
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using Microsoft.Extensions.Configuration;

    public interface IBotClientConfigs
    {
        string ApiBaseUrl { get; }

        string ApiKey { get; }
    }

    public class BotClientConfigs : IBotClientConfigs
    {
        public BotClientConfigs(
            string apiBaseUrl,
            string apiKey)
        {
            ApiBaseUrl = apiBaseUrl;
            ApiKey = apiKey;
        }

        public string ApiBaseUrl { get; }

        public string ApiKey { get; }

        public static IBotClientConfigs GetDefaulBotClientConfig()
            => AppSettings.BotClientConfigs;
    }

    internal static class AppSettings
    {
        /// <summary>
        /// Support for .net core
        /// </summary>
        private readonly static IConfigurationRoot configurationRoot;

#pragma warning disable S3963 // "static" fields should be initialized inline
        static AppSettings()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var configFileName = "appsettings.json";

            if (File.Exists(Path.Combine(currentDirectory, configFileName)))
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(currentDirectory)
                    .AddJsonFile(configFileName);

                configurationRoot = builder.Build();
            }

            var chatbotBaseUrl = GetConfigs("Chatbot:BaseUrl");
            var chatbotApiKey = GetConfigs("Chatbot:ApiKey");

            BotClientConfigs = new BotClientConfigs(chatbotBaseUrl, chatbotApiKey);
        }
#pragma warning restore S3963 // "static" fields should be initialized inline

        private static string GetConfigs(string configName)
        {
            var configValue = configurationRoot == null
                ? Configs[configName]
                : configurationRoot[configName];

            return configValue ?? throw new InvalidOperationException($"Missing config for {configName}");
        }

        /// <summary>
        /// Support for .net framework
        /// </summary>
        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static IBotClientConfigs BotClientConfigs { get; private set; }
    }
}