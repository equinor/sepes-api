using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controllers
{
    [Route("api/provisioningqueue")]
    [ApiController]
    [Authorize()]
    public class ProcessWorkQueueController : ControllerBase
    {
        readonly IResourceProvisioningService _service;

        public ProcessWorkQueueController(IResourceProvisioningService service)
        {
            _service = service;
        }

        [HttpGet("lookforwork")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Authorize()]
        public async Task<IActionResult> Work()
        {
            await _service.DequeueAndHandleWork();
            return new OkResult();
        }
    }
}
