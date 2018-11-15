namespace Chatbot.Modules.Resources
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Modules.Resources.Models;
    using Chatbot.Modules.Resources.Params.Db.Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;

    public interface ResourceService
    {
        Task<Resource> GetResource(int key, string type, string languageCode);
    }

    public class ResourceServiceImpl : ResourceService
    {
        private const string DefaultLanguageCode = "en-US";
        private const string ResourceCacheKey = "Chatbot.Modules.Resources.ResourceServiceImpl";
        private readonly ICacheService cacheService;
        private readonly IDynamicRepository dynamicRepository;

        public ResourceServiceImpl(
            ICacheService cacheService,
            IDynamicRepository dynamicRepository)
        {
            this.cacheService = cacheService;
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<Resource> GetResource(int key, string type, string languageCode)
        {
            var resources = await cacheService.GetOrSetAsync(
                    $"{ResourceCacheKey}.{type}.{key}",
                    () => dynamicRepository.Fetch<Resource>(new GetResourceCriteria(key, type)),
                    new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1)))
                    .ConfigureAwait(false);

            return resources.FirstOrDefault(r => r.LanguageCode == languageCode)
                ?? resources.FirstOrDefault(r => r.LanguageCode == DefaultLanguageCode);
        }
    }
}