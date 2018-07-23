namespace Chatbot.Params.Db
{
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.Models;
    using Fanex.Data.Repository;

    public class UpdateFunctionsCommand : NonQueryCommand
    {
        public int UserId { get; set; }

        public IEnumerable<Function> Functions { get; set; }

        public override string GetSettingKey() => "UpdateFunctionsCommand";

        public override bool IsValid()
            => UserId > 0
            && Functions.Any();
    }
}