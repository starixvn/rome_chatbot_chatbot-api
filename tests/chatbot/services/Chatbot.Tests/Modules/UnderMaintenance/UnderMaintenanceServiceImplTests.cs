namespace Chatbot.Tests.Modules.UnderMaintenance
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Alpha.Services.Client.Services;
    using Chatbot.Models;
    using Chatbot.Modules.Actions;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.Modules.Settings;
    using Chatbot.Modules.UnderMaintenance;
    using Chatbot.Shared.Configurations;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using Xunit;
    using MessengerClient = Messenger.Client;

    public class UnderMaintenanceServiceImplTests
    {
        private readonly SettingService settingService;
        private readonly ActionService actionService;
        private readonly CustomerService customerService;
        private readonly IMessageBuilder messageBuilder;
        private readonly IMessageSender messageSender;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly UnderMaintenanceServiceImpl underMaintenanceService;
        private readonly CompareLogic compareLogic;

        public UnderMaintenanceServiceImplTests()
        {
            settingService = Substitute.For<SettingService>();
            actionService = Substitute.For<ActionService>();
            customerService = Substitute.For<CustomerService>();
            messageBuilder = Substitute.For<IMessageBuilder>();
            messageSender = Substitute.For<IMessageSender>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();

            chatbotConfigs.AllowSendCompleteUMNotificationMessengerUsernames
                .Returns(new List<string> { "*" });

            underMaintenanceService = new UnderMaintenanceServiceImpl(
                settingService,
                actionService,
                customerService,
                messageBuilder,
                messageSender,
                chatbotConfigs);

            compareLogic = new CompareLogic();
        }

        [Fact]
        public void SendCompleteUMNotifications_Always_CallsSettingServiceGetUsers()
        {
            // Act
            underMaintenanceService.SendCompleteUMNotifications().ConfigureAwait(false);

            // Assert
            settingService.Received().GetUsers();
        }

        [Fact]
        public void SendCompleteUMNotifications_UsersIsNull_ReturnsSuccessfulResult()
        {
            // Arrange
            IEnumerable<User> users = null;
            settingService.GetUsers().Returns(Task.FromResult(users));
            var expectedResult = Result.CreateSuccessfulResult();

            // Act
            var actualResult = underMaintenanceService.SendCompleteUMNotifications().GetAwaiter().GetResult();

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public void SendCompleteUMNotifications_UsersIsEmptyList_ReturnsSuccessfulResult()
        {
            // Arrange
            var users = Enumerable.Empty<User>();
            settingService.GetUsers().Returns(Task.FromResult(users));
            var expectedResult = Result.CreateSuccessfulResult();

            // Act
            var actualResult = underMaintenanceService.SendCompleteUMNotifications().GetAwaiter().GetResult();

            // Assert
            Assert.True(compareLogic.Compare(expectedResult, actualResult).AreEqual);
        }

        [Fact]
        public void SendCompleteUMNotifications_UsersIsNotNullOrEmptyList_CallsCustomerServiceIsAccountClosed()
        {
            // Arrange
            var user = new User(1, 2, 3, 0, "en-US", 15);
            IEnumerable<User> users = new List<User> { user };

            settingService.GetUsers().Returns(Task.FromResult(users));

            // Act
            underMaintenanceService.SendCompleteUMNotifications().GetAwaiter().GetResult();

            // Assert
            customerService.Received().IsAccountClosed(2, 0);
        }

        [Fact]
        public void SendCompleteUMNotifications_UsersIsNotClosed_CallsGetMessengers()
        {
            // Arrange
            var user = new User(1, 2, 3, 0, "en-US", 15);
            IEnumerable<User> users = new List<User> { user };
            settingService.GetUsers().Returns(Task.FromResult(users));
            customerService.IsAccountClosed(user.CustId, user.SubAccountId).Returns(Task.FromResult(false));

            // Act
            underMaintenanceService.SendCompleteUMNotifications().GetAwaiter().GetResult();

            // Assert
            settingService.Received().GetMessengers(user.Id, true);
        }

        [Fact]
        public void SendCompleteUMNotifications_MessengerIsNotNullOrEmptyList_CallsGetFunctionsAndRootActions()
        {
            // Arrange
            var user = new User(1, 2, 3, 0, "en-US", 15);
            IEnumerable<User> users = new List<User> { user };
            IEnumerable<Messenger> messengers = new List<Messenger>
            {
                new Messenger(1,"username",true,false)
            };
            settingService.GetUsers().Returns(Task.FromResult(users));
            customerService.IsAccountClosed(user.CustId, user.SubAccountId).Returns(Task.FromResult(false));
            settingService.GetMessengers(1, true).Returns(Task.FromResult(messengers));

            // Act
            underMaintenanceService.SendCompleteUMNotifications().GetAwaiter().GetResult();

            // Assert
            settingService.Received().GetFunctions(user.Id);
            actionService.Received().GetRootActions(user);
        }
    }
}