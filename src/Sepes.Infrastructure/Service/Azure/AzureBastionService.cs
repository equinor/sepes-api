using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.Network.Fluent.Models;
using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Provisioning;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureBastionService : AzureServiceBase, IAzureBastionService
    {
        public AzureBastionService(IConfiguration config, ILogger<AzureBastionService> logger) : base(config, logger)
        {

        }

        public async Task<ResourceProvisioningResult> EnsureCreated(ResourceProvisioningParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Creating Bastion for sandbox with Name: {parameters.SandboxName}! Resource Group: {parameters.ResourceGroupName}");

            if (!parameters.TryGetSharedVariable(AzureCrudSharedVariable.BASTION_SUBNET_ID, out string subnetId))
            {
                throw new ArgumentException("AzureBastionService: Missing Bastion subnet ID from input");
            }

            var bastionHost = await GetResourceInternalAsync(parameters.ResourceGroupName, parameters.Name, false);

            if (bastionHost == null)
            {
                bastionHost = await CreateInternal(parameters.Region, parameters.ResourceGroupName, parameters.Name, subnetId, parameters.Tags, cancellationToken);
                _logger.LogInformation($"Done creating Bastion for sandbox with Id: {parameters.SandboxName}! Bastion Id: {bastionHost.Id}");
            }
            else
            {
                _logger.LogInformation($"Ensure bastion exist for Sandbox: {parameters.SandboxName}! Bastion allready existed. Bastion Id: {bastionHost.Id}");
            }
              
            var result = CreateResult(bastionHost);
           
            return result;
        }

        public async Task<ResourceProvisioningResult> GetSharedVariables(ResourceProvisioningParameters parameters)
        {
            var bastion = await GetResourceAsync(parameters.ResourceGroupName, parameters.Name);
            var result = CreateResult(bastion);
            return result;
        }

        ResourceProvisioningResult CreateResult(BastionHost bastion)
        {
            var crudResult = ResourceProvisioningResultUtil.CreateResultFromIResource(bastion);
            crudResult.CurrentProvisioningState = bastion.ProvisioningState.ToString();
            return crudResult;
        }

        public async Task<ResourceProvisioningResult> Delete(ResourceProvisioningParameters parameters)
        {
            await DeleteInternal(parameters.ResourceGroupName, parameters.Name);

            var provisioningState = await GetProvisioningState(parameters.ResourceGroupName, parameters.Name);
            var crudResult = ResourceProvisioningResultUtil.CreateResultFromProvisioningState(provisioningState);
            return crudResult;
        }


        async Task<BastionHost> CreateInternal(Region region, string resourceGroupName, string bastionName, string subnetId, Dictionary<string, string> tags, CancellationToken cancellationToken = default)
        {
            var publicIpName = AzureResourceNameUtil.BastionPublicIp(bastionName);

            var pip = await _azure.PublicIPAddresses.Define(publicIpName)
             .WithRegion(region)
             .WithExistingResourceGroup(resourceGroupName)
             .WithStaticIP()
             .WithSku(PublicIPSkuType.Standard)
             .WithTags(tags)
             .CreateAsync(cancellationToken);

            using (var client = new Microsoft.Azure.Management.Network.NetworkManagementClient(_credentials))
            {
                client.SubscriptionId = _subscriptionId;

                var ipConfigs = new List<BastionHostIPConfiguration> { new BastionHostIPConfiguration()
                        {
                            Name = $"{bastionName}-ip-config",
                            Subnet =  new SubResource(subnetId),
                            PrivateIPAllocationMethod = "Dynamic",
                            PublicIPAddress = new SubResource(pip.Inner.Id),
                        }
                    };

                var bastion = new BastionHost()
                {
                    Location = region.Name,
                    IpConfigurations = ipConfigs,
                    Tags = tags
                };

                var createdBastion = await client.BastionHosts.CreateOrUpdateAsync(resourceGroupName, bastionName, bastion, cancellationToken);

                return createdBastion;
            }
        }

        async Task DeleteInternal(string resourceGroupName, string bastionHostName)
        {
            using (var client = new Microsoft.Azure.Management.Network.NetworkManagementClient(_credentials))
            {
                client.SubscriptionId = _subscriptionId;

                var bastion = await client.BastionHosts.GetAsync(resourceGroupName, bastionHostName);

                //Ensure resource is is managed by this instance
                CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, bastion.Tags);

                await client.BastionHosts.DeleteAsync(resourceGroupName, bastionHostName);
            }
        }

        public async Task<BastionHost> GetResourceAsync(string resourceGroupName, string bastionHostName)
        {
            return await GetResourceInternalAsync(resourceGroupName, bastionHostName);
        }

        async Task<BastionHost> GetResourceInternalAsync(string resourceGroupName, string bastionHostName, bool failOnNotFound = true)
        {
            try
            {
                using (var client = new Microsoft.Azure.Management.Network.NetworkManagementClient(_credentials))
                {
                    client.SubscriptionId = _subscriptionId;
                    var bastion = await client.BastionHosts.GetAsync(resourceGroupName, bastionHostName);
                    return bastion;
                }
            }
            catch (Exception)
            {

                if (failOnNotFound)
                {
                    throw;
                }

                return null;
            }           
        }


        public async Task<string> GetProvisioningState(string resourceGroupName, string bastionHostName)
        {
            var bastion = await GetResourceAsync(resourceGroupName, bastionHostName);

            if (bastion == null)
            {
                throw NotFoundException.CreateForAzureResource(bastionHostName, resourceGroupName);
            }

            return bastion.ProvisioningState;
        }

        public async Task<IDictionary<string, string>> GetTagsAsync(string resourceGroupName, string resourceName)
        {
            var rg = await GetResourceAsync(resourceGroupName, resourceName);
            return rg.Tags;
        }

        public async Task UpdateTagAsync(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag)
        {
            var resource = await GetResourceAsync(resourceGroupName, resourceName);
            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, resource.Tags);
            // TODO: A bit unsure if this actually updates azure resource...
            resource.Tags[tag.Key] = tag.Value;

        }

        public Task<ResourceProvisioningResult> Update(ResourceProvisioningParameters parameters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

}

