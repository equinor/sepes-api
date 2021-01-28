using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public interface IHasNetworkRules
    {
        Task<List<FirewallRule>> SetRules(string resourceGroupName, string resourceName, List<FirewallRule> rules, CancellationToken cancellationToken = default);    
    }
}
