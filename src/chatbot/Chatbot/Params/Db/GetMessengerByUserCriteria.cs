namespace Chatbot.Params.Db
{
    using Fanex.Data.Repository;

    public class GetMessengerByUserCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "GetMessengerByUserCriteria";

        public override bool IsValid()
            => UserId > 0;
    }
}