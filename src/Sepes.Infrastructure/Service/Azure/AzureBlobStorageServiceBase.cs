using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto.Azure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{

    public abstract class AzureBlobStorageServiceBase : AzureServiceBase
    {
        protected AzureStorageAccountConnectionParameters _connectionParameters;      

        public AzureBlobStorageServiceBase(IConfiguration configuration, ILogger logger)
            : base(configuration, logger)
        {
            
        }

        public void SetConnectionParameters(string connectionStringConfigName)
        {
            _connectionParameters = AzureStorageAccountConnectionParameters.CreateUsingConnectionString(GetStorageConnectionString(connectionStringConfigName));
        }

        public void SetConnectionParameters(string resourceGroupName, string accountName)
        {
            _connectionParameters = AzureStorageAccountConnectionParameters.CreateUsingResourceGroupAndAccountName(resourceGroupName, accountName);
        }

        protected async Task<BlobServiceClient> GetBlobServiceClient(CancellationToken cancellationToken = default)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(_connectionParameters.ConnectionString))
                {
                    return new BlobServiceClient(_connectionParameters.ConnectionString);
                }
                else
                {
                    string accessKey = await GetStorageAccountKey(cancellationToken);

                    var credential = new StorageSharedKeyCredential(_connectionParameters.StorageAccountName, accessKey);

                    //Should have access through subscription? Or neet to get token/access key?
                    return new BlobServiceClient(new Uri($"https://{_connectionParameters.StorageAccountName}.blob.core.windows.net"), credential);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong when creating BlobServiceClient");
                throw new Exception($"Unable to connect to Azure Storage Account", ex);
            }
        }
        
        string GetStorageConnectionString(string nameOfConfig)
        {
            return _config[nameOfConfig];
        }

        protected abstract Task<string> GetStorageAccountKey(CancellationToken cancellationToken = default);      

    }   
}
