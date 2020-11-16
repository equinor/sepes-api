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
        readonly IVirtualMachineLookupService _vmLookupService;
        public VirtualMachineController(IVirtualMachineService vmService, IVirtualMachineLookupService vmLookupService)
        {
            _vmService = vmService;
            _vmLookupService = vmLookupService;
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

        [HttpGet("forsandbox/{sandboxId}")]
        public async Task<IActionResult> GetAllVMsForSandbox(int sandboxId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var virtualMachinesForSandbox = await _vmService.VirtualMachinesForSandboxAsync(sandboxId, cancellationToken);
            return new JsonResult(virtualMachinesForSandbox);
        }

        [HttpGet("{vmId}/extended")]
        public async Task<IActionResult> GetVmExtendedInfo(int vmId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var virtualMachinesForSandbox = await _vmService.GetExtendedInfo(vmId);
            return new JsonResult(virtualMachinesForSandbox);
        }

        //Lookup endpoints


        [HttpPost("{sandboxId}/calculatedprice")]
        public async Task<IActionResult> GetCalculatedPrice(int sandboxId, CalculateVmPriceUserInputDto input)
        {
            var createdVm = await _vmLookupService.CalculatePrice(sandboxId, input);
            return new JsonResult(createdVm);
        }

        [HttpGet("calculateName/{studyName}/{sandboxName}/{userSuffix}")]
        public string CalculateName(string studyName, string sandboxName, string userSuffix)
        {
            return _vmLookupService.CalculateName(studyName, sandboxName, userSuffix);
        }

        [HttpGet("{sandboxId}/sizes")]
        public async Task<IActionResult> GetAvailableVmSizes(int sandboxId, CancellationToken cancellationToken = default)
        {
            var availableSizes = await _vmLookupService.AvailableSizes(sandboxId, cancellationToken: cancellationToken);
            return new JsonResult(availableSizes);
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

        [HttpGet("updateVmSizeCache")]
        public async Task<IActionResult> UpdateVmSizeCache(CancellationToken cancellationToken = default)
        {
            await _vmLookupService.UpdateVmSizeCache(cancellationToken);
            return new NoContentResult();
        }
    }
}
