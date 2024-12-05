using System.Security.Cryptography;
using System.Text;
using Duende.IdentityServer.EntityFramework.DbContexts;
using IdentityServer4.Contrib.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sitecore.Framework.Runtime.Configuration;
using Sitecore.Plugin.IdentityServer.Configuration;
using Sitecore.Plugin.IdentityServer.Storage.ExternalUsers;

namespace SitecoreGroove.Plugin.IdentityServer.CredentialsEncryption
{
    public class ConfigureSitecore
    {
        private readonly ISitecoreConfiguration _scConfig;

        private readonly AppSettings _appSettings;

        public ConfigureSitecore(ISitecoreConfiguration scConfig)
        {
            _scConfig = scConfig;
            _appSettings = _scConfig.GetSection(AppSettings.SectionName).Get<AppSettings>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            byte[] encryptedConnectionString = Convert.FromBase64String(_appSettings.SitecoreMembershipOptions.ConnectionString);
            byte[] decryptedConnectionString = ProtectedData.Unprotect(encryptedConnectionString, null, DataProtectionScope.LocalMachine);

            MembershipOptions membershipOptions = new MembershipOptions
            {
                ConnectionString = Encoding.Unicode.GetString(decryptedConnectionString),
                ApplicationName = _appSettings.SitecoreMembershipOptions.ApplicationName,
                UseRoleProviderSource = _appSettings.SitecoreMembershipOptions.UseRoleProviderSource,
                MaxInvalidPasswordAttempts = _appSettings.SitecoreMembershipOptions.MaxInvalidPasswordAttempts,
                PasswordAttemptWindow = _appSettings.SitecoreMembershipOptions.PasswordAttemptWindow
            };

            services.Replace(new ServiceDescriptor(typeof(MembershipOptions), membershipOptions));

            ReplaceDbContext<ExternalUserDbContext>(services, membershipOptions.ConnectionString);
            ReplaceDbContext<PersistedGrantDbContext>(services, membershipOptions.ConnectionString);
        }

        private void ReplaceDbContext<T>(IServiceCollection services, string connectionStrings) where T : DbContext
        {
            ServiceDescriptor? descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
                services.AddDbContext<T>(builder =>
                {
                    builder.UseSqlServer(connectionStrings);
                });
            }
        }
    }
}