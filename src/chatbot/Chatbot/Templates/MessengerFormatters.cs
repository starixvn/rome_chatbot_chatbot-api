namespace Chatbot.Templates
{
    public interface IMessengerFormatter
    {
        string NewLine();
    }

    public class LetstalkFormatter : IMessengerFormatter
    {
        public string NewLine()
            => "\n";
    }
}