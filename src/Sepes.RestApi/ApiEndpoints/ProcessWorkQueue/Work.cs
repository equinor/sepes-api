using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sepes.Provisioning.Service.Interface;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.ProcessWorkQueue
{
    [Route("api/provisioningqueue")]
    [ApiController]
    [Authorize()]
    public class Work : ControllerBase
    {
        readonly IResourceProvisioningService _service;

        public Work(IResourceProvisioningService service)
        {
            _service = service;
        }

        [HttpGet("lookforwork")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Handle()
        {
            await _service.DequeueAndHandleWork();
            return new OkResult();
        }
    }
}
