using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using SitecoreGroove.Feature.PublishingServiceModule.Services;

namespace SitecoreGroove.Feature.PublishingServiceModule.Handlers
{
    public class AuthorizationHandler : DelegatingHandler
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationHandler(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            TokenResponse token = await _authorizationService.GetTokenResponseAsync();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}