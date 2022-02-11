using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.VirtualMachines
{
    [Route("api/virtualmachines")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class GetBySandboxId : ControllerBase
    {
        readonly IVirtualMachineReadService _virtualMachineReadService;
        public GetBySandboxId(IVirtualMachineReadService virtualMachineReadService)
        {
            _virtualMachineReadService = virtualMachineReadService;
        }

        [HttpGet("forsandbox/{sandboxId}")]
        public async Task<IActionResult> Handle(int sandboxId, CancellationToken cancellationToken = default)
        {
            var virtualMachinesForSandbox = await _virtualMachineReadService.VirtualMachinesForSandboxAsync(sandboxId, cancellationToken);
            return new JsonResult(virtualMachinesForSandbox);
        }
    }
}
