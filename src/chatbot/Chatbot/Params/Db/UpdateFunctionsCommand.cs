namespace Chatbot.Params.Db
{
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.Models;
    using Fanex.Data.Repository;

    public class UpdateFunctionsCommand : NonQueryCommand
    {
        public UpdateFunctionsCommand(int userId, IEnumerable<Function> functions)
        {
            UserId = userId;
            Functions = functions;
        }

        public int UserId { get; }

        public IEnumerable<Function> Functions { get; }

        public override string GetSettingKey() => "UpdateFunctionsCommand";

        public override bool IsValid()
            => UserId > 0
            && Functions.Any();
    }
}