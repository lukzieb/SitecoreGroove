using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace SitecoreGroove.Plugin.PublishingService.Authorization
{
    public sealed class ConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.Authority = "https://sc104identityserver.dev.local";
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