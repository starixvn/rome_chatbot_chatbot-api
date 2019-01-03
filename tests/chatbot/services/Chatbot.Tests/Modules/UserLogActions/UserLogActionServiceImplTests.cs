namespace Chatbot.Tests.Modules.UserLogActions
{
    using Chatbot.Models.Dtos;
    using Chatbot.Modules.UserLogActions;
    using Chatbot.Modules.UserLogActions.Params.Db.Commands;
    using Chatbot.Modules.UserLogActions.Params.Db.Criteria;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;

    public class UserLogActionServiceImplTests
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly UserLogActionServiceImpl userLogActionService;

        public UserLogActionServiceImplTests()
        {
            dynamicRepository = Substitute.For<IDynamicRepository>();
            userLogActionService = new UserLogActionServiceImpl(dynamicRepository);
        }

        [Fact]
        public void IncreaseUserActionCount_Always_CallsDynamicRepositoryExecuteAsync()
        {
            // Arrange
            var userId = 1;
            var actionId = 2;
            var messengerType = 1;
            var messengerUsername = "username";

            // Act
            userLogActionService.IncreaseUserActionCount(userId, actionId, messengerType, messengerUsername).ConfigureAwait(false);

            // Assert
            dynamicRepository.Received().ExecuteAsync(Arg.Is<InsertUserLogActionCommand>(
                c => c.UserId == userId
                    && c.ActionId == actionId
                    && c.MessengerType == messengerType
                    && c.MessengerUsername == messengerUsername));
        }

        [Fact]
        public void GetUserAction_Always_CallsDynamicRepositoryFetchAsync()
        {
            // Arrange
            var userId = 1;
            var actionId = 2;
            var messengerType = 1;
            var messengerUsername = "username";

            // Act
            userLogActionService.GetUserAction(userId, messengerType, messengerUsername, actionId).ConfigureAwait(false);

            // Assert
            dynamicRepository.Received().FetchAsync<UserActionDto>(Arg.Is<GetUserLogActionsCriteria>(
                c => c.UserId == userId
                    && c.ActionId == actionId
                    && c.MessengerType == messengerType
                    && c.MessengerUsername == messengerUsername));
        }

        [Fact]
        public void IsUserReachedTheLimitOfActionInDay_Always_CallsDynamicRepositoryFetchAsync()
        {
            // Arrange
            var userId = 1;
            var actionId = 2;
            var messengerType = 1;

            // Act
            userLogActionService.IsUserReachedTheLimitOfActionInDay(userId, messengerType, actionId, 10).ConfigureAwait(false);

            // Assert
            dynamicRepository.Received().FetchAsync<UserActionDto>(Arg.Is<GetUserLogActionsCriteria>(
                c => c.UserId == userId
                    && c.ActionId == actionId
                    && c.MessengerType == messengerType
                    && c.MessengerUsername == string.Empty));
        }
    }
}