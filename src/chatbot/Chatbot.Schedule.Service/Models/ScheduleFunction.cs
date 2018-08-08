namespace Chatbot.Schedule.Service.Models
{
    public class ScheduleFunction
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string GroupName { get; set; }

        public bool IsActive { get; set; }

        public string CronExpression { get; set; }
    }
}