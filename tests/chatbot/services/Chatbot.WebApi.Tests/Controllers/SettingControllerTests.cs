namespace Chatbot.WebApi.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.SettingLogs.Models;
    using Chatbot.Modules.Settings;
    using Chatbot.WebApi.Controllers;
    using Chatbot.WebApi.Params;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using Xunit;

    [Trait("Controllers", "Setting")]
    public class SettingControllerTests
    {
        private readonly CompareLogic compareLogic;
        private readonly SettingService settingService;
        private readonly SettingController settingController;

        public SettingControllerTests()
        {
            compareLogic = new CompareLogic();
            settingService = Substitute.For<SettingService>();
            settingController = new SettingController(settingService);
        }

        [Fact]
        public void AddUser_Always_CallsSettingServiceAddUser()
        {
            // Arrange
            var logParam = CreateDefaultLogParam();
            var param = new UserParam
            {
                CustId = 1,
                SubAccountId = 0,
                CustName = "custname",
                Language = "en-US",
                CustLevel = 2,
                MessengerType = 1,
                MessengerUsername = "username",
                LogParam = logParam
            };
            var messenger = new Messenger(param.MessengerType, param.MessengerUsername);
            var logData = new LinkAccountLog(
                logParam.ActorId,
                logParam.CustId,
                logParam.IP,
                messenger);

            // Act
            settingController.AddUser(param).ConfigureAwait(false);

            // Assert
            settingService.Received().AddUser(
                param.CustId,
                param.SubAccountId,
                param.CustName,
                param.Language,
                Arg.Is<Messenger>(m => compareLogic.Compare(m, messenger).AreEqual),
                param.CustLevel,
                Arg.Is<LinkAccountLog>(l => compareLogic.Compare(l, logData).AreEqual));
        }

        [Fact]
        public void GetUser_Always_CallsSettingServiceGetUser()
        {
            // Arrange
            var custId = 1;
            var subAccountId = 0;
            var language = "en-US";

            // Act
            settingController.GetUser(custId, subAccountId, language).ConfigureAwait(false);

            // Assert
            settingService.Received().GetUser(custId, subAccountId, language);
        }

        [Fact]
        public void UpdateFunctions_Always_CallsGetFunctionsAndUpdateFunctions()
        {
            // Arrange
            var userId = 1;
            var logParam = CreateDefaultLogParam();
            var param = new FunctionParam
            {
                Functions = new List<Function>
                {
                    new Function
                    {
                        Id = 1,
                        Name = "Function 1",
                        Type =1,
                        Command = "/command",
                        Enabled = true,
                        Environment = 1
                    }
                },
                LogParam = logParam
            };
            IEnumerable<Function> oldFunctions = new List<Function>
            {
                new Function
                {
                    Id = 1,
                    Name = "Function 1",
                    Type =1,
                    Command = "/command",
                    Enabled = false,
                    Environment = 1
                }
            };
            settingService.GetFunctions(userId).Returns(Task.FromResult(oldFunctions));

            var logData = new UpdateFunctionsLog(logParam.ActorId, logParam.CustId, logParam.IP, param.Functions, oldFunctions);

            // Act
            settingController.UpdateFunctions(userId, param).ConfigureAwait(false);

            // Assert
            settingService.Received().GetFunctions(userId);
            settingService.Received().UpdateFunctions(
                userId,
                param.Functions,
                Arg.Is<UpdateFunctionsLog>(l => compareLogic.Compare(l, logData).AreEqual));
        }

        [Fact]
        public void GetFunctions_Always_CallsSettingServiceGetFunctions()
        {
            // Arrange
            var userId = 1;
            var language = "en-US";

            // Act
            settingController.GetFunctions(userId, language).ConfigureAwait(false);

            // Assert
            settingService.Received().GetFunctions(userId, language);
        }

        [Fact]
        public void UpdateLanguage_Always_CallsGetUserAndUpdateLanguage()
        {
            // Arrange
            var userId = 1;
            var language = "vi-VN";
            var logParam = CreateDefaultLogParam();
            var user = new User(1, 2, 2, 0, "en-US", null);
            settingService.GetUser(userId).Returns(Task.FromResult(user));
            var logData = new UpdateLanguageLog(logParam.ActorId, logParam.CustId, logParam.IP, language, user.Language);

            // Act
            settingController.UpdateLanguage(userId, language, logParam).ConfigureAwait(false);

            // Assert
            settingService.Received().GetUser(userId);
            settingService.Received().UpdateLanguage(
                userId,
                language,
                Arg.Is<UpdateLanguageLog>(l => compareLogic.Compare(l, logData).AreEqual));
        }

        [Fact]
        public void GenerateVerificationCode_Always_CallsSettingServiceGenerateVerificationCode()
        {
            // Arrange
            var userId = 1;
            var param = new VerificationCodeParam
            {
                MessengerType = 1,
                MessengerUsername = "username"
            };

            // Act
            settingController.GenerateVerificationCode(userId, param).ConfigureAwait(false);

            // Assert
            settingService.Received().GenerateVerificationCode(
                userId,
                Arg.Is<Messenger>(m => m.Type == param.MessengerType && m.Username == param.MessengerUsername));
        }

        [Fact]
        public void VerifyVerificationCode_Always_CallSettingServiceVerifyVerificationCode()
        {
            // Arrange
            var userId = 1;
            var logParam = CreateDefaultLogParam();
            var param = new VerificationCodeParam
            {
                VerificationCode = "123456",
                MessengerType = 1,
                MessengerUsername = "username",
                LogParam = logParam
            };
            var messenger = new Messenger(param.MessengerType, param.MessengerUsername);
            var logData = new VerifyAccountLog(
                logParam.ActorId,
                logParam.CustId,
                logParam.IP,
                messenger);

            // Act
            settingController.VerifyVerificationCode(userId, param).ConfigureAwait(false);

            // Assert
            settingService.VerifyVerificationCode(
                userId,
                param.VerificationCode,
                Arg.Is<Messenger>(m => compareLogic.Compare(m, messenger).AreEqual),
                Arg.Is<VerifyAccountLog>(l => compareLogic.Compare(l, logData).AreEqual));
        }

        [Fact]
        public void Unsubscribe_Always_CallsSettingServiceUnsubscribe()
        {
            // Arrange
            var userId = 1;
            var messengerType = 1;
            var messengerUsername = "username";
            var logParam = CreateDefaultLogParam();
            var messenger = new Messenger(messengerType, messengerUsername);
            var logData = new UnlinkAccountLog(logParam.ActorId, logParam.CustId, logParam.IP, messenger);

            // Act
            settingController.Unsubscribe(userId, messengerType, messengerUsername, logParam).ConfigureAwait(false);

            // Assert
            settingService.Unsubscribe(
                userId,
                Arg.Is<Messenger>(m => compareLogic.Compare(m, messenger).AreEqual),
                Arg.Is<UnlinkAccountLog>(l => compareLogic.Compare(l, logData).AreEqual));
        }

        [Fact]
        public void GetMessenger_Always_CallsSettingServiceGetMessenger()
        {
            // Arrange
            var messengerType = 1;
            var messengerUsername = "username";

            // Act
            settingController.GetMessenger(messengerType, messengerUsername).ConfigureAwait(false);

            // Assert
            settingService.Received().GetMessenger(messengerType, messengerUsername);
        }

        [Fact]
        public void GetUsersByFunction_Always_CallsSettingServiceGetUsersByFunction()
        {
            // Arrange
            var functionId = 1;
            var environment = 0;

            // Act
            settingController.GetUsersByFunction(functionId, environment).ConfigureAwait(false);

            // Assert
            settingService.Received().GetUsersByFunction(functionId, environment);
        }

        [Fact]
        public void GetAllFunctions_Always_CallsSettingServiceGetAllFunctions()
        {
            // Arrange
            var language = "en-US";

            // Act
            settingController.GetAllFunctions(language).ConfigureAwait(false);

            // Assert
            settingService.Received().GetAllFunctions(language);
        }

        private static LogParam CreateDefaultLogParam()
            => new LogParam
            {
                ActorId = 1,
                CustId = 1,
                IP = "127.0.0.1"
            };
    }
}