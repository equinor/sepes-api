using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureNetworkSecurityGroupService : AzureServiceBase, IAzureNetworkSecurityGroupService
    {
        public AzureNetworkSecurityGroupService(IConfiguration config, ILogger<AzureNetworkSecurityGroupService> logger)
             : base(config, logger)
        {


        }

        public async Task<CloudResourceCRUDResult> Create(CloudResourceCRUDInput parameters)
        {
            _logger.LogInformation($"Creating Network Security Group for sandbox with Id: {parameters.SandboxName}! Resource Group: {parameters.ResourceGrupName}");

            var nsgName = AzureResourceNameUtil.NetworkSecGroupSubnet(parameters.SandboxName);
            var nsg = await CreateSecurityGroup(parameters.Region, parameters.ResourceGrupName, nsgName, parameters.Tags);
            var result = CreateResult(nsg);

            _logger.LogInformation($"Done creating Network Security Group for sandbox with Id: {parameters.SandboxName}! Id: {nsg.Id}");
            return result;
        }

        CloudResourceCRUDResult CreateResult(INetworkSecurityGroup nsg)
        {
            var crudResult = CloudResourceCRUDUtil.CreateResultFromIResource(nsg);
            crudResult.CurrentProvisioningState = nsg.Inner.ProvisioningState.ToString();
            crudResult.NewSharedVariables.Add(AzureCrudSharedVariable.NETWORK_SECURITY_GROUP_NAME, nsg.Name);
            return crudResult;
        }

        //public async Task<INetworkSecurityGroup> CreateSecurityGroupForSubnet(Region region, string resourceGroupName, string sandboxName, Dictionary<string, string> tags)
        //{
        //    var nsgName = AzureResourceNameUtil.NetworkSecGroupSubnet(sandboxName);
        //    return await CreateSecurityGroup(region, resourceGroupName, nsgName, tags);
        //}

        public async Task<INetworkSecurityGroup> CreateSecurityGroup(Region region, string resourceGroupName, string nsgName, Dictionary<string, string> tags)
        {
            var nsg = await _azure.NetworkSecurityGroups
                .Define(nsgName)
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroupName)
                .WithTags(tags)
                .CreateAsync();
            return nsg;

            //Add rules obligatory to every pod. This will block AzureLoadBalancer from talking to the VMs inside sandbox
            // await this.NsgApplyBaseRules(nsg);
        }

        public async Task DeleteSecurityGroup(string resourceGroupName, string securityGroupName)
        {
            await _azure.NetworkSecurityGroups.DeleteByResourceGroupAsync(resourceGroupName, securityGroupName);
        }

        public async Task<INetworkSecurityGroup> GetResourceAsync(string resourceGroupName, string resourceName)
        {
            var resource = await _azure.NetworkSecurityGroups.GetByResourceGroupAsync(resourceGroupName, resourceName);
            return resource;
        }

        public async Task<string> GetProvisioningState(string resourceGroupName, string resourceName)
        {
            var resource = await GetResourceAsync(resourceGroupName, resourceName);

            if (resource == null)
            {
                throw NotFoundException.CreateForAzureResource(resourceName, resourceGroupName);
            }

            return resource.Inner.ProvisioningState.ToString();
        }

        public async Task<IDictionary<string, string>> GetTagsAsync(string resourceGroupName, string resourceName)
        {
            var rg = await GetResourceAsync(resourceGroupName, resourceName);
            return AzureResourceTagsFactory.TagReadOnlyDictionaryToDictionary(rg.Tags);
        }

        public async Task UpdateTagAsync(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag)
        {
            var rg = await GetResourceAsync(resourceGroupName, resourceName);
            _ = await rg.UpdateTags().WithoutTag(tag.Key).ApplyTagsAsync();
            _ = await rg.UpdateTags().WithTag(tag.Key, tag.Value).ApplyTagsAsync();
        }
    }
}
