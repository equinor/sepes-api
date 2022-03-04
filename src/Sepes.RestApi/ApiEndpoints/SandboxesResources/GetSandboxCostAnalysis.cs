using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.SandboxesResources
{
    [Route("api")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class GetSandboxCostAnalysis: ControllerBase
    {
        readonly ISandboxResourceReadService _sandboxResourceReadService;
        
        public GetSandboxCostAnalysis(ISandboxResourceReadService sandboxResourceReadService)
        {
            _sandboxResourceReadService = sandboxResourceReadService;
        }

        [HttpGet("sandboxes/{sandboxId}/costanalysis")]
        public async Task<IActionResult> Handle(int sandboxId)
        {
            var costAnalasysLink = await _sandboxResourceReadService.GetSandboxCostanlysis(sandboxId);
            return new JsonResult(costAnalasysLink);
        }
    }
}
