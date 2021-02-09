using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public interface IHasCorsRules
    {
        Task SetCorsRules(string resourceGroupName, string resourceName, List<CorsRule> rules, CancellationToken cancellationToken = default);    
    }
}
