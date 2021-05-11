using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto.Provisioning;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureResourceGroupService : AzureServiceBase, IAzureResourceGroupService
    {
        public AzureResourceGroupService(IConfiguration config, ILogger<AzureResourceGroupService> logger)
            : base(config, logger)
        {

        }

        public async Task<ResourceProvisioningResult> EnsureCreated(ResourceProvisioningParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Creating Resource Group for sandbox with Name: {parameters.SandboxName}! Resource Group: {parameters.ResourceGroupName}");

            var resourceGroup = await GetResourceGroupAsync(parameters.ResourceGroupName, false);

            if (resourceGroup == null)
            {
                _logger.LogInformation($"Resource group not found, creating");
                resourceGroup = await CreateInternal(parameters.ResourceGroupName, parameters.Region, parameters.Tags);
            }
            else
            {
                _logger.LogInformation($"Resource group allready exists");
            }

            var crudResult = ResourceProvisioningResultUtil.CreateFromIResource(resourceGroup);
            crudResult.CurrentProvisioningState = resourceGroup.ProvisioningState.ToString();

            _logger.LogInformation($"Done ensuring Resource Group for sandbox with Id: {parameters.SandboxName}! Resource Group Id: {resourceGroup.Id}");
            return crudResult;
        }

        public async Task<ResourceProvisioningResult> GetSharedVariables(ResourceProvisioningParameters parameters)
        {
            var resourceGroup = await GetResourceGroupAsync(parameters.Name);
            var crudResult = ResourceProvisioningResultUtil.CreateFromIResource(resourceGroup);
            crudResult.CurrentProvisioningState = resourceGroup.ProvisioningState.ToString();
            return crudResult;
        }

        async Task<IResourceGroup> CreateInternal(string resourceGroupName, Region region, Dictionary<string, string> tags, CancellationToken cancellationToken = default)
        {
            IResourceGroup resourceGroup = await _azure.ResourceGroups
                    .Define(resourceGroupName)
                    .WithRegion(region)
                    .WithTags(tags)
                    .CreateAsync(cancellationToken);

            return resourceGroup;
        }

        public async Task<ResourceProvisioningResult> EnsureDeleted(ResourceProvisioningParameters parameters)
        {
            await Delete(parameters.ResourceGroupName);
            var provisioningState = await GetProvisioningState(parameters.ResourceGroupName);
            var crudResult = ResourceProvisioningResultUtil.CreateFromProvisioningState(provisioningState);
            return crudResult;
        }

        async Task<IResourceGroup> GetResourceGroupAsync(string resourceGroupName, bool failIfNotFound = true)
        {
            try
            {
                var resource = await _azure.ResourceGroups.GetByNameAsync(resourceGroupName);
                return resource;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("could not be found"))
                {
                    if (failIfNotFound)
                    {
                        throw;
                    }
                    else
                    {
                        return null;
                    }
                }

                throw;
            }
        }

        public async Task<string> GetProvisioningState(string resourceGroupName)
        {
            var resource = await GetResourceGroupAsync(resourceGroupName, false);

            if (resource == null)
            {
                return null;
            }

            return resource.ProvisioningState;
        }

        public async Task Delete(string resourceGroupName, CancellationToken cancellation = default)
        {
            var resourceGroup = await GetResourceGroupAsync(resourceGroupName, false);

            if (resourceGroup == null)
            {
                //Allready deleted
                _logger.LogWarning($"Deleting resource group {resourceGroupName} failed because it was not found. Assuming allready deleted");
                return;
            }

            EnsureResourceIsManagedByThisIEnvironmentThrowIfNot(resourceGroupName, resourceGroup.Tags);

            await _azure.ResourceGroups.DeleteByNameAsync(resourceGroupName, cancellation);
        }

        public Task<string> GetProvisioningState(string resourceGroupName, string resourceName)
        {
            return GetProvisioningState(resourceGroupName);
        }

        public async Task<IDictionary<string, string>> GetTagsAsync(string resourceGroupName, string resourceName)
        {
            var rg = await GetResourceGroupAsync(resourceGroupName);
            return AzureResourceTagsFactory.TagReadOnlyDictionaryToDictionary(rg.Tags);
        }

        public async Task UpdateTagAsync(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag)
        {
            var rg = await GetResourceGroupAsync(resourceGroupName);

            //Ensure resource is is managed by this instance
            EnsureResourceIsManagedByThisIEnvironmentThrowIfNot(resourceGroupName, rg.Tags);

            _ = await rg.Update().WithoutTag(tag.Key).ApplyAsync();
            _ = await rg.Update().WithTag(tag.Key, tag.Value).ApplyAsync();
        }

        public Task<ResourceProvisioningResult> Update(ResourceProvisioningParameters parameters, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
