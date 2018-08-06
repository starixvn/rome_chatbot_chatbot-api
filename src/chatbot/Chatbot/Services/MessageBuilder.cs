namespace Chatbot.Services
{
    using SimpleJson;

    public interface IMessageBuilder
    {
        string BuildVerificationCodeMessage(string verificationCode, int expiredVerificationCodeInMinutes);

        string BuildCommandResultMessage(object data);

        string BuildNotRecognizedCommandMessage();

        string BuildWelcomeMessage();

        string BuildErrorMessage();
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

        public string BuildErrorMessage()
            => "Oops...you are here because of a system error, please notify the administrators";

        public string BuildNotRecognizedCommandMessage()
            => "Your command is not recognized.";

        public string BuildVerificationCodeMessage(string verificationCode, int expiredVerificationCodeInMinutes)
            => $"Your Letstalk validation code is {verificationCode}. The validation code will be expired after {expiredVerificationCodeInMinutes} minutes.";

        public string BuildWelcomeMessage()
            => "Your account has been verified, Chatbot is fully functioning. You can turn off any feature via the Chatbot configuration page on your agency site.";
    }
}