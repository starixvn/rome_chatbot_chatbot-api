using LetstalkMockAPI.Models;

namespace LetstalkMockAPI.ViewModels
{
    public class PushMessageViewModel
    {
        public string[] Receiver { get; set; }

        public Payload Payload { get; set; }
    }
}
