namespace ChatR.Configurations
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

    public static class AppSettings
    {
        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static string WebHooksUrl => Configs["WeHookUrl"]
            ?? throw new InvalidOperationException("Missing config for WeHookUrl");
    }
}