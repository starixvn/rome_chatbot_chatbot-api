namespace Chatbot.Templates
{
    using System.Globalization;
    using Alpha.Services.Client.Models;
    using Chatbot.Helpers;

    public class BodyTemplate5 : IBodyTemplate
    {
        public string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var message = DataHelper.Parse<Message>(data);

            return message.Content;
        }
    }
}