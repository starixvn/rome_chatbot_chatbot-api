namespace Chatbot.Models
{
    /// <summary>
    /// This class does not immutable because it is used for deserialize object in controllers.
    /// </summary>
    public class Function
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public string Command { get; set; }

        public bool Enabled { get; set; }

        public int Environment { get; set; }
    }
}