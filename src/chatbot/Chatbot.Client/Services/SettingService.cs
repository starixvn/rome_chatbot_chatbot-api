[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Chatbot.Client.Tests")]

namespace Chatbot.Client.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Client.Helpers;
    using Chatbot.Client.Models;

    public interface SettingService
    {
        Task<List<Function>> GetFunctions(int userId, string language);

        Task<Result> UpdateFunctions(int userId, IEnumerable<Function> functions, LogParam logParam);

        Task<Result> UpdateLanguage(int userId, string language, LogParam logParam);

        Task<Result> GenerateVerificationCode(int userId, string messengerUsername, int messengerType = MessengerType.Letstalk);

        Task<VerificationCodeResult> VerifyVerificationCode(int userId, string verificationCode, string messengerUsername, int messengerType, LogParam logParam);

        Task<AddingUserResult> AddUser(int custId, int custLevel, int subAccountId, string language, string messengerUsername, int messengerType, LogParam logParam);

        Task<User> GetUser(int custId, int subAccountId, string language);

        Task<Result> Unsubscribe(int userId, int messengerType, string messengerUsername, LogParam logParam);

        Task<Messenger> GetMessenger(int messengerType, string messengerUsername);

        Task<List<User>> GetUsersByFunction(int functionId, int environment);
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

        public Task<List<Function>> GetFunctions(int userId, string language)
        {
            InputValidator.ValidateInputParam(nameof(userId), userId);

            return restClient.ExecuteGetAsync<List<Function>>($"setting/{userId}/function/{language}");
        }

        public Task<Result> UpdateFunctions(int userId, IEnumerable<Function> functions, LogParam logParam)
        {
            InputValidator.ValidateInputParam(nameof(userId), userId);
            InputValidator.ValidateInputParam(nameof(functions), functions);
            InputValidator.ValidateInputParam(nameof(logParam), logParam);

            return restClient.ExecutePostAsync<Result>(
                $"setting/{userId}/function",
                new
                {
                    functions,
                    logParam
                });
        }

        public Task<Result> UpdateLanguage(int userId, string language, LogParam logParam)
        {
            InputValidator.ValidateInputParam(nameof(userId), userId);
            InputValidator.ValidateInputParam(nameof(language), language);
            InputValidator.ValidateInputParam(nameof(logParam), logParam);

            return restClient.ExecutePostAsync<Result>($"setting/{userId}/language/{language}", logParam);
        }

        public Task<Result> GenerateVerificationCode(int userId, string messengerUsername, int messengerType = 1)
        {
            InputValidator.ValidateInputParam(nameof(messengerUsername), messengerUsername);
            InputValidator.ValidateInputParam(nameof(messengerType), messengerType);

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
            int messengerType,
            LogParam logParam)
        {
            InputValidator.ValidateInputParam(nameof(custId), custId);
            InputValidator.ValidateInputParam(nameof(messengerUsername), messengerUsername);
            InputValidator.ValidateInputParam(nameof(messengerType), messengerType);
            InputValidator.ValidateInputParam(nameof(language), language);
            InputValidator.ValidateInputParam(nameof(logParam), logParam);

            return restClient.ExecutePostAsync<AddingUserResult>(
                $"setting/user",
                new
                {
                    custId,
                    custLevel,
                    subAccountId,
                    language,
                    messengerUsername,
                    messengerType,
                    logParam
                });
        }

        public Task<User> GetUser(int custId, int subAccountId, string language)
            => restClient.ExecuteGetAsync<User>($"setting/user?custId={custId}&subAccountId={subAccountId}&language={language}");

        public Task<Result> Unsubscribe(
            int userId,
            int messengerType,
            string messengerUsername,
            LogParam logParam)
        {
            InputValidator.ValidateInputParam(nameof(userId), userId);
            InputValidator.ValidateInputParam(nameof(messengerType), messengerType);
            InputValidator.ValidateInputParam(nameof(logParam), logParam);

            return restClient.ExecutePostAsync<Result>($"setting/{userId}/unsubscribe/{messengerType}/{messengerUsername}", logParam);
        }

        public Task<VerificationCodeResult> VerifyVerificationCode(
            int userId,
            string verificationCode,
            string messengerUsername,
            int messengerType,
            LogParam logParam)
        {
            InputValidator.ValidateInputParam(nameof(userId), userId);
            InputValidator.ValidateInputParam(nameof(verificationCode), verificationCode);
            InputValidator.ValidateInputParam(nameof(messengerUsername), messengerUsername);
            InputValidator.ValidateInputParam(nameof(messengerType), messengerType);

            return restClient.ExecutePostAsync<VerificationCodeResult>(
                $"setting/{userId}/verifyVerificationCode",
                new
                {
                    verificationCode,
                    messengerUsername,
                    messengerType,
                    logParam
                });
        }

        public Task<Messenger> GetMessenger(int messengerType, string messengerUsername)
        {
            InputValidator.ValidateInputParam(nameof(messengerType), messengerType);
            InputValidator.ValidateInputParam(nameof(messengerUsername), messengerUsername);

            return restClient.ExecuteGetAsync<Messenger>($"setting/messenger/{messengerType}/{messengerUsername}");
        }

        public Task<List<User>> GetUsersByFunction(int functionId, int environment)
        {
            InputValidator.ValidateInputParam(nameof(functionId), functionId);

            return restClient.ExecuteGetAsync<List<User>>($"setting/users/function/{functionId}/{environment}");
        }
    }
}