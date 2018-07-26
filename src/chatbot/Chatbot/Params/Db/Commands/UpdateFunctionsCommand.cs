namespace Chatbot.Params.Db.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.Models;

    public class UpdateFunctionsCommand : BaseCommand
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