namespace Chatbot.Modules.Resources
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Chatbot.Modules.Resources.Models;
    using Chatbot.Modules.Resources.Params.Db.Criteria;
    using Fanex.Data.Repository;

    public interface ResourceService
    {
        Task<IEnumerable<Resource>> GetResources(int key, string type);
    }

    public class ResourceServiceImpl : ResourceService
    {
        private readonly IDynamicRepository dynamicRepository;

        public ResourceServiceImpl(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<IEnumerable<Resource>> GetResources(int key, string type)
            => await dynamicRepository.FetchAsync<Resource>(
                new GetResourceCriteria(key, type)).ConfigureAwait(false);
    }
}