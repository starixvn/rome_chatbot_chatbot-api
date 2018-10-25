namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System.Collections.Generic;
    using System.Globalization;
    using Alpha.Services.Client.Models;
    using Chatbot.Helpers;

    public class BodyTemplate9 : BodyTemplate
    {
        public override string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var outstandingDetail = DataHelper.Parse<OutstandingDetail>(data);

            if (outstandingDetail == null)
            {
                return base.ToString(data, cultureInfo, messengerFormatter);
            }

            var textContainer = new List<string>
            {
                $"{GetResource("Username", cultureInfo)} | {GetResource("Member", cultureInfo)} | {GetResource(outstandingDetail.CustLevel, cultureInfo)}"
            };

            var total = outstandingDetail.Total;
            textContainer.Add($"{GetResource("Total", cultureInfo)}  {GetFormattedValue(total.MemberOutstanding)}  {GetFormattedValue(total.Outstanding)}");
            textContainer.Add("------------------------------------");

            foreach (var outstanding in outstandingDetail.TopOutstandings)
            {
                textContainer.Add($"{outstanding.CustName}  {GetFormattedValue(outstanding.MemberOutstanding)}  {GetFormattedValue(outstanding.Outstanding)}");
            }

            return string.Join(messengerFormatter.NewLine(), textContainer);
        }
    }
}