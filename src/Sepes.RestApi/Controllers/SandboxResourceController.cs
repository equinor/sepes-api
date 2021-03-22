using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class SandboxResourceController : ControllerBase
    {      
        readonly ISandboxResourceReadService _sandboxResourceReadService;
        readonly ISandboxResourceRetryService _sandboxResourceRetryService;

        public SandboxResourceController(ISandboxResourceReadService sandboxResourceReadService, ISandboxResourceRetryService sandboxResourceRetryService)
        {         
            _sandboxResourceReadService = sandboxResourceReadService;
            _sandboxResourceRetryService = sandboxResourceRetryService;
        }

        [HttpGet("sandboxes/{sandboxId}/resources")]
        public async Task<IActionResult> GetSandboxResources(int sandboxId)
        {
            var sandboxes = await _sandboxResourceReadService.GetSandboxResourcesLight(sandboxId);
            return new JsonResult(sandboxes);
        }

        [HttpPut("resources/{resourceId}/retry")]
        public async Task<IActionResult> RetryLastOperation(int resourceId)
        {
            var resource = await _sandboxResourceRetryService.RetryResourceFailedOperation(resourceId);
            return new JsonResult(resource);
        }       

        [HttpGet("sandboxes/{sandboxId}/costanalysis")]
        public async Task<IActionResult> GetSandboxCostanalysis(int sandboxId)
        {
            var sandboxes = await _sandboxResourceReadService.GetSandboxCostanlysis(sandboxId);
            return new JsonResult(sandboxes);
        }
    }
}
