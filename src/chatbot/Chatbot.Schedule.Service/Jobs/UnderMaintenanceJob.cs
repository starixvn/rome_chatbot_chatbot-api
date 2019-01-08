namespace Chatbot.Schedule.Service.Jobs
{
    using System.Threading.Tasks;
    using Chatbot.Client.Services;
    using ONEbook.UM.Services;
    using Quartz;

    public class UnderMaintenanceJob : IJob
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly MessageService messageService;

        public UnderMaintenanceJob()
            : this(null, null)
        {
        }

        public UnderMaintenanceJob(
            IMaintenanceService maintenanceService,
            MessageService messageService)
        {
            this.maintenanceService = maintenanceService ?? new MaintenanceService();
            this.messageService = messageService ?? new MessageServiceImpl();
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var umInfo = await maintenanceService.GetUnderMaintenanceTime();

            if (umInfo.IsUnderMaintenanceTime != GlobalVariables.IsUM)
            {
                GlobalVariables.IsUM = umInfo.IsUnderMaintenanceTime;

                if (!umInfo.IsUnderMaintenanceTime)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Run(() => messageService.SendCompleteUMNotifications());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
        }
    }
}