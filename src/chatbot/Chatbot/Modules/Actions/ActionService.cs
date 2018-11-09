namespace Chatbot.Modules.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.ActionHandlers;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Actions.Params.DB.Commands;
    using Chatbot.Modules.Actions.Params.DB.Criteria;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Messenger.Client.Models;
    using Shared.Extensions;
    using Action = Models.Action;

    using MessengerClient = Messenger.Client;
    using Result = Chatbot.Models.Result;

    public interface ActionService
    {
        Task<ActionResult> BuildAction(string command, string buttonData, User user);

        Task ResetUserAction(User user);
    }

    public class ActionServiceImpl : ActionService
    {
        private const string AllActionsCacheKey = "Chatbot.Modules.Actions.AllActions";
        private readonly IDynamicRepository dynamicRepository;

        private readonly ICacheService cacheService;
        private readonly IMessageBuilder messageBuilder;
        private readonly IDictionary<int, IActionValidator> actionValidators;

        public ActionServiceImpl(
            IDynamicRepository dynamicRepository,
            ICacheService cacheService,
            IMessageBuilder messageBuilder,
            IDictionary<int, IActionValidator> actionValidators)
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheService = cacheService;
            this.messageBuilder = messageBuilder;
            this.actionValidators = actionValidators;
        }

        /// <summary>
        /// refactor code later :)
        /// </summary>
        public async Task<ActionResult> BuildAction(string command, string buttonData, User user)
        {
            var actionId = 0;
            var userActions = await GetUserActions(user).ConfigureAwait(false);
            var lastAction = userActions.LastOrDefault();

            if (userActions.Any(userAction => userAction.IsActionCompletedOrCancel))
            {
                lastAction = null;
            }

            var compiledCommand = string.Empty;
            Action action = null;
            if (string.IsNullOrWhiteSpace(buttonData))
            {
                // Handle case not button
                if (lastAction == null)
                {
                    // show root button
                    await ResetUserAction(user).ConfigureAwait(false);
                    return new ActionResult(await BuildRootActionMessage(user.Language), 0);
                }
                actionId = lastAction.ActionId;

                if (actionValidators.Keys.Contains(actionId))
                {
                    var validationResult = actionValidators[actionId].ValidateAction(actionId, user, command);
                    if (validationResult.IsNotOk)
                    {
                        return await BuildWrongValidationResult(actionId, user, validationResult).ConfigureAwait(false);
                    }
                }

                compiledCommand = await CompileCommand(userActions, command).ConfigureAwait(false);
                await UpdateUserAction(lastAction.Id, command, compiledCommand).ConfigureAwait(false);
            }
            else
            {
                var actionInfo = ParseActionInfo(buttonData);
                var isCancelRequest = actionInfo.Item1;
                actionId = actionInfo.Item2;

                if (actionId > 0)
                {
                    action = await GetActionById(actionId).ConfigureAwait(false);

                    if (isCancelRequest)
                    {
                        return await HandleCancelRequest(actionId, action.FunctionId, user).ConfigureAwait(false);
                    }

                    if (action.ParentId == 0)
                    {
                        userActions = new List<UserAction>();
                    }
                    else if (lastAction == null)
                    {
                        return new ActionResult(await BuildRootActionMessage(user.Language).ConfigureAwait(false), 0);
                    }

                    userActions.Add(new UserAction(0, user.Id, action.Id, action.Command, DateTime.Now));
                    compiledCommand = await CompileCommand(userActions, string.Empty).ConfigureAwait(false);
                    await InsertUserAction(user, actionId, string.Empty, compiledCommand).ConfigureAwait(false);
                }
            }

            action = action ?? await GetActionById(actionId).ConfigureAwait(false);

            return action.IsEndAction
                ? new ActionResult(compiledCommand, action.FunctionId)
                : new ActionResult(await BuildNextMessage(actionId, user).ConfigureAwait(false), action.FunctionId);
        }

        private async Task<ActionResult> BuildWrongValidationResult(int actionId, User user, Result result)
        {
            var action = await GetActionById(actionId).ConfigureAwait(false);

            return new ActionResult(
                await BuildMessageWithAbortButton(actionId, user.Language),
                result.Message,
                action.FunctionId);
        }

        private async Task<MessengerClient.Models.Message> BuildRootActionMessage(string language)
        {
            var messageActions = new List<MessageAction>();
            var rootActions = (await GetAllActions().ConfigureAwait(false)).Where(action => action.ParentId == 0);

            foreach (var rootAction in rootActions)
            {
                messageActions.Add(new MessageAction
                {
                    Label = await messageBuilder.BuildActionName(rootAction.Id, language).ConfigureAwait(false),
                    Type = "postback",
                    Data = $"ActionId={rootAction.Id}"
                });
            }

            return new MessengerClient.Models.Message
            {
                Content = messageBuilder.BuildRootActionTitle(language),
                MessageMarkup = new MessageMarkup
                {
                    MessageActions = messageActions
                }
            };
        }

        public async Task InsertUserAction(User user, int actionId, string userResponse, string compileCommand)
            => await dynamicRepository.ExecuteAsync(new InsertUserActionCommand(
                user.Id,
                actionId,
                userResponse,
                compileCommand)).ConfigureAwait(false);

        public async Task UpdateUserAction(int userActionId, string userResponse, string compiledCommand)
            => await dynamicRepository
            .ExecuteAsync(new UpdateUserActionCommand(userActionId, userResponse, compiledCommand))
            .ConfigureAwait(false);

        public async Task<string> CompileCommand(
            IList<UserAction> userActions,
            string userResponse)
        {
            var lastAction = userActions.LastOrDefault();

            if (lastAction != null)
            {
                var actionInfo = await GetActionById(lastAction.ActionId).ConfigureAwait(false);
                var actionHandler = new DefaultActionHandler();
                var actionChain = await GetActionChain(lastAction.ActionId).ConfigureAwait(false);
                lastAction.UserResponse = userResponse;

                return actionHandler.BuildCommand(actionChain, userActions);
            }

            return string.Empty;
        }

        private async Task<Action> GetActionById(int actionId)
            => (await GetAllActions().ConfigureAwait(false))
            .FirstOrDefault(action => action.Id == actionId);

        private (bool, int) ParseActionInfo(string buttonData)
        {
            if (string.IsNullOrWhiteSpace(buttonData))
            {
                return (false, 0);
            }

            var keyValues = buttonData?.Split('&');

            if (keyValues == null
                || keyValues.Count() == 0)
            {
                throw new InvalidOperationException($"Wrong button data format {buttonData}");
            }

            return (buttonData.Contains("abort", StringComparison.OrdinalIgnoreCase),
                int.Parse(keyValues.FirstOrDefault()?.Split('=')[1]));
        }

        private async Task<IList<UserAction>> GetUserActions(User user)
            => (await dynamicRepository
            .FetchAsync<UserAction>(new GetActionsByUser(user.Id))
            .ConfigureAwait(false))
            .OrderBy(userAction => userAction.ActionId)
            .ThenByDescending(userAction => userAction.Id)
            .DistinctBy(userAction => userAction.ActionId)
            .ToList();

        private async Task<IEnumerable<Action>> GetAllActions()
            => await cacheService.GetOrSetAsync(
                       AllActionsCacheKey,
                       () => dynamicRepository
                                        .Fetch<Action>(new GetAllActionsCriteria()),
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0)))
                        .ConfigureAwait(false);

        private async Task<IDictionary<int, IEnumerable<Action>>> GetActionChains()
        {
            var actions = await dynamicRepository
                .FetchAsync<Action>(new GetAllActionsCriteria())
                .ConfigureAwait(false);
            var endActions = actions.Where(action => action.IsEndAction);
            var actionChains = new Dictionary<int, IEnumerable<Action>>();

            foreach (var endAction in endActions)
            {
                var tempAction = endAction;
                var actionChain = new List<Action>();

                while (tempAction != null)
                {
                    actionChain.Add(tempAction);

                    tempAction = await GetPrevAction(tempAction.ParentId).ConfigureAwait(false);
                }

                actionChain.Reverse();
                actionChains.Add(endAction.Id, actionChain);
            }

            return actionChains;
        }

        private async Task<IEnumerable<Action>> GetActionChain(int actionId)
        {
            var actionChains = await GetActionChains().ConfigureAwait(false);
            foreach (var actionChain in actionChains)
            {
                if (actionChain.Value.Any(chain => chain.Id == actionId))
                {
                    return actionChain.Value;
                }
            }

            return Enumerable.Empty<Action>();
        }

        private async Task<Action> GetPrevAction(int parentId)
            => (await GetAllActions().ConfigureAwait(false))
                .FirstOrDefault(action => action.Id == parentId);

        private async Task<MessengerClient.Models.Message> BuildNextMessage(int actionId, User user)
        {
            var nextActions = (await GetAllActions().ConfigureAwait(false))
                .Where(action => action.ParentId == actionId)
                .OrderBy(action => action.DisplayOrder);

            if (ActionIsMessage(nextActions))
            {
                var messageAction = nextActions.FirstOrDefault();
                await InsertUserAction(user, messageAction.Id, string.Empty, string.Empty).ConfigureAwait(false);

                return await BuildMessageWithAbortButton(messageAction.Id, user.Language).ConfigureAwait(false);
            }

            return await BuildButtons(actionId, user.Language, nextActions).ConfigureAwait(false);
        }

        private async Task<MessengerClient.Models.Message> BuildButtons(
            int actionId,
            string language,
            IEnumerable<Action> nextActions)
        {
            var messageActions = new List<MessageAction>();

            foreach (var nextAction in nextActions)
            {
                messageActions.Add(new MessageAction
                {
                    Label = await messageBuilder.BuildActionName(nextAction.Id, language).ConfigureAwait(false),
                    Type = "postback",
                    Data = $"ActionId={nextAction.Id}"
                });
            }

            messageActions.Add(await BuildCancelButton(actionId, language).ConfigureAwait(false));

            return new MessengerClient.Models.Message
            {
                Content = messageBuilder.BuildActionTitle(language),
                MessageMarkup = new MessageMarkup
                {
                    MessageActions = messageActions
                }
            };
        }

        private async Task<MessageAction> BuildCancelButton(int actionId, string language)
        {
            var actionChain = await GetActionChain(actionId).ConfigureAwait(false);
            var rootAction = actionChain.FirstOrDefault();

            return new MessageAction
            {
                Label = messageBuilder.BuildCancelRequestMessage(language),
                Type = "postback",
                Data = $"abortActionId={rootAction?.Id}"
            };
        }

        private async Task<ActionResult> HandleCancelRequest(int rootActionId, int functionId, User user)
        {
            var userActions = await GetUserActions(user).ConfigureAwait(false);
            var rootUserAction = userActions.FirstOrDefault(userAction => userAction.ActionId == rootActionId);

            if (rootUserAction != null)
            {
                await UpdateUserActionStatus(rootUserAction.Id, true);
            }

            return new ActionResult(
                new MessengerClient.Models.Message
                {
                    Content = await messageBuilder.BuildResponseCancelRequestMessage(rootActionId, user.Language).ConfigureAwait(false)
                },
                functionId);
        }

        private async Task<MessengerClient.Models.Message> BuildMessageWithAbortButton(
            int actionId,
            string language)
        {
            return new MessengerClient.Models.Message
            {
                Content = await messageBuilder.BuildActionName(actionId, language).ConfigureAwait(true),
                MessageMarkup = new MessageMarkup
                {
                    MessageActions = new List<MessageAction>
                    {
                        await BuildCancelButton(actionId, language).ConfigureAwait(false)
                    }
                }
            };
        }

        private static bool ActionIsMessage(IEnumerable<Action> nextActions)
            => nextActions.Count() == 1
                            && nextActions.FirstOrDefault().Type == ActionType.Message;

        private Task UpdateUserActionStatus(int userActionId, bool isCompletedOrCancel)
            => dynamicRepository
                .ExecuteAsync(new UpdateUserActionStatusCommand(userActionId, isCompletedOrCancel));

        public Task ResetUserAction(User user)
            => dynamicRepository.ExecuteAsync(new ResetUserActionCommand(user.Id));
    }
}