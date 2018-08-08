namespace Chatbot.Schedule.Service
{
    using System;
    using System.Threading.Tasks;
    using Chatbot.Schedule.Service.Models;
    using Quartz;

    public interface IJobRunner
    {
        Task Start(ScheduleFunction scheduleFunction, IJobDetail job = null, ITrigger trigger = null);
    }

    public class JobRunner : IJobRunner
    {
        private readonly ISchedulerFactory schedulerFactory;

        public JobRunner(ISchedulerFactory schedulerFactory)
        {
            this.schedulerFactory = schedulerFactory;
        }

        public async Task Start(ScheduleFunction scheduleFunction, IJobDetail job = null, ITrigger trigger = null)
        {
            var scheduler = await schedulerFactory.GetScheduler();

            await scheduler.ScheduleJob(
                job ?? CreateJob(scheduleFunction),
                trigger ?? CreateTrigger(scheduleFunction));

            await scheduler.Start();
        }

        private static IJobDetail CreateJob(ScheduleFunction scheduleFunction)
        {
            var jobType = Type.GetType($"Chatbot.Schedule.Service.Jobs.{scheduleFunction.Name}Job");

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