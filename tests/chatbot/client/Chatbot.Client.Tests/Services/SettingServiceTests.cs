namespace Chatbot.Client.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using Chatbot.Client.Helpers;
    using Chatbot.Client.Models;
    using Chatbot.Client.Services;
    using NSubstitute;
    using Xunit;

    public class SettingServiceTests
    {
        private readonly IRestClient restClient;
        private readonly SettingService settingService;

        public SettingServiceTests()
        {
            restClient = Substitute.For<IRestClient>();

            settingService = new SettingServiceImpl(restClient);
        }

        [Fact]
        public void GetFunctions_Alway_ExecuteExecuteGetAsync()
        {
            var userId = 1;
            var language = "en-US";

            settingService.GetFunctions(userId, language).GetAwaiter().GetResult();

            restClient.Received().ExecuteGetAsync<List<Function>>("setting/1/function/en-US");
        }

        [Fact]
        public void UpdateFunctions_Alway_ExecuteExecutePostAsync()
        {
            var userId = 1;
            var functions = new List<Function>
            {
                new Function{ Id = 1, Command = "cm" },
                new Function{ Id = 2, Command = "cm2" }
            };
            var logParam = new LogParam
            {
                ActorId = 1,
                CustId = 2,
                IP = "127.0.0.1"
            };

            settingService.UpdateFunctions(userId, functions, logParam).GetAwaiter().GetResult();

            Func<object, bool> assert = data =>
            {
                var type = data.GetType();
                var actualFunctions = type.GetProperty("functions").GetValue(data);
                var actualLogParam = type.GetProperty("logParam").GetValue(data);

                return actualFunctions == functions
                    && actualLogParam == logParam;
            };

            restClient.Received().ExecutePostAsync<Result>(
                "setting/1/function",
                Arg.Is<object>(data => assert(data)));
        }

        [Fact]
        public void UpdateLanguage_Alway_ExecuteExecutePostAsync()
        {
            var userId = 1;
            var language = "en-US";
            var logParam = new LogParam
            {
                ActorId = 1,
                CustId = 2,
                IP = "127.0.0.1"
            };

            settingService.UpdateLanguage(userId, language, logParam).GetAwaiter().GetResult();

            restClient.Received().ExecutePostAsync<Result>(
                "setting/1/language/en-US",
                Arg.Is<object>(data => data == logParam));
        }

        [Fact]
        public void GenerateVerificationCode_Alway_ExecuteExecutePostAsync()
        {
            var userId = 1;
            var messengerUsername = "username";
            var messengerType = 2;

            settingService.GenerateVerificationCode(userId, messengerUsername, messengerType).GetAwaiter().GetResult();

            Func<object, bool> assert = data =>
            {
                var type = data.GetType();
                var actualMessengerUsername = type.GetProperty("messengerUsername").GetValue(data);
                var actualMessengerType = (int)type.GetProperty("messengerType").GetValue(data);

                return actualMessengerUsername.Equals("username")
                    && actualMessengerType == messengerType;
            };

            restClient.Received().ExecutePostAsync<Result>(
                "setting/1/generateVerificationCode",
                Arg.Is<object>(data => assert(data)));
        }

        [Fact]
        public void AddUser_Alway_ExecuteExecutePostAsync()
        {
            var custId = 1;
            var custLevel = 2;
            var subAccountId = 3;
            var language = "en-US";
            var messengerUsername = "username";
            var messengerType = 2;
            var logParam = new LogParam
            {
                ActorId = custId,
                CustId = custId,
                IP = "172.0.0.1"
            };

            settingService.AddUser(custId, custLevel, subAccountId, language, messengerUsername, messengerType, logParam).GetAwaiter().GetResult();

            Func<object, bool> assert = data =>
            {
                var type = data.GetType();
                var actualMessengerUsername = type.GetProperty("messengerUsername").GetValue(data);
                var actualMessengerType = (int)type.GetProperty("messengerType").GetValue(data);
                var actualCustId = (int)type.GetProperty("custId").GetValue(data);
                var actualCustLevel = (int)type.GetProperty("custLevel").GetValue(data);
                var actualSubAccountId = (int)type.GetProperty("subAccountId").GetValue(data);
                var actualLanguage = type.GetProperty("language").GetValue(data);
                var actualLogParam = type.GetProperty("logParam").GetValue(data);

                return actualMessengerUsername.Equals("username")
                    && actualMessengerType == messengerType
                    && actualCustId == custId
                    && actualCustLevel == custLevel
                    && actualLanguage.Equals(language)
                    && actualSubAccountId == subAccountId
                    && actualLogParam == logParam;
            };

            restClient.Received().ExecutePostAsync<AddingUserResult>(
                "setting/user",
                Arg.Is<object>(data => assert(data)));
        }

        [Fact]
        public void GetUser_Alway_ExecuteExecuteGetAsync()
        {
            var custId = 1;
            var subAccountId = 2;
            var language = "en-US";

            settingService.GetUser(custId, subAccountId, language).GetAwaiter().GetResult();

            restClient.Received().ExecuteGetAsync<User>("setting/user?custId=1&subAccountId=2&language=en-US");
        }

        [Fact]
        public void Unsubscribe_Alway_ExecuteExecutePostAsync()
        {
            var userId = 1;
            var messengerType = 2;
            var messengerUsername = "messenger-name";
            var logParam = new LogParam
            {
                ActorId = 1,
                CustId = 2,
                IP = "172.0.0.1"
            };

            settingService.Unsubscribe(userId, messengerType, messengerUsername, logParam).GetAwaiter().GetResult();

            restClient.Received().ExecutePostAsync<Result>(
                "setting/1/unsubscribe/2/messenger-name",
                Arg.Is<object>(data => data == logParam));
        }

        [Fact]
        public void VerifyVerificationCode_Alway_ExecuteExecutePostAsync()
        {
            var userId = 1;
            var messengerUsername = "username";
            var messengerType = 2;
            var verificationCode = "343434";
            var logParam = new LogParam
            {
                ActorId = 1,
                CustId = 2,
                IP = "172.0.0.1"
            };

            settingService.VerifyVerificationCode(userId, verificationCode, messengerUsername, messengerType, logParam).GetAwaiter().GetResult();

            Func<object, bool> assert = data =>
            {
                var type = data.GetType();
                var actualMessengerUsername = type.GetProperty("messengerUsername").GetValue(data);
                var actualMessengerType = (int)type.GetProperty("messengerType").GetValue(data);
                var actualVerificationCode = type.GetProperty("verificationCode").GetValue(data);
                var actualLogParam = type.GetProperty("logParam").GetValue(data);

                return actualMessengerUsername.Equals("username")
                    && actualMessengerType == messengerType
                    && actualVerificationCode.Equals(verificationCode)
                    && actualLogParam == logParam;
            };

            restClient.Received().ExecutePostAsync<VerificationCodeResult>(
                "setting/1/verifyVerificationCode",
                Arg.Is<object>(data => assert(data)));
        }

        [Fact]
        public void GetMessenger_Alway_ExecuteExecuteGetAsync()
        {
            var messengerType = 2;
            var messengerUsername = "messenger-name";

            settingService.GetMessenger(messengerType, messengerUsername).GetAwaiter().GetResult();

            restClient.Received().ExecuteGetAsync<Messenger>("setting/messenger/2/messenger-name");
        }

        [Fact]
        public void GetUsersByFunction_Alway_ExecuteExecuteGetAsync()
        {
            var functionId = 2;
            var environment = 3;

            settingService.GetUsersByFunction(functionId, environment).GetAwaiter().GetResult();

            restClient.Received().ExecuteGetAsync<List<User>>("setting/users/function/2/3");
        }

        [Fact]
        public void GetAllFunctions_Alway_ExecuteExecuteGetAsync()
        {
            // Arrange
            var language = "en-US";

            // Act
            settingService.GetAllFunctions(language).GetAwaiter().GetResult();

            // Assert
            restClient.Received().ExecuteGetAsync<List<Function>>($"setting/functions/{language}");
        }
    }
}