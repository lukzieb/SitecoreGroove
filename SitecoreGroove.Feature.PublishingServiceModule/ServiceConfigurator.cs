using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using Sitecore.DependencyInjection;
using SitecoreGroove.Feature.PublishingServiceModule.Handlers;
using SitecoreGroove.Feature.PublishingServiceModule.Services;

namespace SitecoreGroove.Feature.PublishingServiceModule
{
    public class ServiceConfigurator : IServicesConfigurator
    {
        public virtual void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<AuthorizationHandler>();
            serviceCollection.AddSingleton<IMemoryCache, MemoryCache>();
            serviceCollection.AddSingleton<IAuthorizationService, AuthorizationService>();
            serviceCollection.Decorate<IAuthorizationService, CachedAuthorizationService>();

            serviceCollection.AddTransient<IConfigureOptions<HttpClientFactoryOptions>>(serviceProvider => new ConfigureNamedOptions<HttpClientFactoryOptions>("refitClient", options =>
            {
                options.HttpMessageHandlerBuilderActions.Add(builder =>
                {
                    builder.AdditionalHandlers.Add(serviceProvider.GetRequiredService<AuthorizationHandler>());
                });
            }));
        }
    }
}