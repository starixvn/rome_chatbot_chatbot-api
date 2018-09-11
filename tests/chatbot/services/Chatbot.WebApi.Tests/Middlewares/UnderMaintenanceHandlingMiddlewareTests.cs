namespace Chatbot.WebApi.Tests.Middlewares
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Chatbot.WebApi.Middlewares;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;
    using ONEbook.UM.AspNetCore.Services;
    using ONEbook.UM.Core.Models;
    using Xunit;

    public class UnderMaintenanceHandlingMiddlewareTests
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly Controller controller;
        private readonly HttpContext httpContext;
        private readonly HttpRequest httpRequest;
        private readonly HttpResponse httpResponse;
        private readonly RequestDelegate requestDelegate;
        private readonly UnderMaintenanceHandlingMiddleware underMaintenanceHandlingMiddleware;

        public UnderMaintenanceHandlingMiddlewareTests()
        {
            maintenanceService = Substitute.For<IMaintenanceService>();
            controller = Substitute.For<Controller>();
            httpContext = Substitute.For<HttpContext>();
            httpRequest = Substitute.For<HttpRequest>();
            httpResponse = Substitute.For<HttpResponse>();
            requestDelegate = Substitute.For<RequestDelegate>();

            httpContext.Request.Returns(httpRequest);
            httpContext.Response.Returns(httpResponse);

            underMaintenanceHandlingMiddleware = new UnderMaintenanceHandlingMiddleware(requestDelegate, maintenanceService);
        }

        [Fact]
        public void Invoke_RequestIsNotRunCommand_DontExecuteGetUnderMaintenanceTime()
        {
            httpRequest.Path.Returns(new PathString("/command/run"));

            underMaintenanceHandlingMiddleware.Invoke(httpContext).GetAwaiter().GetResult();

            maintenanceService.DidNotReceive().GetUnderMaintenanceTime();
        }

        [Fact]
        public void Invoke_IsNotUMTime_ExecuteNextDelegate()
        {
            httpRequest.Path.Returns(new PathString("/request"));
            maintenanceService
                .GetUnderMaintenanceTime()
                .Returns(Task.FromResult(new UnderMaintenanceTime
                {
                    IsUnderMaintenanceTime = false
                }));

            underMaintenanceHandlingMiddleware.Invoke(httpContext).GetAwaiter().GetResult();

            requestDelegate.Received().Invoke(httpContext);
        }

        [Fact]
        public void Invoke_UMTime_ReponseUMTimeMessage()
        {
            maintenanceService
                .GetUnderMaintenanceTime()
                .Returns(Task.FromResult(new UnderMaintenanceTime
                {
                    IsUnderMaintenanceTime = true,
                    From = "4:00",
                    To = "5:00"
                }));
            var context = new DefaultHttpContext();

            underMaintenanceHandlingMiddleware.Invoke(context).GetAwaiter().GetResult();

            Assert.Equal("application/json", context.Response.ContentType);
            Assert.Equal((int)HttpStatusCode.ServiceUnavailable, context.Response.StatusCode);
        }
    }
}