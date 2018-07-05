namespace Alpha.Chatbot.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Alpha.Chatbot.Client.Helpers;
    using Alpha.Chatbot.Client.Models;

    public partial interface SettingService
    {
        IEnumerable<Function> GetFunctions(int custId);

        Result UpdateFunctions(int custId, IEnumerable<IFunction> functions);

        string GetLanguage(int custId);

        Result UpdateLanguage(int custId, string language);

        Result AddBotAccount(int custId, string accountName);

        Result CheckVerificationCode(int custId, string verificationCode);

        Result GenerateVerificationCode(int custId);

        Result UnsubscribeBotAccount(int custId);

        Task<IEnumerable<Function>> GetFunctionsAsync(int custId);

        Task<Result> UpdateFunctionsAsync(int custId, IEnumerable<IFunction> functions);

        Task<string> GetLanguageAsync(int custId);

        Task<Result> UpdateLanguageAsync(int custId, string language);

        Task<Result> AddBotAccountAsync(int custId, string accountName);

        Task<Result> CheckVerificationCodeAsync(int custId, string verificationCode);

        Task<Result> GenerateVerificationCodeAsync(int custId);

        Task<Result> UnsubscribeBotAccountAsync(int custId);
    }

    public class SettingServiceImpl : SettingService
    {
        private readonly IRestClientHelper restClientHelper;

        public SettingServiceImpl(IRestClientHelper restClientHelper = null)
        {
            this.restClientHelper = restClientHelper ?? new RestClientHelper();
        }

        public Result AddBotAccount(int custId, string accountName)
            => AddBotAccountAsync(custId, accountName).Result;

        public Task<Result> AddBotAccountAsync(int custId, string accountName)
        {
            ValidateInputParam(nameof(custId), custId);
            ValidateInputParam(nameof(accountName), accountName);

            return restClientHelper.ExecutePostAsync<Result>($"bot/{custId}/add", new { accountName });
        }

        public Result CheckVerificationCode(int custId, string verificationCode)
            => CheckVerificationCodeAsync(custId, verificationCode).Result;

        public Task<Result> CheckVerificationCodeAsync(int custId, string verificationCode)
        {
            ValidateInputParam(nameof(custId), custId);
            ValidateInputParam(nameof(verificationCode), verificationCode);

            return restClientHelper.ExecutePostAsync<Result>($"bot/{custId}/checkVerificationCode", new { verificationCode });
        }

        public Result GenerateVerificationCode(int custId)
            => GenerateVerificationCodeAsync(custId).Result;

        public Task<Result> GenerateVerificationCodeAsync(int custId)
        {
            ValidateInputParam(nameof(custId), custId);

            return restClientHelper.ExecutePostAsync<Result>($"bot/{custId}/generateVerificationCode");
        }

        public string GetLanguage(int custId)
            => GetLanguageAsync(custId).Result;

        public Task<string> GetLanguageAsync(int custId)
        {
            ValidateInputParam(nameof(custId), custId);

            return restClientHelper.ExecuteGetAsync<string>($"bot/{custId}/language");
        }

        public IEnumerable<Function> GetFunctions(int custId)
            => GetFunctionsAsync(custId).Result;

        public Task<IEnumerable<Function>> GetFunctionsAsync(int custId)
        {
            ValidateInputParam(nameof(custId), custId);

            return restClientHelper.ExecuteGetAsync<IEnumerable<Function>>($"bot/{custId}/setting");
        }

        public Result UnsubscribeBotAccount(int custId)
            => UnsubscribeBotAccountAsync(custId).Result;

        public Task<Result> UnsubscribeBotAccountAsync(int custId)
        {
            ValidateInputParam(nameof(custId), custId);

            return restClientHelper.ExecutePostAsync<Result>($"bot/{custId}/unsubscribe");
        }

        public Result UpdateLanguage(int custId, string language)
            => UpdateLanguageAsync(custId, language).Result;

        public Task<Result> UpdateLanguageAsync(int custId, string language)
        {
            ValidateInputParam(nameof(custId), custId);
            ValidateInputParam(nameof(language), language);

            return restClientHelper.ExecutePostAsync<Result>($"bot/{custId}/language", new { language });
        }

        public Result UpdateFunctions(int custId, IEnumerable<IFunction> settingItems)
            => UpdateFunctionsAsync(custId, settingItems).Result;

        public Task<Result> UpdateFunctionsAsync(int custId, IEnumerable<IFunction> settingItems)
        {
            ValidateInputParam(nameof(custId), custId);
            ValidateInputParam(nameof(settingItems), settingItems);

            return restClientHelper.ExecutePostAsync<Result>($"bot/{custId}/setting", settingItems);
        }

        private static void ValidateInputParam(string paramName, object paramValue)
        {
            if (paramValue == null
                || (paramValue is string && string.IsNullOrWhiteSpace(paramValue as string)))
            {
                throw new ArgumentNullException(paramName);
            }
        }

        private static void ValidateInputParam<T>(string paramName, IEnumerable<T> paramValue)
        {
            if (paramValue == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (!paramValue.Any())
            {
                throw new ArgumentOutOfRangeException($"{paramName} does not contain any item");
            }
        }
    }
}