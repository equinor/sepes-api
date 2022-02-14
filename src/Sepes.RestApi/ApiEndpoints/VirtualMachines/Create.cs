using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.RestApi.ApiEndpoints.VirtualMachines
{
    [Route("api/virtualmachines")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Create : ControllerBase
    {
        readonly IVirtualMachineCreateService _virtualMachineCreateService;
        public Create(IVirtualMachineCreateService virtualMachineCreateService)
        {
            _virtualMachineCreateService = virtualMachineCreateService;
        }

        [HttpPost("{sandboxId}")]
        public async Task<IActionResult> Handle(int sandboxId, VirtualMachineCreateDto newVm)
        {
            var createdVm = await _virtualMachineCreateService.CreateAsync(sandboxId, newVm);
            return new JsonResult(createdVm);
        }
    }
}
