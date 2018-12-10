namespace Chatbot.Tests.Modules.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models;
    using Chatbot.Modules.Actions;
    using Chatbot.Modules.Actions.Builders.ActionName;
    using Chatbot.Modules.Actions.Builders.ActionTitle;
    using Chatbot.Modules.Actions.Compilers;
    using Chatbot.Modules.Actions.Models;
    using Chatbot.Modules.Actions.Params.DB.Commands;
    using Chatbot.Modules.Actions.Params.DB.Criteria;
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Settings;
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
        private readonly SettingService settingService;
        private readonly ResourceService resourceService;
        private readonly IChatbotConfigs chatbotConfigs;
        private readonly ActionNameBuilderFactory actionNameBuilderFactory;
        private readonly ActionTitleBuilderFactory actionTitleBuilderFactory;

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
            settingService = Substitute.For<SettingService>();
            resourceService = Substitute.For<ResourceService>();
            getDateTimeNowFunc = Substitute.For<Func<DateTime>>();
            actionNameBuilderFactory = Substitute.For<ActionNameBuilderFactory>();
            actionTitleBuilderFactory = Substitute.For<ActionTitleBuilderFactory>();

            actionServiceImpl = new ActionServiceImpl(
                dynamicRepository,
                cacheService,
                messageBuilder,
                actionValidators,
                actionCompilers,
                settingService,
                resourceService,
                chatbotConfigs,
                actionNameBuilderFactory,
                actionTitleBuilderFactory,
                getDateTimeNowFunc);

            actionValidator
                .ValidateAction(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<IList<UserAction>>(), Arg.Any<Messenger>())
                .Returns(ActionValidationResult.CreateSuccessfulValidationResult());
        }

        [Fact]
        public void HandleUserReponseByMessage_LastActionIsNull_ReturnUserRootActions()
        {
            var user = new User(1, 1, 4, 0, "en-US", null);
            var messenger = new Messenger(1, "ltid");

            StubActions();
            StubUserFuntions(user);

            messageBuilder.BuildRootActionTitle(user.Language).Returns("message title");
            actionNameBuilderFactory.GetActionNameBuilder(1).BuildActionName(1, user).Returns("outstanding label");

            var actionResult = actionServiceImpl.HandleUserReponseByMessage(user, "message", messenger)
                .GetAwaiter()
                .GetResult()
                .Item3;

            Assert.Equal(2, actionResult.Message.MessageMarkup.MessageActions.Count());
            Assert.Equal("OUTSTANDING LABEL", actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Label);
            Assert.Equal("{\"ActionId\":1,\"UserActions\":null,\"IsCancel\":false,\"CompiledCommand\":\"\",\"SentTime\":\"0001-01-01T00:00:00\",\"IsConfirmButton\":false,\"Label\":null}", actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Data);
            Assert.Equal("{\"ActionId\":2,\"UserActions\":null,\"IsCancel\":false,\"CompiledCommand\":\"/wl\",\"SentTime\":\"0001-01-01T00:00:00\",\"IsConfirmButton\":false,\"Label\":null}", actionResult.Message.MessageMarkup.MessageActions.ElementAt(1).Data);
            Assert.Equal("message title", actionResult.Message.Content);
        }

        [Fact]
        public void HandleUserReponseByMessage_UserReponseIsIncorrectFormat_ReturnWrongInputMessage()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US", null);
            var messenger = new Messenger(1, "ltid");
            StubUserActions(user.Id);
            StubUserFuntions(user);
            actionValidator.ValidateAction(user, "user reponse", Arg.Any<IList<UserAction>>(), messenger)
                .Returns(new ActionValidationResult(1, "failed"));

            var actionResult = actionServiceImpl.HandleUserReponseByMessage(
                user, "user reponse", messenger)
                .GetAwaiter()
                .GetResult()
                .Item3;

            Assert.Equal("failed", actionResult.ValidationFailedMessage);
        }

        [Fact]
        public void HandleUserReponseByMessage_UserReponseIsIncorrectFormat_ReturnRepeatMessage()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US", null);
            var messenger = new Messenger(1, "ltid");
            StubUserFuntions(user);
            actionValidator.ValidateAction(user, "user reponse", Arg.Any<IList<UserAction>>(), messenger)
                .Returns(new ActionValidationResult(1, "failed"));
            StubUserActions(user.Id);
            actionNameBuilderFactory.GetActionNameBuilder(4).BuildActionName(4, user, Arg.Any<IList<UserAction>>()).Returns("outstanding label");
            messageBuilder.BuildCancelRequestMessage("en-US").Returns("cancel button");

            var actionResult = actionServiceImpl.HandleUserReponseByMessage(
                user,
                "user reponse",
                messenger)
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
            var user = new User(1, 1, 4, 0, "en-US", null);
            var messenger = new Messenger(1, "ltid");
            StubUserActions(user.Id);
            StubUserFuntions(user);
            actionCompiler.Compile(Arg.Any<IEnumerable<Action>>(), Arg.Any<IEnumerable<UserAction>>())
                .Returns("compiled command");

            var compiledCommand = actionServiceImpl.HandleUserReponseByMessage(
                user, "user reponse", messenger)
                .GetAwaiter()
                .GetResult()
                .Item1;

            Assert.Equal("compiled command", compiledCommand);
        }

        [Fact]
        public void HandleUserReponseByMessage_Always_ReturnLastUserActionId()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US", null);
            var messenger = new Messenger(1, "ltid");
            StubUserActions(user.Id);
            StubUserFuntions(user);
            actionCompiler
                .Compile(Arg.Any<IEnumerable<Action>>(), Arg.Any<IEnumerable<UserAction>>())
                .Returns("compiled command");

            var lastActionId = actionServiceImpl.HandleUserReponseByMessage(
                user,
                "user reponse",
                messenger)
                .GetAwaiter()
                .GetResult()
                .Item2;

            Assert.Equal(4, lastActionId);
        }

        [Fact]
        public void HandleUserReponseByButton_CancelRequest_ExecuteUpdateUserActionsToCancel()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US", null);
            StubUserActions(user.Id);
            var messenger = new Messenger(1, "ltid");

            actionServiceImpl.HandleUserReponseByButton(
                user,
                "{\"ActionId\":1,\"UserActions\":null,\"IsCancel\":true,\"CompiledCommand\":\"\"}",
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
            var user = new User(1, 1, 4, 0, "en-US", null);
            StubUserActions(user.Id);
            var messenger = new Messenger(1, "ltid");
            messageBuilder
                .BuildResponseCancelRequestMessage(1, user.Language)
                .Returns("cancel action");

            var actionResult = actionServiceImpl.HandleUserReponseByButton(
                user,
                "{\"ActionId\":1,\"UserActions\":null,\"IsCancel\":true,\"CompiledCommand\":\"\"}",
                messenger)
                    .GetAwaiter()
                    .GetResult()
                    .Item3;

            Assert.Equal("cancel action", actionResult.Message.Content);
        }

        [Fact]
        public void HandleUserReponseByButton_ButtonHistoryIsClick_InsertHistoryActionToDatabase()
        {
            // Arrange
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US", null);
            var messenger = new Messenger(1, "ltid");
            StubUserFuntions(user);
            messageBuilder.BuildRootActionTitle(user.Language).Returns("message title");
            actionNameBuilderFactory.GetActionNameBuilder(1).BuildActionName(1, user).Returns("outstanding label");

            // Act
            actionServiceImpl.HandleUserReponseByButton(
                user,
                "{\"ActionId\":4,\"UserActions\":[{\"Id\":1,\"ActionId\":1,\"UserResponse\":\"123\",\"ChatTime\":\"2018-12-04T13:58:17.273015+07:00\",\"CompiledCommand\":\"compile-1\",\"Status\":1,\"IsActionValid\":true},{\"Id\":2,\"ActionId\":4,\"UserResponse\":\"test\",\"ChatTime\":\"2018-12-04T13:58:17.2734022+07:00\",\"CompiledCommand\":\"compile-2\",\"Status\":1,\"IsActionValid\":true}],\"IsCancel\":false,\"CompiledCommand\":\"\"}",
                messenger).ConfigureAwait(false);

            // Assert
            dynamicRepository.Received().ExecuteAsync(Arg.Is<InsertUserActionCommand>(
               command => command.UserId == 1
                           && command.ActionId == 1
                           && command.UserResponse == "123"
                           && command.CompiledCommand == "compile-1"));
            dynamicRepository.Received().ExecuteAsync(Arg.Is<InsertUserActionCommand>(
               command => command.UserId == 1
                           && command.ActionId == 4
                           && command.UserResponse == "test"
                           && command.CompiledCommand == "compile-2"));
        }

        [Fact]
        public void HandleUserReponseByButton_ActionIsValid_ExecuteInsertUserAction()
        {
            var user = new User(1, 1, 4, 0, "en-US", null);
            var messenger = new Messenger(1, "ltid");
            StubActions();
            StubUserFuntions(user);
            StubUserActions(user.Id);
            actionCompiler.Compile(Arg.Any<IEnumerable<Action>>(), Arg.Any<IEnumerable<UserAction>>())
                .Returns("compiled command");

            actionServiceImpl.HandleUserReponseByButton(
                user,
                "{\"ActionId\":4,\"UserActions\":[{\"Id\":1,\"ActionId\":1,\"UserResponse\":\"\",\"ChatTime\":\"2018-12-04T13:58:17.273015+07:00\",\"CompiledCommand\":null,\"Status\":1,\"IsActionValid\":true},{\"Id\":2,\"ActionId\":4,\"UserResponse\":\"\",\"ChatTime\":\"2018-12-04T13:58:17.2734022+07:00\",\"CompiledCommand\":null,\"Status\":1,\"IsActionValid\":true}],\"IsCancel\":false,\"CompiledCommand\":\"\"}",
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
            var user = new User(1, 1, 4, 0, "en-US", null);
            StubUserActions(user.Id);
            StubUserFuntions(user);
            messageBuilder
                    .BuildResponseCancelRequestMessage(1, user.Language)
                    .Returns("cancel action");

            var actionResult = actionServiceImpl
                .BuildAction("cancel", "{\"ActionId\":1,\"UserActions\":null,\"IsCancel\":true,\"CompiledCommand\":null}", user, messenger)
                .GetAwaiter()
                .GetResult();

            Assert.Equal("cancel action", actionResult.Message.Content);
            Assert.Equal(2, actionResult.Message.MessageMarkup.MessageActions.Count());
            Assert.Equal("{\"ActionId\":1,\"UserActions\":null,\"IsCancel\":false,\"CompiledCommand\":\"\",\"SentTime\":\"0001-01-01T00:00:00\",\"IsConfirmButton\":false,\"Label\":null}", actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Data);
            Assert.Equal("{\"ActionId\":2,\"UserActions\":null,\"IsCancel\":false,\"CompiledCommand\":\"/wl\",\"SentTime\":\"0001-01-01T00:00:00\",\"IsConfirmButton\":false,\"Label\":null}", actionResult.Message.MessageMarkup.MessageActions.ElementAt(1).Data);
        }

        [Fact]
        public void BuildAction_ActionIsEnd_ReturnCompileCommand()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US", null);
            var messenger = new Messenger(1, "ltid");
            StubUserActionsWithLastActionIsMessage(user.Id);
            StubUserFuntions(user);
            actionCompiler.Compile(Arg.Any<IEnumerable<Action>>(), Arg.Any<IEnumerable<UserAction>>())
                .Returns("compiled command");

            var actionResult = actionServiceImpl.BuildAction(
                "command", string.Empty, user, messenger)
                .GetAwaiter()
                .GetResult();

            Assert.Equal("compiled command", actionResult.Command);
        }

        [Fact]
        public void BuildAction_ActionIsNotEndAndNextActionIsButtons_ReturnNextButtonActions()
        {
            StubActions();
            var messenger = new Messenger(1, "ltid");
            var user = new User(1, 1, 4, 0, "en-US", null);
            StubUserActions(user.Id);
            StubUserFuntions(user);

            var actionResult = actionServiceImpl.BuildAction(
                "command", "{\"ActionId\":1,\"UserActions\":null,\"IsCancel\":false,\"CompiledCommand\":null}", user, messenger)
                .GetAwaiter()
                .GetResult();

            Assert.Equal(4, actionResult.Message.MessageMarkup.MessageActions.Count());
            Assert.Equal(
                "{\"ActionId\":5,\"UserActions\":[{\"Id\":1,\"ActionId\":1,\"UserResponse\":\"\",\"ChatTime\":\"1989-05-28T00:00:00\",\"CompiledCommand\":null,\"Status\":1,\"IsActionValid\":true},{\"Id\":2,\"ActionId\":4,\"UserResponse\":\"\",\"ChatTime\":\"1989-06-28T00:00:00\",\"CompiledCommand\":null,\"Status\":1,\"IsActionValid\":true}],\"IsCancel\":false,\"CompiledCommand\":\"/os 50\",\"SentTime\":\"0001-01-01T00:00:00\",\"IsConfirmButton\":false,\"Label\":\"\"}",
                actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Data);
            Assert.Equal(
                "{\"ActionId\":4,\"UserActions\":[{\"Id\":1,\"ActionId\":1,\"UserResponse\":\"\",\"ChatTime\":\"1989-05-28T00:00:00\",\"CompiledCommand\":null,\"Status\":1,\"IsActionValid\":true},{\"Id\":2,\"ActionId\":4,\"UserResponse\":\"\",\"ChatTime\":\"1989-06-28T00:00:00\",\"CompiledCommand\":null,\"Status\":1,\"IsActionValid\":true}],\"IsCancel\":false,\"CompiledCommand\":\"\",\"SentTime\":\"0001-01-01T00:00:00\",\"IsConfirmButton\":false,\"Label\":\"\"}",
                actionResult.Message.MessageMarkup.MessageActions.ElementAt(1).Data);
            Assert.Equal(
                "{\"ActionId\":7,\"UserActions\":[{\"Id\":1,\"ActionId\":1,\"UserResponse\":\"\",\"ChatTime\":\"1989-05-28T00:00:00\",\"CompiledCommand\":null,\"Status\":1,\"IsActionValid\":true},{\"Id\":2,\"ActionId\":4,\"UserResponse\":\"\",\"ChatTime\":\"1989-06-28T00:00:00\",\"CompiledCommand\":null,\"Status\":1,\"IsActionValid\":true}],\"IsCancel\":false,\"CompiledCommand\":\"\",\"SentTime\":\"0001-01-01T00:00:00\",\"IsConfirmButton\":false,\"Label\":\"\"}",
                actionResult.Message.MessageMarkup.MessageActions.ElementAt(2).Data);
            Assert.Equal(
                "{\"ActionId\":1,\"UserActions\":null,\"IsCancel\":true,\"CompiledCommand\":null,\"SentTime\":\"0001-01-01T00:00:00\",\"IsConfirmButton\":false,\"Label\":null}",
                actionResult.Message.MessageMarkup.MessageActions.ElementAt(3).Data);
        }

        [Fact]
        public void BuildAction_ActionIsNotEndAndNextActionIsMessage_ReturnNextMessage()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US", null);
            var messenger = new Messenger(1, "ltid");
            StubUserActions(user.Id);
            StubUserFuntions(user);

            actionNameBuilderFactory.GetActionNameBuilder(6).BuildActionName(6, user, Arg.Any<IList<UserAction>>())
                .Returns("next action name");

            var actionResult = actionServiceImpl.BuildAction(
                "command", "", user, messenger)
                .GetAwaiter()
                .GetResult();

            Assert.Equal("next action name", actionResult.Message.Content);
            Assert.Single(actionResult.Message.MessageMarkup.MessageActions);
            Assert.Equal("{\"ActionId\":1,\"UserActions\":null,\"IsCancel\":true,\"CompiledCommand\":null,\"SentTime\":\"0001-01-01T00:00:00\",\"IsConfirmButton\":false,\"Label\":null}",
                actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Data);
        }

        private void StubUserFuntions(User user)
            => user.Functions = new List<Function>
                    {
                        new Function { Id = 1, Enabled = true },
                        new Function { Id = 2, Enabled = true }
                    };

        private void StubUserActions(int userId)
        {
            var userActions = new List<UserAction>
            {
                new UserAction(1, userId, 1, string.Empty, new DateTime(1989, 5, 28)),
                new UserAction(2, userId, 4, string.Empty, new DateTime(1989, 6, 28))
            };

            dynamicRepository
                .FetchAsync<UserAction>(Arg.Is<GetActionByUserCriteria>(criteria => criteria.UserId == userId))
                .Returns(Task.FromResult(userActions as IEnumerable<UserAction>));
        }

        private void StubUserActionsWithLastActionIsMessage(int userId)
        {
            var userActions = new List<UserAction>
            {
                new UserAction(1, userId, 1, string.Empty, new DateTime(1989, 5, 28)),
                new UserAction(2, userId, 4, string.Empty, new DateTime(1989, 6, 28)),
                new UserAction(3, userId, 6, string.Empty, new DateTime(1989, 7, 28))
            };

            dynamicRepository
                .FetchAsync<UserAction>(Arg.Is<GetActionByUserCriteria>(criteria => criteria.UserId == userId))
                .Returns(Task.FromResult(userActions as IEnumerable<UserAction>));
        }

        private void StubActions()
        {
            var actionsStub = new List<Action>
            {
                new Action(1, "View Outstanding Report", "/os", 10, 1, 0, false, 0, 1, 0, 0),
                new Action(2, "View Today Win Loss", "/wl", 20, 2, 0, true, 0, 1, 0, 0),
                new Action(3, "View Balance Info", "/bal", 30, 3, 0, true, 0, 1, 0, 0),

                new Action(4, "Specific Direct Downline", "", 50, 1, 1, false, 0, 1, 0, 0),
                new Action(5, "Top 50 Outstanding", "50", 40, 1, 1, true, 0, 1, 0, 0),
                new Action(6, "Please input username", "", 60, 1, 4, true, 0, 2, 0, 0),

                new Action(7, "Top x Outstanding", "", 70, 1, 1, false, 0, 1, 0, 0),
                new Action(8, "Please input num of downline", "", 60, 1, 7, true, 0, 2, 0, 0),
            };

            cacheService.GetOrSetAsync(
                "Chatbot.Modules.Actions.AllActions",
                Arg.Any<Func<IEnumerable<Action>>>(),
                Arg.Any<CacheItemOptions>())
                .Returns(Task.FromResult(actionsStub as IEnumerable<Action>));
        }
    }
}