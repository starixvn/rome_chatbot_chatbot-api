namespace Chatbot.Client.Configurations
{
    public interface IBotClientConfigs
    {
#pragma warning disable S3996 // URI properties should not be strings
        string ApiBaseUrl { get; }
#pragma warning restore S3996 // URI properties should not be strings

        string ApiKey { get; }
    }

    public class BotClientConfigs : IBotClientConfigs
    {
        public BotClientConfigs(
#pragma warning disable S3994 // URI Parameters should not be strings
            string apiBaseUrl,
#pragma warning restore S3994 // URI Parameters should not be strings
            string apiKey)
        {
            ApiBaseUrl = apiBaseUrl;
            ApiKey = apiKey;
        }

#pragma warning disable S3996 // URI properties should not be strings
        public string ApiBaseUrl { get; }
#pragma warning restore S3996 // URI properties should not be strings

        public string ApiKey { get; }
    }
}