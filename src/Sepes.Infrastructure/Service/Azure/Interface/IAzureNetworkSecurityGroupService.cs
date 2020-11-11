using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto.Azure;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureNetworkSecurityGroupService : IHasProvisioningState, IHasTags, IPerformCloudResourceCRUD
    {
     
        Task<INetworkSecurityGroup> Create(Region region, string resourceGroupName, string nsgName, Dictionary<string, string> tags, CancellationToken cancellationToken = default(CancellationToken));
        
        Task Delete(string resourceGroupName, string securityGroupName);
      
        //Task<Dictionary<string, NsgRuleDto>> GetNsgRulesByPrefix(string resourceGroupName, string nsgName, string withNamePrefix, CancellationToken cancellationToken = default);

        Task<Dictionary<string, NsgRuleDto>> GetNsgRulesForAddress(string resourceGroupName, string nsgName, string address, CancellationToken cancellationToken = default);      

        Task AddInboundRule(string resourceGroupName, string securityGroupName, NsgRuleDto rule, CancellationToken cancellationToken = default);

        Task UpdateInboundRule(string resourceGroupName, string securityGroupName, NsgRuleDto rule, CancellationToken cancellationToken = default);

        Task AddOutboundRule(string resourceGroupName, string securityGroupName, NsgRuleDto rule, CancellationToken cancellationToken = default);

        Task UpdateOutboundRule(string resourceGroupName, string securityGroupName, NsgRuleDto rule, CancellationToken cancellationToken = default);
        Task DeleteRule(string resourceGroupName, string securityGroupName, string ruleName, CancellationToken cancellationToken = default);

        //Task UpdateRule(string resourceGroupName, string securityGroupName, NsgRuleDto rule, CancellationToken cancellationToken = default);

        //Task DeleteRule(string resourceGroupName, string securityGroupName, NsgRuleDto rule, CancellationToken cancellationToken = default);
    }
}