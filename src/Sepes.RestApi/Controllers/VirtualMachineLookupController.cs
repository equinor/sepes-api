using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controllers
{
    [Route("api/virtualmachines")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class VirtualMachineLookupController : ControllerBase
    {       
        readonly IVirtualMachineSizeService _virtualMachineSizeService;
        readonly IVirtualMachineSizeImportService _virtualMachineSizeImportService;
        readonly IVirtualMachineDiskSizeService _virtualMachineDiskSizeService;
        readonly IVirtualMachineOperatingSystemService _virtualMachineOperatingSystemService;     

        public VirtualMachineLookupController(
            IVirtualMachineSizeService virtualMachineSizeService,
            IVirtualMachineSizeImportService virtualMachineSizeImportService,
            IVirtualMachineDiskSizeService virtualMachineDiskSizeService,
            IVirtualMachineOperatingSystemService virtualMachineOperatingSystemService)
        {          
            _virtualMachineSizeService = virtualMachineSizeService;
            _virtualMachineSizeImportService = virtualMachineSizeImportService;
            _virtualMachineDiskSizeService = virtualMachineDiskSizeService;
            _virtualMachineOperatingSystemService = virtualMachineOperatingSystemService; 
        }       

        [HttpPost("{sandboxId}/calculatedVmprice")]
        public async Task<IActionResult> GetCalculatedVmPrice(int sandboxId, CalculateVmPriceUserInputDto input)
        {
            var createdVm = await _virtualMachineSizeService.CalculateVmPrice(sandboxId, input);
            return new JsonResult(createdVm);
        }    

        [HttpGet("{sandboxId}/sizes")]
        public async Task<IActionResult> GetAvailableVmSizes(int sandboxId, CancellationToken cancellationToken = default)
        {
            var availableSizes = await _virtualMachineSizeService.AvailableSizes(sandboxId, cancellationToken: cancellationToken);
            return new JsonResult(availableSizes);
        }

        [HttpGet("updateVmSizeCache")]
        public async Task<IActionResult> UpdateVmSizeCache(CancellationToken cancellationToken = default)
        {
            await _virtualMachineSizeImportService.UpdateVmSizeCache(cancellationToken);
            return new NoContentResult();
        }

        [HttpGet("{sandboxId}/disks")]
        public async Task<IActionResult> GetAvailableDisks(int sandboxId, CancellationToken cancellationToken = default)
        {
            var availableSizes = await _virtualMachineDiskSizeService.AvailableDisks(sandboxId, cancellationToken);
            return new JsonResult(availableSizes);
        }

        [HttpGet("{sandboxId}/operatingsystems")]
        public async Task<IActionResult> GetAvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default)
        {
            var availableSizes = await _virtualMachineOperatingSystemService.AvailableOperatingSystems(sandboxId, cancellationToken);
            return new JsonResult(availableSizes);
        }    
    }
}
