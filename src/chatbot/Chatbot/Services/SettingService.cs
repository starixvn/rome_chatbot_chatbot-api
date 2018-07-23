namespace Chatbot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Models.Dtos;
    using Chatbot.Params.Db;
    using Fanex.Data.Repository;

    public interface SettingService
    {
        Task<IEnumerable<Function>> GetFunctions(int userId);

        Task<Result> UpdateFunctions(int userId, IEnumerable<Function> functions);

        Task<Result> UpdateLanguage(int userId, string language);

        Task<string> GenerateVerificationCode(int userId, Messenger messenger);

        Task<Result> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger);

        Task<AddingUserResult> AddUser(int custId, int subAccountId, string language, Messenger messenger);

        Task<IEnumerable<Function>> GetAllFunctions();

        Task<User> GetUser(int custId, int subAccountId);

        Task<Result> Unsubscribe(int userId, int messengerType);
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

        public async Task<string> GenerateVerificationCode(int userId, Messenger messenger)
        {
            var verificationCode = RandomSixDigitsNumber();
            var client = messengerFactory.CreateMessengerClient(messenger.Type);

            var returnCode = await dynamicRepository.ExecuteAsync(new InsertVerificationCodeCommand
            {
                MessengerType = messenger.Type,
                UserId = userId,
                VerificationCode = verificationCode
            });

            if(returnCode != 0)
            {
                return string.Empty;
            }

            var result = await client.PushAsync(messenger.Account, verificationCode);

            if (result.IsNotOk)
            {
                throw new InvalidOperationException(result.Message);
            }

            return verificationCode;
        }

        private string RandomSixDigitsNumber()
            => new Random().Next(0, 999999).ToString("D6");

        public async Task<IEnumerable<Function>> GetAllFunctions()
        {
            var funtions = await dynamicRepository.FetchAsync<Function>(new GetAllFunctionsCriteria());

            return funtions;
        }

        public async Task<User> GetUser(int custId, int subAccountId)
        {
            var userDto = await dynamicRepository.FetchAsync<UserDto>(new GetUserCriteria());

            if (!userDto.Any())
            {
                return default(User);
            }

            var firstDto = userDto.ElementAt(0);
            var userId = firstDto.UserId;
            var user = new User
            {
                Id = userId,
                CustId = firstDto.CustId,
                Language = firstDto.Language,
                SubAccountId = firstDto.SubAccountId,
                MessengerAccounts = userDto.Select(dto => new Messenger
                {
                    Account = dto.MessengerAcc,
                    Enabled = dto.Enabled,
                    Type = dto.MessengerType
                })
            };

            return user;
        }

        public async Task<IEnumerable<Function>> GetFunctions(int userId)
        {
            var functions = await dynamicRepository.FetchAsync<Function>(new GetFunctionsCriteria
            {
                UserId = userId
            });

            foreach (var function in functions)
            {
                var functionLanguages = await dynamicRepository.FetchAsync<FunctionLanguageDto>(new GetFunctionNameCriteria
                {
                    FunctionId = function.Id
                });

                function.NameByLanguage = functionLanguages.ToDictionary(lang => lang.LanguageCode, lang => lang.FunctionName);
            }

            return functions;
        }

        public async Task<Result> UpdateFunctions(int userId, IEnumerable<Function> functions)
        {
            var code = await dynamicRepository.ExecuteAsync(new UpdateFunctionsCommand
            {
                UserId = userId,
                Functions = functions
            });

            return new Result
            {
                Code = code
            };
        }

        public async Task<Result> UpdateLanguage(int userId, string language)
        {
            var code = await dynamicRepository.ExecuteAsync(new UpdateLanguageCommand
            {
                UserId = userId,
                Language = language
            });

            return new Result
            {
                Code = code
            };
        }

        /// <summary>
        /// Need more refactor here
        /// </summary>
        public async Task<AddingUserResult> AddUser(int custId, int subAccountId, string language, Messenger messenger)
        {
            var isExisted = await CheckMessengerAccountExisted(messenger);

            if(!isExisted)
            {
                return AddingUserResult.CreateAccountDoesNotExistResult();
            }

            var insertCommand = new InsertUserCommand
            {
                CustId = custId,
                SubAccountId = subAccountId,
                Language = language
            };

            var code = await dynamicRepository.ExecuteAsync(insertCommand);

            // Need refactor code here
            if(code == 0)
            {
                var userId = insertCommand.UserId;
                var verificationCode = await GenerateVerificationCode(userId, messenger);
                
                if(!string.IsNullOrWhiteSpace(verificationCode))
                {
                    return new AddingUserResult
                    {
                        Code = code,
                        UserId = insertCommand.UserId,
                        VerificationCode = verificationCode
                    };
                }
            }

            return new AddingUserResult
            {
                Code = -1,
                UserId = insertCommand.UserId
            };
        }

        private async Task<bool> CheckMessengerAccountExisted(Messenger messenger)
        {
            var client = messengerFactory.CreateMessengerClient(messenger.Type);
            var profileResult = await client.GetProfileAsync(messenger.Account);

            return profileResult.Item2;
        }

        public async Task<Result> Unsubscribe(int userId, int messengerType)
        {
            var code = await dynamicRepository.ExecuteAsync(new DisabledMessengerCommand
            {
                UserId = userId,
                MessengerType = messengerType
            });

            return new Result
            {
                Code = code
            };
        }

        public Task<Result> VerifyVerificationCode(int userId, string verificationCode, Messenger messenger)
        {
            throw new NotImplementedException();
        }
    }
}