namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System.Collections.Generic;
    using System.Globalization;
    using Alpha.Services.Client.Models;
    using Chatbot.Helpers;

    public class BodyTemplate9 : BodyTemplate
    {
        private const int MaxLength = 50;

        public override string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var outstandingDetail = DataHelper.Parse<OutstandingDetail>(data);

            if (outstandingDetail == null)
            {
                return base.ToString(data, cultureInfo, messengerFormatter);
            }

            //var textContainer = new List<string>();

            //var total = outstandingDetail.Total;
            //textContainer.Add($"[Member total]  >>  {GetFormattedValue(total.MemberOutstanding)}");
            //textContainer.Add($"[{GetResource(outstandingDetail.CustLevel, cultureInfo)} total]  >>  {GetFormattedValue(total.Outstanding)}");

            //textContainer.Add("------------------------------------");

            //var index = 1;
            //foreach (var outstanding in outstandingDetail.TopOutstandings)
            //{
            //    textContainer.Add($"{index}. {outstanding.CustName}  >> {GetFormattedValue(outstanding.MemberOutstanding)}");
            //    textContainer.Add($"   [{GetResource(outstandingDetail.CustLevel, cultureInfo)}]  >>  {GetFormattedValue(outstanding.Outstanding)}");

            //    index++;
            //}

            //var textContainer = new List<string>();

            //textContainer.Add("Username / Member  Agent");
            //textContainer.Add("------------------------------------");
            //textContainer.Add("\u2022Total\u2022");

            //var total = outstandingDetail.Total;
            //textContainer.Add($"    {GetFormattedValue(total.MemberOutstanding)}  \u2022  {GetFormattedValue(total.Outstanding)}");

            //var index = 1;
            //foreach (var outstanding in outstandingDetail.TopOutstandings)
            //{
            //    textContainer.Add($"{index}. {outstanding.CustName}");
            //    textContainer.Add($"    {GetFormattedValue(outstanding.MemberOutstanding)}  \u2022  {GetFormattedValue(outstanding.Outstanding)}");

            //    index++;
            //}

            var textContainer = new List<string>();

            textContainer.Add("Username / Member  Agent");
            textContainer.Add("------------------------------------");
            textContainer.Add("\u2022Total\u2022");

            var total = outstandingDetail.Total;
            textContainer.Add(AlignRight(total.MemberOutstanding));
            textContainer.Add(AlignRight(total.Outstanding));

            var index = 1;
            foreach (var outstanding in outstandingDetail.TopOutstandings)
            {
                textContainer.Add($"{index}. {outstanding.CustName}");
                textContainer.Add(AlignRight(outstanding.MemberOutstanding));
                textContainer.Add(AlignRight(outstanding.Outstanding));
                index++;
            }

            return string.Join(messengerFormatter.NewLine(), textContainer);
        }

        private string AlignRight(object value)
        {
            var formatedValue = GetFormattedValue(value);

            return string.Format("{0," + (MaxLength - formatedValue.Length) + "}", formatedValue);
        }
    }
}