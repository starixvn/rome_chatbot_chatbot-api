namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Chatbot.AppResources;
    using Chatbot.Helpers;
    using Extensions;

    public interface IBodyTemplate
    {
        string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter);
    }

    public class BodyTemplate : IBodyTemplate
    {
        public virtual string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var keyValues = DataHelper.Parse<Dictionary<string, object>>(data);
            var textContainer = new List<string>();

            if (!keyValues.Keys.Any())
            {
                return GetResource(nameof(Languages.NoData), cultureInfo);
            }

            foreach (var keyValue in keyValues)
            {
                if (keyValue.Value != null)
                {
                    textContainer.Add($"{GetResource(keyValue.Key, cultureInfo)}:");
                    textContainer.Add($"    {GetFormattedValue(keyValue.Value)}");
                }
            }

            return string.Join(messengerFormatter.NewLine(), textContainer);
        }

        protected static string GetFormattedValue(object value)
        {
            if (value is string)
            {
                return value as string;
            }

            if (value is double)
            {
                return ((double)value).ToFormattedNumber();
            }

            if (value is decimal)
            {
                return ((decimal)value).ToFormattedNumber();
            }

            return value.ToString();
        }

        protected virtual string GetResource(string key, CultureInfo cultureInfo)
            => Languages.ResourceManager.GetString(key, cultureInfo);
    }
}