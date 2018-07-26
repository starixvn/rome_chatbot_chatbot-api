namespace Chatbot.Models
{
    public class Messenger
    {
        public Messenger(int type, string account, bool enabled = false)
        {
            Type = type;
            Account = account;
            Enabled = enabled;
        }

        public int Type { get; }

        public string Account { get; }

        public bool Enabled { get; }
    }
}