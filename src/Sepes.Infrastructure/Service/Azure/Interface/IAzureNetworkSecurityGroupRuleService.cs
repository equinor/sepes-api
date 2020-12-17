using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureNetworkSecurityGroupRuleService
    {
        Task AddInboundRule(string resourceGroupName, string securityGroupName, NsgRuleDto rule, CancellationToken cancellationToken = default);

        Task UpdateInboundRule(string resourceGroupName, string securityGroupName, NsgRuleDto rule, CancellationToken cancellationToken = default);

        Task AddOutboundRule(string resourceGroupName, string securityGroupName, NsgRuleDto rule, CancellationToken cancellationToken = default);

        Task UpdateOutboundRule(string resourceGroupName, string securityGroupName, NsgRuleDto rule, CancellationToken cancellationToken = default);
        Task DeleteRule(string resourceGroupName, string securityGroupName, string ruleName, CancellationToken cancellationToken = default);
        Task<Dictionary<string, NsgRuleDto>> GetNsgRulesContainingName(string resourceGroupName, string nsgName, string nameContains, CancellationToken cancellationToken = default);

        Task<Dictionary<string, NsgRuleDto>> GetNsgRulesForDirection(string resourceGroupName, string nsgName, string direction, CancellationToken cancellationToken = default);
        Task<bool> IsRuleSetTo(string resourceGroupName, string nsgName, string ruleName, RuleAction action, CancellationToken cancellationToken = default);
    }
}