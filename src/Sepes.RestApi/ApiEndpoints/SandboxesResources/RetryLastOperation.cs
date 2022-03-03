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
    public class RetryLastOperation : ControllerBase
    {
        readonly ISandboxResourceRetryService _sandboxResourceRetryService;

        public RetryLastOperation(ISandboxResourceRetryService sandboxResourceRetryService)
        {
            _sandboxResourceRetryService = sandboxResourceRetryService;
        }

        [HttpPut("resources/{resourceId}/retry")]
        public async Task<IActionResult> Handle(int resourceId)
        {
            var retriedResource = await _sandboxResourceRetryService.RetryResourceFailedOperation(resourceId);
            return new JsonResult(retriedResource);
        }
    }
}
