using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controllers
{
    [Route("api/virtualmachines/{vmId}/rules/")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class VirtualMachineRuleController : ControllerBase
    {
        readonly IVirtualMachineRuleService _service;

        public VirtualMachineRuleController(IVirtualMachineRuleService vmService)
        {
            _service = vmService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRules(int vmId)
        {
            var newRule = await _service.GetRules(vmId);
            return new JsonResult(newRule);
        }

        [HttpGet("{ruleId}")]
        public async Task<IActionResult> GetRule(int vmId, string ruleId)
        {
            var rule = await _service.GetRuleById(vmId, ruleId);
            return new JsonResult(rule);
        }

        [HttpPost("")]
        public async Task<IActionResult> SetRules(int vmId, List<VmRuleDto> updatedRuleSet)
        {
            var allRules = await _service.SetRules(vmId, updatedRuleSet);
            return new JsonResult(allRules);
        }

        //[HttpDelete("{ruleId}")]
        //public async Task<IActionResult> DeleteRule(int vmId, string ruleId)
        //{
        //    var deletedRule = await _service.DeleteRule(vmId, ruleId);
        //    return new JsonResult(deletedRule);
        //}       
    }
}
