namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System.Globalization;
    using Alpha.Services.Client.Models;
    using Alpha.Services.Client.Extensions;
    using Chatbot.Helpers;

    public class BodyTemplate5 : IBodyTemplate
    {
        private static readonly string[] NewLineHtmlTags = new[] { "<br>", "<br />" };

        public string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var message = DataHelper.Parse<Message>(data);

            foreach (var tag in NewLineHtmlTags)
            {
                message.Content = message.Content.Replace(tag, messengerFormatter.NewLine());
            }

            return message.Content.StripHTML();
        }
    }
}