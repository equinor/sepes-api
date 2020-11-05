using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
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
        public async Task<IActionResult> CreateAsync(int sandboxId, CreateVmUserInputDto newVm)
        {
            var createdVm = await _vmService.CreateAsync(sandboxId, newVm);
            return new JsonResult(createdVm);
        }

        [HttpPost("{sandboxId}/calculatedprice")]
        public async Task<IActionResult> GetCalculatedPrice(int sandboxId, CalculateVmPriceUserInputDto input)
        {
            var createdVm = await _vmService.CalculatePrice(sandboxId, input);
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

        [HttpGet("{vmId}/rules")]
        public async Task<IActionResult> GetRules(int vmId)
        {
            var newRule = await _vmService.GetRules(vmId);
            return new JsonResult(newRule);
        }

        [HttpGet("{vmId}/rules/{ruleId}")]
        public async Task<IActionResult> GetRules(int vmId, string ruleId)
        {
            var rule = await _vmService.GetRuleById(vmId, ruleId);
            return new JsonResult(rule);
        }

        [HttpPost("{vmId}/rules")]
        public async Task<IActionResult> AddRule(int vmId, List<VmRuleDto> updatedRuleSet)
        {
            var allRules = await _vmService.SetRules(vmId, updatedRuleSet);
            return new JsonResult(allRules);
        }

        [HttpPut("{vmId}/rules")]
        public async Task<IActionResult> UpdateRule(int vmId, VmRuleDto input)
        {
            var updatedRule = await _vmService.UpdateRule(vmId, input);
            return new JsonResult(updatedRule);
        }

        [HttpDelete("{vmId}/rules/{ruleId}")]
        public async Task<IActionResult> DeleteRule(int vmId, string ruleId)
        {
            var deletedRule = await _vmService.DeleteRule(vmId, ruleId);
            return new JsonResult(deletedRule);
        }

        //Lookup endpoints

        [HttpGet("{sandboxId}/sizes")]
        public async Task<IActionResult> GetAvailableVmSizes(int sandboxId, CancellationToken cancellationToken = default)
        {
            var availableSizes = await _vmService.AvailableSizes(sandboxId, cancellationToken: cancellationToken);
            return new JsonResult(availableSizes);
        }

        [HttpGet("disks/")]
        public async Task<IActionResult> GetAvailableDisks()
        {
            var availableSizes = await _vmService.AvailableDisks();
            return new JsonResult(availableSizes);
        }

        [HttpGet("{sandboxId}/operatingsystems")]
        public async Task<IActionResult> GetAvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default)
        {
            var availableSizes = await _vmService.AvailableOperatingSystems(sandboxId, cancellationToken);
            return new JsonResult(availableSizes);
        }
    }
}
