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
    using Chatbot.Modules.Actions.Validators;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.Settings;
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
        private readonly SettingService settingService;
        private readonly IActionValidator actionValidator;
        private readonly IActionCompiler actionCompiler;
        private readonly Func<DateTime> getDateTimeNowFunc;
        private readonly ActionServiceImpl actionServiceImpl;
        private readonly IDynamicRepository dynamicRepository;

        public ActionServiceTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            cacheService = Substitute.For<ICacheService>();
            messageBuilder = Substitute.For<IMessageBuilder>();
            actionValidator = Substitute.For<IActionValidator>();
            actionCompiler = Substitute.For<IActionCompiler>();

            actionValidators = new Dictionary<int, IActionValidator>
            {
                { 4, actionValidator }
            };
            actionCompilers = new Dictionary<int, IActionCompiler>
            {
                { 4, actionCompiler }
            };
            settingService = Substitute.For<SettingService>();
            getDateTimeNowFunc = Substitute.For<Func<DateTime>>();

            actionServiceImpl = new ActionServiceImpl(
                dynamicRepository,
                cacheService,
                messageBuilder,
                actionValidators,
                actionCompilers,
                settingService,
                getDateTimeNowFunc);
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
            Assert.Equal("outstanding label", actionResult.Message.MessageMarkup.MessageActions.ElementAt(0).Label);
            Assert.Equal("ActionId=3", actionResult.Message.MessageMarkup.MessageActions.ElementAt(1).Data);
            Assert.Equal("message title", actionResult.Message.Content);
        }

        [Fact]
        public void HandleUserReponseByMessage_UserReponseIsIncorrectFormat_ReturnWrongInputMessage()
        {
            StubActions();
            var user = new User(1, 1, 4, 0, "en-US");
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
            actionValidator.ValidateAction(4, user, "user reponse").Returns(new Result(1, "failed"));
            messageBuilder.BuildActionName(4, user.Language).Returns("outstanding label");
            messageBuilder.BuildCancelRequestMessage("en-US").Returns("cancel button");

            var actionResult = actionServiceImpl.HandleUserReponseByMessage(lastUserAction, null, user, "user reponse")
                .GetAwaiter()
                .GetResult()
                .Item3;

            Assert.Equal("outstanding label", actionResult.Message.Content);
            Assert.Equal("cancel button", actionResult.Message.MessageMarkup.MessageActions.First().Label);
        }

        private void StubUserFuntions(User user)
        {
            settingService
                .GetFunctions(user.Id)
                .Returns(Task.FromResult(
                    new List<Function>
                    {
                        new Function { Id = 1, Enabled = true },
                        new Function { Id = 3, Enabled = true }
                    } as IEnumerable<Function>));
        }

        private void StubActions()
        {
            var actionsStub = new List<Action>
            {
                new Action(1, "View Outstanding Report", "/os", 10, 1, 0, false, 0, 1),
                new Action(2, "View Today Win Loss", "/wl", 20, 2, 0, true, 0, 1),
                new Action(3, "View Balance Info", "/bal", 30, 3, 0, true, 0, 1),

                new Action(4, "Specific Direct Downline", "", 40, 1, 1, false, 0, 1),
                new Action(5, "Top 50 Outstanding", "50", 50, 1, 1, true, 0, 1),
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