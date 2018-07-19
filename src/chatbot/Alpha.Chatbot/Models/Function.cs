namespace Alpha.Chatbot.Models
{
    public interface IFunction
    {
        int Id { get; }

        string Name { get; }

        bool Enabled { get; }

        int Type { get; }

        string Command { get; }
    }

    public class Function : IFunction
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public int Type { get; set; }

        public string Command { get; set; }
    }
}