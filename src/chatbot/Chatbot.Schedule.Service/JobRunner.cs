namespace Chatbot.Schedule.Service
{
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Models;
    using Quartz;

    public interface IJobRunner
    {
        Task Start(ScheduleFunction scheduleFunction, IJobDetail job = null, ITrigger trigger = null);
    }

    public class JobRunner : IJobRunner
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public JobRunner(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task Start(ScheduleFunction scheduleFunction, IJobDetail job = null, ITrigger trigger = null)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            await scheduler.ScheduleJob(
                job ?? CreateJob(scheduleFunction),
                trigger ?? CreateTrigger(scheduleFunction));

            await scheduler.Start();
        }

        private static IJobDetail CreateJob(ScheduleFunction scheduleFunction)
        {
            var jobType = Assembly.GetExecutingAssembly()
                 .GetTypes()
                 .FirstOrDefault(t => t.Name == $"{scheduleFunction.Name}Job");

            return JobBuilder.Create(jobType)
                .WithIdentity(scheduleFunction.Name, scheduleFunction.GroupName)
                .Build();
        }

        private static ITrigger CreateTrigger(ScheduleFunction scheduleFunction)
            => TriggerBuilder.Create()
                        .WithIdentity($"{scheduleFunction.Name}Trigger", scheduleFunction.GroupName)
                        .WithCronSchedule(scheduleFunction.CronExpression)
                        .Build();
    }
}