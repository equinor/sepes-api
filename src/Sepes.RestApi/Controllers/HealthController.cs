using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controllers
{
    [Route("api/health")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize(Roles = AppRoles.Admin)]
    public class HealthController : ControllerBase
    {
       readonly IHealthService _healthService;

        public HealthController(IHealthService healthService)
        {
            _healthService = healthService;
        }        

        [HttpGet()]
        public async Task<IActionResult> GetHealthSummary(CancellationToken cancellation = default)
        {
            var healthSummary = await _healthService.GetHealthSummaryAsync(HttpContext, cancellation);
            return new JsonResult(healthSummary);
        }
    }
}
