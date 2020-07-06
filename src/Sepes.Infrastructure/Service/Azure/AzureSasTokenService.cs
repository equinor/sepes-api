using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Sepes.Infrastructure.Model.Config;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureSasTokenService
    {
        readonly IConfiguration _config;

        public AzureSasTokenService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<BlobServiceClient> CreateAdAuthenticatedClient()
        {
            var tenantId = _config[ConfigConstants.TENANT_ID];
            var clientId = _config[ConfigConstants.AZ_CLIENT_ID];
            var clientSecret = _config[ConfigConstants.AZ_CLIENT_SECRET];

            //Uri authorityUri = new Uri($"{_config[ConfigConstants.INSTANCE]}{tenantId}/v2.0/");//  new Uri(_config[ConfigConstants.INSTANCE]);
            Uri authorityUri = new Uri(_config[ConfigConstants.INSTANCE]);//  new Uri(_config[ConfigConstants.INSTANCE]);

            // Create a token credential that can use our Azure Active
            // Directory application to authenticate with Azure Storage
            var credential =
                new ClientSecretCredential(
                    tenantId,
                    clientId,
                    clientSecret,
                    new TokenCredentialOptions() { AuthorityHost = authorityUri });

            var conString = _config["AzureStorageConnectionString"];

            var servicex = new BlobServiceClient(conString);

            var blobUrl = new Uri(string.Format("https://{0}.blob.core.windows.net", servicex.AccountName));

            // Create a client that can authenticate using our token credential
            var service = new BlobServiceClient(blobUrl, credential);

            // Make a service request to verify we've successfully authenticated
            // await service.GetPropertiesAsync();

            return service;
        }
    }
}
