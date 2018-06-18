namespace LetstalkMockAPI.Models.WebHookEvents
{
    public class MessageEvent : WebHookEvent
    {
        public Payload Payload { get; set; }
    }
}