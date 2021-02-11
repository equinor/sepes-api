using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Response;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IHealthService
    {
        Task<HealthSummaryResponse> GetHealthSummaryAsync(HttpContext context, CancellationToken cancellation = default);
    }
}
