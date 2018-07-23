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
        Task<IEnumerable<Function>> GetFunctions(int userId);

        Task<Result> UpdateFunctions(int userId, IEnumerable<Function> functions);

        Task<Result> UpdateLanguage(int userId, string language);

        Task<string> GenerateVerificationCode(int userId, string messengerAccount, int messengerType = MessengerType.Letstalk);

        Task<Result> VerifyVerificationCode(int userId, string verificationCode, string messengerAccount, int messengerType = MessengerType.Letstalk);

        Task<AddingUserResult> AddUser(int custId, int subAccountId, string language, string messengerAccount, int messengerType = MessengerType.Letstalk);

        Task<IEnumerable<Function>> GetAllFunctions();

        Task<User> GetUser(int custId, int subAccountId);

        Task<Result> Unsubscribe(int userId, int messengerType);
    }

    public class SettingServiceImpl : SettingService
    {
        private readonly IRestClientHelper restClientHelper;

        public SettingServiceImpl(IRestClientHelper restClientHelper = null)
        {
            this.restClientHelper = restClientHelper ?? new RestClientHelper();
        }

        public Task<IEnumerable<Function>> GetFunctions(int userId)
        {
            ValidateInputParam(nameof(userId), userId);

            return restClientHelper.ExecuteGetAsync<IEnumerable<Function>>($"setting/{userId}/function");
        }

        public Task<Result> UpdateFunctions(int userId, IEnumerable<Function> functions)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(functions), functions);

            return restClientHelper.ExecutePostAsync<Result>($"setting/{userId}/function", functions);
        }

        public Task<Result> UpdateLanguage(int userId, string language)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(language), language);

            return restClientHelper.ExecutePostAsync<Result>($"setting/{userId}/language/{language}");
        }

        public Task<string> GenerateVerificationCode(int userId, string messengerAccount, int messengerType = 1)
        {
            ValidateInputParam(nameof(messengerAccount), messengerAccount);
            ValidateInputParam(nameof(messengerType), messengerType);

            return restClientHelper.ExecutePostAsync<string>(
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

            return restClientHelper.ExecutePostAsync<AddingUserResult>(
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

        public Task<IEnumerable<Function>> GetAllFunctions()
            => restClientHelper.ExecuteGetAsync<IEnumerable<Function>>($"setting/allfunctions");

        public Task<User> GetUser(int custId, int subAccountId)
            => restClientHelper.ExecuteGetAsync<User>($"setting/user?custId={custId}&subAccountId={subAccountId}");

        public Task<Result> Unsubscribe(int userId, int messengerType)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messengerType), messengerType);

            return restClientHelper.ExecutePostAsync<Result>($"setting/{userId}/unsubscribe/{messengerType}");
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

        public Task<Result> VerifyVerificationCode(int userId, string verificationCode, string messengerAccount, int messengerType = 1)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(verificationCode), verificationCode);
            ValidateInputParam(nameof(messengerAccount), messengerAccount);
            ValidateInputParam(nameof(messengerType), messengerType);

            return restClientHelper.ExecutePostAsync<Result>(
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