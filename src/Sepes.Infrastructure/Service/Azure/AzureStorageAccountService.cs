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
    public class AzureStorageAccountService : AzureServiceBase, IAzureStorageAccountService
    {
        public AzureStorageAccountService(IConfiguration config, ILogger<AzureStorageAccountService> logger) : base(config, logger)
        {

        }

        public async Task<CloudResourceCRUDResult> Create(CloudResourceCRUDInput parameters)
        {
            _logger.LogInformation($"Creating Storage Account for sandbox with Name: {parameters.SandboxName}! Resource Group: {parameters.ResourceGrupName}");         
        
            var nameIsAvailable = await _azure.StorageAccounts.CheckNameAvailabilityAsync(parameters.Name);

            if (!(bool)nameIsAvailable.IsAvailable)
            {
                _logger.LogError($"StorageAccountName not available/invalid. Message: {nameIsAvailable.Message}");
                throw new ArgumentException($"StorageAccountName not available/invalid. Message: {nameIsAvailable.Message}");
            }

            // Create storage account
            var account = await _azure.StorageAccounts.Define(parameters.Name)
                .WithRegion(parameters.Region)
                .WithExistingResourceGroup(parameters.ResourceGrupName)
                .WithAccessFromAllNetworks()
                .WithGeneralPurposeAccountKindV2()
                .WithOnlyHttpsTraffic()
                .WithSku(StorageAccountSkuType.Standard_LRS)
                .WithTags(parameters.Tags)
                .CreateAsync();

            var result = CreateResult(account);

            _logger.LogInformation($"Done creating Storage Account for sandbox with Name: {parameters.SandboxName}! Id: {account.Id}");

            return result;
        }

        CloudResourceCRUDResult CreateResult(IStorageAccount storageAccount)
        {
            var result = CloudResourceCRUDUtil.CreateResultFromIResource(storageAccount);
            result.CurrentProvisioningState = storageAccount.ProvisioningState.ToString();
            return result;
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

        //public async Task<IStorageAccount> CreateDiagnosticsStorageAccount(Region region, string sandboxName, string resourceGroupName, Dictionary<string, string> tags)
        //{
        //    string storageAccountName = AzureResourceNameUtil.DiagnosticsStorageAccount(sandboxName);
        //    var nameIsAvailable = await _azure.StorageAccounts.CheckNameAvailabilityAsync(storageAccountName);
        //    if (!(bool)nameIsAvailable.IsAvailable)
        //    {
        //        _logger.LogError($"StorageAccountName not available/invalid. Message: {nameIsAvailable.Message}");
        //        throw new ArgumentException($"StorageAccountName not available/invalid. Message: {nameIsAvailable.Message}");
        //    }
        //    // Create storage account
        //    var account = await _azure.StorageAccounts.Define(storageAccountName)
        //        .WithRegion(region)
        //        .WithExistingResourceGroup(resourceGroupName)
        //        .WithAccessFromAllNetworks()
        //        .WithGeneralPurposeAccountKindV2()
        //        .WithOnlyHttpsTraffic()
        //        .WithSku(StorageAccountSkuType.Standard_LRS)
        //        .WithTags(tags)
        //        .CreateAsync();

        //    return account;
        //}

        public async Task DeleteStorageAccount(string resourceGroupName, string storageAccountName)
        {
            var resource = await GetResourceAsync(resourceGroupName, storageAccountName);
            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, resource.Tags);

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
            var resource = await GetResourceAsync(resourceGroupName, resourceName);
       
            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, resource.Tags);

       
            _ = await resource.Update().WithoutTag(tag.Key).ApplyAsync();
            _ = await resource.Update().WithTag(tag.Key, tag.Value).ApplyAsync();
        }

        public Task<CloudResourceCRUDResult> Delete(CloudResourceCRUDInput parameters)
        {
            throw new NotImplementedException();
        }
    }
}
