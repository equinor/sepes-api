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
    public class GetAvailableOperatingSystems : ControllerBase
    {
        readonly IVirtualMachineOperatingSystemService _virtualMachineOperatingSystemService;

        public GetAvailableOperatingSystems(IVirtualMachineOperatingSystemService virtualMachineOperatingSystemService)
        {
            _virtualMachineOperatingSystemService = virtualMachineOperatingSystemService;
        }

        [HttpGet("{sandboxId}/operatingsystems")]
        public async Task<IActionResult> Handle(int sandboxId)
        {
            var availableSizes = await _virtualMachineOperatingSystemService.AvailableOperatingSystems(sandboxId);
            return new JsonResult(availableSizes);
        }
    }
}
