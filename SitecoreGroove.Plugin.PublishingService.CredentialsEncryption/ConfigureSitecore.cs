using Microsoft.Extensions.Configuration;
using Sitecore.Framework.Runtime.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace SitecoreGroove.Plugin.PublishingService.CredentialsEncryption
{
    public sealed class ConfigureSitecore
    {
        public ConfigureSitecore(ISitecoreConfiguration scConfig)
        {
            IEnumerable<IConfigurationSection> connectionStringSections = scConfig.GetSection("Publishing:ConnectionStrings").GetChildren();

            foreach (IConfigurationSection section in connectionStringSections.Where(x=>x.Key != "Service"))
            {
                scConfig[$"Publishing:ConnectionStrings:{section.Key}"] = Decrypt(section.Value);
            }
        }

        private static string Decrypt(string value)
        {
            byte[] encryptedConnectionString = Convert.FromBase64String(value);
            byte[] decryptedConnectionString = ProtectedData.Unprotect(encryptedConnectionString, null, DataProtectionScope.LocalMachine);

            return Encoding.Unicode.GetString(decryptedConnectionString);
        }
    }
}