using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Sepes.RestApi.ApiEndpoints.VirtualMachinesRules
{
    [Route("api/virtualmachines/{vmId}/rules/")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Get : ControllerBase
    {
        readonly IVirtualMachineRuleService _service;

        public Get(IVirtualMachineRuleService vmService)
        {
            _service = vmService;
        }

        [HttpGet("{ruleId}")]
        public async Task<IActionResult> Handle(int vmId, string ruleId)
        {
            var rule = await _service.GetRuleById(vmId, ruleId);
            return new JsonResult(rule);
        }
    }
}
