using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
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
        readonly ICloudResourceService _sandboxResourceService;
        readonly ISandboxCloudResourceService _sandboxCloudResourceService;

        public SandboxResourceController(ICloudResourceService sandboxResourceService, ISandboxCloudResourceService sandboxCloudResourceService)
        {
            _sandboxResourceService = sandboxResourceService;
            _sandboxCloudResourceService = sandboxCloudResourceService;
        }       

        [HttpGet("sandboxes/{sandboxId}/resources")]
        public async Task<IActionResult> GetSandboxResources(int sandboxId)
        {
            var sandboxes = await _sandboxResourceService.GetSandboxResourcesLight(sandboxId);
            return new JsonResult(sandboxes);
        }

        [HttpPut("resources/{resourceId}/retry")]
        public async Task<IActionResult> RetryLastOperation(int resourceId)
        {
            var resource = await _sandboxCloudResourceService.RetryLastOperation(resourceId);
            return new JsonResult(resource);
        }

        [HttpPut("sandboxes/{sandboxId}/retryCreate")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> ReScheduleCreation(int sandboxId)
        {
            await _sandboxCloudResourceService.ReScheduleSandboxResourceCreation(sandboxId);
            return new NoContentResult();
        }
    }
}
