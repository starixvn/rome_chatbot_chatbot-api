namespace Chatbot.Client.Models
{
    public static class MessengerType
    {
        public const int Letstalk = 1;
    }

    public static class VerificationStatusCode
    {
        public const int Verifying = 1;
        public const int Done = 2;
    }

    public class Messenger
    {
        public int Type { get; set; }

        public string Account { get; set; }

        public bool Enabled { get; set; }

        public int VerificationStatus { get; set; }

        public bool IsVerifying => VerificationStatus == VerificationStatusCode.Verifying;
    }
}