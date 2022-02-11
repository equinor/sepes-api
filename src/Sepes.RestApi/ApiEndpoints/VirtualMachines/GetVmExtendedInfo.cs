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
    public class GetVmExtendedInfo : ControllerBase
    {
        readonly IVirtualMachineReadService _virtualMachineReadService;
        public GetVmExtendedInfo(IVirtualMachineReadService virtualMachineReadService)
        {
            _virtualMachineReadService = virtualMachineReadService;
        }

        [HttpGet("{vmId}/extended")]
        public async Task<IActionResult> Handle(int vmId, CancellationToken cancellationToken = default)
        {
            var virtualMachinesForSandbox = await _virtualMachineReadService.GetExtendedInfo(vmId, cancellationToken);
            return new JsonResult(virtualMachinesForSandbox);
        }
    }
}
