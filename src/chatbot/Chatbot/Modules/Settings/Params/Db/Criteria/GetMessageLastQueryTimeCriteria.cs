namespace Chatbot.Modules.Settings.Params.Db.Criteria
{
    using Fanex.Data.Repository;

    public class GetMessageLastQueryTimeCriteria : CriteriaBase
    {
        public GetMessageLastQueryTimeCriteria(
           int userId,
           int functionId,
           int messageType)
        {
            UserId = userId;
            FunctionId = functionId;
            MessageType = messageType;
        }

        public int UserId { get; }

        public int FunctionId { get; }

        public int MessageType { get; }

        public override string GetSettingKey() => "GetMessageLastQueryTime";

        public override bool IsValid() => UserId > 0 && FunctionId > 0 && MessageType > -1;
    }
}