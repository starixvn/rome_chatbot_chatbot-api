namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Alpha.Services.Client.Models;
    using Chatbot.AppResources;
    using Chatbot.Models;
    using Chatbot.Shared.Extensions;
    using Chatbot.Shared.Helpers;

    public class BodyTemplate9 : BodyTemplate
    {
        public override string ToString(object data, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var result = DataHelper.Parse<CopyCustomerResult>(data);

            if (!result.IsSuccess)
            {
                return result.Message;
            }

            var successMessage = string.Format(
                StaticResources.Get($"CopyCustomerSuccessMessage_CustLevel_{result.CustLevel}", cultureInfo),
                result.SourceUsername);

            var textContainer = new List<string>
            {
                $"{successMessage}{messengerFormatter.NewLine()}",
                $"{StaticResources.Get(nameof(Languages.Username), cultureInfo)}: {result.CustName}",
                $"{StaticResources.Get(nameof(Languages.Password), cultureInfo)}: {result.Password}",
                $"{StaticResources.Get(nameof(Languages.credit), cultureInfo)}: {result.Credit.ToFormattedNumber()}",
            };

            if (result.CustLevel == CustLevel.AgentLevel)
            {
                textContainer.Add($"{StaticResources.Get(nameof(Languages.maxMemberCredit), cultureInfo)}: {result.MaxCredit.ToFormattedNumber()}");
            }

            if (IsShowHint())
            {
                textContainer.Add(BuildHintCommand(result, cultureInfo, messengerFormatter));
            }

            return string.Join(messengerFormatter.NewLine(), textContainer);
        }

        private static string BuildHintCommand(CopyCustomerResult result, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(messengerFormatter.NewLine());
            stringBuilder.Append($"{StaticResources.Get(nameof(Languages.HintMessage), cultureInfo)}{messengerFormatter.NewLine()}");

            var hintCommand = result.CustLevel == CustLevel.AgentLevel
                  ? $"/copy {result.SourceUsername} {result.Credit} {result.MaxCredit}"
                  : $"/copy {result.SourceUsername} {result.Credit}";
            stringBuilder.Append(hintCommand);

            return stringBuilder.ToString();
        }
    }
}