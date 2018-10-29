namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System.Collections.Generic;
    using System.Globalization;
    using Alpha.Services.Client.Models;
    using Chatbot.Helpers;

    public class BodyTemplate1 : BodyTemplate
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
                $"{GetResource("Username", cultureInfo)} / {GetResource("Member", cultureInfo)} {messengerFormatter.Point()} {GetResource(outstandingDetail.CustLevel, cultureInfo)}",
                "------------------------------------",
                $"{messengerFormatter.Point()}{GetResource("Total", cultureInfo)}{messengerFormatter.Point()}"
            };

            var total = outstandingDetail.Total;
            textContainer.Add($"    {GetFormattedValue(total.MemberOutstanding)}  {messengerFormatter.Point()}  {GetFormattedValue(total.Outstanding)}");

            var index = 1;
            foreach (var outstanding in outstandingDetail.TopOutstandings)
            {
                textContainer.Add($"{index}. {outstanding.CustName}");
                textContainer.Add($"    {GetFormattedValue(outstanding.MemberOutstanding)}  {messengerFormatter.Point()}  {GetFormattedValue(outstanding.Outstanding)}");

                index++;
            }

            return string.Join(messengerFormatter.NewLine(), textContainer);
        }
    }
}