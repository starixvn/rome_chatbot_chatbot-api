namespace Chatbot.WebApi.Params
{
    public class CommandParam
    {
        public string Command { get; set; }

        public string ButtonData { get; set; }

        public int MessengerType { get; set; }

        public string MessengerUsername { get; set; }
    }
}