using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.DataModelService.Interface;
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
    public class GetAvailableDisks : ControllerBase
    {
        readonly IVirtualMachineDiskSizeService _virtualMachineDiskSizeService;

        public GetAvailableDisks(IVirtualMachineDiskSizeService virtualMachineDiskSizeService)
        {
            _virtualMachineDiskSizeService = virtualMachineDiskSizeService;
        }

        [HttpGet("{sandboxId}/disks")]
        public async Task<IActionResult> Handle(int sandboxId, CancellationToken cancellationToken = default)
        {
            var availableSizes = await _virtualMachineDiskSizeService.AvailableDisks(sandboxId, cancellationToken);
            return new JsonResult(availableSizes);
        }
    }
}
