namespace Messenger.Client.Configurations
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

    public static class AppSettings
    {
        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static string MessengerApiEndpoint => Configs["MessengerAPIClient:ApiEndpoint"]
            ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:ApiEndpoint");

        public static string MessengerApiKey => Configs["MessengerAPIClient:ApiKey"]
            ?? throw new InvalidOperationException("Missing config for MessengerAPIClient:ApiKey");
    }
}