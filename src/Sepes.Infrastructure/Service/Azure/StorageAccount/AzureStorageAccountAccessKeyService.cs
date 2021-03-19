using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util.Azure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureStorageAccountAccessKeyService : AzureServiceBase, IAzureStorageAccountAccessKeyService
    {
        public AzureStorageAccountAccessKeyService(IConfiguration configuration, ILogger<AzureStorageAccountAccessKeyService> logger)
            : base(configuration, logger)
        {

        }

        public async Task<string> GetStorageAccountKey(IStorageAccount storageAccount, CancellationToken cancellationToken = default)
        {
            if (storageAccount == null)
            {              
                throw new ArgumentNullException("storageAccount");
            }

            return await AzureStorageUtils.GetStorageAccountKeyByName(storageAccount, cancellationToken: cancellationToken);
        }

        public async Task<string> GetStorageAccountKey(string resourceGroup, string accountName, CancellationToken cancellationToken = default)
        {
            var storageAccount = await _azure.StorageAccounts.GetByResourceGroupAsync(resourceGroup, accountName, cancellationToken);

            if (storageAccount == null)
            {
                _logger.LogError($"Storage account {accountName} not found in resource group {resourceGroup}");
                throw new Exception("Storage account not found");
            }

            return await GetStorageAccountKey(storageAccount, cancellationToken: cancellationToken);
        }

        public async Task<string> GetStorageAccountKey(string storageAccountId, CancellationToken cancellationToken = default)
        {
            var storageAccount = await _azure.StorageAccounts.GetByIdAsync(storageAccountId, cancellationToken);
            return await GetStorageAccountKey(storageAccount, cancellationToken: cancellationToken);
        }      
    }
}
