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
        readonly ISandboxService _sandboxService;

        public SandboxResourceController(ISandboxService sandboxService)
        {
            _sandboxService = sandboxService;
        }       

        [HttpGet("sandboxes/{sandboxId}/resources")]
        public async Task<IActionResult> GetSandboxResources(int sandboxId)
        {
            var sandboxes = await _sandboxService.GetSandboxResources(sandboxId);
            return new JsonResult(sandboxes);
        }

        [HttpPut("resources/{resourceId}/retry")]
        public async Task<IActionResult> RetryLastOperation(int resourceId)
        {
            var resource = await _sandboxService.RetryLastOperation(resourceId);
            return new JsonResult(resource);
        }     
    }
}
