using System.Threading.Tasks;
using IdentityModel.Client;

namespace SitecoreGroove.Feature.PublishingServiceModule.Services
{
    public interface IAuthorizationService
    {
        Task<TokenResponse> GetTokenResponseAsync();
    }
}