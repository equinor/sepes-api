using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Interface;
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
        public async Task<IActionResult> CreateAsync(int sandboxId, CreateVmUserInputDto newVm)
        {
            var createdVm = await _vmService.CreateAsync(sandboxId, newVm);
            return new JsonResult(createdVm);
        }

        //[HttpPut("{vmId}")]
        //public async Task<IActionResult> UpdateAsync(int vmId, NewVmDto upadatedVm)
        //{
        //    var createdVm = await _vmService.UpdateAsync(sandboxId, upadatedVm);
        //    return new JsonResult(createdVm);
        //}

        [HttpDelete("{vmId}")]
        public async Task<IActionResult> DeleteAsync(int vmId)
        {
            var deleted = await _vmService.DeleteAsync(vmId);
            return new JsonResult(deleted);
        }

        [HttpGet("calculateName/{studyName}/{sandboxName}/{userSuffix}")]
        public string CalculateName(string studyName, string sandboxName, string userSuffix)
        {
            return _vmService.CalculateName(studyName, sandboxName, userSuffix);
        }

        [HttpGet("forsandbox/{sandboxId}")]
        public async Task<IActionResult> GetAllVMsForSandbox(int sandboxId)
        {
            var virtualMachinesForSandbox = await _vmService.VirtualMachinesForSandboxAsync(sandboxId);
            return new JsonResult(virtualMachinesForSandbox);
        }

        [HttpGet("sizes/")]
        public async Task<IActionResult> GetAvailableVmSizes()
        {
            var availableSizes = await _vmService.AvailableSizes();
            return new JsonResult(availableSizes);
        }

        [HttpGet("disks/")]
        public async Task<IActionResult> GetAvailableDisks()
        {
            var availableSizes = await _vmService.AvailableDisks();
            return new JsonResult(availableSizes);
        }

        [HttpGet("operatingsystems/")]
        public async Task<IActionResult> GetAvailableOperatingSystems()
        {
            var availableSizes = await _vmService.AvailableOperatingSystems();
            return new JsonResult(availableSizes);
        }
    }
}
