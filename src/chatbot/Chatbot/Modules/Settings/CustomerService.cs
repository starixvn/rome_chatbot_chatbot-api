namespace Chatbot.Modules.Settings
{
    using System;
    using System.Threading.Tasks;
    using Chatbot.Modules.Settings.Params.Db.Commands;
    using Fanex.Caching;
    using Fanex.Data.Repository;

    public interface CustomerService
    {
        Task<bool> IsAccountClosed(int custId, int subAccountId);
    }

    public class CustomerServiceImpl : CustomerService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheService cacheService;
        private const string customerClosedCacheKey = "CustomerServiceImpl.IsAccountClosed.{0}.{1}";

        public CustomerServiceImpl(IDynamicRepository dynamicRepository, ICacheService cacheService)
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheService = cacheService;
        }

        public async Task<bool> IsAccountClosed(int custId, int subAccountId)
        {
            var isClosed = await cacheService.GetOrSetAsync(
                string.Format(customerClosedCacheKey, custId, subAccountId),
                () => !IsAccountActive(custId, subAccountId),
                new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(0, 0, 30)));

            return isClosed;
        }

        private bool IsAccountActive(int custId, int subAccountId)
        {
            var command = new GetCustomerStatusCommand(custId, subAccountId);

            dynamicRepository.Execute(command);

            return command.IsActive;
        }
    }
}