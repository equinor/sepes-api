using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.RestApi.ApiEndpoints.VirtualMachines
{
    [Route("api/virtualmachines")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class CalculateName : ControllerBase
    {
        readonly IVirtualMachineValidationService _virtualMachineValidationService;

        public CalculateName(IVirtualMachineValidationService virtualMachineValidationService)
        {
            _virtualMachineValidationService = virtualMachineValidationService;
        }

        [HttpPost("calculateName")]
        public string Handle(VmCalculateNameDto input)
        {
            return _virtualMachineValidationService.CalculateName(input.studyName, input.sandboxName, input.userSuffix);
        }
    }
}
