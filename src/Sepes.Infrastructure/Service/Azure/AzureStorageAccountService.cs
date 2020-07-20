using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureStorageAccountService : AzureServiceBase, IAzureStorageAccountService
    {
        public AzureStorageAccountService(IConfiguration config, ILogger logger) : base(config, logger)
        {

        }

        public async Task<IStorageAccount> CreateStorageAccount(Region region, string sandboxName, string resourceGroupName, Dictionary<string, string> tags)
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
                 .WithTags(tags)
                .CreateAsync();

            // Get keys to build connectionstring with
            //var keys = await account.GetKeysAsync();

            // Build connection string. Maybe return this? Or should access happen through SAS-key?
            //string connectionString = $"DefaultEndpointsProtocol=https;AccountName={account.Name};AccountKey={keys[0].Value};EndpointSuffix=core.windows.net";

            // Connect
            //var connectedAccount = CloudStorageAccount.Parse(connectionString);

            return account;
        }

        public async Task<IStorageAccount> CreateDiagnosticsStorageAccount(Region region, string sandboxName, string resourceGroupName, Dictionary<string, string> tags)
        {
            string storageAccountName = AzureResourceNameUtil.DiagnosticsStorageAccount(sandboxName);
            var nameIsAvailable = await _azure.StorageAccounts.CheckNameAvailabilityAsync(storageAccountName);
            if (!(bool)nameIsAvailable.IsAvailable)
            {
                _logger.LogError($"StorageAccountName not available/invalid. Message: {nameIsAvailable.Message}");
                throw new ArgumentException($"StorageAccountName not available/invalid. Message: {nameIsAvailable.Message}");
            }
            // Create storage account
            var account = await _azure.StorageAccounts.Define(storageAccountName)
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroupName)
                .WithAccessFromAllNetworks()
                .WithGeneralPurposeAccountKindV2()
                .WithOnlyHttpsTraffic()
                .WithSku(StorageAccountSkuType.Standard_LRS)
                .WithTags(tags)
                .CreateAsync();

            return account;
        }

        public async Task DeleteStorageAccount(string resourceGroupName, string storageAccountName)
        {
            await _azure.StorageAccounts.DeleteByResourceGroupAsync(resourceGroupName, storageAccountName);
        }

        public async Task<bool> Exists(string resourceGroupName, string storageAccountName)
        {
            var stgAcc = await _azure.StorageAccounts.GetByResourceGroupAsync(resourceGroupName, storageAccountName);

            if (stgAcc == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(stgAcc.Id);
        }
    }
}
