using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto.VirtualMachine;
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
        readonly IVirtualMachineSizeService _vmSizeService;
        readonly IVirtualMachineLookupService _vmLookupService;

        public VirtualMachineLookupController(IVirtualMachineSizeService vmSizeService, IVirtualMachineLookupService vmLookupService)
        {          
            _vmSizeService = vmSizeService;
            _vmLookupService = vmLookupService;
        }       

        [HttpPost("{sandboxId}/calculatedVmprice")]
        public async Task<IActionResult> GetCalculatedVmPrice(int sandboxId, CalculateVmPriceUserInputDto input)
        {
            var createdVm = await _vmSizeService.CalculateVmPrice(sandboxId, input);
            return new JsonResult(createdVm);
        }

        [HttpPost("validateUsername")]
        public IActionResult ValidateUsername(VmUsernameDto input)
        {
            var usernameValidationResult = _vmLookupService.CheckIfUsernameIsValidOrThrow(input);
            return new JsonResult(usernameValidationResult);

        }

        [HttpPost("calculateName")]
        public string CalculateName(VmCalculateNameDto input)
        {
            return _vmLookupService.CalculateName(input.studyName, input.sandboxName, input.userSuffix);
        }

        [HttpGet("{sandboxId}/sizes")]
        public async Task<IActionResult> GetAvailableVmSizes(int sandboxId, CancellationToken cancellationToken = default)
        {
            var availableSizes = await _vmSizeService.AvailableSizes(sandboxId, cancellationToken: cancellationToken);
            return new JsonResult(availableSizes);
        }

        [HttpGet("updateVmSizeCache")]
        public async Task<IActionResult> UpdateVmSizeCache(CancellationToken cancellationToken = default)
        {
            await _vmSizeService.UpdateVmSizeCache(cancellationToken);
            return new NoContentResult();
        }

        [HttpGet("disks/")]
        public async Task<IActionResult> GetAvailableDisks()
        {
            var availableSizes = await _vmLookupService.AvailableDisks();
            return new JsonResult(availableSizes);
        }

        [HttpGet("{sandboxId}/operatingsystems")]
        public async Task<IActionResult> GetAvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default)
        {
            var availableSizes = await _vmLookupService.AvailableOperatingSystems(sandboxId, cancellationToken);
            return new JsonResult(availableSizes);
        }    
    }
}
