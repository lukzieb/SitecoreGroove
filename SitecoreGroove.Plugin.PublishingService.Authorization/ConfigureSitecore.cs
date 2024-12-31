using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Framework.Runtime.Configuration;

namespace SitecoreGroove.Plugin.PublishingService.Authorization
{
    public sealed class ConfigureSitecore
    {
        private readonly string _identityServerAuthority;
        public ConfigureSitecore(ISitecoreConfiguration scConfig)
        {
            _identityServerAuthority = scConfig.GetSection("IdentityServer:IdentityServerAuthority").Value;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.Authority = _identityServerAuthority;
                 options.TokenValidationParameters.ValidateAudience = false;
             });

            AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
                .RequireClaim("scope", new[] { "sitecore.publishingService.api" })
                .Build();

            services.Configure<MvcOptions>(x => x.Filters.Add(new AuthorizeFilter(policy)));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }
    }
}