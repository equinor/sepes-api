using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Dto.Provisioning;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureNetworkSecurityGroupService : AzureServiceBase, IAzureNetworkSecurityGroupService
    {
        public AzureNetworkSecurityGroupService(IConfiguration config, ILogger<AzureNetworkSecurityGroupService> logger)
             : base(config, logger)
        {


        }

        public async Task<ResourceProvisioningResult> EnsureCreated(ResourceProvisioningParameters parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Ensuring Network Security Group exists for sandbox with Name: {parameters.SandboxName}! Resource Group: {parameters.ResourceGroupName}");
          
            var nsg = await GetResourceAsync(parameters.ResourceGroupName, parameters.Name);

            if (nsg == null)
            {
                _logger.LogInformation($"Network Security Group not foundfor sandbox with Name: {parameters.SandboxName}! Resource Group: {parameters.ResourceGroupName}. Creating!");

                nsg = await CreateInternal(parameters.Region, parameters.ResourceGroupName, parameters.Name, parameters.Tags, cancellationToken);
            }

            var result = CreateResult(nsg);

            _logger.LogInformation($"Done ensuring Network Security Group exists for sandbox with Id: {parameters.SandboxName}! Id: {nsg.Id}");

            return result;
        }

        public async Task<ResourceProvisioningResult> GetSharedVariables(ResourceProvisioningParameters parameters)
        {
            var nsg = await GetResourceAsync(parameters.ResourceGroupName, parameters.Name);

            var result = CreateResult(nsg);

            return result;
        }

        public async Task<ResourceProvisioningResult> Delete(ResourceProvisioningParameters parameters)
        {
            await DeleteInternal(parameters.ResourceGroupName, parameters.Name);

            var provisioningState = await GetProvisioningState(parameters.ResourceGroupName, parameters.Name);
            var crudResult = ResourceProvisioningResultUtil.CreateResultFromProvisioningState(provisioningState);
            return crudResult;
        }

        ResourceProvisioningResult CreateResult(INetworkSecurityGroup nsg)
        {
            var crudResult = ResourceProvisioningResultUtil.CreateResultFromIResource(nsg);
            crudResult.CurrentProvisioningState = nsg.Inner.ProvisioningState.ToString();
            crudResult.NewSharedVariables.Add(AzureCrudSharedVariable.NETWORK_SECURITY_GROUP_NAME, nsg.Name);
            return crudResult;
        }

        async Task<INetworkSecurityGroup> CreateInternal(Region region, string resourceGroupName, string nsgName, Dictionary<string, string> tags, CancellationToken cancellationToken = default)
        {
            var nsg = await _azure.NetworkSecurityGroups
                .Define(nsgName)
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroupName)
                .WithTags(tags)
                .CreateAsync(cancellationToken);


            await NsgApplyBaseRules(nsg);

            return nsg;
        }

        async Task NsgApplyBaseRules(INetworkSecurityGroup nsg)
        {
            await nsg.Update()
            .DefineRule(AzureVmConstants.RulePresets.ALLOW_FOR_SERVICETAG_VNET)
            .AllowOutbound()
            .FromAnyAddress()
            .FromAnyPort()
            .ToAddress("VirtualNetwork")
            .ToAnyPort()
            .WithAnyProtocol()
            .WithPriority(AzureVmConstants.RulePresets.ALLOW_FOR_SERVICETAG_VNET_PRIORITY)
            .Attach()
            .ApplyAsync();
        }

        async Task DeleteInternal(string resourceGroupName, string securityGroupName)
        {
            var resource = await GetResourceAsync(resourceGroupName, securityGroupName);

            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, resource.Tags);

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
            var resource = await GetResourceAsync(resourceGroupName, resourceName);

            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, resource.Tags);

            _ = await resource.UpdateTags().WithoutTag(tag.Key).ApplyTagsAsync();
            _ = await resource.UpdateTags().WithTag(tag.Key, tag.Value).ApplyTagsAsync();
        }        

        public async Task<Dictionary<string, NsgRuleDto>> GetNsgRulesContainingName(string resourceGroupName, string nsgName, string nameContains, CancellationToken cancellationToken = default)
        {
            var nsg = await GetResourceAsync(resourceGroupName, nsgName);

            var result = new Dictionary<string, NsgRuleDto>();

            foreach (var curRuleKvp in nsg.SecurityRules)
            {
                if (curRuleKvp.Value.Name.Contains(nameContains))
                {
                    if (!result.ContainsKey(curRuleKvp.Value.Name))
                    {
                        result.Add(curRuleKvp.Value.Name, new NsgRuleDto() { Key = curRuleKvp.Key, Name = curRuleKvp.Value.Name, Description = curRuleKvp.Value.Description, Protocol = curRuleKvp.Value.Protocol });
                    }
                }
            }

            return result;
        }  
        
        public Task<ResourceProvisioningResult> Update(ResourceProvisioningParameters parameters, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
