namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class GetVerificationCodeCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int MessengerType { get; set; }

        public override string GetSettingKey() => "GetVerificationCodeCriteria";

        public override bool IsValid()
            => UserId > 0
            && MessengerType > 0;
    }
}