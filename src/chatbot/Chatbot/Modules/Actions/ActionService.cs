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
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Messenger.Client.Models;
    using Shared.Extensions;
    using Action = Models.Action;

    using MessengerClient = Messenger.Client;

    public interface ActionService
    {
        Task<ActionResult> BuildAction(string command, string buttonData, User user);
    }

    public class ActionServiceImpl : ActionService
    {
        //private const string DefaultLanguage = "en-US";
        private const string AllActionsCacheKey = "Chatbot.Modules.Actions.AllActions";

        //private const string ActionNameCacheKey = "Chatbot.Modules.Actions.ActionName.{0}";
        private readonly IDynamicRepository dynamicRepository;

        private readonly ICacheService cacheService;

        public ActionServiceImpl(
            IDynamicRepository dynamicRepository,
            ICacheService cacheService)
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheService = cacheService;
        }

        public async Task<ActionResult> BuildAction(string command, string buttonData, User user)
        {
            var actionId = 0;
            var userActions = await GetUserActions(user);
            var compiledCommand = string.Empty;
            Action action = null;
            if (string.IsNullOrWhiteSpace(buttonData))
            {
                // Handle case not button
                var lastAction = userActions.LastOrDefault();
                if (lastAction == null)
                {
                    // show root button
                    return new ActionResult(await BuildRootActionMessage(user.Language), 0);
                }

                compiledCommand = await CompileCommand(userActions, command);
                UpdateUserAction(lastAction.ActionId, command, compiledCommand);
                actionId = lastAction.ActionId;
            }
            else
            {
                var actionInfo = ParseActionInfo(buttonData);
                var isCancelRequest = actionInfo.Item1;
                actionId = actionInfo.Item2;

                if (isCancelRequest)
                {
                    // cancel request
                }

                if (actionId > 0)
                {
                    action = await GetActionById(actionId);
                    userActions.Add(new UserAction(0, user.Id, action.Id, action.Command, DateTime.Now));
                    compiledCommand = await CompileCommand(userActions, string.Empty);
                    await InsertUserAction(user, actionId, string.Empty, compiledCommand);
                }
            }

            action = action ?? await GetActionById(actionId);

            return action.IsEndAction
                ? new ActionResult(compiledCommand, action.FunctionId)
                : new ActionResult(await BuildNextMessage(actionId, user), action.FunctionId);
        }

        private async Task<MessengerClient.Models.Message> BuildRootActionMessage(string language)
        {
            var messageActions = new List<MessageAction>();
            var rootActions = (await GetAllActions()).Where(action => action.ParentId == 0);

            foreach (var rootAction in rootActions)
            {
                messageActions.Add(new MessageAction
                {
                    Label = await GetActionName(rootAction.Id, language),
                    Type = "postback",
                    Data = $"ActionId={rootAction.Id}"
                });
            }

            return new MessengerClient.Models.Message
            {
                Content = await GetRootActionTitle(language),
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
                compileCommand));

        public async Task UpdateUserAction(int actionId, string userResponse, string compiledCommand)
            => await dynamicRepository.ExecuteAsync(new UpdateUserActionCommand(actionId, userResponse, compiledCommand));

        public async Task<string> CompileCommand(
            IEnumerable<UserAction> userActions,
            string userResponse)
        {
            var lastAction = userActions.LastOrDefault();

            if (lastAction != null)
            {
                var actionInfo = await GetActionById(lastAction.ActionId);
                var actionHandler = new DefaultActionHandler();
                var actionChain = await GetActionChain(lastAction.ActionId);
                lastAction.UserResponse = userResponse;

                return actionHandler.BuildCommand(actionChain, userActions);
            }

            return string.Empty;
        }

        private async Task<Action> GetActionById(int actionId)
            => (await GetAllActions()).FirstOrDefault(action => action.Id == actionId);

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
            .OrderByDescending(userAction => userAction.ActionId)
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
            var actions = await dynamicRepository.FetchAsync<Action>(new GetAllActionsCriteria());
            var endActions = actions.Where(action => action.IsEndAction);
            var actionChains = new Dictionary<int, IEnumerable<Action>>();

            foreach (var endAction in endActions)
            {
                var tempAction = endAction;
                var actionChain = new List<Action>();

                while (tempAction != null)
                {
                    actionChain.Add(tempAction);

                    tempAction = await GetPrevAction(tempAction.ParentId);
                }

                actionChain.Reverse();
                actionChains.Add(endAction.Id, actionChain);
            }

            return actionChains;
        }

        private async Task<IEnumerable<Action>> GetActionChain(int actionId)
        {
            var actionChains = await GetActionChains();
            foreach (var actionChain in actionChains)
            {
                if (actionChain.Value.Any(chain => chain.Id == actionId))
                {
                    return actionChain.Value;
                }
            }

            return Enumerable.Empty<Action>();
        }

        private async Task<IEnumerable<Action>> GetNextAction(int actionId)
            => (await GetAllActions()).Where(action => action.ParentId == actionId);

        private async Task<Action> GetPrevAction(int parentId)
            => (await GetAllActions()).Where(action => action.Id == parentId).FirstOrDefault();

        private async Task<MessengerClient.Models.Message> BuildNextMessage(int actionId, User user)
        {
            var nextActions = (await GetAllActions()).Where(action => action.ParentId == actionId);

            if (ActionIsMessage(nextActions))
            {
                var messageAction = nextActions.FirstOrDefault();
                await InsertUserAction(user, messageAction.Id, string.Empty, string.Empty);

                return await BuildMessageWithAbortButton(actionId, user.Language);
            }

            return await BuildButtons(actionId, user.Language, nextActions);
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
                    Label = await GetActionName(nextAction.Id, language),
                    Type = "postback",
                    Data = $"ActionId={nextAction.Id}"
                });
            }

            messageActions.Add(new MessageAction
            {
                Label = await GetActionName(actionId, language),
                Type = "postback",
                Data = $"abortActionId={actionId}"
            });

            return new MessengerClient.Models.Message
            {
                Content = await GetActionTitle(actionId, language),
                MessageMarkup = new MessageMarkup
                {
                    MessageActions = messageActions
                }
            };
        }

        private async Task<MessengerClient.Models.Message> BuildMessageWithAbortButton(
            int actionId,
            string language)
        {
            return new MessengerClient.Models.Message
            {
                Content = await GetActionMessage(actionId, language).ConfigureAwait(true),
                MessageMarkup = new MessageMarkup
                {
                    MessageActions = new List<MessageAction>
                    {
                        new MessageAction
                        {
                            Label = await GetActionName(actionId, language),
                            Type = "postback",
                            Data = $"abortActionId={actionId}"
                        }
                    }
                }
            };
        }

        private static bool ActionIsMessage(IEnumerable<Action> nextActions)
            => nextActions.Count() == 1
                            && nextActions.FirstOrDefault().Type == ActionType.Message;

        private Task<string> GetActionName(int actionId, string language)
            => Task.FromResult($"{actionId}_{language}");

        private Task<string> GetActionTitle(int actionId, string language)
            => Task.FromResult($"{actionId}_{language}");

        private Task<string> GetRootActionTitle(string language)
            => Task.FromResult($"RootActionTitle_{language}");

        private Task<string> GetActionMessage(int actionId, string language)
            => Task.FromResult($"{actionId}_{language}");

        //private async Task<string> GetActionName(int actionId, string language)
        //{
        //    var functionLanguages = await cacheService.GetOrSetAsync(
        //                string.Format(ActionNameCacheKey, actionId),
        //                () => GetFunctionLanguage(functionId).Result,
        //                new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0)))
        //                .ConfigureAwait(false);

        //    var functionName = functionLanguages
        //        .FirstOrDefault(languageDto => languageDto.LanguageCode == language)
        //        ?.FunctionName;

        //    if (string.IsNullOrWhiteSpace(functionName))
        //    {
        //        functionName = functionLanguages
        //        .FirstOrDefault(languageDto => languageDto.LanguageCode == DefaultLanguage)
        //        ?.FunctionName;
        //    }

        //    return functionName;
        //}

        //private Task<IEnumerable<FunctionLanguageDto>> GetFunctionLanguage(int functionId)
        //    => dynamicRepository.FetchAsync<FunctionLanguageDto>(new GetFunctionNameCriteria(functionId));
    }
}