namespace Chatbot.Modules.Settings.Params.Db.Commands
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateMessageLastQueryTimeCommand : NonQueryCommand
    {
        public UpdateMessageLastQueryTimeCommand(
           int functionId,
           DateTime lastQueryTime)
        {
            FunctionId = functionId;
            LastQueryTime = lastQueryTime;
        }

        public int FunctionId { get; }

        public DateTime LastQueryTime { get; }

        public override string GetSettingKey() => "UpdateMessageLastQueryTime";

        public override bool IsValid() => FunctionId > 0;
    }
}