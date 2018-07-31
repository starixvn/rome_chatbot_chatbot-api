namespace Chatbot.AutoUM.Service
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using Chatbot.Constants;
    using Chatbot.Services;
    using Fanex.Caching;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using ONEbook.UM.AspNetCore.Services;
    using ONEbook.UM.Core.Models;

    internal class AutoUMService : IHostedService
    {
        private const string UMInfoCachedKey = "UMInfoCachedKey";
        private readonly IMaintenanceService _maintenanceService;
        private readonly ICacheService _cacheService;
        private readonly MessengerFactory _messengerFactory;
        private readonly IConfiguration _configuration;

        public AutoUMService(
            IMaintenanceService maintenanceService,
            ICacheService cacheService,
            MessengerFactory messengerFactory,
            IConfiguration configuration)
        {
            _maintenanceService = maintenanceService;
            _cacheService = cacheService;
            _messengerFactory = messengerFactory;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(GetUnderMaintenanceTime);
            timer.Interval = Convert.ToDouble(_configuration.GetSection("GetUnderMaintenanceInfoInterval")?.Value ?? "10000");
            timer.Enabled = true;

            return Task.CompletedTask;
        }

        private async void GetUnderMaintenanceTime(object sender, EventArgs e)
        {
            var umInfo = _maintenanceService.GetUnderMaintenanceTime();
            var messengerClient = _messengerFactory.CreateMessengerClient(MessengerType.Letstalk);

            if (IsNewUMInfo(umInfo))
            {
                _cacheService.Set(
                    UMInfoCachedKey,
                    umInfo,
                    new CacheItemOptions { SlidingExpiration = TimeSpan.FromHours(2) });

                if (umInfo.IsUnderMaintenanceTime)
                {
                    await messengerClient.SetAutoReplyAsync(
                        $"System is under maintenance from {umInfo.From} to {umInfo.To}",
                        true,
                        Convert.ToDateTime(umInfo.From),
                        Convert.ToDateTime(umInfo.To));
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool IsNewUMInfo(UnderMaintenanceTime umInfo)
        {
            if (umInfo == null)
            {
                return false;
            }

            var oldUMInfo = _cacheService.Get<UnderMaintenanceTime>(UMInfoCachedKey);

            return oldUMInfo == null ||
                umInfo.From != oldUMInfo.From ||
                umInfo.To != oldUMInfo.To ||
                umInfo.ErrorCode != oldUMInfo.ErrorCode ||
                umInfo.IsUnderMaintenanceTime != oldUMInfo.IsUnderMaintenanceTime;
        }

        private async Task SendSkypeTest(UnderMaintenanceTime umInfo)
        {
            umInfo.From = DateTime.Now.ToString();
            umInfo.To = DateTime.Now.AddHours(2).ToString();
            var skypeClient = _messengerFactory.CreateMessengerClient(MessengerType.Skype);
            await skypeClient.SetAutoReplyAsync(
                  $"System is under maintenance  from {umInfo.From} to {umInfo.To}",
                  true,
                  Convert.ToDateTime(umInfo.From),
                  Convert.ToDateTime(umInfo.To));
        }
    }
}