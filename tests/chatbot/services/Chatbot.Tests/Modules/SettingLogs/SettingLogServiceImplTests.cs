namespace Chatbot.Tests.Modules.SettingLogs
{
    using Alpha.Services.Client.Services;
    using Chatbot.Modules.SettingLogs;
    using NSubstitute;

    public class SettingLogServiceImplTests
    {
        private readonly LogService logService;
        private readonly SettingLogServiceImpl settingLogService;

        public SettingLogServiceImplTests()
        {
            logService = Substitute.For<LogService>();
            settingLogService = new SettingLogServiceImpl(logService);
        }

        //[Fact]
        //public void WriteLog_Always_CallsLogServiceWriteLog()
        //{
        //    // Arrange
        //    var logData = Substitute.For<ILogData>();

        //    // Act
        //    settingLogService.WriteLog(logData);

        //    // Assert
        //    logService.Received().WriteLog(Arg.Is<ILogData>(logData));
        //}
    }
}