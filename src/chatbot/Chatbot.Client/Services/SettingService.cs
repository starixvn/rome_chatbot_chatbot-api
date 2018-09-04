namespace Chatbot.Client.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Client.Helpers;
    using Chatbot.Client.Models;

    public interface SettingService
    {
        Task<List<Function>> GetFunctions(int userId, string language);

        Task<Result> UpdateFunctions(int userId, IEnumerable<Function> functions);

        Task<Result> UpdateLanguage(int userId, string language);

        Task<Result> GenerateVerificationCode(int userId, string messengerUsername, int messengerType = MessengerType.Letstalk);

        Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, string messengerUsername, int messengerType = MessengerType.Letstalk);

        Task<AddingUserResult> AddUser(int custId, int custLevel, int subAccountId, string language, string messengerUsername, int messengerType = MessengerType.Letstalk);

        Task<User> GetUser(int custId, int subAccountId, string language);

        Task<Result> Unsubscribe(int userId, int messengerType, string messengerUsername);

        Task<Messenger> GetMessenger(int messengerType, string messengerUsername);

        Task<List<User>> GetUsersByFunction(int functionId, int environment);
    }

    public class SettingServiceImpl : ServiceBase, SettingService
    {
        private readonly IRestClient restClient;

        public SettingServiceImpl() : this(null)
        {
        }

        internal SettingServiceImpl(IRestClient restClient)
        {
            this.restClient = restClient ?? ChatbotClientManager.RestClient;
        }

        public Task<List<Function>> GetFunctions(int userId, string language)
        {
            ValidateInputParam(nameof(userId), userId);

            return restClient.ExecuteGetAsync<List<Function>>($"setting/{userId}/function/{language}");
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

        public Task<Result> GenerateVerificationCode(int userId, string messengerUsername, int messengerType = 1)
        {
            ValidateInputParam(nameof(messengerUsername), messengerUsername);
            ValidateInputParam(nameof(messengerType), messengerType);

            return restClient.ExecutePostAsync<Result>(
                $"setting/{userId}/generateVerificationCode",
                new
                {
                    messengerUsername,
                    messengerType
                });
        }

        public Task<AddingUserResult> AddUser(
            int custId, 
            int custLevel,
            int subAccountId, 
            string language, 
            string messengerUsername, 
            int messengerType = 1)
        {
            ValidateInputParam(nameof(custId), custId);
            ValidateInputParam(nameof(messengerUsername), messengerUsername);
            ValidateInputParam(nameof(messengerType), messengerType);
            ValidateInputParam(nameof(language), language);

            return restClient.ExecutePostAsync<AddingUserResult>(
                $"setting/user",
                new
                {
                    custId,
                    custLevel,
                    subAccountId,
                    language,
                    messengerUsername,
                    messengerType
                });
        }

        public Task<User> GetUser(int custId, int subAccountId, string language)
            => restClient.ExecuteGetAsync<User>($"setting/user?custId={custId}&subAccountId={subAccountId}&language={language}");

        public Task<Result> Unsubscribe(int userId, int messengerType, string messengerUsername)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(messengerType), messengerType);

            return restClient.ExecutePostAsync<Result>($"setting/{userId}/unsubscribe/{messengerType}/{messengerUsername}");
        }

        public Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, string messengerUsername, int messengerType = 1)
        {
            ValidateInputParam(nameof(userId), userId);
            ValidateInputParam(nameof(verificationCode), verificationCode);
            ValidateInputParam(nameof(messengerUsername), messengerUsername);
            ValidateInputParam(nameof(messengerType), messengerType);

            return restClient.ExecutePostAsync<VerificationCodeResult>(
                $"setting/{userId}/verifyVerificationCode",
                new
                {
                    verificationCode,
                    messengerUsername,
                    messengerType
                });
        }

        public Task<Messenger> GetMessenger(int messengerType, string messengerUsername)
        {
            ValidateInputParam(nameof(messengerType), messengerType);
            ValidateInputParam(nameof(messengerUsername), messengerUsername);

            return restClient.ExecutePostAsync<Messenger>($"setting/messenger/{messengerType}/{messengerUsername}");
        }

        public Task<List<User>> GetUsersByFunction(int functionId, int environment)
        {
            ValidateInputParam(nameof(functionId), functionId);

            return restClient.ExecuteGetAsync<List<User>>($"setting/users/function/{functionId}/{environment}");
        }
    }
}