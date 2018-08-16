namespace Chatbot.Templates
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Alpha.Services.Client.Models;
    using Chatbot.Helpers;

    public class BodyTemplate5 : IBodyTemplate
    {
        public string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var messages = DataHelper.Parse<IEnumerable<Message>>(data);
            var messageContents = messages.Select(message => message.Content).ToList();

            return string.Join(messengerFormatter.NewLine(), messageContents);
        }
    }
}