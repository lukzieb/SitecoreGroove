using System;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;

namespace SitecoreGroove.Feature.PublishingServiceModule.Services
{
    public class CachedAuthorizationService : IAuthorizationService
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IMemoryCache _memoryCache;

        public CachedAuthorizationService(IAuthorizationService authorizationService, IMemoryCache memoryCache)
        {
            _authorizationService = authorizationService;
            _memoryCache = memoryCache;
        }

        public async Task<TokenResponse> GetTokenResponseAsync()
        {
            return await _memoryCache.GetOrCreateAsync("token", async cacheEntry =>
            {
                TokenResponse tokenResponse = await _authorizationService.GetTokenResponseAsync();

                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenResponse.IsError ? 1 : tokenResponse.ExpiresIn);
                cacheEntry.SetValue(tokenResponse.AccessToken);

                return tokenResponse;
            });
        }
    }
}