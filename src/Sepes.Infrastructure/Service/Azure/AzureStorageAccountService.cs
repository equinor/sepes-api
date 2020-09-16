using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureStorageAccountService : AzureServiceBase, IAzureStorageAccountService, IPerformCloudResourceCRUD
    {
        public AzureStorageAccountService(IConfiguration config, ILogger<AzureStorageAccountService> logger) : base(config, logger)
        {

        }

        public async Task<CloudResourceCRUDResult> Create(CloudResourceCRUDInput parameters)
        {
            string storageAccountName = AzureResourceNameUtil.DiagnosticsStorageAccount(parameters.SandboxName);
            var nameIsAvailable = await _azure.StorageAccounts.CheckNameAvailabilityAsync(storageAccountName);

            if (!(bool)nameIsAvailable.IsAvailable)
            {
                _logger.LogError($"StorageAccountName not available/invalid. Message: {nameIsAvailable.Message}");
                throw new ArgumentException($"StorageAccountName not available/invalid. Message: {nameIsAvailable.Message}");
            }

            // Create storage account
            var account = await _azure.StorageAccounts.Define(storageAccountName)
                .WithRegion(parameters.Region)
                .WithExistingResourceGroup(parameters.ResourceGrupName)
                .WithAccessFromAllNetworks()
                .WithGeneralPurposeAccountKindV2()
                .WithOnlyHttpsTraffic()
                .WithSku(StorageAccountSkuType.Standard_LRS)
                .WithTags(parameters.Tags)
                .CreateAsync();

            var result = CreateResult(account); 

            return result;
        }

        CloudResourceCRUDResult CreateResult(IStorageAccount storageAccount)
        {
            var result = CreateResultFromIResource(storageAccount);
            result.CurrentProvisioningState = storageAccount.ProvisioningState.ToString();
            return result;
        }

        CloudResourceCRUDResult CreateResultFromIResource(IResource resource)
        {
            return new CloudResourceCRUDResult() { Resource = resource, Success = true };
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

        public async Task<IStorageAccount> GetResourceAsync(string resourceGroupName, string resourceName)
        {
            var resource = await _azure.StorageAccounts.GetByResourceGroupAsync(resourceGroupName, resourceName);
            return resource;
        }


        public async Task<string> GetProvisioningState(string resourceGroupName, string resourceName)
        {
            var resource = await GetResourceAsync(resourceGroupName, resourceName);

            if (resource == null)
            {
                throw NotFoundException.CreateForAzureResource(resourceName, resourceGroupName);
            }

            return resource.ProvisioningState.ToString();
        }

        public async Task<IDictionary<string, string>> GetTagsAsync(string resourceGroupName, string resourceName)
        {
            var storageAccount = await GetResourceAsync(resourceGroupName, resourceName);
            return AzureResourceTagsFactory.TagReadOnlyDictionaryToDictionary(storageAccount.Tags);
        }

        public async Task UpdateTagAsync(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag)
        {
            var rg = await GetResourceAsync(resourceGroupName, resourceName);
            _ = await rg.Update().WithoutTag(tag.Key).ApplyAsync();
            _ = await rg.Update().WithTag(tag.Key, tag.Value).ApplyAsync();
        }
    }
}
