using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{

    public abstract class AzureBlobStorageServiceBase : AzureSdkServiceBase
    {
        protected AzureStorageAccountConnectionParameters _connectionParameters;
        protected IAzureStorageAccountAccessKeyService _azureStorageAccountAccessKeyService;

        public AzureBlobStorageServiceBase(IConfiguration configuration, ILogger logger, IAzureCredentialService azureCredentialService, IAzureStorageAccountAccessKeyService azureStorageAccountAccessKeyService)
            : base(configuration, logger, azureCredentialService)
        {
            _azureStorageAccountAccessKeyService = azureStorageAccountAccessKeyService;
        }

        public void SetConnectionParameters(string connectionStringConfigName)
        {
            _connectionParameters = AzureStorageAccountConnectionParameters.CreateUsingConnectionString(GetStorageConnectionString(connectionStringConfigName));
        }

        public void SetConnectionParameters(string resourceGroupName, string accountName)
        {
            _connectionParameters = AzureStorageAccountConnectionParameters.CreateUsingResourceGroupAndAccountName(resourceGroupName, accountName);
        }      
        
        string GetStorageConnectionString(string nameOfConfig)
        {
            return _config[nameOfConfig];
        }

        protected async Task<string> GetStorageAccountKey(CancellationToken cancellationToken = default)
        {
            string accessKey = null;

            if (!String.IsNullOrWhiteSpace(_connectionParameters.StorageAccountId))
            {
                accessKey = await _azureStorageAccountAccessKeyService.GetStorageAccountKey(_connectionParameters.StorageAccountId, cancellationToken);
            }
            else if (!String.IsNullOrWhiteSpace(_connectionParameters.StorageAccountResourceGroup)
                && !String.IsNullOrWhiteSpace(_connectionParameters.StorageAccountName))
            {
                accessKey = await _azureStorageAccountAccessKeyService.GetStorageAccountKey(
                    _connectionParameters.StorageAccountResourceGroup,
                    _connectionParameters.StorageAccountName,
                    cancellationToken);
            }

            return accessKey;
        }           

    }   
}
