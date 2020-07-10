using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Azure.Management.Storage.Fluent.Models;
using Microsoft.Azure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    class AzureStorageAccountService : AzureServiceBase, IAzureStorageAccountService
    {
        public AzureStorageAccountService(IConfiguration config, ILogger logger) : base(config, logger)
        {

        }

        public async Task<IStorageAccount> CreateStorageAccount(Region region, string sandboxName, string resourceGroupName)
        {
            string storageAccountName = AzureResourceNameUtil.StorageAccount(sandboxName);

            // Create storage account
            var account = await _azure.StorageAccounts.Define(storageAccountName)
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroupName)
                .WithAccessFromAllNetworks()
                .WithGeneralPurposeAccountKindV2()
                .WithOnlyHttpsTraffic()
                .WithSku(StorageAccountSkuType.Standard_LRS)
                .CreateAsync();

            // Get keys to build connectionstring with
            var keys = await account.GetKeysAsync();

            // Build connection string Maybe return this? Or should access happen through SAS-key?
            string connectionString = $"DefaultEndpointsProtocol=https;AccountName={account.Name};AccountKey={keys[0].Value};EndpointSuffix=core.windows.net";

            // Connect
            //var connectedAccount = CloudStorageAccount.Parse(connectionString);

            return account;
        }

        public async Task DeleteStorageAccount(string resourceGroupName, string storageAccountName)
        {
            await _azure.StorageAccounts.DeleteByResourceGroupAsync(resourceGroupName, storageAccountName);
        }

    }
}
