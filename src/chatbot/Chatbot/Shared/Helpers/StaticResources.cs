namespace Chatbot.Shared.Helpers
{
    using System.Globalization;
    using Chatbot.AppResources;

    public static class StaticResources
    {
        public static string Get(string key, string language)
            => Languages.ResourceManager.GetString(key, CultureInfo.CreateSpecificCulture(language));

        public static string Get(string key, CultureInfo culture)
            => Languages.ResourceManager.GetString(key, culture);
    }
}