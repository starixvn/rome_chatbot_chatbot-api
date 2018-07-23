namespace Chatbot.Models
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
        private const int MessengerAccountDoesNotExist = 2;

        public static AddingUserResult CreateAccountDoesNotExistResult()
            => new AddingUserResult { Code = MessengerAccountDoesNotExist };

        public string VerificationCode { get; set; }

        public int UserId { get; set; }
    }
}