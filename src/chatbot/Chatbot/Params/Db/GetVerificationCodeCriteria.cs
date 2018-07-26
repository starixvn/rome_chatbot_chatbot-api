namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class GetVerificationCodeCriteria : CriteriaBase
    {
        public GetVerificationCodeCriteria(int userId, int messengerType)
        {
            UserId = userId;
            MessengerType = messengerType;
        }

        public int UserId { get; }

        public int MessengerType { get; }

        public override string GetSettingKey() => "GetVerificationCodeCriteria";

        public override bool IsValid()
            => UserId > 0
            && MessengerType > 0;
    }
}