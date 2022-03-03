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
    public class List : ControllerBase
    {
        readonly IVirtualMachineRuleService _service;

        public List(IVirtualMachineRuleService vmService)
        {
            _service = vmService;
        }

        [HttpGet]
        public async Task<IActionResult> Handle(int vmId)
        {
            var newRule = await _service.GetRules(vmId);
            return new JsonResult(newRule);
        }
    }
}
