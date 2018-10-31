namespace Chatbot.Modules.MessageBuilders
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Chatbot.AppResources;
    using Chatbot.Modules.MessageBuilders.Templates;

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
                return GetResource(nameof(Languages.NoData), language);
            }

            var template = GetTemplate(functionId);
            var cultureInfo = GetCultureInfo(language);
            var messengerFormatter = GetMessengerFormatter(messengerType);

            return template.ToString(functionId, data, cultureInfo, messengerFormatter);
        }

        public string BuildVerificationCodeMessage(string language, int messengerType, string verificationCode, int expiredVerificationCodeInMinutes)
            => string.Join(
                GetMessengerFormatter(messengerType).NewLine(),
                string.Format(GetResource(nameof(Languages.VerificationCode_1), language), verificationCode),
                string.Format(GetResource(nameof(Languages.VerificationCode_2), language), expiredVerificationCodeInMinutes));

        public string BuildErrorMessage(string language)
            => GetResource(nameof(Languages.DefaultErrorMessage), language);

        public string BuildNotRecognizedCommandMessage(string language)
            => GetResource(nameof(Languages.CommandIsNotRecognized), language);

        public string BuildWelcomeMessage(string language, int messengerType)
            => string.Join(
                GetMessengerFormatter(messengerType).NewLine(),
                GetResource(nameof(Languages.InformingVerifiedMessage_1), language),
                GetResource(nameof(Languages.InformingVerifiedMessage_2), language));

        public string BuildAccountClosedMessage(string language)
            => GetResource(nameof(Languages.YourAccountIsClosed), language);

        public string BuildUnderMaintenanceMessage(int messengerType, string from, string to)
        {
            var stringBuilder = new StringBuilder();

            foreach (var langCode in underMaintenanceLanguageCodes)
            {
                var message = string.Format(GetResource(nameof(Languages.UnderMaintenance), langCode), from, to);
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
                    .Append(GetResource(nameof(Languages.ServiceNotAvailable), langCode))
                    .Append(GetMessengerFormatter(messengerType).NewLine())
                    .Append(GetMessengerFormatter(messengerType).NewLine());
            }

            return stringBuilder.ToString();
        }

        public string BuildInvalidCustNameMessage(string language)
            => GetResource(nameof(Languages.InvalidCustName), language);

        public string BuildUnlinkMessage(string language)
            => GetResource(nameof(Languages.UnlinkMessage), language);

        private IMessengerFormatter GetMessengerFormatter(int messengerType)
            => messengerFormatters.ContainsKey(messengerType)
                            ? messengerFormatters[messengerType]
                            : new LetstalkFormatter();

        private IMessageTemplate GetTemplate(int functionId)
           => messageTemplates.ContainsKey(functionId)
               ? messageTemplates[functionId]
               : defaultTemplate;

        private string GetResource(string key, string language)
            => Languages.ResourceManager.GetString(key, GetCultureInfo(language));

        private CultureInfo GetCultureInfo(string language)
            => cultureInfoList.ContainsKey(language)
                            ? cultureInfoList[language]
                            : defaultCultureInfo;
    }
}