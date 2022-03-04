using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.Sandboxes
{
    [Route("api")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class MoveToNextPhase : ControllerBase
    {
        readonly ISandboxPhaseService _sandboxPhaseService;

        public MoveToNextPhase(ISandboxPhaseService sandboxPhaseService)
        {
            _sandboxPhaseService = sandboxPhaseService;
        }

        [HttpPost("sandboxes/{sandboxId}/nextPhase")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Handle(int sandboxId, CancellationToken cancellation = default)
        {
            var sandbox = await _sandboxPhaseService.MoveToNextPhaseAsync(sandboxId, cancellation);
            return new JsonResult(sandbox);
        }
    }
}
