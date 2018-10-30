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
                return base.ToString(data, cultureInfo, messengerFormatter);
            }

            var messageHeader = string.Format("{0}  /  {1}  {2}  {3}",
                GetResource($"CustLevel_{outstandingDetail.CustLevel - 1}", cultureInfo),
                GetResource(nameof(Languages.Member), cultureInfo), messengerFormatter.Point(),
                GetResource($"CustLevel_{outstandingDetail.CustLevel}", cultureInfo));

            var textContainer = new List<string>
            {
                messageHeader,
                "------------------------------------------",
                $"{messengerFormatter.Point()}{GetResource("Total", cultureInfo)}{messengerFormatter.Point()}"
            };

            var total = outstandingDetail.Total;
            textContainer.Add($"{messengerFormatter.Tab()}{GetFormattedValue(total.MemberOutstanding)}  {messengerFormatter.Point()}  {GetFormattedValue(total.Outstanding)}");

            var index = 1;
            foreach (var outstanding in outstandingDetail.TopOutstandings)
            {
                textContainer.Add($"{index}. {outstanding.CustName}");
                textContainer.Add($"{messengerFormatter.Tab()}{GetFormattedValue(outstanding.MemberOutstanding)}  {messengerFormatter.Point()}  {GetFormattedValue(outstanding.Outstanding)}");

                index++;
            }

            if (outstandingDetail.HiddenDownlinesCount > 0)
            {
                var hiddenDownlinesMessage = string.Format(
                    GetResource(nameof(Languages.HiddenDownlinesCount),
                    cultureInfo), outstandingDetail.HiddenDownlinesCount,
                    GetResource($"CustLevel_{outstandingDetail.CustLevel - 1}", cultureInfo));

                textContainer.Add(hiddenDownlinesMessage);
            }

            if (IsShowHint())
            {
                var hintMessage = string.Format(
                    GetResource(nameof(Languages.HintMessage), cultureInfo),
                    GetResource($"CustLevel_{outstandingDetail.CustLevel - 1}", cultureInfo));
                textContainer.Add(hintMessage);
            }

            return string.Join(messengerFormatter.NewLine(), textContainer);
        }

        /// <summary>
        /// Random rate of 20%
        /// </summary>
        /// <returns></returns>
        private static bool IsShowHint()
        {
            var random = new Random();
            var value = random.Next(1, 6);
            return value == 1;
        }
    }
}