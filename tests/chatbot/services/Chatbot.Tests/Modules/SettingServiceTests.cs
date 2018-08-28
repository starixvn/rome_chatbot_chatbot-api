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
