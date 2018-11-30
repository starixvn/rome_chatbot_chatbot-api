namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System.Collections.Generic;
    using System.Globalization;
    using Alpha.Services.Client.Models;
    using Chatbot.AppResources;
    using Chatbot.Shared.Helpers;

    public class BodyTemplate9 : BodyTemplate
    {
        public override string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var copyCustomerResult = DataHelper.Parse<CopyCustomerResult>(data);

            if (!copyCustomerResult.IsSuccess)
            {
                return copyCustomerResult.Message;
            }

            var textContainer = new List<string>
            {
                $"{GetResource(nameof(Languages.Username),cultureInfo)}: {copyCustomerResult.CustName}",
                $"{GetResource(nameof(Languages.Password),cultureInfo)}: {copyCustomerResult.Password}"
            };

            return string.Join(messengerFormatter.NewLine(), textContainer);
        }
    }
}