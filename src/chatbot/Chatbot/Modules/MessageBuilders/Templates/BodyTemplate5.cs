namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System.Globalization;
    using Alpha.Services.Client.Extensions;
    using Alpha.Services.Client.Models;
    using Chatbot.Shared.Helpers;

    public class BodyTemplate5 : IBodyTemplate
    {
        private const string TabChar = "\t";
        private static readonly string[] NewLineHtmlTags = new[] { "<br>", "<br />" };
        private static readonly string[] ParagraphHtmlTags = new[] { "</p>", "</ p>", "< /p>" };

        public string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var message = DataHelper.Parse<Message>(data);

            foreach (var tag in ParagraphHtmlTags)
            {
                message.Content = message.Content.Replace(tag, tag + messengerFormatter.NewLine());
            }

            foreach (var tag in NewLineHtmlTags)
            {
                message.Content = message.Content.Replace(tag, messengerFormatter.NewLine());
            }

            return message.Content.StripHTML().Replace(TabChar, string.Empty);
        }
    }
}