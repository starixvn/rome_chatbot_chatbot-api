namespace Chatbot.Tests.Modules.Resources
{
    using System;
    using System.Collections.Generic;
    using Chatbot.Modules.Resources;
    using Chatbot.Modules.Resources.Constants;
    using Chatbot.Modules.Resources.Models;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using NSubstitute;
    using Xunit;

    public class ResourceServiceImplTests
    {
        private readonly ICacheService cacheService;
        private readonly IDynamicRepository dynamicRepository;
        private readonly ResourceServiceImpl resourceService;

        public ResourceServiceImplTests()
        {
            cacheService = Substitute.For<ICacheService>();
            dynamicRepository = Substitute.For<IDynamicRepository>();
            resourceService = Substitute.ForPartsOf<ResourceServiceImpl>(cacheService, dynamicRepository);
        }

        [Fact]
        public void GetResource_Always_CallsGetResources()
        {
            // Arrange
            var key = 1;

            // Act
            resourceService.GetResource(key, ResourceType.Action, "en-US").ConfigureAwait(false);

            // Assert
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            resourceService.Received().GetResources(key, ResourceType.Action);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        [Fact]
        public void GetResource_Always_CallsGetOrSetAsyncFromCache()
        {
            // Act
            resourceService.GetResources(1, ResourceType.Action).ConfigureAwait(false);

            // Assert
            cacheService.Received().GetOrSetAsync(
                "Chatbot.Modules.Resources.ResourceServiceImpl.Action.1",
                Arg.Any<Func<IEnumerable<Resource>>>(),
                Arg.Any<CacheItemOptions>());
        }
    }
}