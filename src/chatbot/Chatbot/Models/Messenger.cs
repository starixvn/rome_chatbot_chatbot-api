namespace Chatbot.Models
{
    public class Messenger
    {
        public Messenger() { }

        public Messenger(
            int type, 
            string username, 
            bool enabled = false,
            bool isVerifying = false)
        {
            Type = type;
            Username = username;
            Enabled = enabled;
            IsVerifying = isVerifying;
        }

        public int Type { get; private set; }

        public string Username { get; private set; }

        public bool Enabled { get; private set; }

        public bool IsVerifying { get; private set; }
    }
}