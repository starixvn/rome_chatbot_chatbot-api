namespace Chatbot.Modules.SettingLogs
{
    using System.Threading.Tasks;
    using Alpha.Services.Client.Models;
    using Alpha.Services.Client.Services;

    public interface SettingLogService
    {
        void WriteLog(ILogData logData);
    }

    public class SettingLogServiceImpl : SettingLogService
    {
        private readonly LogService logService;

        public SettingLogServiceImpl(LogService logService)
        {
            this.logService = logService;
        }

        public void WriteLog(ILogData logData)
            => Task.Run(() => logService.WriteLog(logData));
    }
}