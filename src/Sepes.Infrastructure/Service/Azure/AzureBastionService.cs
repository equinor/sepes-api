using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.Network.Fluent.Models;
using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureBastionService : AzureServiceBase, IAzureBastionService
    {
        public AzureBastionService(IConfiguration config, ILogger<AzureBastionService> logger) : base(config, logger)
        {

        }

        public async Task<CloudResourceCRUDResult> Create(CloudResourceCRUDInput parameters)
        {
            _logger.LogInformation($"Creating Bastion for sandbox with Name: {parameters.SandboxName}! Resource Group: {parameters.ResourceGrupName}");

            string subnetId = null;

            if (parameters.TryGetSharedVariable(AzureCrudSharedVariable.BASTION_SUBNET_ID, out subnetId) == false)
            {
                throw new ArgumentException("AzureBastionService: Missing Bastion subnet ID from input");
            }

            var bastionHost = await Create(parameters.Region, parameters.ResourceGrupName, parameters.StudyName, parameters.SandboxName, subnetId, parameters.Tags);
            var result = CreateResult(bastionHost);

            _logger.LogInformation($"Done creating Bastion for sandbox with Id: {parameters.SandboxName}! Bastion Id: {bastionHost.Id}");
            return result;
        }

        CloudResourceCRUDResult CreateResult(BastionHost bastion)
        {
            var crudResult = CloudResourceCRUDUtil.CreateResultFromIResource(bastion);
            crudResult.CurrentProvisioningState = bastion.ProvisioningState.ToString();
            return crudResult;
        }

        public async Task<CloudResourceCRUDResult> Delete(CloudResourceCRUDInput parameters)
        {
          await Delete(parameters.ResourceGrupName, parameters.Name);

            var provisioningState = await GetProvisioningState(parameters.ResourceGrupName, parameters.Name);
            var crudResult = CloudResourceCRUDUtil.CreateResultFromProvisioningState(provisioningState);
            return crudResult;
        }


        public async Task<BastionHost> Create(Region region, string resourceGroupName, string studyName, string sandboxName, string subnetId, Dictionary<string, string> tags)
        {
            var publicIpName = AzureResourceNameUtil.BastionPublicIp(studyName, sandboxName);

            var pip = await _azure.PublicIPAddresses.Define(publicIpName)
             .WithRegion(region)
             .WithExistingResourceGroup(resourceGroupName)
             .WithStaticIP()
             .WithSku(PublicIPSkuType.Standard)
             .WithTags(tags)
             .CreateAsync();

            using (var client = new Microsoft.Azure.Management.Network.NetworkManagementClient(_credentials))
            {
                client.SubscriptionId = _subscriptionId;

                var bastionName = AzureResourceNameUtil.Bastion(studyName, sandboxName);

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

                var createdBastion = await client.BastionHosts.CreateOrUpdateAsync(resourceGroupName, bastionName, bastion);

                return createdBastion;
            }
        }

        public async Task Delete(string resourceGroupName, string bastionHostName)
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
            using (var client = new Microsoft.Azure.Management.Network.NetworkManagementClient(_credentials))
            {
                client.SubscriptionId = _subscriptionId;
                var bastion = await client.BastionHosts.GetAsync(resourceGroupName, bastionHostName);
                return bastion;
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

       
    }

}

