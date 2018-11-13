namespace Chatbot.Modules.Actions.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Chatbot.Modules.Actions.Models;
    using Shared.Extensions;

    public interface IActionCompiler
    {
        string Compile(IEnumerable<Models.Action> actionChain, IEnumerable<UserAction> userActions);
    }

    public class DefaultActionCompiler : IActionCompiler
    {
        public virtual string Compile(IEnumerable<Models.Action> actionChain, IEnumerable<UserAction> userActions)
        {
            if (!actionChain.Any())
            {
                return string.Empty;
            }

            var distinctUserActions = userActions
                .DistinctBy(userAction => userAction.ActionId)
                .OrderBy(userAction => userAction.ActionId)
                .Where(userAction => actionChain.Any(action => action.Id == userAction.ActionId));

            var command = new StringBuilder();

            foreach (var userAction in distinctUserActions)
            {
                AppendCommand(actionChain, command, userAction);
            }

            return command.ToString().TrimEnd();
        }

        private static void AppendCommand(
            IEnumerable<Models.Action> actionChain,
            StringBuilder command,
            UserAction userAction)
        {
            var action = actionChain.FirstOrDefault(act => act.Id == userAction.ActionId)
                                ?? throw new InvalidOperationException($"Cannot found action {userAction.ActionId} of user {userAction.UserId}");

            var actionCommand = action.Type == ActionType.Button
                ? action.Command
                : userAction.UserResponse;

            if (!string.IsNullOrWhiteSpace(actionCommand))
            {
                command
                    .Append(actionCommand)
                    .Append(" ");
            }
        }
    }
}