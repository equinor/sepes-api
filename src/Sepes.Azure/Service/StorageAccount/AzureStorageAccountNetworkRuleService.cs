using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Azure.Management.Storage.Fluent.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Action = Microsoft.Azure.Management.Storage.Fluent.Models.Action;

namespace Sepes.Azure.Service
{
    public class AzureStorageAccountNetworkRuleService : AzureStorageAccountBaseService, IAzureStorageAccountNetworkRuleService
    {
        public AzureStorageAccountNetworkRuleService(IConfiguration config, ILogger<AzureStorageAccountNetworkRuleService> logger)
            : base(config, logger)
        {

        }

        public async Task<List<FirewallRule>> SetFirewallRules(string resourceGroupName, string resourceName, List<FirewallRule> rules, CancellationToken cancellationToken = default)
        {
            var account = await GetResourceAsync(resourceGroupName, resourceName, cancellationToken: cancellationToken);

            var ruleSet = GetNetworkRuleSetForUpdate(account, false);
            ruleSet.IpRules = rules?.Select(alw => new IPRule(alw.Address, (Action)alw.Action)).ToList();
            var updateParameters = new StorageAccountUpdateParameters() { NetworkRuleSet = ruleSet };

            var updateResult = await _azure.StorageAccounts.Inner.UpdateAsync(resourceGroupName, resourceName, updateParameters, cancellationToken);

            return rules;
        }

        public async Task AddStorageAccountToVNet(string resourceGroupForStorageAccount, string storageAccountName, string resourceGroupForVnet, string vNetName, CancellationToken cancellation)
        {
            try
            {
                var storageAccount = await GetResourceAsync(resourceGroupForStorageAccount, storageAccountName, cancellationToken: cancellation);
                var network = await _azure.Networks.GetByResourceGroupAsync(resourceGroupForVnet, vNetName, cancellation);

                if (network == null)
                {
                    throw NotFoundException.CreateForAzureResource(vNetName, resourceGroupForVnet);
                }

                var sandboxSubnet = AzureVNetUtil.GetSandboxSubnetOrThrow(network);

                var networkRuleSet = GetNetworkRuleSetForUpdate(storageAccount, true);

                if (!GetRuleForSubnet(networkRuleSet, sandboxSubnet.Inner.Id, Microsoft.Azure.Management.Storage.Fluent.Models.Action.Allow, out VirtualNetworkRule existingRule))
                {
                    networkRuleSet.VirtualNetworkRules.Add(new VirtualNetworkRule()
                    {
                        Action = Microsoft.Azure.Management.Storage.Fluent.Models.Action.Allow,
                        VirtualNetworkResourceId = sandboxSubnet.Inner.Id
                    });

                    var updateParameters = new StorageAccountUpdateParameters() { NetworkRuleSet = networkRuleSet };

                   await _azure.StorageAccounts.Inner.UpdateAsync(resourceGroupForStorageAccount, storageAccountName, updateParameters, cancellation);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not add Storage Account {storageAccountName} to VNet {vNetName}: {ex.Message}", ex);
            }
        }

        public async Task RemoveStorageAccountFromVNet(string resourceGroupForStorageAccount, string storageAccountName, string resourceGroupForVnet, string vNetName, CancellationToken cancellation)
        {
            try
            {
                var storageAccount = await GetResourceAsync(resourceGroupForStorageAccount, storageAccountName, cancellationToken: cancellation);
                var network = await _azure.Networks.GetByResourceGroupAsync(resourceGroupForVnet, vNetName, cancellation);

                if (network == null)
                {
                    throw NotFoundException.CreateForAzureResource(vNetName, resourceGroupForVnet);
                }

                var sandboxSubnet = AzureVNetUtil.GetSandboxSubnetOrThrow(network);

                var networkRuleSet = GetNetworkRuleSetForUpdate(storageAccount, true);

                if (GetRuleForSubnet(networkRuleSet, sandboxSubnet.Inner.Id, Microsoft.Azure.Management.Storage.Fluent.Models.Action.Allow, out VirtualNetworkRule existingRule))
                {
                    networkRuleSet = RemoveVNetFromRuleSet(networkRuleSet, sandboxSubnet.Inner.Id);

                    var updateParameters = new StorageAccountUpdateParameters() { NetworkRuleSet = networkRuleSet };

                    await _azure.StorageAccounts.Inner.UpdateAsync(resourceGroupForStorageAccount, storageAccountName, updateParameters, cancellation);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not add Storage Account {storageAccountName} to VNet {vNetName}", ex);
            }
        }

        NetworkRuleSet GetNetworkRuleSetForUpdate(IStorageAccount storageAccount, bool createVirtualNetworkRules)
        {
            var networkRuleSet = storageAccount.Inner.NetworkRuleSet;

            if (networkRuleSet == null)
            {
                networkRuleSet = CreateNetworkRuleSet();
            }
            else
            {
                networkRuleSet.DefaultAction = DefaultAction.Deny;
            }

            if (networkRuleSet.VirtualNetworkRules == null)
            {
                if (createVirtualNetworkRules)
                {
                    networkRuleSet.VirtualNetworkRules = new List<VirtualNetworkRule>();
                }
            }
            else
            {
                foreach (var curExistingNetworkRule in networkRuleSet.VirtualNetworkRules.Where(r=> r.State == State.NetworkSourceDeleted).ToList())
                {
                    networkRuleSet.VirtualNetworkRules.Remove(curExistingNetworkRule);
                }
            }

            return networkRuleSet;
        }

        NetworkRuleSet CreateNetworkRuleSet()
        {
            return new NetworkRuleSet() { DefaultAction = DefaultAction.Deny };
        }

        NetworkRuleSet RemoveVNetFromRuleSet(NetworkRuleSet oldRuleSet, string subnetId)
        {
            var newRuleSet = new NetworkRuleSet
            {
                Bypass = oldRuleSet.Bypass,
                DefaultAction = oldRuleSet.DefaultAction,
                IpRules = oldRuleSet.IpRules
            };

            if (newRuleSet.VirtualNetworkRules == null)
            {
                newRuleSet.VirtualNetworkRules = new List<VirtualNetworkRule>();
            }

            foreach (var curVirtualNetworkRule in oldRuleSet.VirtualNetworkRules.Where(r=> r.VirtualNetworkResourceId != subnetId))
            {
                newRuleSet.VirtualNetworkRules.Add(curVirtualNetworkRule);
            }

            return newRuleSet;
        }

        bool GetRuleForSubnet(NetworkRuleSet networkRuleSet, string subnetId, Microsoft.Azure.Management.Storage.Fluent.Models.Action action, out VirtualNetworkRule virtualNetworkRule)
        {
            foreach (var curRule in networkRuleSet.VirtualNetworkRules.Where(r => r.VirtualNetworkResourceId == subnetId && r.Action == action))
            {
                virtualNetworkRule = curRule;
                return true;
            }

            virtualNetworkRule = null;
            return false;
        }
    }
}
