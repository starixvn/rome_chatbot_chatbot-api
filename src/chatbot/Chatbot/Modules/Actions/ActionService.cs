namespace Chatbot.Modules.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Compilers;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Actions.Params.DB.Commands;
    using Chatbot.Modules.Actions.Params.DB.Criteria;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.Settings;
    using Chatbot.Shared.Configurations;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Messenger.Client.Models;
    using Shared.Extensions;
    using Action = Models.Action;

    using MessengerClient = Messenger.Client;
    using Result = Chatbot.Models.Result;

    public interface ActionService
    {
        Task<ActionResult> BuildAction(string command, string buttonData, User user, Messenger messenger);

        Task ResetUserAction(User user);

        Task<bool> NoRootActionExist();
    }

    public class ActionServiceImpl : ActionService
    {
        private const string PostbackType = "postback";
        private const string AllActionsCacheKey = "Chatbot.Modules.Actions.AllActions";
        private readonly IDynamicRepository dynamicRepository;

        private readonly ICacheService cacheService;
        private readonly IMessageBuilder messageBuilder;
        private readonly IDictionary<int, IActionValidator> actionValidators;
        private readonly IDictionary<int, IActionCompiler> actionCompilers;
        private readonly SettingService settingService;
        private readonly Func<DateTime> getDateTimeNowFunc;
        private readonly IChatbotConfigs chatbotConfigs;

#pragma warning disable S107 // Methods should not have too many parameters
        public ActionServiceImpl(
            IDynamicRepository dynamicRepository,
            ICacheService cacheService,
            IMessageBuilder messageBuilder,
            IDictionary<int, IActionValidator> actionValidators,
            IDictionary<int, IActionCompiler> actionCompilers,
            SettingService settingService,
            IChatbotConfigs chatbotConfigs,
            Func<DateTime> getDateTimeNowFunc = null)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheService = cacheService;
            this.messageBuilder = messageBuilder;
            this.actionValidators = actionValidators;
            this.actionCompilers = actionCompilers;
            this.settingService = settingService;
            this.getDateTimeNowFunc = getDateTimeNowFunc ?? GetDateTimeNow;
            this.chatbotConfigs = chatbotConfigs;
        }

        private static DateTime GetDateTimeNow()
            => DateTime.Now;

        public async Task<ActionResult> BuildAction(string command, string buttonData, User user, Messenger messenger)
        {
            var userActions = await GetUserActions(user).ConfigureAwait(false);
            var lastAction = userActions.Any(userAction => userAction.IsActionCompletedOrCancel)
                ? null
                : userActions.LastOrDefault();
            var isUserChatMessage = string.IsNullOrWhiteSpace(buttonData);

            var handlingActionResult = isUserChatMessage
                ? await HandleUserReponseByMessage(lastAction, userActions, user, command).ConfigureAwait(false)
                : await HandleUserReponseByButton(lastAction, userActions, user, buttonData, messenger).ConfigureAwait(false);

            // Exception cases
            if (handlingActionResult.Item3 != null)
            {
                return handlingActionResult.Item3;
            }

            var compiledCommand = handlingActionResult.Item1;
            var currentActionId = handlingActionResult.Item2;
            var currentAction = await GetActionById(currentActionId).ConfigureAwait(false);

            return currentAction.IsEndAction
                ? new ActionResult(compiledCommand, currentAction.FunctionId)
                : new ActionResult(await BuildNextMessage(currentActionId, user).ConfigureAwait(false), currentAction.FunctionId);
        }

        internal async Task<(string, int, ActionResult)> HandleUserReponseByButton(
            UserAction lastAction,
            IList<UserAction> userActions,
            User user,
            string buttonData,
            Messenger messenger)
        {
            var actionInfo = ParseActionInfo(buttonData);
            var isCancelRequest = actionInfo.Item1;
            var currentActionId = actionInfo.Item2;
            var compiledCommand = string.Empty;

            if (currentActionId > 0)
            {
                if (isCancelRequest)
                {
                    return (
                        string.Empty,
                        currentActionId,
                        await HandleCancelRequest(user, messenger.Type).ConfigureAwait(false));
                }

                var currentAction = await GetActionById(currentActionId).ConfigureAwait(false);

                if (await IsActionNotExist(currentActionId).ConfigureAwait(false)
                    || await IsUserFuntionDisabled(user, currentAction).ConfigureAwait(false)
                    || (currentAction.ParentId > 0 && await IsLastActionInvalid(currentActionId, lastAction).ConfigureAwait(false)))
                {
                    var rootMessage = messageBuilder.BuildRootActionTitle(user.Language);
                    return (
                        string.Empty,
                        currentActionId,
                        await BuildRootActions(user, rootMessage).ConfigureAwait(false));
                }

                compiledCommand = await HandleButtonAction(currentAction, userActions, user).ConfigureAwait(false);
            }

            return (compiledCommand, currentActionId, null);
        }

        private async Task<bool> IsUserFuntionDisabled(User user, Action currentAction)
            => !(await settingService
                    .GetFunctions(user.Id)
                    .ConfigureAwait(false)).Any(
                            func => func.Id == currentAction.FunctionId
                                    && func.Enabled);

        internal async Task<(string, int, ActionResult)> HandleUserReponseByMessage(
            UserAction lastAction,
            IList<UserAction> userActions,
            User user,
            string userReponse)
        {
            if (lastAction == null
                || await IsActionNotExist(lastAction.ActionId).ConfigureAwait(false)
                || await IsUserFuntionDisabled(user, await GetActionById(lastAction.ActionId).ConfigureAwait(false)).ConfigureAwait(false))
            {
                return (
                    string.Empty,
                    0,
                    await BuildRootActions(user, messageBuilder.BuildRootActionTitle(user.Language)).ConfigureAwait(false));
            }

            var currentActionId = lastAction.ActionId;

            if (actionValidators.Keys.Contains(currentActionId))
            {
                var validationResult = actionValidators[currentActionId].ValidateAction(currentActionId, user, userReponse);
                if (validationResult.IsNotOk)
                {
                    return (string.Empty, currentActionId, await BuildWrongValidationResult(currentActionId, user, validationResult).ConfigureAwait(false));
                }
            }

            var compiledCommand = await CompileCommand(userActions, userReponse).ConfigureAwait(false);
            await UpdateUserAction(lastAction.Id, userReponse, compiledCommand).ConfigureAwait(false);

            return (compiledCommand, currentActionId, null);
        }

        internal async Task<string> HandleButtonAction(Action currentAction, IList<UserAction> userActions, User user)
        {
            var actualUserActions = currentAction.ParentId == 0
                ? new List<UserAction>()
                : userActions;

            actualUserActions.Add(new UserAction(0, user.Id, currentAction.Id, currentAction.Command, getDateTimeNowFunc()));
            var compiledCommand = await CompileCommand(actualUserActions, string.Empty).ConfigureAwait(false);
            await InsertUserAction(user, currentAction.Id, string.Empty, compiledCommand).ConfigureAwait(false);

            return compiledCommand;
        }

        private async Task<ActionResult> BuildRootActions(User user, string rootMessage)
        {
            await ResetUserAction(user).ConfigureAwait(false);

            return new ActionResult(await BuildRootActionsMessage(user, rootMessage).ConfigureAwait(false), 0);
        }

        private async Task<bool> IsActionNotExist(int actionId)
            => !(await GetAllActions().ConfigureAwait(false)).Any(action => action.Id == actionId);

        public async Task<bool> NoRootActionExist()
            => !(await GetAllActions().ConfigureAwait(false)).Any(action => action.ParentId == 0);

        private async Task<bool> IsLastActionInvalid(int actionId, UserAction lastAction)
        {
            if (lastAction == null)
            {
                return true;
            }

            var actionChain = await GetActionChain(actionId).ConfigureAwait(false);

            return !actionChain.Any(action => action.Id == lastAction.ActionId);
        }

        private async Task<ActionResult> BuildWrongValidationResult(int actionId, User user, Result result)
            => new ActionResult(
                await BuildMessageWithAbortButton(actionId, user.Language).ConfigureAwait(false),
                result.Message,
                (await GetActionById(actionId).ConfigureAwait(false)).FunctionId);

        internal async Task<MessengerClient.Models.Message> BuildRootActionsMessage(User user, string rootMessage)
        {
            var messageActions = new List<MessageAction>();
            var rootActions = (await GetAllActions().ConfigureAwait(false)).Where(action => action.ParentId == 0);
            var userFunctions = await settingService.GetFunctions(user.Id).ConfigureAwait(false);

            foreach (var rootAction in rootActions)
            {
                var userFunction = userFunctions.FirstOrDefault(func => func.Id == rootAction.FunctionId);

                if (userFunction != null && userFunction.Enabled)
                {
                    messageActions.Add(new MessageAction
                    {
                        Label = await messageBuilder.BuildActionName(rootAction.Id, user.Language).ConfigureAwait(false),
                        Type = PostbackType,
                        Data = $"ActionId={rootAction.Id}"
                    });
                }
            }

            return new MessengerClient.Models.Message
            {
                Content = rootMessage,
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

        internal async Task<string> CompileCommand(
            IList<UserAction> userActions,
            string userResponse)
        {
            var lastAction = userActions.LastOrDefault();

            if (lastAction != null)
            {
                var compiler = actionCompilers.ContainsKey(lastAction.ActionId)
                    ? actionCompilers[lastAction.ActionId]
                    : new DefaultActionCompiler();
                var actionChain = await GetActionChain(lastAction.ActionId).ConfigureAwait(false);
                lastAction.UserResponse = userResponse;

                return compiler.Compile(actionChain, userActions);
            }

            return string.Empty;
        }

        private async Task<Action> GetActionById(int actionId)
            => (await GetAllActions().ConfigureAwait(false))
            .FirstOrDefault(action => action.Id == actionId);

        internal static (bool, int) ParseActionInfo(string buttonData)
        {
            if (string.IsNullOrWhiteSpace(buttonData))
            {
                return (false, 0);
            }

            var keyValues = buttonData?.Split('&');

            if (keyValues == null
                || !keyValues.Any())
            {
                throw new InvalidOperationException($"Wrong button data format {buttonData}");
            }

            return (buttonData.Contains("abort", StringComparison.OrdinalIgnoreCase),
                int.Parse(keyValues.FirstOrDefault()?.Split('=')[1]));
        }

        private async Task<IList<UserAction>> GetUserActions(User user)
            => (await dynamicRepository
            .FetchAsync<UserAction>(new GetActionByUserCriteria(user.Id))
            .ConfigureAwait(false))
            .OrderBy(userAction => userAction.ActionId)
            .ThenByDescending(userAction => userAction.Id)
            .DistinctBy(userAction => userAction.ActionId)
            .ToList();

        private async Task<IEnumerable<Action>> GetAllActions()
            => await cacheService.GetOrSetAsync(
                       AllActionsCacheKey,
                       LoadAllActions,
                        new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0)))
                        .ConfigureAwait(false);

        private IEnumerable<Action> LoadAllActions()
            => dynamicRepository
                .Fetch<Action>(new GetAllActionsCriteria())
                .Where(action => !chatbotConfigs.IsProduction || action.IsAvailableOnlyOnPro);

        private async Task<IDictionary<int, IEnumerable<Action>>> GetActionChains()
        {
            var endActions = (await GetAllActions().ConfigureAwait(false))
                                .Where(action => action.IsEndAction);
            var actionChains = new Dictionary<int, IEnumerable<Action>>();

            foreach (var endAction in endActions)
            {
                if (endAction != null)
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
                    Type = PostbackType,
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

        private async Task<ActionResult> HandleCancelRequest(User user, int messengerType)
        {
            var rootMessage = messageBuilder.BuildResponseCancelRequestMessage(messengerType, user.Language);

            return await BuildRootActions(user, rootMessage).ConfigureAwait(false);
        }

        private async Task<MessengerClient.Models.Message> BuildMessageWithAbortButton(
            int actionId,
            string language)
            => new MessengerClient.Models.Message
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

        private static bool ActionIsMessage(IEnumerable<Action> nextActions)
            => nextActions.Count() == 1
                            && nextActions.FirstOrDefault().Type == ActionType.Message;

        public Task ResetUserAction(User user)
            => dynamicRepository.ExecuteAsync(new ResetUserActionCommand(user.Id));
    }
}