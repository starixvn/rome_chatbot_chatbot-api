namespace Chatbot.Tests.Modules.Functions
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Chatbot.Configurations;
    using Chatbot.Modules.Funtions;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;

    public class NotificationServiceImplTests
    {
        private readonly IDynamicRepository subDynamicRepository;
        private readonly IChatbotConfigs subChatbotConfigs;

        public NotificationServiceImplTests()
        {
            subDynamicRepository = Substitute.For<IDynamicRepository>();
            subChatbotConfigs = Substitute.For<IChatbotConfigs>();
            subChatbotConfigs.PersonalMessageFunctionId.Returns(5);
            subChatbotConfigs.SpecialAnnouncementFunctionId.Returns(6);
            subChatbotConfigs.NormalAnnouncementFunctionId.Returns(7);
            subChatbotConfigs.SystemUpdateFunctionId.Returns(8);
        }

        private NotificationServiceImpl CreateNotificationServiceImpl() => new NotificationServiceImpl(subDynamicRepository, subChatbotConfigs);

        [Theory]
        [InlineData(5, "LastQueryPersonalMessage")]
        [InlineData(6, "LastQuerySpecialAnnouncement")]
        [InlineData(7, "LastQueryNormalAnnouncement")]
        [InlineData(8, "LastQuerySystemAnnouncement")]
        public async Task GetLastQueryTime_ExecuteGetFromDynamicRepository_ReturnExpectedResult(int functionId, string settingName)
        {
            // Arrange
            var lastQueryTime = new DateTime(2018, 2, 20);
            subDynamicRepository
                .GetAsync<DateTime>(Arg.Is<GetSettingCriteria>(c => c.SettingName == settingName))
                .Returns(lastQueryTime);
            var service = CreateNotificationServiceImpl();

            // Act
            var actualLastQueryTime = await service.GetLastQueryTime(functionId);

            // Assert
            Assert.Equal(lastQueryTime, actualLastQueryTime);
        }

        [Theory]
        [InlineData(5, "LastQueryPersonalMessage")]
        [InlineData(6, "LastQuerySpecialAnnouncement")]
        [InlineData(7, "LastQueryNormalAnnouncement")]
        [InlineData(8, "LastQuerySystemAnnouncement")]
        public async Task UpdateLastQueryTime_Always_CallExecuteFromDynamicRepository(int functionId, string settingName)
        {
            // Arrange
            var service = CreateNotificationServiceImpl();
            var lastQueryTime = new DateTime(2018, 2, 20);

            // Act
            await service.UpdateLastQueryTime(functionId, lastQueryTime);

            // Assert
            await subDynamicRepository.Received().ExecuteAsync(
                Arg.Is<UpdateSettingCommand>(
                    c => c.SettingName == settingName && c.Value == lastQueryTime.ToString(CultureInfo.InvariantCulture)));
        }
    }
}