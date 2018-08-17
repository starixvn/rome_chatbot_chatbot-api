namespace Chatbot.Modules.Settings.Params.Db.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.Models;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateUserFunctionsCommand : NonQueryCommand
    {
        private const string RootXml = "<ROOT>{0}</ROOT>";

        public UpdateUserFunctionsCommand(int userId, IEnumerable<Function> functions)
        {
            UserId = userId;
            Functions = functions;
        }

        public int UserId { get; }

        [SpParam(Ignored = true)]
        public IEnumerable<Function> Functions { get; }

        public string FunctionXml
            => string.Format(RootXml, FormatedFunctionXml);

        private string FormatedFunctionXml
            => string.Concat(Functions.Select(function => $"<Function UserId=\"{UserId}\" FunctionId=\"{function.Id}\" Command=\"{function.Command}\" Enable=\"{function.Enabled}\"/> "));

        public override string GetSettingKey() => "UpdateUserFunctions";

        public override bool IsValid()
            => UserId > 0
            && Functions.Any();
    }
}