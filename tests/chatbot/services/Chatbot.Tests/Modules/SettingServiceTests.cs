namespace Chatbot.Tests.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Configurations;
    using Chatbot.Models;
    using Chatbot.Models.Dtos;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.MessageServices;
    using Chatbot.Modules.Settings;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;

    public class SettingServiceTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly Func<string> randomNumberFunc;
        private readonly Func<DateTime> getCurrentDateTimeFunc;
        private readonly ICacheService cacheService;
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageLogger messageLogger;
        private readonly IChatbotConfigs chatbotConfigs;

        private readonly SettingService settingService;

        public SettingServiceTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            randomNumberFunc = Substitute.For<Func<string>>();
            getCurrentDateTimeFunc = Substitute.For<Func<DateTime>>();
            cacheService = Substitute.For<ICacheService>();
            messageSender = Substitute.For<IMessageSender>();
            messageBuilder = Substitute.For<IMessageBuilder>();
            messageLogger = Substitute.For<IMessageLogger>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();

            settingService = new SettingServiceImpl(
                dynamicRepository,
                messageSender,
                messageBuilder,
                cacheService,
                messageLogger,
                chatbotConfigs,
                randomNumberFunc,
                getCurrentDateTimeFunc);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void GetUser_UserIdIsNegativeOrZero_ThrowArgumentOutOfRangeException(int userId)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => settingService.GetUser(userId).GetAwaiter().GetResult());
        }

        [Fact]
        public void GetUser_UserDoesNotExist_ReturnDefaultUser()
        {
            var notExistUserId = 1502142;
            dynamicRepository
                .GetAsync<UserDto>(Arg.Is<GetUserCriteria>(criteria => criteria.UserId == notExistUserId))
                .Returns(Task.FromResult<UserDto>(null));

            var user = settingService.GetUser(notExistUserId).GetAwaiter().GetResult();

            Assert.Equal(default(User), user);
        }

        [Fact]
        public void GetUser_UserExists_ReturnUser()
        {
            var existedUser = 1502143;
            var userDto = new UserDto(existedUser, 100012, 1000343, "en-US");
            dynamicRepository
                .GetAsync<UserDto>(Arg.Is<GetUserCriteria>(criteria => criteria.UserId == existedUser))
                .Returns(Task.FromResult(userDto));

            var user = settingService.GetUser(existedUser).GetAwaiter().GetResult();

            Assert.True(userDto.CustId == user.CustId
                && userDto.SubAccountId == user.SubAccountId
                && userDto.Language == user.Language
                && userDto.UserId == user.Id
                && !user.Functions.Any()
                && !user.MessengerAccounts.Any());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void GenerateVerificationCode_UserIdIsNegativeOrZero_ThrowArgumentOutOfRangeException(int userId)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => settingService.GenerateVerificationCode(userId, "Name", default(Messenger)).GetAwaiter().GetResult());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GenerateVerificationCode_CustNameIsEmptyOrNull_ThrowArgumentOutOfRangeException(string custName)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => settingService.GenerateVerificationCode(1, custName, default(Messenger)).GetAwaiter().GetResult());
        }

        [Fact]
        public void GenerateVerificationCode_Always_InsertMessengerInfoWithNotificationCode()
        {
            // Arrange
            var userId = 100;
            var custName = "custName";
            var messenger = new Messenger(1, "messenger-name");

            dynamicRepository
               .GetAsync<UserDto>(Arg.Is<GetUserCriteria>(criteria => criteria.UserId == userId))
               .Returns(Task.FromResult(new UserDto(userId, 2, 3, "en-us")));

            var verificationCode = "124578";
            randomNumberFunc.Invoke().Returns(verificationCode);

            // Act
            settingService.GenerateVerificationCode(userId, custName, messenger).GetAwaiter().GetResult();

            // Assert

            dynamicRepository.Received().ExecuteAsync(
                 Arg.Is<InsertMessengerCommand>(
                     command => command.MessengerType == messenger.Type
                             && command.MessengerUsername == messenger.Username
                             && command.VerificationCode == verificationCode
                             && command.UserId == userId));
        }

        [Fact]
        public void GenerateVerificationCode_Always_UseMessageSenderToSendVerificationCode()
        {
            // Arrange
            var userId = 100;
            var custName = "custName";
            var messenger = new Messenger(1, "messenger-name");

            dynamicRepository
               .GetAsync<UserDto>(Arg.Is<GetUserCriteria>(criteria => criteria.UserId == userId))
               .Returns(Task.FromResult(new UserDto(userId, 2, 3, "en-us")));

            var verificationCode = "124578";
            randomNumberFunc.Invoke().Returns(verificationCode);

            chatbotConfigs.VerificationCodeExpiredTimeInMinutes.Returns(10);

            messageBuilder.BuildVerificationCodeMessage(
                custName, 
                "en-us", 
                messenger.Type, 
                verificationCode, 
                chatbotConfigs.VerificationCodeExpiredTimeInMinutes)
                .Returns("Messsage");

            // Act
            settingService.GenerateVerificationCode(userId, custName, messenger).GetAwaiter().GetResult();

            // Assert
            messageSender.Received().SendMessage(userId, messenger, "Messsage");
        }

        [Fact]
        public void GetMessengers_OnlyEnabledMessengers_ReturnEnabledMessagers()
        {
            var isGetEnabledMessage = true;
            var userId = 23343;

            dynamicRepository
                .FetchAsync<Messenger>(Arg.Is<GetMessengerByUserCriteria>(criteria => criteria.UserId == userId))
                .Returns(Task.FromResult(StubMessengers()));

            var actualMessengers = settingService.GetMessengers(userId, isGetEnabledMessage).GetAwaiter().GetResult();

            var firstMessenger = actualMessengers.ElementAt(0);
            var secondMessenger = actualMessengers.ElementAt(1);
            Assert.True(actualMessengers.Count() == 2
                && firstMessenger.Type == 1
                && firstMessenger.Username == "Messenger1"
                && secondMessenger.Type == 3
                && secondMessenger.Username == "Messenger31");
        }

        [Fact]
        public void GetMessengers_WhenMessengerIsVerifying_ReturnNumberOfWrongVerificationCode()
        {
            var userId = 23343;

            dynamicRepository
                .FetchAsync<Messenger>(Arg.Is<GetMessengerByUserCriteria>(criteria => criteria.UserId == userId))
                .Returns(Task.FromResult(StubMessengers()));

            var wrongVerificationCodeActionId = 121212;
            chatbotConfigs.WrongVerificationCodeActionId.Returns(wrongVerificationCodeActionId);

            dynamicRepository.FetchAsync<UserActionDto>(Arg.Is<GetUserActionsCriteria>(criteria 
                => criteria.UserId == userId
                && criteria.ActionId == wrongVerificationCodeActionId
                && criteria.MessengerType == 2
                && criteria.MessengerUsername == "Messenger2"))
                .Returns(Task.FromResult(new List<UserActionDto> { new UserActionDto(userId, 2, "Messenger2", wrongVerificationCodeActionId, 100, DateTime.Now) } as IEnumerable<UserActionDto>));

            var actualMessengers = settingService.GetMessengers(userId, false).GetAwaiter().GetResult();

            Assert.Equal(5, actualMessengers.Count());
            Assert.Equal(100, actualMessengers.ElementAt(1).InputWrongVerificationCodeTimes);
        }

        [Fact]
        public void GetFunctions_Always_ReturnFunctionList()
        {
            var userId = 23343;
            dynamicRepository
                .FetchAsync<Function>(Arg.Is<GetUserFunctionsCriteria>(criteria => criteria.UserId == userId))
                .Returns(Task.FromResult(new List<Function>
                {
                    new Function
                    {
                        Id = 1,
                        Command = "/os",
                        Name = "name"
                    }
                } as IEnumerable<Function>));

            var actualFunctions = settingService.GetFunctions(userId).GetAwaiter().GetResult();

            var firstFunction = actualFunctions.ElementAt(0);
            Assert.Equal(1, actualFunctions.Count());
            Assert.Equal(1, firstFunction.Id);
            Assert.Equal("/os", firstFunction.Command);
            Assert.Equal("name", firstFunction.Name);
        }

        [Fact]
        public void GetFunctions_AlwaysGetByLanguage_ReturnFunctionListWithName()
        {
            var userId = 23343;
            dynamicRepository
                .FetchAsync<Function>(Arg.Is<GetUserFunctionsCriteria>(criteria => criteria.UserId == userId))
                .Returns(Task.FromResult(new List<Function>
                {
                    new Function
                    {
                        Id = 1,
                        Command = "/os",
                        Name = "name"
                    }
                } as IEnumerable<Function>));

            cacheService.GetOrSetAsync(
                "SettingServiceImpl.FunctionNameByLanguageCacheKey.1",
                Arg.Any<Func<IEnumerable<FunctionLanguageDto>>>(),
                Arg.Any<CacheItemOptions>())
                .Returns(Task.FromResult(new List<FunctionLanguageDto>
                {
                    new FunctionLanguageDto(1, "name-en-us", "function", "en-US")
                } as IEnumerable<FunctionLanguageDto>));

            var actualFunctions = settingService.GetFunctions(userId, "en-US").GetAwaiter().GetResult();

            Assert.Equal(1, actualFunctions.Count());
            Assert.Equal("name-en-us", actualFunctions.ElementAt(0).Name);
        }

        [Fact]
        public void UpdateFunctions_Always_ExecuteUpdateCommand()
        {
            var userId = 23343;
            var functions = new List<Function>
            {
                new Function
                {
                    Id = 1,
                    Command = "/os",
                    Name = "name"
                }
            };

            settingService.UpdateFunctions(userId, functions).GetAwaiter().GetResult();

            dynamicRepository.Received().ExecuteAsync(Arg.Is<UpdateUserFunctionsCommand>(
                command => command.UserId == userId
                        && command.Functions.Count() == 1
                        && command.Functions.ElementAt(0).Id == 1));
        }

        [Fact]
        public void UpdateLanguage_Always_ExecuteUpdateCommand()
        {
            var userId = 23343;
            var language = "en-US";

            settingService.UpdateLanguage(userId, language).GetAwaiter().GetResult();

            dynamicRepository.Received().ExecuteAsync(Arg.Is<UpdateUserLanguageCommand>(
                command => command.UserId == userId
                        && command.Language == language));
        }

        [Fact]
        public void UpdateMessengerStatus__Always_ExecuteUpdateCommand()
        {
            var userId = 23343;
            var messengerType = 1;
            var messengerUsername = "username";
            var enabled = true;

            settingService.UpdateMessengerStatus(userId, messengerType, messengerUsername, enabled).GetAwaiter().GetResult();

            dynamicRepository.Received().ExecuteAsync(Arg.Is<UpdateMessengerStatusCommand>(
                command => command.UserId == userId
                        && command.MessengerType == messengerType
                        && command.MessengerUsername == messengerUsername
                        && command.Enabled == enabled));
        }

        [Fact]
        public void VerifyVerificationCode_MessengerIsBelongedToAnotherCustomer_ReturnFailedResult()
        {
            var userId = 2323;
            var messenger = new MessengerDto(4554, 1222, "messengername111", true, false, "434", DateTime.Now);

            dynamicRepository
                .GetAsync<MessengerDto>(Arg.Is<GetMessengerCriteria>(criteria
                    => criteria.MessengerType == messenger.Type
                        && criteria.MessengerUsername == messenger.Username))
                        .Returns(Task.FromResult(messenger));
            var expectedResult = VerificationCodeResult.CreateMessengerAccountHasBeenRegisteredByAnotherCustomerResult();

            var result = settingService
                .VerifyVerificationCode(userId, "3434", new Messenger(messenger.Type, messenger.Username))
                .GetAwaiter()
                .GetResult();

            Assert.Equal(result.Code, expectedResult.Code);
        }

        [Fact]
        public void VerifyVerificationCode_WrongMessengerName_ReturnFailedResult()
        {
            var userId = 2323;
            var messenger = new MessengerDto(userId, 1222, "messengername111", true, false, "434", DateTime.Now);

            dynamicRepository
                .GetAsync<MessengerDto>(Arg.Is<GetMessengerByUserCriteria>(criteria
                    => criteria.MessengerType == messenger.Type
                        && criteria.MessengerUsername == messenger.Username
                        && criteria.UserId == userId))
                        .Returns(Task.FromResult(messenger));
            var expectedResult = VerificationCodeResult.CreateWrongVerificationCodeResult();

            var result = settingService
                .VerifyVerificationCode(userId, "3434", new Messenger(messenger.Type, "wrongname"))
                .GetAwaiter()
                .GetResult();

            Assert.Equal(result.Code, expectedResult.Code);
        }

        [Fact]
        public void VerifyVerificationCode_VerificationCodeExpired_ReturnFailedResult()
        {
            var userId = 2323;
            var generatedVerificationCodeTime = new DateTime(1989, 5, 28, 10, 10, 0);
            var messenger = new MessengerDto(userId, 1222, "messengername111", true, false, "434", generatedVerificationCodeTime);

            chatbotConfigs.VerificationCodeExpiredTimeInMinutes.Returns(3);
            getCurrentDateTimeFunc.Invoke().Returns(generatedVerificationCodeTime.AddMinutes(4));

            dynamicRepository
                .GetAsync<MessengerDto>(Arg.Is<GetMessengerByUserCriteria>(criteria
                    => criteria.MessengerType == messenger.Type
                        && criteria.MessengerUsername == messenger.Username
                        && criteria.UserId == userId))
                        .Returns(Task.FromResult(messenger));
            var expectedResult = VerificationCodeResult.CreateVerificationCodeExpiredResult();

            var result = settingService
                .VerifyVerificationCode(userId, "3434", new Messenger(messenger.Type, messenger.Username))
                .GetAwaiter()
                .GetResult();

            Assert.Equal(result.Code, expectedResult.Code);
        }

        [Fact]
        public void VerifyVerificationCode_WrongVerificationCode_ReturnFailedResultAndLogFailedAction()
        {
            var userId = 2323;
            var messenger = new MessengerDto(userId, 1222, "messengername111", true, false, "434", DateTime.Now);

            chatbotConfigs.VerificationCodeExpiredTimeInMinutes.Returns(3);
            chatbotConfigs.WrongVerificationCodeActionId.Returns(601);

            dynamicRepository
                .GetAsync<MessengerDto>(Arg.Is<GetMessengerByUserCriteria>(criteria
                    => criteria.MessengerType == messenger.Type
                        && criteria.MessengerUsername == messenger.Username
                        && criteria.UserId == userId))
                        .Returns(Task.FromResult(messenger));
            var expectedResult = VerificationCodeResult.CreateWrongVerificationCodeResult();

            var result = settingService
                .VerifyVerificationCode(userId, "3434", new Messenger(messenger.Type, messenger.Username))
                .GetAwaiter()
                .GetResult();

            Assert.Equal(result.Code, expectedResult.Code);
            dynamicRepository.Received().ExecuteAsync(Arg.Is<InsertUserActionCommand>(
                command => command.UserId == userId
                        && command.MessengerType == messenger.Type
                        && command.MessengerUsername == messenger.Username
                        && command.ActionId == 601));
        }

        [Fact]
        public void VerifyVerificationCode_VerifySuccessful_UpdateMessengerStatus()
        {
            var userId = 2323;
            var messenger = new MessengerDto(userId, 1222, "messengername111", true, false, "434", DateTime.Now);

            chatbotConfigs.VerificationCodeExpiredTimeInMinutes.Returns(3);

            dynamicRepository
                .GetAsync<MessengerDto>(Arg.Is<GetMessengerByUserCriteria>(criteria
                    => criteria.MessengerType == messenger.Type
                        && criteria.MessengerUsername == messenger.Username
                        && criteria.UserId == userId))
                        .Returns(Task.FromResult(messenger));

            var result = settingService
                .VerifyVerificationCode(userId, messenger.VerificationCode, new Messenger(messenger.Type, messenger.Username))
                .GetAwaiter()
                .GetResult();

            dynamicRepository.Received().ExecuteAsync(Arg.Is<UpdateMessengerStatusCommand>(
                command => command.UserId == userId
                        && command.MessengerType == messenger.Type
                        && command.MessengerUsername == messenger.Username
                        && command.Enabled));
        }

        private IEnumerable<Messenger> StubMessengers()
            => new List<Messenger>
            {
                new Messenger(1, "Messenger1", true, false),
                new Messenger(2, "Messenger2", false, true),
                new Messenger(3, "Messenger3", false, false),
                new Messenger(3, "Messenger31", true, false),
                new Messenger(1, "Messenger4", false, false)
            };
    }
}
