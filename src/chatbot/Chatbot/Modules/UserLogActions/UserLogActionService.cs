namespace Chatbot.Modules.UserLogActions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Models.Dtos;
    using Chatbot.Modules.UserLogActions.Params.Db.Commands;
    using Chatbot.Modules.UserLogActions.Params.Db.Criteria;
    using Fanex.Data.Repository;

    public interface UserLogActionService
    {
        Task IncreaseUserActionCount(int userId, int actionId, int messengerType, string messengerUsername);

        Task<bool> IsUserReachedTheLimitOfActionInDay(int userId, int messengerType, int actionId, int limit);

        Task<IEnumerable<UserActionDto>> GetUserAction(int userId, int messengerType, string messengerUsername, int actionId);
    }

    public class UserLogActionServiceImpl : UserLogActionService
    {
        private readonly IDynamicRepository dynamicRepository;

        public UserLogActionServiceImpl(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task IncreaseUserActionCount(
            int userId,
            int actionId,
            int messengerType,
            string messengerUsername)
        {
            var command = new InsertUserLogActionCommand(userId, actionId, messengerType, messengerUsername);

            await dynamicRepository.ExecuteAsync(command).ConfigureAwait(false);
        }

        public async Task<bool> IsUserReachedTheLimitOfActionInDay(int userId, int messengerType, int actionId, int limit)
        {
            var addUserLogActions = await GetUserAction(userId, messengerType, string.Empty, actionId).ConfigureAwait(false);
            var numOfAddTime = addUserLogActions.Sum(action => action.CountTimes);

            return numOfAddTime >= limit;
        }

        public async Task<IEnumerable<UserActionDto>> GetUserAction(
            int userId,
            int messengerType,
            string messengerUsername,
            int actionId)
        {
            var criteria = new GetUserLogActionsCriteria(userId, actionId, messengerType, messengerUsername);

            var userAction = await dynamicRepository.FetchAsync<UserActionDto>(criteria).ConfigureAwait(false);

            return userAction;
        }
    }
}