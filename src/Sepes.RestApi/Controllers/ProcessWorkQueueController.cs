using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Service.Interface;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controllers
{
    [Route("api/provisioningqueue")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class ProcessWorkQueueController : ControllerBase
    {
        readonly ISandboxResourceProvisioningService _service;

        public ProcessWorkQueueController(ISandboxResourceProvisioningService service)
        {
            _service = service;
        }

        [HttpGet("lookforwork")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Work()
        {
           await _service.DequeueWorkAndPerformIfAny();
            return new OkResult();
        }   

    }
}
