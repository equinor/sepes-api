using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.Health
{
    [Route("api/health")]
    [Authorize(Roles = AppRoles.Admin)]
    public class Get : EndpointBase
    {
        readonly IHealthService _healthService;

        public Get(IHealthService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet()]
        public async Task<IActionResult> Handle(CancellationToken cancellation = default)
        {
            var healthSummary = await _healthService.GetHealthSummaryAsync(HttpContext, cancellation);
            return new JsonResult(healthSummary);
        }
    }
}
