namespace Chatbot.Schedule.Service.MessageHandler.Builders
{
    using System.Collections.Generic;

    internal class MessageBuilder4 : IMessageBuilder
    {
        public string GetMessage(IDictionary<string, object> parameters)
        {
            var custId = parameters["custId"];

            return $"WinLoss-Testing-Message\nCustId: {custId}";
        }
    }
}