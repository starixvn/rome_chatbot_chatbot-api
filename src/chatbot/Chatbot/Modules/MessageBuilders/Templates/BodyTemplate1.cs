namespace Chatbot.Modules.MessageBuilders.Templates
{
    using Alpha.Services.Client.Models;
    using Chatbot.AppResources;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class BodyTemplate1 : BodyTemplate
    {
        public override string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var outstandingDetail = (OutstandingDetail)data;
            var isTopXOutstanding = string.IsNullOrWhiteSpace(outstandingDetail.DownlineName);
            var textContainer = new List<string>();

            AddHeaderMessage(textContainer, isTopXOutstanding, outstandingDetail, cultureInfo, messengerFormatter);

            AddTotalBodyMessage(textContainer, outstandingDetail.Total, messengerFormatter);
            AddBodyMessage(textContainer, isTopXOutstanding, outstandingDetail.TopOutstandings, messengerFormatter);

            AddHiddenDownlinesCount(textContainer, outstandingDetail, cultureInfo);

            AddHint(textContainer, cultureInfo, messengerFormatter);

            return string.Join(messengerFormatter.NewLine(), textContainer);
        }

        private void AddHeaderMessage(
            List<string> textContainer,
            bool isTopXOutstanding,
            OutstandingDetail outstandingDetail,
            CultureInfo cultureInfo,
            IMessengerFormatter messengerFormatter)
        {
            if (!isTopXOutstanding)
            {
                textContainer.Add($"{GetResource("For", cultureInfo)} {outstandingDetail.DownlineName}");
            }

            textContainer.Add(string.Format("{0}  /  {1}  {2}  {3}",
                GetResource(isTopXOutstanding ? $"CustLevel_{outstandingDetail.CustLevel - 1}" : "Products", cultureInfo),
                GetResource(nameof(Languages.Member), cultureInfo), messengerFormatter.Point(),
                $"{GetResource($"CustLevel_{outstandingDetail.CustLevel}", cultureInfo)}{messengerFormatter.NewLine()}"));
            textContainer.Add(GetResource("Total", cultureInfo).ToUpperInvariant());
        }

        private static void AddTotalBodyMessage(
            List<string> textContainer,
            OutstandingModel total,
            IMessengerFormatter messengerFormatter)
            => textContainer.Add($"    {GetFormattedValue(total.MemberOutstanding)}  {messengerFormatter.Point()}  {GetFormattedValue(total.Outstanding)}");

        private static void AddBodyMessage(
            List<string> textContainer,
            bool isTopXOutstanding,
            IEnumerable<OutstandingModel> outstandings,
            IMessengerFormatter messengerFormatter)
        {
            var index = 1;

            foreach (var outstanding in outstandings)
            {
                textContainer.Add(isTopXOutstanding ? $"{index}. {outstanding.Name}" : outstanding.Name);
                textContainer.Add($"    {GetFormattedValue(outstanding.MemberOutstanding)}  {messengerFormatter.Point()}  {GetFormattedValue(outstanding.Outstanding)}");

                index++;
            }
        }

        private void AddHiddenDownlinesCount(
            List<string> textContainer,
            OutstandingDetail outstandingDetail,
            CultureInfo cultureInfo)
        {
            if (outstandingDetail.HiddenDownlinesCount > 0)
            {
                var hiddenDownlinesMessage = BuildHiddenDownlinesMessage(
                    outstandingDetail.HiddenDownlinesCount,
                    outstandingDetail.CustLevel - 1,
                    cultureInfo);

                textContainer.Add(hiddenDownlinesMessage);
            }
        }

        private string BuildHiddenDownlinesMessage(int count, int custLevel, CultureInfo cultureInfo)
            => string.Format(GetResource(nameof(Languages.HiddenDownlinesCount), cultureInfo),
                count,
                GetResource($"CustLevel_{custLevel}", cultureInfo));

        private void AddHint(
            List<string> textContainer,
            CultureInfo cultureInfo,
            IMessengerFormatter messengerFormatter)
        {
            if (IsShowHint())
            {
                textContainer.Add($"{messengerFormatter.NewLine()}{BuildHintMessage(cultureInfo)}");
            }
        }

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
    }
}