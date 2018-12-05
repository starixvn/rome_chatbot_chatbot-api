namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Alpha.Services.Client.Models;
    using Chatbot.AppResources;
    using Chatbot.Shared.Helpers;

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

        private static void AddHeaderMessage(
            List<string> textContainer,
            bool isTopXOutstanding,
            OutstandingDetail outstandingDetail,
            CultureInfo cultureInfo,
            IMessengerFormatter messengerFormatter)
        {
            if (!isTopXOutstanding)
            {
                textContainer.Add($"{StaticResources.Get("For", cultureInfo)} {outstandingDetail.DownlineName}");
            }

            textContainer.Add(string.Format("{0}  /  {1}  {2}  {3}",
                StaticResources.Get(isTopXOutstanding ? $"CustLevel_{outstandingDetail.CustLevel - 1}" : "Products", cultureInfo),
                StaticResources.Get(nameof(Languages.Member), cultureInfo), messengerFormatter.Point(),
                $"{StaticResources.Get($"CustLevel_{outstandingDetail.CustLevel}", cultureInfo)}{messengerFormatter.NewLine()}"));
            textContainer.Add(StaticResources.Get("Total", cultureInfo).ToUpperInvariant());
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

        private static void AddHiddenDownlinesCount(
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

        private static void AddHint(
            List<string> textContainer,
            CultureInfo cultureInfo,
            IMessengerFormatter messengerFormatter)
        {
            if (IsShowHint())
            {
                textContainer.Add($"{messengerFormatter.NewLine()}{BuildHintMessage(cultureInfo)}");
            }
        }

        private static string BuildHintMessage(CultureInfo cultureInfo)
        {
            var random = new Random();
            if (random.Next(0, 2) == 0)
            {
                return StaticResources.Get(nameof(Languages.HintTopOutstanding), cultureInfo);
            }

            return StaticResources.Get(nameof(Languages.HintSpecificDownline), cultureInfo);
        }
    }
}