namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Chatbot.AppResources;
    using Chatbot.Shared.Extensions;
    using Chatbot.Shared.Helpers;

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
                return StaticResources.Get(nameof(Languages.NoData), cultureInfo);
            }

            foreach (var keyValue in keyValues)
            {
                if (keyValue.Value != null)
                {
                    textContainer.Add($"{StaticResources.Get(keyValue.Key, cultureInfo)}:");
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

        /// <summary>
        /// Random rate of 20%
        /// </summary>
        /// <returns></returns>
        protected static bool IsShowHint()
        {
            var random = new Random();
            return random.Next(0, 5) == 0;
        }
    }
}