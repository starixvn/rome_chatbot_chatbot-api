namespace Chatbot.Client.Models
{
    public class Result
    {
        public int Code { get; set; }

        public bool IsOk => Code == 0;

        public bool IsNotOk => !IsOk;

        public string Message { get; set; }

        public static Result CreateSuccessfulResult()
            => new Result();
    }

    public class VerificationCodeResult : Result
    {
        private const int VerificationCodeExpiredCode = 3;
        private const int WrongVerificationCode = 4;

        public int FailedTimes { get; set; }

        public bool IsVerificationCodeExpired
            => Code == VerificationCodeExpiredCode;

        public bool IsWrongVerificationCode
            => Code == WrongVerificationCode;
    }

    public class AddingUserResult : Result
    {
        private const int OverMaxAddAccountTimes = 6;
        private const int MessengerAccountDoesNotExist = 2;
        private const int MessengerAccountHasBeenRegisteredByAnotherCustomer = 5;

        public bool IsMessengerAccountHasBeenRegisteredByAnotherCustomer
            => Code == MessengerAccountHasBeenRegisteredByAnotherCustomer;

        public bool IsOverMaxAddAccountTimes
           => Code == OverMaxAddAccountTimes;

        public bool IsMessengerAccountDoesNotExist
           => Code == MessengerAccountDoesNotExist;

        public int UserId { get; set; }
    }
}