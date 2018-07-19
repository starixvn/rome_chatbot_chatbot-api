namespace Alpha.Chatbot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Alpha.Chatbot.Models;
    using Fanex.Data.Repository;
    using Messenger.Client;

    public interface SettingService
    {
        Task<IEnumerable<Function>> GetFunctions(int custId, int subAccountId);

        Task<Result> UpdateFunctions(int custId, int subAccountId, IEnumerable<Function> functions);

        Task<string> GetLanguage(int custId, int subAccountId);

        Task<Result> UpdateLanguage(int custId, int subAccountId, string language);

        Task<string> GenerateVerificationCode(int custId, int subAccountId);

        Task<AddingMessengerAccountResult> AddMessengerAccount(int custId, int subAccountId, MessengerAccount messengerAccount);

        Task<Result> AddChatbotAccount(ChatbotAccount account);

        Task<IDictionary<int, IEnumerable<string>>> GetAllFunctionCommands();

        Task<ChatbotAccount> GetChatbotAccount(int custId, int subAccountId);
    }

    public class SettingServiceImpl : SettingService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly MessengerFactory messengerFactory;

        public SettingServiceImpl(
            IDynamicRepository dynamicRepository,
            MessengerFactory messengerFactory)
        {
            this.dynamicRepository = dynamicRepository;
            this.messengerFactory = messengerFactory;
        }

        public Task<Result> AddChatbotAccount(ChatbotAccount account)
        {
            throw new NotImplementedException();
        }

        public Task<AddingMessengerAccountResult> AddMessengerAccount(int custId, int subAccountId, MessengerAccount messengerAccount)
        {
            var messengerClient = messengerFactory.CreateMessengerClient(messengerAccount.MessengerType);

            //var checkAccountResult = messengerClient.GetProfile();

            return null;
        }

        public Task<string> GenerateVerificationCode(int custId, int subAccountId)
        {
            throw new NotImplementedException();
        }

        public Task<IDictionary<int, IEnumerable<string>>> GetAllFunctionCommands()
        {
            throw new NotImplementedException();
        }

        public Task<ChatbotAccount> GetChatbotAccount(int custId, int subAccountId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Function>> GetFunctions(int custId, int subAccountId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetLanguage(int custId, int subAccountId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateFunctions(int custId, int subAccountId, IEnumerable<Function> functions)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateLanguage(int custId, int subAccountId, string language)
        {
            throw new NotImplementedException();
        }
    }
}
