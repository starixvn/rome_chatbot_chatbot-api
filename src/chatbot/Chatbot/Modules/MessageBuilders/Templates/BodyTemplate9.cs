namespace Chatbot.Modules.MessageBuilders.Templates
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Alpha.Services.Client.Models;
    using Chatbot.AppResources;
    using Chatbot.Models;
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
                GetResource($"CopyCustomerSuccessMessage_CustLevel_{result.CustLevel}", cultureInfo),
                result.SourceUsername);

            var textContainer = new List<string>
            {
                $"{successMessage}{messengerFormatter.NewLine()}",
                $"{GetResource(nameof(Languages.Username),cultureInfo)}: {result.CustName}",
                $"{GetResource(nameof(Languages.Password),cultureInfo)}: {result.Password}",
                IsShowHint() ? BuildHintCommand(result, cultureInfo, messengerFormatter) : string.Empty
            };

            return string.Join(messengerFormatter.NewLine(), textContainer.Where(s => !string.IsNullOrEmpty(s)));
        }

        private string BuildHintCommand(CopyCustomerResult result, CultureInfo cultureInfo, IMessengerFormatter messengerFormatter)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(messengerFormatter.NewLine());
            stringBuilder.Append($"{GetResource(nameof(Languages.HintMessage), cultureInfo)}{messengerFormatter.NewLine()}");

            var hintCommand = result.CustLevel == CustLevel.Agent.Value
                  ? $"/copy {result.SourceUsername} {result.Credit} {result.MaxCredit}"
                  : $"/copy {result.SourceUsername} {result.Credit}";
            stringBuilder.Append(hintCommand);

            return stringBuilder.ToString();
        }
    }
}