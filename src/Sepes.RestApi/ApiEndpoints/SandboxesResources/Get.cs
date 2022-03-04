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
    public class Get : ControllerBase
    {
        readonly ISandboxResourceReadService _sandboxResourceReadService;
        public Get(ISandboxResourceReadService sandboxResourceReadService)
        {
            _sandboxResourceReadService = sandboxResourceReadService;
        }

        [HttpGet("sandboxes/{sandboxId}/resources")]
        public async Task<IActionResult> Handle(int sandboxId)
        {
            var resources = await _sandboxResourceReadService.GetSandboxResourcesLight(sandboxId);
            return new JsonResult(resources);
        }
    }
}
