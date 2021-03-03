using Azure.Storage;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util.Azure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureStorageAccountTokenService : AzureBlobStorageServiceBase, IAzureStorageAccountTokenService
    {

        public AzureStorageAccountTokenService(IConfiguration configuration, ILogger<AzureStorageAccountTokenService> logger)
            : base(configuration, logger)
        {

        }

        public async Task<string> GetStorageAccountKey(string resourceGroup, string accountName, CancellationToken cancellationToken = default)
        {
            var storageAccount = await _azure.StorageAccounts.GetByResourceGroupAsync(resourceGroup, accountName, cancellationToken);

            if (storageAccount == null)
            {
                _logger.LogError($"Storage account {accountName} not found in resource group {resourceGroup}");
                throw new Exception("Storage account not found");
            }

            return await AzureStorageUtils.GetStorageAccountKey(storageAccount, cancellationToken: cancellationToken);
        }

        public async Task<string> GetStorageAccountKey(string storageAccountId, CancellationToken cancellationToken = default)
        {
            var storageAccount = await _azure.StorageAccounts.GetByIdAsync(storageAccountId, cancellationToken);
            return await AzureStorageUtils.GetStorageAccountKey(storageAccount, cancellationToken: cancellationToken);
        }

        public async Task<UriBuilder> CreateFileDownloadUriBuilder(string containerName, CancellationToken cancellationToken = default)
        {
            return await CreateUriBuilderWithSasToken(containerName, cancellationToken: cancellationToken);
        }

        public async Task<UriBuilder> CreateFileUploadUriBuilder(string containerName, CancellationToken cancellationToken = default)
        {
            return await CreateUriBuilderWithSasToken(containerName, permission: BlobContainerSasPermissions.Write, expiresOnMinutes: 30, cancellationToken: cancellationToken);
        }

        public async Task<UriBuilder> CreateFileDeleteUriBuilder(string containerName, CancellationToken cancellationToken = default)
        {
            return await CreateUriBuilderWithSasToken(containerName, permission: BlobContainerSasPermissions.Delete, expiresOnMinutes: 5, cancellationToken: cancellationToken);
        }

        async Task<UriBuilder> CreateUriBuilderWithSasToken(
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
                    var sasBuilder = AzureStorageUtils.CreateBlobSasBuilder(containerName, resourceType, permission, expiresOnMinutes);

                    StorageSharedKeyCredential credential;

                    if (!String.IsNullOrWhiteSpace(_connectionParameters.ConnectionString))
                    {
                        credential = AzureStorageUtils.CreateCredentialFromConnectionString(_connectionParameters.ConnectionString);
                    }
                    else
                    {
                        string accessKey = await GetStorageAccountKey(_connectionParameters.StorageAccountResourceGroup, _connectionParameters.StorageAccountName, cancellationToken);

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

        public async Task<Uri> GetSasKey(string containerName = "files", CancellationToken cancellationToken = default)
        {
            var blobServiceClient = await GetBlobServiceClient();
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            return blobContainerClient.Uri;
        }

        protected override async Task<string> GetStorageAccountKey(CancellationToken cancellationToken = default)
        {
            try
            {
                return await AzureStorageUtils.GetStorageAccountKey(this, _connectionParameters, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get access key");
                throw;
            }
        }
    }
}
