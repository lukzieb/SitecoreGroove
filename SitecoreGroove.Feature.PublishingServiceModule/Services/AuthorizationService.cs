using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Sitecore.Abstractions;

namespace SitecoreGroove.Feature.PublishingServiceModule.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly BaseLog _logger;
        private readonly BaseSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthorizationService(BaseSettings settings, BaseLog logger, IHttpClientFactory httpClientFactory)
        {
            _settings = settings;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<TokenResponse> GetTokenResponseAsync()
        {
            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                TokenResponse tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = $"{_settings.GetSetting("FederatedAuthentication.IdentityServer.Authority")}/connect/token",
                    ClientId = _settings.GetSetting("PublishingService.Client.Id"),
                    ClientSecret = _settings.GetSetting("PublishingService.Client.Secret")
                });

                if (tokenResponse.IsError)
                {
                    _logger.Error($"{nameof(AuthorizationService)} - Error while obtaining token - {tokenResponse.Error} - {tokenResponse.ErrorType} - {tokenResponse.ErrorDescription}", this);
                }

                return tokenResponse;
            }
        }
    }
}