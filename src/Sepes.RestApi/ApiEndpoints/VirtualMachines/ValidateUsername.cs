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
    public class ValidateUsername : ControllerBase
    {
        readonly IVirtualMachineValidationService _virtualMachineValidationService;

        public ValidateUsername(IVirtualMachineValidationService virtualMachineValidationService)
        {
            _virtualMachineValidationService = virtualMachineValidationService;
        }

        [HttpPost("validateUsername")]
        public IActionResult Handle(VmUsernameDto input)
        {
            var usernameValidationResult = _virtualMachineValidationService.CheckIfUsernameIsValidOrThrow(input);
            return new JsonResult(usernameValidationResult);

        }
    }
}
