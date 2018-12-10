namespace Chatbot.Modules.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Chatbot.AppResources;
    using Chatbot.Models;
    using Chatbot.Modules.Actions.Builders.ActionName;
    using Chatbot.Modules.Actions.Builders.ActionTitle;
    using Chatbot.Modules.Actions.Compilers;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Actions.Params.DB.Commands;
    using Chatbot.Modules.Actions.Params.DB.Criteria;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Resources.Constants;
    using Chatbot.Modules.Resources.Models;
    using Chatbot.Modules.Settings;
    using Chatbot.Shared.Configurations;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Messenger.Client.Models;
    using Newtonsoft.Json;
    using Shared.Extensions;
    using Action = Models.Action;

    using MessengerClient = Messenger.Client;

    public interface ActionService
    {
        Task<ActionResult> BuildAction(string command, string buttonData, User user, Messenger messenger);

        Task ResetUserAction(User user);

        Task<bool> NoRootActionExist();
    }

    public class ActionServiceImpl : ActionService
    {
        private const int expiredMinutesForConfirmMessage = 2;
        private const string PostbackType = "postback";
        private const string AllActionsCacheKey = "Chatbot.Modules.Actions.AllActions";
        private readonly IDynamicRepository dynamicRepository;

        private readonly ICacheService cacheService;
        private readonly IMessageBuilder messageBuilder;
        private readonly IDictionary<int, IActionValidator> actionValidators;
        private readonly IDictionary<int, IActionCompiler> actionCompilers;
        private readonly SettingService settingService;
        private readonly ResourceService resourceService;
        private readonly Func<DateTime> getDateTimeNowFunc;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly ActionNameBuilderFactory actionNameBuilderFactory;
        private readonly ActionTitleBuilderFactory actionTitleBuilderFactory;

        public ActionServiceImpl(
            IDynamicRepository dynamicRepository,
            ICacheService cacheService,
            IMessageBuilder messageBuilder,
            IDictionary<int, IActionValidator> actionValidators,
            IDictionary<int, IActionCompiler> actionCompilers,
            SettingService settingService,
            ResourceService resourceService,
            IChatbotConfigs chatbotConfigs,
            ActionNameBuilderFactory actionNameBuilderFactory,
            ActionTitleBuilderFactory actionTitleBuilderFactory,
            Func<DateTime> getDateTimeNowFunc = null)
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheService = cacheService;
            this.messageBuilder = messageBuilder;
            this.actionValidators = actionValidators;
            this.actionCompilers = actionCompilers;
            this.settingService = settingService;
            this.resourceService = resourceService;
            this.getDateTimeNowFunc = getDateTimeNowFunc ?? GetDateTimeNow;
            this.chatbotConfigs = chatbotConfigs;
            this.actionNameBuilderFactory = actionNameBuilderFactory;
            this.actionTitleBuilderFactory = actionTitleBuilderFactory;
        }

        private static DateTime GetDateTimeNow()
            => DateTime.Now;

        public async Task<ActionResult> BuildAction(string command, string buttonData, User user, Messenger messenger)
        {
            var isUserReponseByMessage = string.IsNullOrWhiteSpace(buttonData);

            var handlingActionResult = isUserReponseByMessage
                ? await HandleUserReponseByMessage(user, command, messenger).ConfigureAwait(false)
                : await HandleUserReponseByButton(user, buttonData, messenger).ConfigureAwait(false);

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
            User user,
            string buttonDataText,
            Messenger messenger)
        {
            var buttonData = ParseActionInfo(buttonDataText);
            var isCancelRequest = buttonData.IsCancel;
            var currentActionId = buttonData.ActionId;
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
                    || IsUserFuntionDisabled(user, currentAction))
                {
                    var rootMessage = messageBuilder.BuildRootActionTitle(user.Language);
                    return (
                        string.Empty,
                        currentActionId,
                        await BuildRootActions(user, rootMessage).ConfigureAwait(false));
                }

                var userActions = await GetUserActions(user).ConfigureAwait(false);

                if (actionValidators.Keys.Contains(currentActionId))
                {
                    var validationResult = actionValidators[currentActionId].ValidateAction(user, string.Empty, userActions, messenger);
                    if (validationResult.IsNotOk)
                    {
                        return (string.Empty, currentActionId, await BuildWrongValidationResult(currentActionId, user, userActions, validationResult).ConfigureAwait(false));
                    }
                }

                if (!string.IsNullOrWhiteSpace(buttonData.CompiledCommand))
                {
                    return await HandleCompiledCommand(user, buttonData, currentAction, userActions).ConfigureAwait(false);
                }

                compiledCommand = await HandleButtonAction(currentAction, userActions, user, buttonData).ConfigureAwait(false);
                currentActionId = currentAction.BackToActionId > 0
                    ? currentAction.BackToActionId
                    : currentActionId;
            }

            return (compiledCommand, currentActionId, null);
        }

        private async Task<(string, int, ActionResult)> HandleCompiledCommand(
            User user,
            ButtonData buttonData,
            Action currentAction,
            IList<UserAction> userActions)
        {
            var functionType = user.Functions
                                .FirstOrDefault(func => func.Id == currentAction.FunctionId)?
                                .Type;

            if (functionType == chatbotConfigs.InsertUpdateFunctionType
                    && (!userActions.Any() || buttonData.SentTime < userActions.FirstOrDefault().ChatTime)
                    && !buttonData.IsConfirmButton)
            {
                return (
                    string.Empty,
                    currentAction.Id,
                    await BuildComfirmResultForUpdateAction(currentAction, user, userActions, buttonData).ConfigureAwait(false));
            }

            return (buttonData.CompiledCommand, currentAction.Id, null);
        }

        private async Task<ActionResult> BuildComfirmResultForUpdateAction(
            Action action,
            User user,
            IList<UserAction> userActions,
            ButtonData buttonData)
        {
            var messageActions = new List<MessageAction>();

            buttonData.IsConfirmButton = true;
            messageActions.Add(new MessageAction
            {
                Label = resourceService.GetStaticResource(nameof(Languages.Yes), user.Language),
                Type = PostbackType,
                Data = JsonConvert.SerializeObject(buttonData)
            });

            var reConfirmMessage = resourceService.GetStaticResource(nameof(Languages.ReConfirmMessage), user.Language);
            var message = await CreateMessageWithButtons(
                action.Id,
                user,
                userActions,
                messageActions,
                string.Format(reConfirmMessage, buttonData.Label)).ConfigureAwait(false);
            message.ExpiredTime = DateTime.Now.AddMinutes(expiredMinutesForConfirmMessage);

            return new ActionResult(message, action.FunctionId);
        }

        private static bool IsUserFuntionDisabled(User user, Action currentAction)
            => !user.Functions.Any(
                            func => func.Id == currentAction.FunctionId
                                    && func.Enabled);

        internal async Task<(string, int, ActionResult)> HandleUserReponseByMessage(
            User user,
            string userReponse,
            Messenger messenger)
        {
            var userActions = await GetUserActions(user).ConfigureAwait(false);
            var lastAction = userActions.LastOrDefault();

            if (lastAction == null
                || await IsActionNotExist(lastAction.ActionId).ConfigureAwait(false)
                || IsUserFuntionDisabled(user, await GetActionById(lastAction.ActionId).ConfigureAwait(false)))
            {
                return await HandleUnknownUserResponseMessage(user, userReponse).ConfigureAwait(false);
            }

            var currentActionId = lastAction.ActionId;

            if (actionValidators.Keys.Contains(currentActionId))
            {
                var validationResult = actionValidators[currentActionId].ValidateAction(user, userReponse, userActions, messenger);
                if (validationResult.IsNotOk)
                {
                    return (string.Empty, currentActionId, await BuildWrongValidationResult(currentActionId, user, userActions, validationResult).ConfigureAwait(false));
                }
            }

            var compiledCommand = await CompileCommand(userActions, userReponse).ConfigureAwait(false);
            await UpdateUserAction(lastAction.Id, userReponse, compiledCommand).ConfigureAwait(false);

            return (compiledCommand, currentActionId, null);
        }

        private async Task<(string, int, ActionResult)> HandleUnknownUserResponseMessage(User user, string userReponse)
        {
            var functions = await GetFunctionsByKeyword(userReponse, user.Id).ConfigureAwait(false);

            if (functions.Any())
            {
                return (string.Empty, 0, await BuildConfirmKeywordActions(functions, user).ConfigureAwait(false));
            }

            return (
                string.Empty,
                0,
                await BuildRootActions(user, messageBuilder.BuildRootActionTitle(user.Language)).ConfigureAwait(false));
        }

        private async Task<IEnumerable<Function>> GetFunctionsByKeyword(string userResponse, int userId)
        {
            var keywordFunctions = new List<Function>();
            var userFunctions = (await settingService.GetFunctions(userId).ConfigureAwait(false)).Where(f => f.Enabled);

            foreach (var function in userFunctions)
            {
                var resources = await GetFunctionKeyworks(function.Id).ConfigureAwait(false);

                foreach (var resource in resources)
                {
                    var keywords = resource.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    var isContainsKeyword = keywords.Any(keyword
                        => userResponse.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                            || WebUtility.HtmlDecode(userResponse).Contains(keyword, StringComparison.OrdinalIgnoreCase));

                    if (isContainsKeyword && !keywordFunctions.Contains(function))
                    {
                        keywordFunctions.Add(function);
                    }
                }
            }

            return keywordFunctions;
        }

        private async Task<ActionResult> BuildConfirmKeywordActions(IEnumerable<Function> functions, User user)
        {
            var messageActions = await BuildSuggestionActions(functions, user).ConfigureAwait(false);

            messageActions.Add(new MessageAction
            {
                Label = messageBuilder.BuildNoneOfTheAboveMessage(user.Language).ToUpperInvariant(),
                Type = PostbackType,
                Data = string.Empty
            });

            var message = new MessengerClient.Models.Message
            {
                Content = messageBuilder.BuildConfirmKeywordTitle(user.Language),
                MessageMarkup = new MessageMarkup
                {
                    MessageActions = messageActions
                }
            };

            return new ActionResult(message, 0);
        }

        private async Task<List<MessageAction>> BuildSuggestionActions(
            IEnumerable<Function> functions,
            User user)
        {
            var messageActions = new List<MessageAction>();

            foreach (var function in functions)
            {
                var action = (await GetAllActions().ConfigureAwait(false))
                    .FirstOrDefault(a
                        => a.ParentId == 0
                            && a.FunctionId == function.Id
                            && (a.CustLevel == user.CustLevel || a.CustLevel == 0));

                if (action != null)
                {
                    var actionNameBuilder = actionNameBuilderFactory.GetActionNameBuilder(action.Id);

                    messageActions.Add(new MessageAction
                    {
                        Label = (await actionNameBuilder.BuildActionName(action.Id, user).ConfigureAwait(false)).ToUpperInvariant(),
                        Type = PostbackType,
                        Data = JsonConvert.SerializeObject(new ButtonData
                        {
                            ActionId = action.Id,
                            SentTime = getDateTimeNowFunc()
                        })
                    });
                }
            }

            return messageActions;
        }

        private async Task<IEnumerable<Resource>> GetFunctionKeyworks(int functionId)
            => await resourceService.GetResources(functionId, ResourceType.Keyword).ConfigureAwait(false);

        internal async Task<string> HandleButtonAction(
            Action currentAction,
            IList<UserAction> userActions,
            User user,
            ButtonData buttonData)
        {
            var actualUserActions = currentAction.ParentId == 0
                ? new List<UserAction>()
                : userActions;

            if ((!actualUserActions.Any()
                    || (currentAction.IsNotNextActionOf(actualUserActions.LastOrDefault().ActionId)))
                && IsButtonDataContainsAnyUserAction(buttonData))
            {
                actualUserActions = buttonData.UserActions;

                if (!currentAction.IsEndAction)
                {
                    foreach (var userAction in actualUserActions)
                    {
                        if (currentAction.BackToActionId == 0
                            || currentAction.BackToActionId >= userAction.ActionId)
                        {
                            await InsertUserAction(user, userAction.ActionId, userAction.UserResponse, userAction.CompiledCommand).ConfigureAwait(false);
                        }
                    }
                }
            }

            if (currentAction.BackToActionId > 0)
            {
                return string.Empty;
            }

            actualUserActions.Add(new UserAction(0, user.Id, currentAction.Id, currentAction.Command, getDateTimeNowFunc()));

            var compiledCommand = await CompileCommand(actualUserActions, string.Empty).ConfigureAwait(false);
            await InsertUserAction(user, currentAction.Id, string.Empty, compiledCommand).ConfigureAwait(false);

            return compiledCommand;
        }

        private static bool IsButtonDataContainsAnyUserAction(ButtonData buttonData)
            => buttonData != null
                && buttonData.UserActions != null
                && buttonData.UserActions.Any();

        private async Task<ActionResult> BuildRootActions(User user, string rootMessage)
        {
            await ResetUserAction(user).ConfigureAwait(false);

            return new ActionResult(await BuildRootActionsMessage(user, rootMessage).ConfigureAwait(false), 0);
        }

        private async Task<bool> IsActionNotExist(int actionId)
            => !(await GetAllActions().ConfigureAwait(false)).Any(action => action.Id == actionId);

        public async Task<bool> NoRootActionExist()
            => !(await GetAllActions().ConfigureAwait(false)).Any(action => action.ParentId == 0);

        private async Task<ActionResult> BuildWrongValidationResult(int actionId, User user, IList<UserAction> userActions, ActionValidationResult result)
            => new ActionResult(
                !result.ForcedCancelRequest ? await BuildMessageWithAbortButton(actionId, user, userActions).ConfigureAwait(false) : null,
                result.Message,
                (await GetActionById(actionId).ConfigureAwait(false)).FunctionId);

        internal async Task<MessengerClient.Models.Message> BuildRootActionsMessage(User user, string rootMessage)
        {
            var messageActions = new List<MessageAction>();
            var rootActions = (await GetAllActions().ConfigureAwait(false))
                .Where(action => action.ParentId == 0 && (action.CustLevel == user.CustLevel || action.CustLevel == 0));

            foreach (var rootAction in rootActions)
            {
                var actionNameBuilder = actionNameBuilderFactory.GetActionNameBuilder(rootAction.Id);
                var userFunction = user.Functions.FirstOrDefault(func => func.Id == rootAction.FunctionId);

                if (userFunction != null && userFunction.Enabled)
                {
                    messageActions.Add(new MessageAction
                    {
                        Label = (await actionNameBuilder.BuildActionName(rootAction.Id, user).ConfigureAwait(false)).ToUpperInvariant(),
                        Type = PostbackType,
                        Data = JsonConvert.SerializeObject(new ButtonData
                        {
                            ActionId = rootAction.Id,
                            CompiledCommand = rootAction.IsEndAction
                            ? rootAction.Command
                            : string.Empty,
                            SentTime = getDateTimeNowFunc()
                        })
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

        internal static ButtonData ParseActionInfo(string buttonDataText)
        {
            if (string.IsNullOrWhiteSpace(buttonDataText))
            {
                throw new InvalidOperationException($"Button data is empty");
            }

            var buttonData = JsonConvert.DeserializeObject<ButtonData>(buttonDataText);

            if (buttonData == null
                || buttonData.ActionId == 0)
            {
                throw new InvalidOperationException($"Wrong button data format {buttonDataText}");
            }

            return buttonData;
        }

        private async Task<IList<UserAction>> GetUserActions(User user)
            => (await dynamicRepository
            .FetchAsync<UserAction>(new GetActionByUserCriteria(user.Id))
            .ConfigureAwait(false))
            .Where(userAction => userAction.IsActionValid)
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
                .Where(action => action.ParentId == actionId && (action.CustLevel == user.CustLevel || action.CustLevel == 0))
                .OrderBy(action => action.DisplayOrder);
            var userActions = await GetUserActions(user).ConfigureAwait(false);

            if (ActionIsMessage(nextActions))
            {
                var messageAction = nextActions.FirstOrDefault();
                await InsertUserAction(user, messageAction.Id, string.Empty, string.Empty).ConfigureAwait(false);

                return await BuildMessageWithAbortButton(messageAction.Id, user, userActions).ConfigureAwait(false);
            }

            return await BuildButtons(actionId, user, nextActions, userActions).ConfigureAwait(false);
        }

        private async Task<MessengerClient.Models.Message> BuildButtons(
            int actionId,
            User user,
            IEnumerable<Action> nextActions,
            IList<UserAction> userActions)
        {
            var messageActions = new List<MessageAction>();

            foreach (var nextAction in nextActions)
            {
                var actionNameBuilder = actionNameBuilderFactory.GetActionNameBuilder(nextAction.Id);

                var nextUserAction = new UserAction(int.MaxValue, 0, nextAction.Id, nextAction.Command, getDateTimeNowFunc());
                var nextUserActions = userActions.Concat(new List<UserAction> { nextUserAction }).ToList();
                var label = await actionNameBuilder
                                .BuildActionName(nextAction.Id, user, userActions)
                                .ConfigureAwait(false);

                messageActions.Add(new MessageAction
                {
                    Label = label.ToUpperInvariant(),
                    Type = PostbackType,
                    Data = JsonConvert.SerializeObject(new ButtonData
                    {
                        ActionId = nextAction.Id,
                        UserActions = userActions,
                        CompiledCommand = nextAction.IsEndAction
                            ? (await CompileCommand(nextUserActions, string.Empty).ConfigureAwait(false))
                            : string.Empty,
                        SentTime = getDateTimeNowFunc(),
                        Label = label
                    })
                });
            }

            return await CreateMessageWithButtons(actionId, user, userActions, messageActions).ConfigureAwait(false);
        }

        private async Task<MessengerClient.Models.Message> CreateMessageWithButtons(
            int actionId,
            User user,
            IList<UserAction> userActions,
            List<MessageAction> messageActions,
            string titleMessage = null)
        {
            messageActions.Add(await BuildCancelButton(actionId, user.Language).ConfigureAwait(false));

            if (string.IsNullOrWhiteSpace(titleMessage))
            {
                titleMessage = actionTitleBuilderFactory
                    .GetActionTitleBuilder(actionId)
                    .BuildActionTitle(user, userActions);
            }

            return new MessengerClient.Models.Message
            {
                Content = titleMessage,
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
                Label = messageBuilder.BuildCancelRequestMessage(language).ToUpperInvariant(),
                Type = PostbackType,
                Data = JsonConvert.SerializeObject(new ButtonData
                {
                    ActionId = rootAction != null ? rootAction.Id : 0,
                    IsCancel = true,
                    SentTime = getDateTimeNowFunc()
                })
            };
        }

        private async Task<ActionResult> HandleCancelRequest(User user, int messengerType)
        {
            var rootMessage = messageBuilder.BuildResponseCancelRequestMessage(messengerType, user.Language);

            return await BuildRootActions(user, rootMessage).ConfigureAwait(false);
        }

        private async Task<MessengerClient.Models.Message> BuildMessageWithAbortButton(int actionId, User user, IList<UserAction> userActions)
        {
            var actionNameBuilder = actionNameBuilderFactory.GetActionNameBuilder(actionId);

            return new MessengerClient.Models.Message
            {
                Content = await actionNameBuilder.BuildActionName(actionId, user, userActions).ConfigureAwait(true),
                MessageMarkup = new MessageMarkup
                {
                    MessageActions = new List<MessageAction>
                    {
                        await BuildCancelButton(actionId, user.Language).ConfigureAwait(false)
                    }
                }
            };
        }

        private static bool ActionIsMessage(IEnumerable<Action> nextActions)
            => nextActions.Count() == 1
                            && nextActions.FirstOrDefault().Type == ActionType.Message;

        public Task ResetUserAction(User user)
           => dynamicRepository.ExecuteAsync(new ResetUserActionCommand(user.Id));
    }
}