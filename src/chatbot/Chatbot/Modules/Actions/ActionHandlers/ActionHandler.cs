namespace Chatbot.Modules.Actions.ActionHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Chatbot.Modules.Actions.Models;
    using Shared.Extensions;

    public interface IActionHandler
    {
        string BuildCommand(IEnumerable<Models.Action> actionChain, IEnumerable<UserActionItemDto> userActions);
    }

    public class DefaultActionHandler : IActionHandler
    {
        public virtual string BuildCommand(IEnumerable<Models.Action> actionChain, IEnumerable<UserActionItemDto> userActions)
        {
            if (!actionChain.Any())
            {
                return string.Empty;
            }

            var distinctUserActions = userActions
                .DistinctBy(userAction => userAction.ActionId)
                .OrderBy(userAction => userAction.ActionId);
            var command = string.Empty;

            foreach (var userAction in distinctUserActions)
            {
                var action =
                    actionChain.FirstOrDefault(act => act.Id == userAction.ActionId)
                    ?? throw new InvalidOperationException($"Cannot found action {userAction.ActionId} of user {userAction.UserId}");

                command += action.Type == ActionType.Button
                    ? action.Command
                    : userAction.UserResponse;
            }

            return command.TrimEnd();
        }
    }
}