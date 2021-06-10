using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.VirtualMachine;
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
    public class VirtualMachineValidationController : ControllerBase
    {     
        readonly IVirtualMachineValidationService _virtualMachineValidationService;

        public VirtualMachineValidationController(IVirtualMachineValidationService virtualMachineValidationService)
        {            
            _virtualMachineValidationService = virtualMachineValidationService;
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
