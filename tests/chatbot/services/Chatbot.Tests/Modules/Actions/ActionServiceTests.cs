namespace Chatbot.Tests.Modules.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions;
    using Chatbot.Modules.Actions.Compilers;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Actions.Params.DB.Commands;
    using Chatbot.Modules.Actions.Params.DB.Criteria;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Shared.Configurations;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;
    using Action = Chatbot.Modules.Actions.Models.Action;

    [Trait("Modules", "Actions")]
    public class ActionServiceTests
    {
        private readonly ICacheService cacheService;
        private readonly IMessageBuilder messageBuilder;
        private readonly IDictionary<int, IActionValidator> actionValidators;
        private readonly IDictionary<int, IActionCompiler> actionCompilers;
        private readonly IActionValidator actionValidator;
        private readonly IActionCompiler actionCompiler;
        private readonly Func<DateTime> getDateTimeNowFunc;
        private readonly ActionServiceImpl actionServiceImpl;
        private readonly IDynamicRepository dynamicRepository;
        private readonly IChatbotConfigs chatbotConfigs;

        public ActionServiceTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            cacheService = Substitute.For<ICacheService>();
            messageBuilder = Substitute.For<IMessageBuilder>();
            actionValidator = Substitute.For<IActionValidator>();
            actionCompiler = Substitute.For<IActionCompiler>();
            chatbotConfigs = Substitute.For<IChatbotConfigs>();
            actionValidators = new Dictionary<int, IActionValidator>
            {
                { 4, actionValidator }
            };
            actionCompilers = new Dictionary<int, IActionCompiler>
            {
                { 1, actionCompiler },
                { 4, actionCompiler },
                { 6, actionCompiler }
            };
            getDateTimeNowFunc = Substitute.For<Func<DateTime>>();

            actionServiceImpl = new ActionServiceImpl(
                dynamicRepository,
                cacheService,
                messageBuilder,
                actionValidators,
                actionCompilers,
                chatbotConfigs,
                getDateTimeNowFunc);

            actionValidator
                .ValidateAction(Arg.Any<int>(), Arg.Any<User>(), Arg.Any<string>())
                .Returns(Result.CreateSuccessfulResult());
        }

        [Fact]
        public void HandleUserReponseByMessage_LastActionIsNull_ReturnUserRootActions()
        {
            var user = new User(1, 1, 4, 0, "en-US");

            StubActions();
            StubUserFuntions(user);

            messageBuilder.BuildRootActionTitle(user.Language).Returns("message title");
            messageBuilder.BuildActionName(1, user.Language).Returns("outstanding label");

            var actionResult = actionServiceImpl.HandleUserReponseByMessage(null, null, user, "message")
                .GetAwaiter()
                .GetResult()
                .Item3;

            Assert.Equal(2, actionResult.Message.MessageMarkup.MessageActions.Count());
            Assert.Equal("ActionId=1", actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Data);
            Assert.Equal("OUTSTANDING LABEL", actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Label);
            Assert.Equal("ActionId=2", actionResult.Message.MessageMarkup.MessageActions.ElementAt(1).Data);
            Assert.Equal("message title", actionResult.Message.Content);
        }

        [Fact]
        public void HandleUserReponseByMessage_UserReponseIsIncorrectFormat_ReturnWrongInputMessage()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US");
            StubUserFuntions(user);
            var lastUserAction = new UserAction(1, 1, 4, "user reponse", DateTime.Now);
            actionValidator.ValidateAction(4, user, "user reponse").Returns(new Result(1, "failed"));

            var actionResult = actionServiceImpl.HandleUserReponseByMessage(lastUserAction, null, user, "user reponse")
                .GetAwaiter()
                .GetResult()
                .Item3;

            Assert.Equal("failed", actionResult.ValidationFailedMessage);
        }

        [Fact]
        public void HandleUserReponseByMessage_UserReponseIsIncorrectFormat_ReturnRepeatMessage()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US");
            var lastUserAction = new UserAction(1, 1, 4, "user reponse", DateTime.Now);
            StubUserFuntions(user);
            actionValidator.ValidateAction(4, user, "user reponse").Returns(new Result(1, "failed"));
            messageBuilder.BuildActionName(4, user.Language).Returns("outstanding label");
            messageBuilder.BuildCancelRequestMessage("en-US").Returns("cancel button");

            var actionResult = actionServiceImpl.HandleUserReponseByMessage(lastUserAction, null, user, "user reponse")
                .GetAwaiter()
                .GetResult()
                .Item3;

            Assert.Equal("outstanding label", actionResult.Message.Content);
            Assert.Equal("CANCEL BUTTON", actionResult.Message.MessageMarkup.MessageActions.First().Label);
        }

        [Fact]
        public void HandleUserReponseByMessage_Always_ReturnCompiledCommand()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US");
            var lastUserAction = new UserAction(1, 1, 4, "user reponse", DateTime.Now);
            var userActions = StubUserActions(user.Id);
            StubUserFuntions(user);
            actionCompiler.Compile(Arg.Any<IEnumerable<Action>>(), Arg.Any<IEnumerable<UserAction>>())
                .Returns("compiled command");

            var compiledCommand = actionServiceImpl.HandleUserReponseByMessage(
                lastUserAction, userActions, user, "user reponse")
                .GetAwaiter()
                .GetResult()
                .Item1;

            Assert.Equal("compiled command", compiledCommand);
        }

        [Fact]
        public void HandleUserReponseByMessage_Always_ReturnLastUserActionId()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US");
            var lastUserAction = new UserAction(1, 1, 4, "user reponse", DateTime.Now);
            var userActions = StubUserActions(user.Id);
            StubUserFuntions(user);
            actionCompiler.Compile(Arg.Any<IEnumerable<Action>>(), Arg.Any<IEnumerable<UserAction>>())
                .Returns("compiled command");

            var lastActionId = actionServiceImpl.HandleUserReponseByMessage(
                lastUserAction, userActions, user, "user reponse")
                .GetAwaiter()
                .GetResult()
                .Item2;

            Assert.Equal(4, lastActionId);
        }

        [Fact]
        public void HandleUserReponseByButton_CancelRequest_ExecuteUpdateUserActionsToCancel()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US");
            var userActions = StubUserActions(user.Id);
            var messenger = new Messenger(1, "ltid");

            actionServiceImpl.HandleUserReponseByButton(
                userActions.FirstOrDefault(),
                userActions,
                user,
                "abort=1",
                messenger)
                    .GetAwaiter()
                    .GetResult();

            dynamicRepository
                .Received()
                .ExecuteAsync(
                    Arg.Is<ResetUserActionCommand>(
                        command => command.UserId == 1));
        }

        [Fact]
        public void HandleUserReponseByButton_CancelRequest_ReturnCancelMessageAndRootButtons()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US");
            var userActions = StubUserActions(user.Id);
            var messenger = new Messenger(1, "ltid");
            messageBuilder
                .BuildResponseCancelRequestMessage(1, user.Language)
                .Returns("cancel action");

            var actionResult = actionServiceImpl.HandleUserReponseByButton(
                userActions.FirstOrDefault(),
                userActions,
                user,
                "abort=1",
                messenger)
                    .GetAwaiter()
                    .GetResult()
                    .Item3;

            Assert.Equal("cancel action", actionResult.Message.Content);
        }

        [Fact]
        public void HandleUserReponseByButton_LastActonIsInvalid_ReturnRootActions()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US");
            var messenger = new Messenger(1, "ltid");
            StubUserFuntions(user);
            messageBuilder.BuildRootActionTitle(user.Language).Returns("message title");
            messageBuilder.BuildActionName(1, user.Language).Returns("outstanding label");

            var actionResult = actionServiceImpl.HandleUserReponseByButton(null, null, user, "action=4", messenger)
               .GetAwaiter()
               .GetResult()
               .Item3;

            Assert.Equal(2, actionResult.Message.MessageMarkup.MessageActions.Count());
            Assert.Equal("ActionId=1", actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Data);
            Assert.Equal("OUTSTANDING LABEL", actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Label);
            Assert.Equal("ActionId=2", actionResult.Message.MessageMarkup.MessageActions.ElementAt(1).Data);
            Assert.Equal("message title", actionResult.Message.Content);
        }

        [Fact]
        public void HandleUserReponseByButton_ActionIsValid_ExecuteInsertUserAction()
        {
            var user = new User(1, 1, 4, 0, "en-US");
            var messenger = new Messenger(1, "ltid");
            StubActions();
            StubUserFuntions(user);
            var userActions = StubUserActions(user.Id);
            actionCompiler.Compile(Arg.Any<IEnumerable<Action>>(), Arg.Any<IEnumerable<UserAction>>())
                .Returns("compiled command");

            actionServiceImpl.HandleUserReponseByButton(
                userActions.First(),
                userActions,
                user,
                "action=4",
                messenger)
                   .GetAwaiter()
                   .GetResult();

            dynamicRepository.Received().ExecuteAsync(Arg.Is<InsertUserActionCommand>(
                command => command.UserId == 1
                            && command.ActionId == 4
                            && command.UserResponse == string.Empty
                            && command.CompiledCommand == "compiled command"));
        }

        [Fact]
        public void BuildAction_ActionIsCancelled_ReturnCancelMessageAndRootButton()
        {
            StubActions();
            var messenger = new Messenger(1, "ltid");
            var user = new User(1, 1, 4, 0, "en-US");
            StubUserActions(user.Id);
            StubUserFuntions(user);
            messageBuilder
                    .BuildResponseCancelRequestMessage(1, user.Language)
                    .Returns("cancel action");

            var actionResult = actionServiceImpl
                .BuildAction("cancel", "abort=1", user, messenger)
                .GetAwaiter()
                .GetResult();

            Assert.Equal("cancel action", actionResult.Message.Content);
            Assert.Equal(2, actionResult.Message.MessageMarkup.MessageActions.Count());
            Assert.Equal("ActionId=1", actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Data);
            Assert.Equal("ActionId=2", actionResult.Message.MessageMarkup.MessageActions.ElementAt(1).Data);
        }

        [Fact]
        public void BuildAction_ActionIsEnd_ReturnCompileCommand()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US");
            var messenger = new Messenger(1, "ltid");
            StubUserActions(user.Id);
            StubUserFuntions(user);
            actionCompiler.Compile(Arg.Any<IEnumerable<Action>>(), Arg.Any<IEnumerable<UserAction>>())
                .Returns("compiled command");

            var actionResult = actionServiceImpl.BuildAction(
                "command", "action=6", user, messenger)
                .GetAwaiter()
                .GetResult();

            Assert.Equal("compiled command", actionResult.Command);
        }

        [Fact]
        public void BuildAction_ActionIsNotEndAndNextActionIsButtons_ReturnNextButtonActions()
        {
            StubActions();
            var messenger = new Messenger(1, "ltid");
            var user = new User(1, 1, 4, 0, "en-US");
            StubUserActions(user.Id);
            StubUserFuntions(user);

            var actionResult = actionServiceImpl.BuildAction(
                "command", "action=1", user, messenger)
                .GetAwaiter()
                .GetResult();

            Assert.Equal(4, actionResult.Message.MessageMarkup.MessageActions.Count());
            Assert.Equal("ActionId=5", actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Data);
            Assert.Equal("ActionId=4", actionResult.Message.MessageMarkup.MessageActions.ElementAt(1).Data);
            Assert.Equal("ActionId=7", actionResult.Message.MessageMarkup.MessageActions.ElementAt(2).Data);
            Assert.Equal("abortActionId=1", actionResult.Message.MessageMarkup.MessageActions.ElementAt(3).Data);
        }

        [Fact]
        public void BuildAction_ActionIsNotEndAndNextActionIsMessage_ReturnNextMessage()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US");
            var messenger = new Messenger(1, "ltid");
            StubUserActions(user.Id);
            StubUserFuntions(user);

            messageBuilder.BuildActionName(6, user.Language).Returns("next action name");

            var actionResult = actionServiceImpl.BuildAction(
                "command", "", user, messenger)
                .GetAwaiter()
                .GetResult();

            Assert.Equal("next action name", actionResult.Message.Content);
            Assert.Single(actionResult.Message.MessageMarkup.MessageActions);
            Assert.Equal("abortActionId=1", actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Data);
        }

        private void StubUserFuntions(User user)
            => user.Functions = new List<Function>
                    {
                        new Function { Id = 1, Enabled = true },
                        new Function { Id = 2, Enabled = true }
                    };

        private IList<UserAction> StubUserActions(int userId)
        {
            var userActions = new List<UserAction>
            {
                new UserAction(1, userId, 1, string.Empty, DateTime.Now),
                new UserAction(2, userId, 4, string.Empty, DateTime.Now)
            };

            dynamicRepository
                .FetchAsync<UserAction>(Arg.Is<GetActionByUserCriteria>(criteria => criteria.UserId == userId))
                .Returns(Task.FromResult(userActions as IEnumerable<UserAction>));

            return userActions;
        }

        private void StubActions()
        {
            var actionsStub = new List<Action>
            {
                new Action(1, "View Outstanding Report", "/os", 10, 1, 0, false, 0, 1),
                new Action(2, "View Today Win Loss", "/wl", 20, 2, 0, true, 0, 1),
                new Action(3, "View Balance Info", "/bal", 30, 3, 0, true, 0, 1),

                new Action(4, "Specific Direct Downline", "", 50, 1, 1, false, 0, 1),
                new Action(5, "Top 50 Outstanding", "50", 40, 1, 1, true, 0, 1),
                new Action(6, "Please input username", "", 60, 1, 4, true, 0, 2),

                new Action(7, "Top x Outstanding", "", 70, 1, 1, false, 0, 1),
                 new Action(8, "Please input num of downline", "", 60, 1, 7, true, 0, 2),
            };

            cacheService.GetOrSetAsync(
                "Chatbot.Modules.Actions.AllActions",
                Arg.Any<Func<IEnumerable<Action>>>(),
                Arg.Any<CacheItemOptions>())
                .Returns(Task.FromResult(actionsStub as IEnumerable<Action>));
        }
    }
}