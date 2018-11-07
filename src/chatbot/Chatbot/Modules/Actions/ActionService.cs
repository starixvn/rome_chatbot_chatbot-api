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
        Task Execute(string command, string buttonData, Messenger messenger, User user);
    }

    public class ActionServiceImpl : ActionService
    {
        private const string DefaultLanguage = "en-US";
        private const string AllActionsCacheKey = "Chatbot.Modules.Actions.AllActions";
        private const string ActionNameCacheKey = "Chatbot.Modules.Actions.ActionName.{0}";
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheService cacheService;

        public ActionServiceImpl(
            IDynamicRepository dynamicRepository,
            ICacheService cacheService)
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheService = cacheService;
        }

        public async Task Execute(string command, string buttonData, Messenger messenger, User user)
        {
            var actionId = 0;
            if (string.IsNullOrWhiteSpace(buttonData))
            {
                // Handle case not button
                var userActions = await GetUserActions(user);
                var lastAction = userActions.LastOrDefault();
                if (lastAction == null)
                {
                    // show no information
                }

                UpdateUserAction(userActions, command);
                actionId = lastAction.ActionId;
            }
            else
            {
                var actionInfo = ParseActionInfo(buttonData);
                actionId = actionInfo.Item2;

                if (actionId > 0)
                {
                    var action = await GetActionById(actionId);
                    await InsertUserAction(user, actionId, string.Empty, true, action.Command);
                }
            }

            //var userActions = await GetUserActions(user);
            //var lastAction = userActions.LastOrDefault();

            //var actionInfo = ParseActionInfo(buttonData);
            ////var isCancelRequest = actionInfo.Item1;
            //var actionId = actionInfo.Item2;
            //var action = await GetActionById(actionId);

            //var userReponse = string.IsNullOrWhiteSpace(buttonData) ? command : string.Empty;

            //// No user action exist
            //if (lastAction == null && actionId > 0)
            //{
            //    await InsertUserAction(user, actionId, userReponse, true, action.Command);
            //}

            //if(actionId == 0
            //    && lastAction != null)
            //{
            //    actionId = lastAction.ActionId;
            //}

            //var currentAction = GetActionById(actionId);

            //var nextMessage = await BuildNextMessage(actionId, user.Language);

            //if (lastAction.IsEndAction)
            //{
            //}
        }

        public async Task InsertUserAction(User user, int actionId, string userResponse, bool isBeginAction, string compileCommand)
            => await dynamicRepository.ExecuteAsync(new InsertUserActionCommand(
                user.Id,
                actionId,
                userResponse,
                isBeginAction,
                compileCommand));

        public async Task UpdateUserAction(IEnumerable<UserActionItemDto> userActions, string userResponse)
        {
            var lastAction = userActions.LastOrDefault();
            var actionInfo = await GetActionById(lastAction.ActionId);
            var actionHandler = new DefaultActionHandler();

            lastAction.UserResponse = userResponse;
            var actionChain = await GetActionChain(lastAction.ActionId);

            var compiledCommand = actionHandler.BuildCommand(actionChain, userActions);
            lastAction.CompiledCommand = compiledCommand;

            await dynamicRepository.ExecuteAsync(new UpdateUserActionCommand(lastAction.UserActionId, userResponse, compiledCommand));
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

            return (buttonData.Contains("abort"),
                int.Parse(keyValues.FirstOrDefault()?.Split('=')[0]));
        }

        private async Task<IEnumerable<UserActionItemDto>> GetUserActions(User user)
            => (await dynamicRepository
            .FetchAsync<UserActionItemDto>(new GetActionsByUser(user.Id))
            .ConfigureAwait(false))
            .OrderByDescending(userAction => userAction.ActionId)
            .DistinctBy(userAction => userAction.ActionId);

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

                while (tempAction.ParentId == 0)
                {
                    var prevAction = await GetPrevAction(tempAction.ParentId);

                    actionChain.Add(tempAction);

                    tempAction = prevAction;
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

        private async Task<MessengerClient.Models.Message> BuildNextMessage(int actionId, string language)
        {
            var nextActions = (await GetAllActions()).Where(action => action.ParentId == actionId);

            if (ActionIsMessage(nextActions))
            {
                return new MessengerClient.Models.Message
                {
                    Content = await GetActionMessage(actionId, language).ConfigureAwait(true)
                };
            }

            return await BuildButtons(actionId, language, nextActions);
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