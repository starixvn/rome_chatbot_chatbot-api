namespace Chatbot.Modules.MessageServices.Params.Db
{
    using System;
    using Fanex.Data.Repository;

    public class GetMessageLogCriteria : CriteriaBase
    {
        public GetMessageLogCriteria(
            int userId,
            DateTime fromDate,
            DateTime toDate,
            int functionId,
            int messengerType)
        {
            UserId = userId;
            FromDate = fromDate.Date;
            ToDate = toDate.Date.AddDays(1).AddSeconds(-1);
            FunctionId = functionId;
            MessengerType = messengerType;
        }

        public int UserId { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public int FunctionId { get; }

        public int MessengerType { get; }

        public override string GetSettingKey()
            => "Chatbot_GetMessageLog";

        public override bool IsValid()
            => UserId > 0 && ToDate > FromDate;
    }
}