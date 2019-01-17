namespace Chatbot.Client.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using Chatbot.Client.Helpers;
    using Chatbot.Client.Models;
    using Chatbot.Client.Services;
    using NSubstitute;
    using Xunit;

    public class MessageServiceTests
    {
        private readonly IRestClient restClient;
        private readonly MessageServiceImpl messageService;

        public MessageServiceTests()
        {
            restClient = Substitute.For<IRestClient>();

            messageService = new MessageServiceImpl(restClient);
        }

        [Fact]
        public void SendMessage_Alway_ExecuteExecutePostAsync()
        {
            var userId = 1;
            var message = "message";
            var functionId = 2;
            int? selfdestructionPeriod = 30;

            messageService.SendMessage(userId, message, selfdestructionPeriod, functionId).GetAwaiter().GetResult();

            Func<object, bool> assert = data =>
            {
                var type = data.GetType();
                var actualMessage = type.GetProperty("message").GetValue(data);
                var actualUserId = (int)type.GetProperty("userId").GetValue(data);
                var actualFunctionId = (int)type.GetProperty("functionId").GetValue(data);
                var actualSelfdestructionPeriod = (int)type.GetProperty("selfdestructionPeriod").GetValue(data);

                return actualMessage.Equals(message)
                    && actualUserId == userId
                    && actualFunctionId == functionId
                    && actualSelfdestructionPeriod == selfdestructionPeriod;
            };

            restClient.Received().ExecutePostAsync<Result>(
                "message/send",
                Arg.Is<object>(data => assert(data)));
        }

        [Fact]
        public void GetMessageLogs_Alway_ExecuteExecutePostAsync()
        {
            var userId = 1;
            var functionId = 2;
            var messengerType = 4;
            DateTimeOffset fromDate = new DateTime(1989, 1, 1);
            DateTimeOffset toDate = new DateTime(1989, 2, 1);

            messageService.GetMessageLogs(userId, fromDate, toDate, functionId, messengerType).GetAwaiter().GetResult();

            Func<object, bool> assert = data =>
            {
                var type = data.GetType();
                var actualUserId = (int)type.GetProperty("userId").GetValue(data);
                var actualFunctionId = (int)type.GetProperty("functionId").GetValue(data);
                var actualMessengerType = (int)type.GetProperty("messengerType").GetValue(data);
                var actualFromDate = (long)type.GetProperty("fromDate").GetValue(data);
                var actualToDate = (long)type.GetProperty("toDate").GetValue(data);

                return actualUserId == userId
                    && actualFunctionId == functionId
                    && actualMessengerType == messengerType
                    && actualFromDate == fromDate.ToUnixTimeMilliseconds()
                    && actualToDate == toDate.ToUnixTimeMilliseconds();
            };

            restClient.Received().ExecutePostAsync<List<Message>>(
                "message/getMessageLogs",
                Arg.Is<object>(data => assert(data)));
        }

        [Fact]
        public void GetFunctionMessages_Always_CallsExecuteGetAsync()
        {
            // Act
            messageService.GetFunctionMessages(1);

            // Assert
            restClient.Received().ExecuteGetAsync<IEnumerable<FunctionMessage>>(
                $"message/getFunctionMessages?functionId=1");
        }

        [Fact]
        public void GetFunctionMessages_Always_ReturnsCorrectValue()
        {
            // Arrange
            var functionId = 1;
            IEnumerable<FunctionMessage> functionMessages = new List<FunctionMessage>
            {
                new FunctionMessage{ Message="message", UserId=1, SelfdestructionPeriod=15 }
            };
            restClient
                .ExecuteGetAsync<IEnumerable<FunctionMessage>>($"message/getFunctionMessages?functionId={functionId}")
                .Returns(functionMessages);

            // Act
            var actualResult = messageService.GetFunctionMessages(functionId)
                .GetAwaiter()
                .GetResult();

            // Assert
            Assert.Equal(functionMessages, actualResult);
        }

        [Fact]
        public void SendCompleteUMNotifications_Always_CallsExecutePostAsync()
        {
            // Act
            messageService.SendCompleteUMNotifications();

            // Assert
            restClient.ExecutePostAsync<Result>("message/sendCompleteUMNotifications");
        }
    }
}