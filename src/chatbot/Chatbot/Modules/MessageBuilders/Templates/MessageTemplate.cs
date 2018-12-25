namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public interface IMessageTemplate
    {
        IHeaderTemplate Header { get; }

        IBodyTemplate Body { get; }

        IFooterTemplate Footer { get; }

        string ToString(int functionId, object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter);
    }

    public class MessageTemplate : IMessageTemplate
    {
        public MessageTemplate(
            IHeaderTemplate header,
            IBodyTemplate body,
            IFooterTemplate footer = null)
        {
            Header = header;
            Body = body;
            Footer = footer;
        }

        public IHeaderTemplate Header { get; }

        public IBodyTemplate Body { get; }

        public IFooterTemplate Footer { get; }

        public string ToString(
            int functionId,
            object data,
            CultureInfo cultureInfo,
            IMessengerFormatter messengerFormatter)
        {
            var templates = new List<string>
            {
                Header?.ToString(functionId, cultureInfo),
                Body.ToString(data, cultureInfo, messengerFormatter),
                Footer?.ToString()
            };

            return string.Join(
                    messengerFormatter.NewLine(),
                    templates.Where(template => !string.IsNullOrWhiteSpace(template)));
        }
    }
}