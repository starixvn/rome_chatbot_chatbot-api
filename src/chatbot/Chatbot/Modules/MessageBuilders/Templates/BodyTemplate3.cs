namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System.Collections.Generic;
    using System.Globalization;
    using Alpha.Services.Client.Models;
    using Chatbot.AppResources;
    using Chatbot.Shared.Extensions;
    using Chatbot.Shared.Helpers;

    public class BodyTemplate3 : BodyTemplate
    {
        private const string BalanceRowFormat = "    {0}  {1}  {2}";

        public override string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var bodyMessage = string.Empty;

            bodyMessage = data is BalanceTotal balanceTotal
                ? BuildTopDownlinesBodyMessage(balanceTotal, cultureInfo, messengerFormatter)
                : base.ToString(data, cultureInfo, messengerFormatter);

            var newLine = messengerFormatter.NewLine();

            return IsShowHint()
                ? $"{bodyMessage}{newLine}{newLine}{BuildHint(cultureInfo)}"
                : bodyMessage;
        }

        private static string BuildTopDownlinesBodyMessage(BalanceTotal balanceTotal, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var textContainer = new List<string>
            {
                StaticResources.Get($"CustLevel_{balanceTotal.CustLevel}", cultureInfo)
            };

            textContainer.Add(
                string.Format(
                    BalanceRowFormat,
                    StaticResources.Get(nameof(Languages.Total), cultureInfo),
                    messengerFormatter.Point(),
                    StaticResources.Get(nameof(Languages.yesterday), cultureInfo)
                ));

            textContainer.Add(StaticResources.Get(nameof(Languages.Total), cultureInfo).ToUpperInvariant());
            textContainer.Add(
                string.Format(
                    BalanceRowFormat,
                    balanceTotal.Total.Balance.ToFormattedNumber(),
                    messengerFormatter.Point(),
                    balanceTotal.Total.YesterdayBalance.ToFormattedNumber()));

            var index = 1;
            foreach (var balance in balanceTotal.Balances)
            {
                textContainer.Add($"{index}. {balance.Username}");
                textContainer.Add(string.Format(
                    BalanceRowFormat,
                    balance.Balance.ToFormattedNumber(),
                    messengerFormatter.Point(),
                    balance.YesterdayBalance.ToFormattedNumber()));

                index++;
            }

            if (balanceTotal.HiddenDownlinesCount > 0)
            {
                var hiddenDownlinesMessage = BuildHiddenDownlinesMessage(
                    balanceTotal.HiddenDownlinesCount,
                    balanceTotal.CustLevel - 1,
                    cultureInfo);

                textContainer.Add(hiddenDownlinesMessage);
            }

            return string.Join(messengerFormatter.NewLine(), textContainer);
        }

        private static string BuildHint(CultureInfo culture)
            => StaticResources.Get(nameof(Languages.BalanceTopQueryHint), culture);
    }
}