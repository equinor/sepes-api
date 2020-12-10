using AutoMapper;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Azure.Management.Storage.Fluent.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureStorageAccountService : AzureServiceBase, IAzureStorageAccountService
    {

        IMapper _mapper;

        public AzureStorageAccountService(IConfiguration config, ILogger<AzureStorageAccountService> logger, IMapper mapper)
            : base(config, logger)
        {
            _mapper = mapper;
        }

        public async Task<CloudResourceCRUDResult> EnsureCreated(CloudResourceCRUDInput parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Ensuring Diagnostic Storage Account exists for sandbox with Name: {parameters.SandboxName}! Resource Group: {parameters.ResourceGroupName}");

            var diagnosticStorageAccount = await GetResourceAsync(parameters.ResourceGroupName, parameters.Name);

            if (diagnosticStorageAccount == null)
            {
                _logger.LogInformation($"Storage account not found, creating");

                var nameIsAvailable = await _azure.StorageAccounts.CheckNameAvailabilityAsync(parameters.Name);

                if (!(bool)nameIsAvailable.IsAvailable)
                {
                    _logger.LogError($"StorageAccountName not available/invalid. Message: {nameIsAvailable.Message}");
                    throw new ArgumentException($"StorageAccountName not available/invalid. Message: {nameIsAvailable.Message}");
                }

                // Create storage account
                diagnosticStorageAccount = await _azure.StorageAccounts.Define(parameters.Name)
                    .WithRegion(parameters.Region)
                    .WithExistingResourceGroup(parameters.ResourceGroupName)
                    .WithAccessFromAllNetworks()
                    .WithGeneralPurposeAccountKindV2()
                    .WithOnlyHttpsTraffic()
                    .WithSku(StorageAccountSkuType.Standard_LRS)
                    .WithTags(parameters.Tags)
                    .CreateAsync(cancellationToken);

                _logger.LogInformation($"Done creating storage account");
            }

            var result = CreateResult(diagnosticStorageAccount);

            return result;
        }

        public async Task<CloudResourceCRUDResult> GetSharedVariables(CloudResourceCRUDInput parameters)
        {
            var diagnosticStorageAccount = await GetResourceAsync(parameters.ResourceGroupName, parameters.Name);

            var result = CreateResult(diagnosticStorageAccount);

            return result;
        }

        CloudResourceCRUDResult CreateResult(IStorageAccount storageAccount)
        {
            var result = CloudResourceCRUDUtil.CreateResultFromIResource(storageAccount);
            result.CurrentProvisioningState = storageAccount.ProvisioningState.ToString();
            return result;
        }

        public async Task DeleteStorageAccount(string resourceGroupName, string storageAccountName, CancellationToken cancellationToken = default)
        {
            var resource = await GetResourceAsync(resourceGroupName, storageAccountName, cancellationToken);

            if(resource != null)
            {
                //Ensure resource is is managed by this instance
                CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, resource.Tags);

                await _azure.StorageAccounts.DeleteByResourceGroupAsync(resourceGroupName, storageAccountName, cancellationToken);
            }      
        }

        public async Task<IStorageAccount> GetResourceAsync(string resourceGroupName, string resourceName, CancellationToken cancellationToken = default)
        {
            var resource = await _azure.StorageAccounts.GetByResourceGroupAsync(resourceGroupName, resourceName, cancellationToken);
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

        public Task<CloudResourceCRUDResult> Update(CloudResourceCRUDInput parameters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<AzureStorageAccountDto> CreateStorageAccount(Region region, string resourceGroupName, string name, Dictionary<string, string> tags, List<string> onlyAllowAccessFrom = null, CancellationToken cancellationToken = default)
        {
            var storageAccount = await CreateStorageAccountInternal(region, resourceGroupName, name, tags, onlyAllowAccessFrom, cancellationToken);

            return _mapper.Map<AzureStorageAccountDto>(storageAccount);
        }

        async Task<IStorageAccount> CreateStorageAccountInternal(Region region, string resourceGroupName, string name, Dictionary<string, string> tags, List<string> onlyAllowAccessFrom = null, CancellationToken cancellationToken = default)
        {
            var creator = _azure.StorageAccounts.Define(name)
            .WithRegion(region)
            .WithExistingResourceGroup(resourceGroupName);

            if (onlyAllowAccessFrom == null)
            {
                creator = creator.WithAccessFromAllNetworks();
            }
            else
            {
                creator = creator.WithAccessFromAzureServices();

                foreach (var curAddr in onlyAllowAccessFrom)
                {
                    creator = creator.WithAccessFromIpAddress(curAddr);
                }
            }

            creator = creator
            .WithGeneralPurposeAccountKindV2()
            .WithOnlyHttpsTraffic()
            .WithSku(StorageAccountSkuType.Standard_LRS)
            .WithTags(tags);

            return await creator.CreateAsync();
        }

        public async Task<AzureStorageAccountDto> SetStorageAccountAllowedIPs(string resourceGroupName, string storageAccountName, List<string> onlyAllowAccessFrom = null, CancellationToken cancellationToken = default)
        {
            var account = await GetResourceAsync(resourceGroupName, storageAccountName, cancellationToken);
            var ipRulesList = onlyAllowAccessFrom == null ? null : onlyAllowAccessFrom.Select(alw => new IPRule(alw, Microsoft.Azure.Management.Storage.Fluent.Models.Action.Allow)).ToList();
            var updateParameters = new StorageAccountUpdateParameters() { NetworkRuleSet = new NetworkRuleSet() { IpRules = ipRulesList, Bypass = Bypass.AzureServices } };
            var updateResult = await _azure.StorageAccounts.Inner.UpdateAsync(resourceGroupName, storageAccountName, updateParameters, cancellationToken);
            return _mapper.Map<AzureStorageAccountDto>(account);
        }
    }
}
