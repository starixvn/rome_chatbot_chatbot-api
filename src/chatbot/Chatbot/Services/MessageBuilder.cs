namespace Chatbot.Services
{
    using System.Collections.Generic;
    using System.Globalization;
    using Chatbot.AppResources;
    using Chatbot.Constants;
    using Chatbot.Templates;

    public interface IMessageBuilder
    {
        string BuildVerificationCodeMessage(string custName, string language, string verificationCode, int expiredVerificationCodeInMinutes);

        string BuildCommandResultMessage(string language, int functionId, int messengerType, object data);

        string BuildNotRecognizedCommandMessage(string language);

        string BuildWelcomeMessage(string language);

        string BuildErrorMessage(string language);
    }

    public class MessageBuilder : IMessageBuilder
    {
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

        private IMessengerFormatter GetMessengerFormatter(int messengerType)
            => messengerFormatters.ContainsKey(messengerType)
                            ? messengerFormatters[messengerType]
                            : new LetstalkFormatter();

        private CultureInfo GetCultureInfo(string language)
            => cultureInfoList.ContainsKey(language)
                            ? cultureInfoList[language]
                            : defaultCultureInfo;

        public string BuildVerificationCodeMessage(string custName, string language, string verificationCode, int expiredVerificationCodeInMinutes)
            => string.Join(
                "\n",
                $"You have requested to setup Chatbot for your account {custName}.",
                $"Your verification code is {verificationCode}.",
                $"The code will be expired in {expiredVerificationCodeInMinutes} minutes.",
                "To secure your information, please delete this message after completing verification.");

        private IMessageTemplate GetTemplate(int functionId)
            => messageTemplates.ContainsKey(functionId)
                ? messageTemplates[functionId]
                : defaultTemplate;
                

        public string BuildErrorMessage(string language)
            => GetResource(nameof(Languages.DefaultErrorMessage), language);

        public string BuildNotRecognizedCommandMessage(string language)
            => GetResource(nameof(Languages.CommandIsNotRecognized), language);

        public string BuildWelcomeMessage(string language)
            => GetResource(nameof(Languages.InformingVerifiedMessage), language);

        private string GetResource(string key, string language)
            => Languages.ResourceManager.GetString(key, GetCultureInfo(language));
    }
}