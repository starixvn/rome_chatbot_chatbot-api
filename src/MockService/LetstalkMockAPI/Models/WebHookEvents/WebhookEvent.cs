namespace LetstalkMockAPI.Models.WebHookEvents
{
    public class WebHookEvent
    {
        public string Type { get; set; }

        public long TimeStamp { get; set; }

        public Source Source { get; set; }
    }
}