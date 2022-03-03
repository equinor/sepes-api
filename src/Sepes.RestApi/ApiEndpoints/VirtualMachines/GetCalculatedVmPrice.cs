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
    public class GetCalculatedVmPrice : ControllerBase
    {
        readonly IVirtualMachineSizeService _virtualMachineSizeService;

        public GetCalculatedVmPrice(
            IVirtualMachineSizeService virtualMachineSizeService)
        {
            _virtualMachineSizeService = virtualMachineSizeService;
        }

        [HttpPost("{sandboxId}/calculatedVmprice")]
        public async Task<IActionResult> Handle(int sandboxId, CalculateVmPriceUserInputDto input)
        {
            var createdVm = await _virtualMachineSizeService.CalculateVmPrice(sandboxId, input);
            return new JsonResult(createdVm);
        }
    }
}
