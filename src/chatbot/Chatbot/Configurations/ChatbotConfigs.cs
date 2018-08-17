namespace Chatbot.Configurations
{
    using System;
    using System.ComponentModel;
    using Microsoft.Extensions.Configuration;

    public interface IChatbotConfigs
    {
        int MaxAddActionTimes { get; }

        int AddAccountActionId { get; }

        int WrongVerificationCodeActionId { get; }

        int VerificationCodeExpiredTimeInMinutes { get; }

        int OutstandingFunctionId { get; }

        int WinlossFunctionId { get; }

        int BalanceFunctionId { get; }

        bool ReturnRawErrorToMessenger { get; }

        string SpDirectory { get; }
    }

    public class ChatbotConfigs : IChatbotConfigs
    {
        private readonly IConfiguration configuration;

        public ChatbotConfigs(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            MaxAddActionTimes = GetValue<int>("Chatbot:MaxAddActionTimes");
            AddAccountActionId = GetValue<int>("Chatbot:AddAccountActionId");
            WrongVerificationCodeActionId = GetValue<int>("Chatbot:WrongVerificationCodeActionId");
            VerificationCodeExpiredTimeInMinutes = GetValue<int>("Chatbot:VerificationCodeExpiredTimeInMinutes");
            OutstandingFunctionId = GetValue<int>("Chatbot:OutstandingFunctionId");
            WinlossFunctionId = GetValue<int>("Chatbot:WinlossFunctionId");
            BalanceFunctionId = GetValue<int>("Chatbot:BalanceFunctionId");
            ReturnRawErrorToMessenger = GetValue<bool>("Chatbot:ReturnRawErrorToMessenger");
            SpDirectory = GetValue<string>("Chatbot:SpDirectory");
            YesterdayWinLossFunctionId = GetValue<int>("Chatbot:YesterdayWinLossFunctionId");
            PersonalMessageFunctionId = GetValue<int>("Chatbot:PersonalMessageFunctionId");
            SpecialAnnouncementFunctionId = GetValue<int>("Chatbot:SpecialAnnouncementFunctionId");
            NormalAnnouncementFunctionId = GetValue<int>("Chatbot:NormalAnnouncementFunctionId");
            SystemUpdateFunctionId = GetValue<int>("Chatbot:SystemUpdateFunctionId");
        }

        public int MaxAddActionTimes { get; }

        public int AddAccountActionId { get; }

        public int WrongVerificationCodeActionId { get; }

        public int VerificationCodeExpiredTimeInMinutes { get; }

        public int OutstandingFunctionId { get; }

        public int WinlossFunctionId { get; }

        public int BalanceFunctionId { get; }

        public int YesterdayWinLossFunctionId { get; }

        public int PersonalMessageFunctionId { get; }

        public int SpecialAnnouncementFunctionId { get; }

        public int NormalAnnouncementFunctionId { get; }

        public int SystemUpdateFunctionId { get; }

        public bool ReturnRawErrorToMessenger { get; }

        public string SpDirectory { get; }

        private T GetValue<T>(string key)
        {
            var value = configuration[key];

            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
    }
}