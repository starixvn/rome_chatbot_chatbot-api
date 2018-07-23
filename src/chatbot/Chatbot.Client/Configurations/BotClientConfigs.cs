namespace Chatbot.Client.Configurations
{
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
    }
}