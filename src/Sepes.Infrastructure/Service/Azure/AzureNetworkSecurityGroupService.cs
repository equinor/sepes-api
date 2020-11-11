using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.Network.Fluent.NetworkSecurityGroup.Update;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Dto.VirtualMachine;
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
    public class AzureNetworkSecurityGroupService : AzureServiceBase, IAzureNetworkSecurityGroupService
    {
        public AzureNetworkSecurityGroupService(IConfiguration config, ILogger<AzureNetworkSecurityGroupService> logger)
             : base(config, logger)
        {


        }

        public async Task<CloudResourceCRUDResult> EnsureCreated(CloudResourceCRUDInput parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Ensuring Network Security Group exists for sandbox with Name: {parameters.SandboxName}! Resource Group: {parameters.ResourceGroupName}");

            var nsg = await GetResourceAsync(parameters.ResourceGroupName, parameters.Name);

            if (nsg == null)
            {
                _logger.LogInformation($"Network Security Group not foundfor sandbox with Name: {parameters.SandboxName}! Resource Group: {parameters.ResourceGroupName}. Creating!");

                nsg = await Create(parameters.Region, parameters.ResourceGroupName, parameters.Name, parameters.Tags, cancellationToken);
            }

            var result = CreateResult(nsg);

            _logger.LogInformation($"Done ensuring Network Security Group exists for sandbox with Id: {parameters.SandboxName}! Id: {nsg.Id}");

            return result;
        }

        public async Task<CloudResourceCRUDResult> GetSharedVariables(CloudResourceCRUDInput parameters)
        {
            var nsg = await GetResourceAsync(parameters.ResourceGroupName, parameters.Name);

            var result = CreateResult(nsg);

            return result;
        }

        public async Task<CloudResourceCRUDResult> Delete(CloudResourceCRUDInput parameters)
        {
            await Delete(parameters.ResourceGroupName, parameters.Name);

            var provisioningState = await GetProvisioningState(parameters.ResourceGroupName, parameters.Name);
            var crudResult = CloudResourceCRUDUtil.CreateResultFromProvisioningState(provisioningState);
            return crudResult;
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

        public async Task<INetworkSecurityGroup> Create(Region region, string resourceGroupName, string nsgName, Dictionary<string, string> tags, CancellationToken cancellationToken = default)
        {
            var nsg = await _azure.NetworkSecurityGroups
                .Define(nsgName)
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroupName)
                .WithTags(tags)
                .CreateAsync(cancellationToken);
            return nsg;

            //Add rules obligatory to every pod. This will block AzureLoadBalancer from talking to the VMs inside sandbox
            // await this.NsgApplyBaseRules(nsg);
        }

        public async Task Delete(string resourceGroupName, string securityGroupName)
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

        //public async Task<Dictionary<string, NsgRuleDto>> GetNsgRulesByPrefix(string resourceGroupName, string nsgName, string withNamePrefix, CancellationToken cancellationToken = default)
        //{
        //    var nsg = await GetResourceAsync(resourceGroupName, nsgName);

        //    var result = new Dictionary<string, NsgRuleDto>();

        //    foreach (var curRuleKvp in nsg.SecurityRules)
        //    {
        //        if (curRuleKvp.Value.Name.StartsWith(withNamePrefix))
        //        {
        //            if (!result.ContainsKey(curRuleKvp.Key))
        //            {
        //                result.Add(curRuleKvp.Key, new NsgRuleDto());
        //            }
        //        }
        //    }

        //    return result;
        //}

        public async Task<Dictionary<string, NsgRuleDto>> GetNsgRulesForAddress(string resourceGroupName, string nsgName, string address, CancellationToken cancellationToken = default)
        {
            var nsg = await GetResourceAsync(resourceGroupName, nsgName);

            var result = new Dictionary<string, NsgRuleDto>();

            foreach (var curRuleKvp in nsg.SecurityRules)
            {
                if (
                    (curRuleKvp.Value.Direction == "Inbound" && curRuleKvp.Value.DestinationAddressPrefixes.Contains(address))
                    ||
                    (curRuleKvp.Value.Direction == "Outbound" && curRuleKvp.Value.SourceAddressPrefixes.Contains(address))
                    )
                {
                    if (!result.ContainsKey(curRuleKvp.Value.Name))
                    {
                        result.Add(curRuleKvp.Value.Name, new NsgRuleDto() { Key = curRuleKvp.Key, Name = curRuleKvp.Value.Name, Description = curRuleKvp.Value.Description, Protocol = curRuleKvp.Value.Protocol });
                    }
                }
            }

            return result;
        }

        public async Task AddInboundRule(string resourceGroupName, string securityGroupName,
                                      NsgRuleDto rule, CancellationToken cancellationToken = default)
        {
            var createOperation = _azure.NetworkSecurityGroups
                 .GetByResourceGroup(resourceGroupName, securityGroupName)
                 .Update()
                 .DefineRule(rule.Name);

            var operationWithRules = await (rule.Action == RuleAction.Allow ? createOperation.AllowInbound() : createOperation.DenyInbound())
            .FromAddresses(rule.SourceAddress)
            .FromAnyPort()
            .ToAddresses(rule.DestinationAddress)
            .ToPort(rule.DestinationPort)
            .WithAnyProtocol()
            .WithPriority(rule.Priority)
            .Attach()
            .ApplyAsync(cancellationToken);
        }

        public async Task UpdateInboundRule(string resourceGroupName, string securityGroupName,
                                 NsgRuleDto rule, CancellationToken cancellationToken = default)
        {
            var updateNsgOperation = _azure.NetworkSecurityGroups
                 .GetByResourceGroup(resourceGroupName, securityGroupName)
                 .Update();


            var updateRuleOp = updateNsgOperation
             .UpdateRule(rule.Name);
            //Decide of allow or deny
            (rule.Action == RuleAction.Allow ? updateRuleOp.AllowInbound() : updateRuleOp.DenyInbound())

                .FromAddresses(rule.SourceAddress)
            .FromAnyPort()
              .ToAddresses(rule.DestinationAddress)
           .ToPort(rule.DestinationPort)
           .WithAnyProtocol()
           .WithPriority(rule.Priority);

            await updateNsgOperation.ApplyAsync();
        }


        //public async Task UpdateInboundRule(string resourceGroupName, string securityGroupName,
        //                           NsgRuleDto rule, CancellationToken cancellationToken = default)
        //{
        //    var updateNsgOperation = _azure.NetworkSecurityGroups
        //         .GetByResourceGroup(resourceGroupName, securityGroupName)
        //         .Update();


        //     _ = updateNsgOperation
        //      .UpdateRule(rule.Name)
        //      .AllowInbound()
        //        .FromAddresses(rule.SourceAddress)
        //    .FromAnyPort()
        //      .ToAddresses(rule.DestinationAddress)
        //   .ToPort(rule.DestinationPort)
        //   .WithAnyProtocol()
        //   .WithPriority(rule.Priority);

        //    await updateNsgOperation.ApplyAsync();
        //}

        public async Task AddOutboundRule(string resourceGroupName, string securityGroupName,
                                    NsgRuleDto rule, CancellationToken cancellationToken = default)
        {
            var createOperation = await _azure.NetworkSecurityGroups
                 .GetByResourceGroup(resourceGroupName, securityGroupName)
                 .Update()
                 .DefineRule(rule.Name)
                 .AllowOutbound()
                 .FromAddresses(rule.SourceAddress)
                 .FromPort(rule.SourcePort)
                 .ToAddresses(rule.DestinationAddress)
                 .ToAnyPort()
                 .WithAnyProtocol()
                 .WithPriority(rule.Priority)
                 .Attach()
                 .ApplyAsync(cancellationToken);
        }


        public async Task UpdateOutboundRule(string resourceGroupName, string securityGroupName,
                                   NsgRuleDto rule, CancellationToken cancellationToken = default)
        {
            var updateNsgOperation = _azure.NetworkSecurityGroups
                 .GetByResourceGroup(resourceGroupName, securityGroupName)
                 .Update();


            _ = updateNsgOperation
             .UpdateRule(rule.Name)
             .AllowOutbound()
                 .FromAddresses(rule.SourceAddress)
                 .FromPort(rule.SourcePort)
                 .ToAddresses(rule.DestinationAddress)
                 .ToAnyPort()
                 .WithAnyProtocol()
                 .WithPriority(rule.Priority);

            await updateNsgOperation.ApplyAsync();
        }



        //public async Task UpdateRule(string resourceGroupName, string securityGroupName,
        //                           NsgRuleDto rule, CancellationToken cancellationToken = default)
        //{
        //    var updateOperation = _azure.NetworkSecurityGroups
        //         .GetByResourceGroup(resourceGroupName, securityGroupName)
        //         .Update()
        //         .UpdateRule(rule.Name)
        //         .AllowInbound()
        //         .FromAddresses(rule.SourceAddress)
        //          .FromPort(rule.SourcePort)
        //         .ToAnyAddress()
        //          .ToPort(rule.DestinationPort)
        //         .WithAnyProtocol()
        //         .WithPriority(rule.Priority);

        //}

        public async Task DeleteRule(string resourceGroupName, string securityGroupName,
                                string ruleName, CancellationToken cancellationToken = default)
        {
            var updatedNsg = await _azure.NetworkSecurityGroups
                 .GetByResourceGroup(resourceGroupName, securityGroupName)
                 .Update()
                 .WithoutRule(ruleName)
                .ApplyAsync();

        }

        public async Task NsgAllowInboundPort(string resourceGroupName, string securityGroupName,
                                              string ruleName,
                                              int priority,
                                              string[] internalAddresses,
                                              int toPort, CancellationToken cancellationToken = default)
        {
            await _azure.NetworkSecurityGroups
                .GetByResourceGroup(resourceGroupName, securityGroupName) //can be changed to get by ID
                .Update()
                .DefineRule(ruleName)//Maybe "AllowOutgoing" + portvariable
                .AllowInbound()
                .FromAddresses(internalAddresses)
                .FromAnyPort()
                .ToAnyAddress()
                .ToPort(toPort)
                .WithAnyProtocol()
                .WithPriority(priority)
                .Attach()
                .ApplyAsync();
        }

        public async Task NsgAllowOutboundPort(string resourceGroupName, string securityGroupName,
                                               string ruleName,
                                               int priority,
                                               string[] externalAddresses,
                                               int toPort, CancellationToken cancellationToken = default)
        {
            await _azure.NetworkSecurityGroups
                .GetByResourceGroup(resourceGroupName, securityGroupName) //can be changed to get by ID
                .Update()
                .DefineRule(ruleName)
                .AllowOutbound()
                .FromAnyAddress()
                .FromAnyPort()
                .ToAddresses(externalAddresses)
                .ToPort(toPort)
                .WithAnyProtocol()
                .WithPriority(priority)
                .Attach()
                .ApplyAsync();
        }


        public Task<CloudResourceCRUDResult> Update(CloudResourceCRUDInput parameters, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
