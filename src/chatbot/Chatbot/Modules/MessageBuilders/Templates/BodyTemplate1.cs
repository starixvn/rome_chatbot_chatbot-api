namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Alpha.Services.Client.Models;
    using Chatbot.AppResources;

    public class BodyTemplate1 : BodyTemplate
    {
        public override string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            if (!(data is OutstandingDetail outstandingDetail))
            {
                return BuildDownlineOutstandings((IEnumerable<DownlineOutstanding>)data, cultureInfo, messengerFormatter);
            }

            var messageHeader = string.Format("{0}  /  {1}  {2}  {3}",
                GetResource($"CustLevel_{outstandingDetail.CustLevel - 1}", cultureInfo),
                GetResource(nameof(Languages.Member), cultureInfo), messengerFormatter.Point(),
                GetResource($"CustLevel_{outstandingDetail.CustLevel}", cultureInfo));

            var textContainer = new List<string>
            {
                $"{messageHeader}{messengerFormatter.NewLine()}",
                GetResource("Total", cultureInfo).ToUpperInvariant()
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

            if (outstandingDetail.HiddenDownlinesCount > 0)
            {
                var hiddenDownlinesMessage = BuildHiddenDownlinesMessage(
                    outstandingDetail.HiddenDownlinesCount,
                    outstandingDetail.CustLevel - 1,
                    cultureInfo);

                textContainer.Add(hiddenDownlinesMessage);
            }

            if (IsShowHint())
            {
                var hintMessage = BuildHintMessage(cultureInfo);
                textContainer.Add($"{messengerFormatter.NewLine()}{hintMessage}");
            }

            return string.Join(messengerFormatter.NewLine(), textContainer);
        }

        private string BuildHiddenDownlinesMessage(int count, int custLevel, CultureInfo cultureInfo)
            => string.Format(GetResource(nameof(Languages.HiddenDownlinesCount), cultureInfo),
                count,
                GetResource($"CustLevel_{custLevel}", cultureInfo));

        /// <summary>
        /// Random rate of 20%
        /// </summary>
        /// <returns></returns>
        private static bool IsShowHint()
        {
            var random = new Random();
            return random.Next(0, 5) == 0;
        }

        private string BuildHintMessage(CultureInfo cultureInfo)
        {
            var random = new Random();
            if (random.Next(0, 2) == 0)
            {
                return GetResource(nameof(Languages.HintTopOutstanding), cultureInfo);
            }

            return GetResource(nameof(Languages.HintSpecificDownline), cultureInfo);
        }

        private string BuildDownlineOutstandings(
            IEnumerable<DownlineOutstanding> outstandings,
            CultureInfo cultureInfo,
            IMessengerFormatter messengerFormatter)
        {
            //var messageHeader = string.Format("{0}  /  {1}  {2}  {3}",
            //    GetResource($"CustLevel_{outstandingDetail.CustLevel - 1}", cultureInfo),
            //    GetResource(nameof(Languages.Member), cultureInfo), messengerFormatter.Point(),
            //    GetResource($"CustLevel_{outstandingDetail.CustLevel}", cultureInfo));

            //var textContainer = new List<string>
            //{
            //    messageHeader,
            //    "------------------------------------",
            //    $"{messengerFormatter.Point()}{GetResource("Total", cultureInfo)}{messengerFormatter.Point()}"
            //};

            //foreach (var outstanding in outstandings)
            //{
            //    textContainer.Add($"{outstanding.ProductName}");
            //    textContainer.Add($"    {GetFormattedValue(outstanding.MemberOutstanding)}  {messengerFormatter.Point()}  {GetFormattedValue(outstanding.Outstanding)}");
            //}

            return null;
        }
    }
}