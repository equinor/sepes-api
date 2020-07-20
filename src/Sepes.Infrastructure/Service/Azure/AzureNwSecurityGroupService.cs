using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureNwSecurityGroupService : AzureServiceBase, IAzureNwSecurityGroupService
    {


        public AzureNwSecurityGroupService(IConfiguration config, ILogger logger)
             : base(config, logger)
        {


        }



        public async Task<INetworkSecurityGroup> CreateSecurityGroupForSubnet(Region region, string resourceGroupName, string sandboxName, Dictionary<string, string> tags)
        {
            var nsgName = AzureResourceNameUtil.NetworkSecGroupSubnet(sandboxName);

            return await CreateSecurityGroup(region, resourceGroupName, nsgName, tags);
        }

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

        public async Task<bool> Exists(string resourceGroupName, string nsgName)
        {
            var nsg = await _azure.Networks.GetByResourceGroupAsync(resourceGroupName, nsgName);

            if (nsg == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(nsg.Id);
        }
    }
}
