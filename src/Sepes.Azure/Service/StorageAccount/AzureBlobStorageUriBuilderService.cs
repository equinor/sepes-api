using Azure.Storage;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Azure.Util;

namespace Sepes.Azure.Service
{
    public class AzureBlobStorageUriBuilderService : AzureBlobStorageServiceBase, IAzureBlobStorageUriBuilderService
    {
        public AzureBlobStorageUriBuilderService(IConfiguration configuration, ILogger<AzureBlobStorageUriBuilderService> logger, IAzureStorageAccountAccessKeyService azureStorageAccountAccessKeyService)
            : base(configuration, logger, azureStorageAccountAccessKeyService)
        {

        }    
      

        public async Task<UriBuilder> CreateUriBuilder(
            string containerName,
            string resourceType = "c",
            BlobContainerSasPermissions permission = BlobContainerSasPermissions.Read,
            int expiresOnMinutes = 10,
            CancellationToken cancellationToken = default
            )
        {
            var accountName = AzureStorageUtils.GetAccountName(_connectionParameters);

            if (!String.IsNullOrWhiteSpace(accountName))
            {
                var uriBuilder = new UriBuilder()
                {
                    Scheme = "https",
                    Host = string.Format("{0}.blob.core.windows.net", accountName)
                };

                if (_connectionParameters.IsDevelopmentStorage)
                {
                    return uriBuilder;
                }
                else
                {
                    var sasBuilder = CreateBlobSasBuilder(containerName, resourceType, permission, expiresOnMinutes);

                    StorageSharedKeyCredential credential;

                    if (!String.IsNullOrWhiteSpace(_connectionParameters.ConnectionString))
                    {
                        credential = AzureStorageUtils.CreateCredentialFromConnectionString(_connectionParameters.ConnectionString);
                    }
                    else
                    {
                        string accessKey = await GetStorageAccountKey(cancellationToken);

                        credential = new StorageSharedKeyCredential(_connectionParameters.StorageAccountName, accessKey);
                    }

                    var sasToken = sasBuilder.ToSasQueryParameters(credential);

                    uriBuilder.Query = sasToken.ToString();

                    return uriBuilder;
                }
            }
            else
            {
                return null;
            }
        }

        BlobSasBuilder CreateBlobSasBuilder(string containerName, string resourceType = "c", BlobContainerSasPermissions permission = BlobContainerSasPermissions.Read, int expiresOnMinutes = 10)
        {
            var sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                Resource = resourceType,
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiresOnMinutes)
            };

            sasBuilder.SetPermissions(permission);

            return sasBuilder;
        }

    }
}
