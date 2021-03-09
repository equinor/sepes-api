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
        readonly IVirtualMachineService _vmService;

        public VirtualMachineController(IVirtualMachineService vmService)
        {
            _vmService = vmService;
        }

        [HttpPost("{sandboxId}")]
        public async Task<IActionResult> CreateAsync(int sandboxId, VirtualMachineCreateDto newVm)
        {
            var createdVm = await _vmService.CreateAsync(sandboxId, newVm);
            return new JsonResult(createdVm);
        }

        [HttpDelete("{vmId}")]
        public async Task<IActionResult> DeleteAsync(int vmId)
        {
            await _vmService.DeleteAsync(vmId);
            return new NoContentResult();
        }

        [HttpGet("forsandbox/{sandboxId}")]
        public async Task<IActionResult> GetAllVMsForSandbox(int sandboxId, CancellationToken cancellationToken = default)
        {
            var virtualMachinesForSandbox = await _vmService.VirtualMachinesForSandboxAsync(sandboxId, cancellationToken);
            return new JsonResult(virtualMachinesForSandbox);
        }

        [HttpGet("{vmId}/extended")]
        public async Task<IActionResult> GetVmExtendedInfo(int vmId, CancellationToken cancellationToken = default)
        {
            var virtualMachinesForSandbox = await _vmService.GetExtendedInfo(vmId);
            return new JsonResult(virtualMachinesForSandbox);
        }

        [HttpGet("{vmId}/externalLink")]
        public async Task<IActionResult> GetVmExternalLink(int vmId, CancellationToken cancellationToken = default)
        {
            var virtualMachinesForSandbox = await _vmService.GetExternalLink(vmId);
            return new JsonResult(virtualMachinesForSandbox);
        }

        [HttpPost("validateUsername")]
        public IActionResult ValidateUsername(VmUsernameDto input)
        {
            var usernameValidationResult = _vmService.CheckIfUsernameIsValidOrThrow(input);
            return new JsonResult(usernameValidationResult);

        }

        [HttpPost("calculateName")]
        public string CalculateName(VmCalculateNameDto input)
        {
            return _vmService.CalculateName(input.studyName, input.sandboxName, input.userSuffix);
        }
    }
}
