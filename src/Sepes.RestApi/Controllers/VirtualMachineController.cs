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
    public class VirtualMachineController : ControllerBase
    {
        readonly IVirtualMachineCreateService _virtualMachineCreateService;
        readonly IVirtualMachineReadService _virtualMachineReadService;
        readonly IVirtualMachineDeleteService _virtualMachineDeleteService;
        readonly IVirtualMachineValidationService _virtualMachineValidationService;

        public VirtualMachineController(
            IVirtualMachineCreateService virtualMachineCreateService,
            IVirtualMachineReadService virtualMachineReadService,
            IVirtualMachineDeleteService virtualMachineDeleteService,
            IVirtualMachineValidationService virtualMachineValidationService)
        {
            _virtualMachineCreateService = virtualMachineCreateService;
            _virtualMachineReadService = virtualMachineReadService;
            _virtualMachineDeleteService = virtualMachineDeleteService;
            _virtualMachineValidationService = virtualMachineValidationService;
        }

        [HttpPost("{sandboxId}")]
        public async Task<IActionResult> CreateAsync(int sandboxId, VirtualMachineCreateDto newVm)
        {
            var createdVm = await _virtualMachineCreateService.CreateAsync(sandboxId, newVm);
            return new JsonResult(createdVm);
        }

        [HttpDelete("{vmId}")]
        public async Task<IActionResult> DeleteAsync(int vmId)
        {
            await _virtualMachineDeleteService.DeleteAsync(vmId);
            return new NoContentResult();
        }

        [HttpGet("forsandbox/{sandboxId}")]
        public async Task<IActionResult> GetAllVMsForSandbox(int sandboxId, CancellationToken cancellationToken = default)
        {
            var virtualMachinesForSandbox = await _virtualMachineReadService.VirtualMachinesForSandboxAsync(sandboxId, cancellationToken);
            return new JsonResult(virtualMachinesForSandbox);
        }

        [HttpGet("{vmId}/extended")]
        public async Task<IActionResult> GetVmExtendedInfo(int vmId, CancellationToken cancellationToken = default)
        {
            var virtualMachinesForSandbox = await _virtualMachineReadService.GetExtendedInfo(vmId);
            return new JsonResult(virtualMachinesForSandbox);
        }

        [HttpGet("{vmId}/externalLink")]
        public async Task<IActionResult> GetVmExternalLink(int vmId, CancellationToken cancellationToken = default)
        {
            var virtualMachinesForSandbox = await _virtualMachineReadService.GetExternalLink(vmId);
            return new JsonResult(virtualMachinesForSandbox);
        }

        [HttpPost("validateUsername")]
        public IActionResult ValidateUsername(VmUsernameDto input)
        {
            var usernameValidationResult = _virtualMachineValidationService.CheckIfUsernameIsValidOrThrow(input);
            return new JsonResult(usernameValidationResult);

        }

        [HttpPost("calculateName")]
        public string CalculateName(VmCalculateNameDto input)
        {
            return _virtualMachineValidationService.CalculateName(input.studyName, input.sandboxName, input.userSuffix);
        }
    }
}
