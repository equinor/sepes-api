using Sepes.Common.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IHasCorsRules
    {
        Task SetCorsRules(string resourceGroupName, string resourceName, List<CorsRule> rules, CancellationToken cancellationToken = default);    
    }
}
