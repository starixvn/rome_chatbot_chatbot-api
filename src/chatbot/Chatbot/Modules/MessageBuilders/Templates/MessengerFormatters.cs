namespace Chatbot.Modules.MessageBuilders.Templates
{
    public interface IMessengerFormatter
    {
        string NewLine();

        string Point();
    }

    public class LetstalkFormatter : IMessengerFormatter
    {
        public string NewLine()
            => "\n";

        public string Point()
            => "\u2022";
    }
}