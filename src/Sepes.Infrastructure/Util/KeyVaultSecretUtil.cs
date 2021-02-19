using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util
{
    public static class KeyVaultSecretUtil
    {
        public static SecretClient GetKeyVaultClient(ILogger logger, IConfiguration config, string nameOfKeyVaultUrlSetting)
        {
            var keyVaultUrl = config[nameOfKeyVaultUrlSetting];
            var tenantId = config[ConfigConstants.AZ_TENANT_ID];
            var clientId = config[ConfigConstants.AZ_CLIENT_ID];
            var clientSecret = config[ConfigConstants.AZ_CLIENT_SECRET];

            return new SecretClient(new Uri(keyVaultUrl), new ClientSecretCredential(tenantId, clientId, clientSecret));
        }

        public static async Task<Uri> AddKeyVaultSecret(ILogger logger, IConfiguration config, string nameOfKeyVaultUrlSetting, string secretName, string secretValue)
        {
            var client = GetKeyVaultClient(logger, config, nameOfKeyVaultUrlSetting);
            var setResult = await client.SetSecretAsync(secretName, secretValue);
            return setResult.Value.Id;
        }

        public static async Task<string> GetKeyVaultSecretValue(ILogger logger, IConfiguration config, string nameOfKeyVaultUrlSetting, string secretName)
        {
            var client = GetKeyVaultClient(logger, config, nameOfKeyVaultUrlSetting);
            var secret = await client.GetSecretAsync(secretName);

            return secret.Value.Value;
        }

        public static async Task<string> DeleteKeyVaultSecretValue(ILogger logger, IConfiguration config, string nameOfKeyVaultUrlSetting, string secretName, bool purge = false)
        {
            var client = GetKeyVaultClient(logger, config, nameOfKeyVaultUrlSetting);
            var secret = await client.StartDeleteSecretAsync(secretName);

            if (purge)
            {
                try
                {
                    _ = await client.PurgeDeletedSecretAsync(secretName);
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Unable to purge secret {secretName}", ex);
                }

            }

            return secret.Value.Value;
        }

        //Key vault sometimes get bloated with vm passwords, use this routine to delete and purge those. Remember not to leave reference to this so it runs in production
        public static async Task ClearAllVmPasswords(ILogger logger, IConfiguration config, string nameOfKeyVaultUrlSetting)
        {

            try
            {
                var client = GetKeyVaultClient(logger, config, nameOfKeyVaultUrlSetting);


                foreach (var cur in client.GetPropertiesOfSecrets())
                {
                    if (cur.Name.ToLower().IndexOf("newvmpassword-") == 0)
                    {
                        if (cur.CreatedOn.HasValue)
                        {
                            if (cur.CreatedOn.Value.AddHours(1) < DateTime.UtcNow)
                            {
                                await client.StartDeleteSecretAsync(cur.Name);
    
                        }
                        }
                    }
                }

                foreach (var cur in client.GetDeletedSecrets())
                {
                    if (cur.Name.ToLower().IndexOf("newvmpassword-") == 0)
                    {

                        await client.PurgeDeletedSecretAsync(cur.Name);
                    }
                }


            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Failed to delete VM passwords from keyvault");
            }
        }
    }
}
