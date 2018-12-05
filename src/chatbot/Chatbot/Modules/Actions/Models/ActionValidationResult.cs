namespace Chatbot.Modules.Actions.Models
{
    using Chatbot.Models;

    public class ActionValidationResult : Result
    {
        public ActionValidationResult(
            int code,
            string message = "",
            bool forcedCancelRequest = false)
            : base(code, message)
        {
            ForcedCancelRequest = forcedCancelRequest;
        }

        public bool ForcedCancelRequest { get; }

        public static ActionValidationResult CreateSuccessfulValidationResult(string message = "", bool forcedCancelRequest = false)
            => new ActionValidationResult(0, message, forcedCancelRequest);

        public static ActionValidationResult CreateErrorValidationResult(string message = "", bool forcedCancelRequest = false)
            => new ActionValidationResult(1, message, forcedCancelRequest);
    }
}