namespace Chatbot.Tests.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Models;
    using Chatbot.Models;
    using Chatbot.Models.Dtos;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.SettingLogs;
    using Chatbot.Modules.SettingLogs.Models;
    using Chatbot.Modules.Settings;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Chatbot.Shared.Configurations;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;

    [Trait("Modules", "Settings")]
    public class SettingServiceTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly Func<string> randomNumberFunc;
        private readonly Func<DateTime> getCurrentDateTimeFunc;
        private readonly ICacheService cacheService;
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly SettingLogService settingLogService;
        private readonly ResourceService resourceService;

        private readonly SettingService settingService;

        public SettingServiceTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            randomNumberFunc = Substitute.For<Func<string>>();
            getCurrentDateTimeFunc = Substitute.For<Func<DateTime>>();
            cacheService = Substitute.For<ICacheService>();
            messageSender = Substitute.For<IMessageSender>();
            messageBuilder = Substitute.For<IMessageBuilder>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();
            settingLogService = Substitute.For<SettingLogService>();
            resourceService = Substitute.For<ResourceService>();

            settingService = new SettingServiceImpl(
                dynamicRepository,
                messageSender,
                messageBuilder,
                cacheService,
                chatbotConfigs,
                settingLogService,
                resourceService,
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
                .GetAsync<User>(Arg.Is<GetUserCriteria>(criteria => criteria.UserId == notExistUserId))
                .Returns(Task.FromResult<User>(null));

            var user = settingService.GetUser(notExistUserId).GetAwaiter().GetResult();

            Assert.Equal(default(User), user);
        }

        [Fact]
        public void GetUser_UserExists_ReturnUser()
        {
            var existedUser = 1502143;
            var user = new User(existedUser, 100012, 1, 1000343, "en-US", null);
            dynamicRepository
                .GetAsync<User>(Arg.Is<GetUserCriteria>(criteria => criteria.UserId == existedUser))
                .Returns(Task.FromResult(user));

            var actualUser = settingService.GetUser(existedUser).GetAwaiter().GetResult();

            Assert.True(actualUser.CustId == user.CustId
                && actualUser.SubAccountId == user.SubAccountId
                && actualUser.Language == user.Language
                && actualUser.Id == user.Id
                && !actualUser.Functions.Any()
                && !actualUser.MessengerAccounts.Any());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void GenerateVerificationCode_UserIdIsNegativeOrZero_ThrowArgumentOutOfRangeException(int userId)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => settingService.GenerateVerificationCode(userId, default(Messenger)).GetAwaiter().GetResult());
        }

        [Fact]
        public void GenerateVerificationCode_Always_InsertMessengerInfoWithNotificationCode()
        {
            // Arrange
            var userId = 100;
            var messenger = new Messenger(1, "messenger-name");

            dynamicRepository
               .GetAsync<User>(Arg.Is<GetUserCriteria>(criteria => criteria.UserId == userId))
               .Returns(Task.FromResult(new User(userId, 2, 2, 3, "en-us", null)));

            var verificationCode = "124578";
            randomNumberFunc.Invoke().Returns(verificationCode);

            // Act
            settingService.GenerateVerificationCode(userId, messenger).GetAwaiter().GetResult();

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
            var messenger = new Messenger(1, "messenger-name");

            dynamicRepository
               .GetAsync<User>(Arg.Is<GetUserCriteria>(criteria => criteria.UserId == userId))
               .Returns(Task.FromResult(new User(userId, 2, 2, 3, "en-us", null)));

            var verificationCode = "124578";
            randomNumberFunc.Invoke().Returns(verificationCode);

            chatbotConfigs.VerificationCodeExpiredTimeInMinutes.Returns(10);

            messageBuilder.BuildVerificationCodeMessage(
                "en-us",
                messenger.Type,
                verificationCode,
                chatbotConfigs.VerificationCodeExpiredTimeInMinutes)
                .Returns("Messsage");

            // Act
            settingService.GenerateVerificationCode(userId, messenger).GetAwaiter().GetResult();

            // Assert
            messageSender.Received().SendMessage(userId, messenger, "Messsage", null);
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
            SubGetAllActions();

            var actualFunctions = settingService.GetFunctions(userId).GetAwaiter().GetResult();

            var firstFunction = actualFunctions.ElementAt(0);
            Assert.Single(actualFunctions);
            Assert.Equal(1, firstFunction.Id);
            Assert.Equal("/os", firstFunction.Command);
            Assert.Equal("name", firstFunction.Name);
        }

        private void SubGetAllActions()
        {
            cacheService.GetOrSetAsync(
                            "SettingServiceImpl.AllFunctionsCacheKey",
                            Arg.Any<Func<IEnumerable<Function>>>(),
                            Arg.Any<CacheItemOptions>())
                            .Returns(Task.FromResult(new List<Function>
                            {
                    new Function { Id = 1, Environment = 0 }
                            } as IEnumerable<Function>));
        }

        [Fact]
        public void UpdateFunctions_Always_ExecuteUpdateCommand()
        {
            // Arrange
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
            var logData = new UpdateFunctionsLog(1, 2, "127.0.0.1", Enumerable.Empty<Function>(), Enumerable.Empty<Function>());

            // Act
            settingService.UpdateFunctions(userId, functions, logData).GetAwaiter().GetResult();

            // Assert
            dynamicRepository.Received().ExecuteAsync(Arg.Is<UpdateUserFunctionsCommand>(
                command => command.UserId == userId
                        && command.Functions == functions));
        }

        [Fact]
        public void UpdateFunctions_Always_WriteLog()
        {
            // Arrange
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
            var logData = new UpdateFunctionsLog(1, 2, "127.0.0.1", Enumerable.Empty<Function>(), Enumerable.Empty<Function>());

            // Act
            settingService.UpdateFunctions(userId, functions, logData).GetAwaiter().GetResult();

            // Assert
            settingLogService.Received().WriteLog(Arg.Is<ILogData>(logData));
        }

        [Fact]
        public void UpdateLanguage_Always_ExecuteUpdateCommand()
        {
            // Arrange
            var userId = 23343;
            var language = "en-US";
            var logData = new UpdateLanguageLog(1, 1, "127.0.0.1", "zh-CN", "en-US");

            // Act
            settingService.UpdateLanguage(userId, language, logData).GetAwaiter().GetResult();

            // Assert
            dynamicRepository.Received().ExecuteAsync(Arg.Is<UpdateUserLanguageCommand>(
                command => command.UserId == userId
                        && command.Language == language));
        }

        [Fact]
        public void UpdateLanguage_Always_WriteLog()
        {
            // Arrange
            var userId = 23343;
            var language = "en-US";
            var logData = new UpdateLanguageLog(1, 1, "127.0.0.1", "zh-CN", "en-US");

            // Act
            settingService.UpdateLanguage(userId, language, logData).GetAwaiter().GetResult();

            // Assert
            settingLogService.Received().WriteLog(Arg.Is<ILogData>(logData));
        }

        [Fact]
        public void UpdateMessengerStatus_Always_ExecuteUpdateCommand()
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
            // Arrange
            var userId = 2323;
            var messengerDto = new MessengerDto(4554, 1222, "messengername111", true, false, "434", DateTime.Now);
            var messenger = new Messenger(messengerDto.Type, messengerDto.Username);
            var logData = new VerifyAccountLog(1, 1, "127.0.0.1", messenger);

            dynamicRepository
                .GetAsync<MessengerDto>(Arg.Is<GetMessengerCriteria>(criteria
                    => criteria.MessengerType == messengerDto.Type
                        && criteria.MessengerUsername == messengerDto.Username))
                        .Returns(Task.FromResult(messengerDto));
            var expectedResult = VerificationCodeResult.CreateMessengerAccountHasBeenRegisteredByAnotherCustomerResult();

            // Act
            var result = settingService
                .VerifyVerificationCode(userId, "3434", messenger, logData)
                .GetAwaiter()
                .GetResult();

            // Assert
            Assert.Equal(result.Code, expectedResult.Code);
        }

        [Fact]
        public void VerifyVerificationCode_WrongMessengerName_ReturnFailedResult()
        {
            // Arrange
            var userId = 2323;
            var messengerDto = new MessengerDto(userId, 1222, "messengername111", true, false, "434", DateTime.Now);
            var messenger = new Messenger(messengerDto.Type, messengerDto.Username);
            var logData = new VerifyAccountLog(1, 1, "127.0.0.1", messenger);

            dynamicRepository
                .GetAsync<MessengerDto>(Arg.Is<GetMessengerByUserCriteria>(criteria
                    => criteria.MessengerType == messengerDto.Type
                        && criteria.MessengerUsername == messengerDto.Username
                        && criteria.UserId == userId))
                        .Returns(Task.FromResult(messengerDto));
            var expectedResult = VerificationCodeResult.CreateWrongVerificationCodeResult();

            // Act
            var result = settingService
                .VerifyVerificationCode(userId, "3434", messenger, logData)
                .GetAwaiter()
                .GetResult();

            // Assert
            Assert.Equal(result.Code, expectedResult.Code);
        }

        [Fact]
        public void VerifyVerificationCode_VerificationCodeExpired_ReturnFailedResult()
        {
            // Arrange
            var userId = 2323;
            var generatedVerificationCodeTime = new DateTime(1989, 5, 28, 10, 10, 0);
            var messengerDto = new MessengerDto(userId, 1222, "messengername111", true, false, "434", generatedVerificationCodeTime);
            var messenger = new Messenger(messengerDto.Type, messengerDto.Username);
            var logData = new VerifyAccountLog(1, 1, "127.0.0.1", messenger);

            chatbotConfigs.VerificationCodeExpiredTimeInMinutes.Returns(3);
            getCurrentDateTimeFunc.Invoke().Returns(generatedVerificationCodeTime.AddMinutes(4));

            dynamicRepository
                .GetAsync<MessengerDto>(Arg.Is<GetMessengerByUserCriteria>(criteria
                    => criteria.MessengerType == messengerDto.Type
                        && criteria.MessengerUsername == messengerDto.Username
                        && criteria.UserId == userId))
                        .Returns(Task.FromResult(messengerDto));
            var expectedResult = VerificationCodeResult.CreateVerificationCodeExpiredResult();

            // Act
            var result = settingService
                .VerifyVerificationCode(userId, "3434", messenger, logData)
                .GetAwaiter()
                .GetResult();

            // Assert
            Assert.Equal(result.Code, expectedResult.Code);
        }

        [Fact]
        public void VerifyVerificationCode_WrongVerificationCode_ReturnFailedResultAndLogFailedAction()
        {
            // Arrange
            var userId = 2323;
            var messengerDto = new MessengerDto(userId, 1222, "messengername111", true, false, "434", DateTime.Now);
            var messenger = new Messenger(messengerDto.Type, messengerDto.Username);
            var logData = new VerifyAccountLog(1, 1, "127.0.0.1", messenger);

            chatbotConfigs.VerificationCodeExpiredTimeInMinutes.Returns(3);
            chatbotConfigs.WrongVerificationCodeActionId.Returns(601);

            dynamicRepository
                .GetAsync<MessengerDto>(Arg.Is<GetMessengerByUserCriteria>(criteria
                    => criteria.MessengerType == messengerDto.Type
                        && criteria.MessengerUsername == messengerDto.Username
                        && criteria.UserId == userId))
                        .Returns(Task.FromResult(messengerDto));
            var expectedResult = VerificationCodeResult.CreateWrongVerificationCodeResult();

            // Act
            var result = settingService
                .VerifyVerificationCode(userId, "3434", messenger, logData)
                .GetAwaiter()
                .GetResult();

            // Assert
            Assert.Equal(result.Code, expectedResult.Code);
            dynamicRepository.Received().ExecuteAsync(Arg.Is<InsertUserActionCommand>(
                command => command.UserId == userId
                        && command.MessengerType == messengerDto.Type
                        && command.MessengerUsername == messengerDto.Username
                        && command.ActionId == 601));
        }

        [Fact]
        public void VerifyVerificationCode_VerifySuccessful_UpdateMessengerStatus()
        {
            // Arrange
            var userId = 2323;
            var messengerDto = new MessengerDto(userId, 1222, "messengername111", true, false, "434", DateTime.Now);
            var messenger = new Messenger(messengerDto.Type, messengerDto.Username);
            var logData = new VerifyAccountLog(1, 1, "127.0.0.1", messenger);

            SetupVerificationCodeSuccessful(userId, messengerDto);

            // Act
            settingService
                .VerifyVerificationCode(userId, messengerDto.VerificationCode, messenger, logData)
                .GetAwaiter()
                .GetResult();

            // Assert
            dynamicRepository.Received().ExecuteAsync(Arg.Is<UpdateMessengerStatusCommand>(
                command => command.UserId == userId
                        && command.MessengerType == messengerDto.Type
                        && command.MessengerUsername == messengerDto.Username
                        && command.Enabled));
        }

        [Fact]
        public void VerifyVerificationCode_VerifySuccessful_WriteLog()
        {
            // Arrange
            var userId = 2323;
            var messengerDto = new MessengerDto(userId, 1222, "messengername111", true, false, "434", DateTime.Now);
            var messenger = new Messenger(messengerDto.Type, messengerDto.Username);
            var logData = new VerifyAccountLog(1, 1, "127.0.0.1", messenger);

            SetupVerificationCodeSuccessful(userId, messengerDto);

            // Act
            settingService
                .VerifyVerificationCode(userId, messengerDto.VerificationCode, messenger, logData)
                .GetAwaiter()
                .GetResult();

            // Assert
            settingLogService.Received().WriteLog(Arg.Is<ILogData>(logData));
        }

        [Fact]
        public void VerifyVerificationCode_VerifySuccessful_ResetFailedVerificationCodeCount()
        {
            // Arrange
            var userId = 2323;
            var messengerDto = new MessengerDto(userId, 1222, "messengername111", true, false, "434", DateTime.Now);
            var messenger = new Messenger(messengerDto.Type, messengerDto.Username);
            var logData = new VerifyAccountLog(1, 1, "127.0.0.1", messenger);

            SetupVerificationCodeSuccessful(userId, messengerDto);
            chatbotConfigs.WrongVerificationCodeActionId.Returns(1515);

            // Act
            settingService
                .VerifyVerificationCode(userId, messengerDto.VerificationCode, messenger, logData)
                .GetAwaiter()
                .GetResult();

            // Assert
            dynamicRepository.Received().ExecuteAsync(Arg.Is<ResetActionCountTimesCommand>(
                command => command.UserId == userId
                        && command.MessengerType == messengerDto.Type
                        && command.MessengerUsername == messengerDto.Username
                        && command.ActionId == 1515));
        }

        [Fact]
        public void VerifyVerificationCode_VerifySuccessful_SendWelcomeMessage()
        {
            // Arrange
            var userId = 2323;
            var messengerDto = new MessengerDto(userId, 1222, "messengername111", true, false, "434", DateTime.Now);
            SetupVerificationCodeSuccessful(userId, messengerDto);
            messageBuilder.BuildWelcomeMessage("en-us", messengerDto.Type).Returns("Welcome Message!");
            var messenger = new Messenger(messengerDto.Type, messengerDto.Username);
            var logData = new VerifyAccountLog(1, 1, "127.0.0.1", messenger);

            // Act
            settingService
                .VerifyVerificationCode(userId, messengerDto.VerificationCode, messenger, logData)
                .GetAwaiter()
                .GetResult();

            // Assert
            messageSender.Received().SendMessage(userId, messenger, "Welcome Message!", null);
        }

        [Fact]
        public void AddUser_ExistUser_NotCallsInsertUserCommand()
        {
            // Arrange
            var custId = 1323;
            var subAccountId = 232;
            var custName = "cust name";
            var language = "en-US";
            var userId = 434;
            var messenger = new Messenger(1, "messenger type", true);
            var custLevel = 2;
            var user = new User(userId, custId, custLevel, subAccountId, language, null);
            var logData = new LinkAccountLog(custId, custId, "127.0.0.1", messenger);
            SetupAddUserSuccessful(custId, subAccountId, language, userId, user);

            // Act
            settingService.AddUser(custId, subAccountId, custName, language, messenger, custLevel, logData)
                .GetAwaiter().GetResult();

            // Assert
            dynamicRepository.DidNotReceive().ExecuteAsync(Arg.Any<InsertUserCommand>());
        }

        [Fact]
        public void AddUser_OverMaxCreateAccountTime_ReturnFailedresult()
        {
            // Arrange
            var custId = 1323;
            var subAccountId = 232;
            var custName = "cust name";
            var language = "en-US";
            var userId = 434;
            var custLevel = 1;
            var messenger = new Messenger(1, "messenger type", true);
            var user = new User(userId, custId, custLevel, subAccountId, language, null);
            var logData = new LinkAccountLog(custId, custId, "127.0.0.1", messenger);
            SetupAddUserSuccessful(custId, subAccountId, language, userId, user);
            var addAccountActionId = 34;
            chatbotConfigs.AddAccountActionId.Returns(addAccountActionId);
            chatbotConfigs.MaxAddActionTimes.Returns(29);

            dynamicRepository.FetchAsync<UserActionDto>(
                Arg.Is<GetUserActionsCriteria>(criteria
                => criteria.UserId == userId
                && criteria.ActionId == addAccountActionId))
                .Returns(Task.FromResult(
                    new List<UserActionDto>
                    {
                        new UserActionDto(userId, 1, "messenger type", addAccountActionId, 10, DateTime.Now),
                        new UserActionDto(userId, 1, "messenger type2", addAccountActionId, 20, DateTime.Now)
                    } as IEnumerable<UserActionDto>));

            // Act
            var result = settingService.AddUser(custId, subAccountId, custName, language, messenger, custLevel, logData)
                .GetAwaiter().GetResult();

            // Assert
            var expectedResult = AddingUserResult.CreateIsOverMaxAddAccountTimesResult();

            Assert.Equal(expectedResult.Code, result.Code);
        }

        [Fact]
        public void AddUser_MessengerAccountDoesNotExist_ReturnFailedresult()
        {
            // Arrange
            var custId = 1323;
            var subAccountId = 232;
            var custName = "cust name";
            var language = "en-US";
            var userId = 434;
            var messenger = new Messenger(1, "messenger type", true);
            var user = new User(userId, custId, 2, subAccountId, language, null);
            var custLevel = 1;
            var logData = new LinkAccountLog(custId, custId, "127.0.0.1", messenger);
            SetupAddUserSuccessful(custId, subAccountId, language, userId, user);

            messageSender.IsMessengerAccountNotExisted(messenger).Returns(Task.FromResult(true));

            // Act
            var result = settingService.AddUser(custId, subAccountId, custName, language, messenger, custLevel, logData)
                .GetAwaiter().GetResult();

            // Assert
            var expectedResult = AddingUserResult.CreateAccountDoesNotExistResult();

            Assert.Equal(expectedResult.Code, result.Code);
        }

        [Fact]
        public void AddUser_MessengerAccountIsBelongToAnotherCustomer_ReturnFailedresult()
        {
            // Arrange
            var custId = 1323;
            var subAccountId = 232;
            var custName = "cust name";
            var language = "en-US";
            var userId = 434;
            var messenger = new Messenger(1, "messenger type", true);
            var custLevel = 3;
            var user = new User(userId, custId, custLevel, subAccountId, language, null);
            var logData = new LinkAccountLog(custId, custId, "127.0.0.1", messenger);
            SetupAddUserSuccessful(custId, subAccountId, language, userId, user);

            dynamicRepository
                .GetAsync<MessengerDto>(Arg.Is<GetMessengerCriteria>(criteria
                    => criteria.MessengerType == messenger.Type
                        && criteria.MessengerUsername == messenger.Username))
                        .Returns(Task.FromResult(new MessengerDto(34342, messenger.Type, messenger.Username, true, false, string.Empty, DateTime.Now)));

            // Act
            var result = settingService.AddUser(custId, subAccountId, custName, language, messenger, custLevel, logData)
                .GetAwaiter().GetResult();

            // Assert
            var expectedResult = AddingUserResult.CreateMessengerAccountHasBeenRegisteredByAnotherCustomerResult();

            Assert.Equal(expectedResult.Code, result.Code);
        }

        [Fact]
        public void AddUser_Aways_SendVerificationCode()
        {
            // Arrange
            var custId = 1323;
            var subAccountId = 232;
            var custName = "cust name";
            var language = "en-US";
            var userId = 434;
            var custLevel = 1;
            var messenger = new Messenger(1, "messenger type", true);
            var user = new User(userId, custId, custLevel, subAccountId, language, null);
            var logData = new LinkAccountLog(custId, custId, "127.0.0.1", messenger);
            SetupAddUserSuccessful(custId, subAccountId, language, userId, user);

            // Act
            settingService.AddUser(custId, subAccountId, custName, language, messenger, custLevel, logData)
                .GetAwaiter().GetResult();

            // Assert
            messageSender.Received().SendMessage(userId, messenger, Arg.Any<string>(), null);
        }

        [Fact]
        public void Unsubscribe_Always_UpdateMessengerStatus()
        {
            // Arrange
            var userId = 1;
            var messenger = new Messenger(1, "messenger type");
            var logData = new UnlinkAccountLog(1, 2, "127.0.0.1", messenger);

            // Act
            settingService.Unsubscribe(userId, messenger, logData).ConfigureAwait(false);

            // Assert
            dynamicRepository.Received().ExecuteAsync(
                Arg.Is<UpdateMessengerStatusCommand>(
                    command => command.UserId == userId
                            && command.MessengerType == messenger.Type
                            && command.MessengerUsername == messenger.Username
                            && !command.Enabled));
        }

        [Fact]
        public void Unsubscribe_Always_WriteLog()
        {
            // Arrange
            var userId = 1;
            var messenger = new Messenger(1, "messenger type");
            var logData = new UnlinkAccountLog(1, 2, "127.0.0.1", messenger);

            // Act
            settingService.Unsubscribe(userId, messenger, logData).ConfigureAwait(false);

            // Assert
            settingLogService.Received().WriteLog(Arg.Is(logData));
        }

        [Fact]
        public void GetUsersByFunction_Always_CallsFetchAsync()
        {
            // Arrange
            var functionId = 1;
            var environment = 2;

            // Act
            settingService.GetUsersByFunction(functionId, environment).ConfigureAwait(false);

            // Assert
            dynamicRepository.Received().FetchAsync<User>(
                Arg.Is<GetUsersByFunctionCriteria>(
                    c => c.FunctionId == functionId
                        && c.Environment == environment));
        }

        [Fact]
        public void GetUser_ValidParams_CallsGetAsync()
        {
            // Arrange
            var custId = 1;
            var subAccountId = 0;
            var language = "en-US";

            // Act
            settingService.GetUser(custId, subAccountId, language).ConfigureAwait(false);

            // Assert
            dynamicRepository.Received().GetAsync<User>(
                Arg.Is<GetUserCriteria>(
                    c => c.CustId == custId
                        && c.SubAccountId == subAccountId));
        }

        private void SetupAddUserSuccessful(int custId, int subAccountId, string language, int userId, User user)
        {
            dynamicRepository.ExecuteAsync(
                                Arg.Is<InsertUserCommand>(
                                    command => command.CustId == custId
                                        && command.SubAccountId == subAccountId
                                        && command.Language == language)).Returns(parameters =>
                                        {
                                            ((InsertUserCommand)parameters[0]).UserId = userId;
                                            return Task.FromResult(1);
                                        });

            dynamicRepository.GetAsync<User>(Arg.Any<GetUserCriteria>()).Returns(user);
        }

        private void SetupVerificationCodeSuccessful(int userId, MessengerDto messenger)
        {
            chatbotConfigs.VerificationCodeExpiredTimeInMinutes.Returns(3);

            dynamicRepository
                .GetAsync<MessengerDto>(Arg.Is<GetMessengerByUserCriteria>(criteria
                    => criteria.MessengerType == messenger.Type
                        && criteria.MessengerUsername == messenger.Username
                        && criteria.UserId == userId))
                        .Returns(Task.FromResult(messenger));

            dynamicRepository
                .GetAsync<User>(Arg.Is<GetUserCriteria>(criteria => criteria.UserId == userId))
                .Returns(Task.FromResult(new User(userId, 2, 2, 3, "en-us", null)));
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

        [Fact]
        public void GetAllFunctions_Always_CallsFetchAsync()
        {
            // Arrange
            var language = string.Empty;

            // Act
            settingService.GetAllFunctions(language).ConfigureAwait(false);

            // Assert
            cacheService.Received().GetOrSetAsync(
                "SettingServiceImpl.AllFunctionsCacheKey",
                Arg.Any<Func<IEnumerable<Function>>>(),
                Arg.Any<CacheItemOptions>());
        }
    }
}