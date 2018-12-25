namespace Chatbot.Modules.MessageBuilders
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Chatbot.AppResources;
    using Chatbot.Models;
    using Chatbot.Modules.MessageBuilders.Templates;
    using Chatbot.Shared.Extensions;
    using Chatbot.Shared.Helpers;

    public interface IMessageBuilder
    {
        string BuildVerificationCodeMessage(string language, int messengerType, string verificationCode, int expiredVerificationCodeInMinutes);

        string BuildCommandResultMessage(string language, int functionId, int messengerType, object data);

        string BuildNotRecognizedCommandMessage(string language);

        string BuildAccountClosedMessage(string language);

        string BuildWelcomeMessage(string language, int messengerType);

        string BuildErrorMessage(string language);

        string BuildUnderMaintenanceMessage(int messengerType, string from, string to);

        string BuildServiceIsUnavailableMessage(int messengerType);

        string BuildInvalidCustNameMessage(string language);

        string BuildUnlinkMessage(string language);

        string BuildInvalidNumerOfDownline(string language);

        string BuildInvalidNumerOfDownlineRange(string language, int maxTopQuery);

        string BuildInvalidDownlineMessage(string username, string language);

        string BuildInvalidDirectDownlineMessage(string username, int directDownlineLevel, string language);

        string BuildRootActionTitle(string language);

        string BuildCancelRequestMessage(string language);

        string BuildResponseCancelRequestMessage(int messengerType, string language);

        string BuildConfirmKeywordTitle(string language);

        string BuildNoneOfTheAboveMessage(string language);

        string BuildInvalidNumericMessage(string value, string language);

        string BuildInvalidMaxCreditMessage(int custLevel, decimal maxCredit, string language);

        string BuildCopyCustomerInformMessage(string language);

        string BuildCopyCustomerReachedLimitMessage(int limit, string language);

        string BuildCopyCustomerInstructionMessage(string username, int custLevel, int messengerType, string language);

        string BuildConfirmUpdateAction(string actionName, int messengerType, string language);
    }

    public class MessageBuilder : IMessageBuilder
    {
        private static readonly IEnumerable<string> unavailableLanguageCodes = new List<string>
        {
            "en-US",
            "zh-CN",
            "zh-TW",
            "vi-VN"
        };

        private static readonly IEnumerable<string> underMaintenanceLanguageCodes = new List<string>
        {
            "en-US",
            "zh-CN",
            "zh-TW"
        };

        private static readonly IDictionary<int, string> CopyCustomerCommandFormats = new Dictionary<int, string>
        {
            [CustLevel.MasterLevel] = "/copy {0}01 1000",
            [CustLevel.AgentLevel] = "/copy {0}01 1000 100",
            [CustLevel.MemberLevel] = "/copy {0}001 1000"
        };

        private static readonly CultureInfo defaultCultureInfo = CultureInfo.CreateSpecificCulture("en-US");
        private static readonly IMessageTemplate defaultTemplate = new MessageTemplate(new HeaderTemplate(), new BodyTemplate());

        private readonly IDictionary<int, IMessengerFormatter> messengerFormatters;
        private readonly IDictionary<string, CultureInfo> cultureInfoList;
        private readonly IDictionary<int, IMessageTemplate> messageTemplates;

        public MessageBuilder(
            IDictionary<string, CultureInfo> cultureInfoList,
            IDictionary<int, IMessageTemplate> messageTemplates,
            IDictionary<int, IMessengerFormatter> messengerFormatters)
        {
            this.cultureInfoList = cultureInfoList;
            this.messageTemplates = messageTemplates;
            this.messengerFormatters = messengerFormatters;
        }

        public string BuildCommandResultMessage(
            string language,
            int functionId,
            int messengerType,
            object data)
        {
            if (data == null)
            {
                return StaticResources.Get(nameof(Languages.NoData), language);
            }

            var template = GetTemplate(functionId);
            var cultureInfo = GetCultureInfo(language);
            var messengerFormatter = GetMessengerFormatter(messengerType);

            return template.ToString(functionId, data, cultureInfo, messengerFormatter);
        }

        public string BuildVerificationCodeMessage(string language, int messengerType, string verificationCode, int expiredVerificationCodeInMinutes)
            => string.Join(
                GetMessengerFormatter(messengerType).NewLine(),
                string.Format(StaticResources.Get(nameof(Languages.VerificationCode_1), language), verificationCode),
                string.Format(StaticResources.Get(nameof(Languages.VerificationCode_2), language), expiredVerificationCodeInMinutes));

        public string BuildErrorMessage(string language)
            => StaticResources.Get(nameof(Languages.DefaultErrorMessage), language);

        public string BuildNotRecognizedCommandMessage(string language)
            => StaticResources.Get(nameof(Languages.CommandIsNotRecognized), language);

        public string BuildWelcomeMessage(string language, int messengerType)
            => string.Join(
                GetMessengerFormatter(messengerType).NewLine(),
                StaticResources.Get(nameof(Languages.InformingVerifiedMessage_1), language),
                StaticResources.Get(nameof(Languages.InformingVerifiedMessage_2), language));

        public string BuildAccountClosedMessage(string language)
            => StaticResources.Get(nameof(Languages.YourAccountIsClosed), language);

        public string BuildUnderMaintenanceMessage(int messengerType, string from, string to)
        {
            var stringBuilder = new StringBuilder();

            foreach (var langCode in underMaintenanceLanguageCodes)
            {
                var message = string.Format(StaticResources.Get(nameof(Languages.UnderMaintenance), langCode), from, to);
                stringBuilder
                    .Append(message)
                    .Append(GetMessengerFormatter(messengerType).NewLine())
                    .Append(GetMessengerFormatter(messengerType).NewLine());
            }

            return stringBuilder.ToString().TrimEnd();
        }

        public string BuildServiceIsUnavailableMessage(int messengerType)
        {
            var stringBuilder = new StringBuilder();

            foreach (var langCode in unavailableLanguageCodes)
            {
                stringBuilder
                    .Append(StaticResources.Get(nameof(Languages.ServiceNotAvailable), langCode))
                    .Append(GetMessengerFormatter(messengerType).NewLine())
                    .Append(GetMessengerFormatter(messengerType).NewLine());
            }

            return stringBuilder.ToString();
        }

        public string BuildInvalidCustNameMessage(string language)
            => StaticResources.Get(nameof(Languages.InvalidCustName), language);

        public string BuildUnlinkMessage(string language)
            => StaticResources.Get(nameof(Languages.UnlinkMessage), language);

        public string BuildInvalidNumerOfDownline(string language)
            => StaticResources.Get(nameof(Languages.InvalidNumerOfDownlines), language);

        public string BuildInvalidNumerOfDownlineRange(string language, int maxTopQuery)
            => string.Format(
                StaticResources.Get(nameof(Languages.InvalidNumerOfDownlinesRange), language),
                maxTopQuery);

        public string BuildInvalidDownlineMessage(string username, string language)
            => string.Format(
                StaticResources.Get(nameof(Languages.InvalidDownline), language),
                username);

        public string BuildInvalidDirectDownlineMessage(string username, int directDownlineLevel, string language)
            => string.Format(
                StaticResources.Get(nameof(Languages.InvalidDirectDownline), language),
                username,
                StaticResources.Get($"CustLevel_{directDownlineLevel}", language));

        public string BuildInvalidNumericMessage(string value, string language)
            => string.Format(StaticResources.Get(nameof(Languages.InvalidNumericMessage), language), value);

        private IMessengerFormatter GetMessengerFormatter(int messengerType)
            => messengerFormatters.ContainsKey(messengerType)
                            ? messengerFormatters[messengerType]
                            : new LetstalkFormatter();

        private IMessageTemplate GetTemplate(int functionId)
           => messageTemplates.ContainsKey(functionId)
               ? messageTemplates[functionId]
               : defaultTemplate;

        private CultureInfo GetCultureInfo(string language)
            => cultureInfoList.ContainsKey(language)
                            ? cultureInfoList[language]
                            : defaultCultureInfo;

        public string BuildRootActionTitle(string language)
            => StaticResources.Get(nameof(Languages.RootActionTitle), language);

        public string BuildCancelRequestMessage(string language)
             => StaticResources.Get(nameof(Languages.CancelThisRequest), language);

        public string BuildResponseCancelRequestMessage(int messengerType, string language)
         => string.Join(
                GetMessengerFormatter(messengerType).NewLine(),
                StaticResources.Get(nameof(Languages.FunctionHasBeenCancelled), language),
                StaticResources.Get(nameof(Languages.WhichActionWouldYouLikeToTake), language));

        public string BuildConfirmKeywordTitle(string language)
            => StaticResources.Get(nameof(Languages.ConfirmKeywordTitle), language);

        public string BuildNoneOfTheAboveMessage(string language)
            => StaticResources.Get(nameof(Languages.NoneOfTheAbove), language);

        public string BuildInvalidMaxCreditMessage(int custLevel, decimal maxCredit, string language)
            => string.Format(StaticResources.Get($"MaxCreditInvalid_CustLevel_{custLevel}", language), 0, maxCredit.ToFormattedNumber());

        public string BuildCopyCustomerInformMessage(string language)
            => StaticResources.Get(nameof(Languages.CopyCustomerInformMessage), language);

        public string BuildCopyCustomerReachedLimitMessage(int limit, string language)
            => string.Format(StaticResources.Get(nameof(Languages.CopyCustomerReachedLimitMessage), language), limit);

        public string BuildCopyCustomerInstructionMessage(string username, int custLevel, int messengerType, string language)
            => string.Join(
                GetMessengerFormatter(messengerType).NewLine(),
                StaticResources.Get(nameof(Languages.InstructionMessage), language),
                StaticResources.Get($"InstructionCopyCustomerCommand_CustLevel_{custLevel}", language),
                StaticResources.Get(nameof(Languages.Example), language),
                string.Format(CopyCustomerCommandFormats[custLevel], username));

        public string BuildConfirmUpdateAction(string actionName, int messengerType, string language)
            => string.Join(
                GetMessengerFormatter(messengerType).NewLine(),
                StaticResources.Get(nameof(Languages.Yes), language),
                actionName);
    }
}