namespace Chatbot.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Client.Helpers;
    using Chatbot.Client.Models;

    public interface SettingService
    {
        Task<IEnumerable<Function>> GetFunctions(int userId, string language);

        Task<Result> UpdateFunctions(int userId, IEnumerable<Function> functions);

        Task<Result> UpdateLanguage(int userId, string language);

        Task<Result> GenerateVerificationCode(int userId, string messengerAccount, int messengerType = MessengerType.Letstalk);

        Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, string messengerAccount, int messengerType = MessengerType.Letstalk);

        Task<AddingUserResult> AddUser(int custId, int subAccountId, string language, string messengerAccount, int messengerType = MessengerType.Letstalk);

        Task<User> GetUser(int custId, int subAccountId, string language);

        Task<Result> Unsubscribe(int userId, int messengerType);
    }

    public class SettingServiceImpl : SettingService
    {
        private readonly IRestClient restClient;

        public SettingServiceImpl() : this(null)
        {
        }

        internal SettingServiceImpl(IRestClient restClient)
        {
            this.restClient = restClient ?? ChatbotClientManager.RestClient;
        }

        public Task<IEnumerable<Function>> GetFunctions(int userId, string language)
        {
            ValidateInputParam(nameof(userId), userId);

            return restClient.ExecuteGetAsync<IEnumerable<Function>>($"setting/{userId}/function/{language}");
        }

        public Task<Result> UpdateFunctions(int userId, IEnumerable<Function> functions)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(functions), functions);

            return restClient.ExecutePostAsync<Result>($"setting/{userId}/function", functions);
        }

        public Task<Result> UpdateLanguage(int userId, string language)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(language), language);

            return restClient.ExecutePostAsync<Result>($"setting/{userId}/language/{language}");
        }

        public Task<Result> GenerateVerificationCode(int userId, string messengerAccount, int messengerType = 1)
        {
            ValidateInputParam(nameof(messengerAccount), messengerAccount);
            ValidateInputParam(nameof(messengerType), messengerType);

            return restClient.ExecutePostAsync<Result>(
                $"setting/{userId}/generateVerificationCode",
                new
                {
                    messengerAccount,
                    messengerType
                });
        }

        public Task<AddingUserResult> AddUser(int custId, int subAccountId, string language, string messengerAccount, int messengerType = 1)
        {
            ValidateInputParam(nameof(custId), custId);
            ValidateInputParam(nameof(messengerAccount), messengerAccount);
            ValidateInputParam(nameof(messengerType), messengerType);
            ValidateInputParam(nameof(language), language);

            return restClient.ExecutePostAsync<AddingUserResult>(
                $"setting/adduser",
                new
                {
                    custId,
                    subAccountId,
                    language,
                    messengerAccount,
                    messengerType
                });
        }

        public Task<User> GetUser(int custId, int subAccountId, string language)
            => restClient.ExecuteGetAsync<User>($"setting/user?custId={custId}&subAccountId={subAccountId}&language={language}");

        public Task<Result> Unsubscribe(int userId, int messengerType)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messengerType), messengerType);

            return restClient.ExecutePostAsync<Result>($"setting/{userId}/unsubscribe/{messengerType}");
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

        public Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, string messengerAccount, int messengerType = 1)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(verificationCode), verificationCode);
            ValidateInputParam(nameof(messengerAccount), messengerAccount);
            ValidateInputParam(nameof(messengerType), messengerType);

            return restClient.ExecutePostAsync<VerificationCodeResult>(
                $"setting/{userId}/verifyVerificationCode",
                new
                {
                    verificationCode,
                    messengerAccount,
                    messengerType
                });
        }
    }
}