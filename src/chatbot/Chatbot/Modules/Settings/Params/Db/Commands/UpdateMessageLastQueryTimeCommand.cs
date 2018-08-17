namespace Chatbot.Modules.Settings.Params.Db.Commands
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateMessageLastQueryTimeCommand : NonQueryCommand
    {
        public UpdateMessageLastQueryTimeCommand(
           int userId,
           int functionId,
           int messageType,
           DateTime lastQueryTime)
        {
            UserId = userId;
            FunctionId = functionId;
            MessageType = messageType;
            LastQueryTime = lastQueryTime;
        }

        public int UserId { get; }

        public int FunctionId { get; }

        public int MessageType { get; }

        public DateTime LastQueryTime { get; }

        public override string GetSettingKey() => "UpdateMessageLastQueryTime";

        public override bool IsValid()
            => UserId > 0 && FunctionId > 0 && MessageType > -1;
    }
}