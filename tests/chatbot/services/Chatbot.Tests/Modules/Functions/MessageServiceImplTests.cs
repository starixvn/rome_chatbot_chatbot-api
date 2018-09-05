namespace Chatbot.Tests.Modules.Functions
{
    using System;
    using System.Threading.Tasks;
    using Chatbot.Modules.Funtions;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Chatbot.Modules.Settings.Params.Db.Criteria;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;

    public class MessageServiceImplTests
    {
        private readonly IDynamicRepository subDynamicRepository;

        public MessageServiceImplTests()
        {
            subDynamicRepository = Substitute.For<IDynamicRepository>();
        }

        private MessageServiceImpl CreateMessageServiceImpl()
        {
            return new MessageServiceImpl(subDynamicRepository);
        }

        [Fact]
        public async Task GetLastQueryTime_ExecuteGetFromDynamicRepository_ReturnExpectedResult()
        {
            // Arrange
            var lastQueryTime = new DateTime(2018, 2, 20);
            subDynamicRepository
                .GetAsync<DateTime>(Arg.Is<GetMessageLastQueryTimeCriteria>(c => c.UserId == 1 && c.FunctionId == 2 && c.MessageType == 3))
                .Returns(lastQueryTime);
            var service = CreateMessageServiceImpl();

            // Act
            var actualLastQueryTime = await service.GetLastQueryTime(1, 2, 3);

            // Assert
            Assert.Equal(lastQueryTime, actualLastQueryTime);
        }

        [Fact]
        public async Task UpdateLastQueryTime_Always_CallExecuteFromDynamicRepository()
        {
            // Arrange
            var service = CreateMessageServiceImpl();
            var lastQueryTime = new DateTime(2018, 2, 20);

            // Act
            await service.UpdateLastQueryTime(1, 2, 3, lastQueryTime);

            // Assert
            await subDynamicRepository.Received().ExecuteAsync(
                Arg.Is<UpdateMessageLastQueryTimeCommand>(
                    c => c.UserId == 1 && c.FunctionId == 2 && c.MessageType == 3 && c.LastQueryTime == lastQueryTime));
        }
    }
}