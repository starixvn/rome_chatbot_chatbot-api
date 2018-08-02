namespace Chatbot.Services
{
    using SimpleJson;

    public interface IMessageBuilder
    {
        string BuildVerificationCodeMessage(string verificationCode);

        string BuildCommandResultMessage(object data);

        string BuildNotRecognizedCommandMessage();
    }

    public class MessageBuilder : IMessageBuilder
    {
        public string BuildCommandResultMessage(object data)
        {
            if (data == null)
            {
                return "There is no data.";
            }

            return SimpleJson.SerializeObject(data);
        }

        public string BuildNotRecognizedCommandMessage()
            => "Your command is not recognized.";

        public string BuildVerificationCodeMessage(string verificationCode)
            => $"Your Letstalk validation code is {verificationCode}. The validation code will be expired after 5 minutes.";
    }
}