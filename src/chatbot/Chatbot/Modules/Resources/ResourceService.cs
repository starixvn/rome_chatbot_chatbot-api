namespace Chatbot.Modules.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Chatbot.Modules.Resources.Models;
    using Chatbot.Modules.Resources.Params.Db.Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;

    public interface ResourceService
    {
        Task<Resource> GetResource(int key, string type, string language);

        Task<IEnumerable<Resource>> GetResources(int key, string type);
    }

    public class ResourceServiceImpl : ResourceService
    {
        private const string DefaultLanguage = "en-US";
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

        public async Task<Resource> GetResource(int key, string type, string language)
        {
            var resources = await GetResources(key, type).ConfigureAwait(false);

            return resources.FirstOrDefault(r => r.LanguageCode == language)
                ?? resources.FirstOrDefault(r => r.LanguageCode == DefaultLanguage);
        }

        public async Task<IEnumerable<Resource>> GetResources(int key, string type)
            => await cacheService.GetOrSetAsync(
                $"{ResourceCacheKey}.{type}.{key}",
                () => dynamicRepository.Fetch<Resource>(new GetResourceCriteria(key, type)),
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1))).ConfigureAwait(false);
    }
}