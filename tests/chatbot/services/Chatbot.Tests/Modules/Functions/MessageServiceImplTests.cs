namespace Chatbot.Tests.Modules.Functions
{
    using System;
    using System.Threading.Tasks;
    using Chatbot.Configurations;
    using Chatbot.Modules.Funtions;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;

    public class MessageServiceImplTests
    {
        private readonly IDynamicRepository subDynamicRepository;
        private readonly IChatbotConfigs subChatbotConfigs;

        public MessageServiceImplTests()
        {
            subDynamicRepository = Substitute.For<IDynamicRepository>();
            subChatbotConfigs = Substitute.For<IChatbotConfigs>();
        }

        private NotificationServiceImpl CreateMessageServiceImpl()
        {
            return new NotificationServiceImpl(subDynamicRepository, subChatbotConfigs);
        }

        [Fact]
        public async Task GetLastQueryTime_ExecuteGetFromDynamicRepository_ReturnExpectedResult()
        {
            // Arrange
            var lastQueryTime = new DateTime(2018, 2, 20);
            subDynamicRepository
                .GetAsync<DateTime>(Arg.Is<GetSettingCriteria>(c => c.SettingName == "SettingName"))
                .Returns(lastQueryTime);
            var service = CreateMessageServiceImpl();

            // Act
            var actualLastQueryTime = await service.GetLastQueryTime(2);

            // Assert
            Assert.Equal(lastQueryTime, actualLastQueryTime);
        }

        [Fact]
        public async Task UpdateLastQueryTime_Always_CallExecuteFromDynamicRepository()
        {
            // Arrange
            var service = CreateMessageServiceImpl();
            var lastQueryTime = new DateTime(2018, 2, 20);

            // Act
            await service.UpdateLastQueryTime(2, lastQueryTime);

            // Assert
            await subDynamicRepository.Received().ExecuteAsync(
                Arg.Is<UpdateSettingCommand>(
                    c => c.SettingName == "SettingName" && c.Value == lastQueryTime.ToString()));
        }
    }
}