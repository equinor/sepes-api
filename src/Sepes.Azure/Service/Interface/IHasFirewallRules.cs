using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public interface IHasFirewallRules
    {
        Task<List<FirewallRule>> SetFirewallRules(string resourceGroupName, string resourceName, List<FirewallRule> rules, CancellationToken cancellationToken = default);    
    }
}
