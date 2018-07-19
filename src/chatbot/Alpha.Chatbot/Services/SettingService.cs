namespace Alpha.Chatbot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Alpha.Chatbot.Models;
    using Alpha.Chatbot.Models.Dtos;
    using Alpha.Chatbot.Params.Db;
    using Fanex.Data.Repository;
    using Messenger.Client;

    public interface SettingService
    {
        Task<IEnumerable<Function>> GetFunctions(int custId, int subAccountId);

        Task<Result> UpdateFunctions(int custId, int subAccountId, IEnumerable<Function> functions);

        Task<string> GetLanguage(int custId, int subAccountId);

        Task<Result> UpdateLanguage(int custId, int subAccountId, string language);

        Task<string> GenerateVerificationCode(MessengerAccount messengerAccount);

        Task<AddingMessengerAccountResult> AddMessengerAccount(int custId, int subAccountId, MessengerAccount messengerAccount);

        Task<Result> AddChatbotAccount(User account);

        Task<IDictionary<int, IEnumerable<string>>> GetAllFunctionCommands();

        Task<User> GetUser(int custId, int subAccountId);
    }

    public class SettingServiceImpl : SettingService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly MessengerFactory messengerFactory;
        private readonly Func<string> randomNumberFunc;

        public SettingServiceImpl(
            IDynamicRepository dynamicRepository,
            MessengerFactory messengerFactory,
            Func<string> randomNumberFunc = null)
        {
            this.dynamicRepository = dynamicRepository;
            this.messengerFactory = messengerFactory;
            this.randomNumberFunc = randomNumberFunc ?? RandomSixDigitsNumber;
        }

        public Task<Result> AddChatbotAccount(User account)
        {
            throw new NotImplementedException();
        }

        public async Task<AddingMessengerAccountResult> AddMessengerAccount(int custId, int subAccountId, MessengerAccount messengerAccount)
        {
            var client = messengerFactory.CreateMessengerClient(messengerAccount.MessengerType);
            var profileResult = await client.GetProfileAsync(messengerAccount.AccountId);

            // Account does not exist
            if(!profileResult.Item2)
            {
                return AddingMessengerAccountResult.CreateAccountDoesNotExistResult();
            }

            var verificcationCode = await GenerateVerificationCode(messengerAccount);

            return new AddingMessengerAccountResult { VerificationCode = verificcationCode };
        }

        public async Task<string> GenerateVerificationCode(MessengerAccount messengerAccount)
        {
            var verificationCode = RandomSixDigitsNumber();
            var client = messengerFactory.CreateMessengerClient(messengerAccount.MessengerType);

            var result = await client.PushAsync(messengerAccount.AccountId, verificationCode);

            if(result.IsNotOk)
            {
                throw new InvalidOperationException(result.Message);
            }

            return verificationCode;
        }

        private string RandomSixDigitsNumber()
            => new Random().Next(0, 999999).ToString("D6");

        public async Task<IDictionary<int, IEnumerable<string>>> GetAllFunctionCommands()
        {
            var funtions = await dynamicRepository.FetchAsync<FunctionDto>(new GetAllFunctionsCriteria());

            if(funtions == null || !funtions.Any())
            {
                return new Dictionary<int, IEnumerable<string>>();
            }

            return funtions.ToDictionary(
                value => value.FunctionId, 
                value => value.DefaultCommand.Split(',').AsEnumerable());
        }

        public Task<User> GetUser(int custId, int subAccountId)
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
