namespace Alpha.Chatbot.Models
{
    using System.Collections.Generic;

    public interface IFunction
    {
        int Id { get; }

        string Name { get; }

        bool Enabled { get; }

        int Type { get; }

        IEnumerable<string> Commands { get; }
    }

    public class Function : IFunction
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public int Type { get; set; }

        public IEnumerable<string> Commands { get; set; }
    }
}
