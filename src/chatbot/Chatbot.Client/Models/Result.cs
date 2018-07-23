namespace Chatbot.Client.Models
{
    public class Result
    {
        public int Code { get; set; }

        public bool IsOk => Code == 0;

        public bool IsNotOk => !IsOk;

        public string Message { get; set; }
    }

    public class AddingUserResult : Result
    {
        private const int AccountDoesNotExistCode = 2;

        public static AddingUserResult CreateAccountDoesNotExistResult()
            => new AddingUserResult { Code = AccountDoesNotExistCode };

        public string VerificationCode { get; set; }

        public string UserId { get; set; }
    }
}