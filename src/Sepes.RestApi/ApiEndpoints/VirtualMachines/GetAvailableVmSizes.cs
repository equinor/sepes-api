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
    public class GetAvailableVmSizes : ControllerBase
    {
        readonly IVirtualMachineSizeService _virtualMachineSizeService;

        public GetAvailableVmSizes(IVirtualMachineSizeService virtualMachineSizeService)
        {
            _virtualMachineSizeService = virtualMachineSizeService;
        }

        [HttpGet("{sandboxId}/sizes")]
        public async Task<IActionResult> Handle(int sandboxId, CancellationToken cancellationToken = default)
        {
            var availableSizes = await _virtualMachineSizeService.AvailableSizes(sandboxId, cancellationToken: cancellationToken);
            return new JsonResult(availableSizes);
        }
    }
}
