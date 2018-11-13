namespace Chatbot.WebApi.Tests.Filters
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Modules.MessageBuilders;
    using Chatbot.Modules.MessageSenders;
    using Chatbot.WebApi.Filters;
    using Chatbot.WebApi.Params;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;
    using Xunit;

    [Trait("Filter", "AutoReplyUMFilter")]
    public class AutoReplyUMFilterAttributeTests
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly IMessageSender messageSender;
        private readonly IMessageBuilder messageBuilder;
        private readonly AutoReplyUMFilterAttribute autoReplyUMFilterAttribute;
        private readonly HttpContext httpContext;

        public AutoReplyUMFilterAttributeTests()
        {
            maintenanceService = Substitute.For<IMaintenanceService>();
            messageSender = Substitute.For<IMessageSender>();
            messageBuilder = Substitute.For<IMessageBuilder>();
            //actionExecutingContext = Substitute.ForPartsOf<ActionExecutingContext>();
            httpContext = new DefaultHttpContext();

            autoReplyUMFilterAttribute = new AutoReplyUMFilterAttribute(maintenanceService, messageSender, messageBuilder);
        }

        [Fact]
        public void OnActionExecutionAsync_ActionIsNotRunCommand_DontGetUnderMaintenanceTime()
        {
            var actionExecutingContext = StubActionExecutingContext(new Dictionary<string, object>());

            autoReplyUMFilterAttribute.OnActionExecutionAsync(
                actionExecutingContext,
                Substitute.For<ActionExecutionDelegate>()).GetAwaiter().GetResult();

            maintenanceService.DidNotReceive().GetUnderMaintenanceTime();
        }

        [Fact]
        public void OnActionExecutionAsync_NowIsUM_SentUMMessage()
        {
            var actionExecutingContext = StubActionExecutingContext(new Dictionary<string, object>
            {
                {
                    "param",
                    new CommandParam
                    {
                        Command = "cm1",
                        MessengerType = 1,
                        MessengerUsername = "Username"
                    }
                }
            });
            var unInfo = new UnderMaintenanceTime
            {
                IsUnderMaintenanceTime = true,
                From = "4:00",
                To = "5:00"
            };
            maintenanceService
                .GetUnderMaintenanceTime()
                .Returns(Task.FromResult(unInfo));
            messageBuilder.BuildUnderMaintenanceMessage(1, unInfo.From, unInfo.To).Returns("UM Message");

            // Act
            autoReplyUMFilterAttribute.OnActionExecutionAsync(
                actionExecutingContext,
                Substitute.For<ActionExecutionDelegate>()).GetAwaiter().GetResult();

            // Assert
            messageSender.Received().SendMessage(
                Arg.Is<Models.Messenger>(
                    messenger => messenger.Type == 1 && messenger.Username == "Username"),
                    "UM Message",
                    null,
                    0,
                    false);
        }

        private ActionExecutingContext StubActionExecutingContext(Dictionary<string, object> actionArguments)
            => new ActionExecutingContext(
                new ActionContext
                {
                    HttpContext = httpContext,
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor(),
                },
                new List<IFilterMetadata>(),
                actionArguments,
                Substitute.For<Controller>());
    }
}