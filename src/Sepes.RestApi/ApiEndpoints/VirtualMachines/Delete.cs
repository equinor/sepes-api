using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace Sepes.RestApi.ApiEndpoints.VirtualMachines
{
    [Route("api/virtualmachines")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Delete : ControllerBase
    {
        readonly IVirtualMachineDeleteService _virtualMachineDeleteService;
        public Delete(IVirtualMachineDeleteService virtualMachineDeleteService)
        {
            _virtualMachineDeleteService = virtualMachineDeleteService;
        }

        [HttpDelete("{vmId}")]
        public async Task<IActionResult> DeleteAsync(int vmId)
        {
            await _virtualMachineDeleteService.DeleteAsync(vmId);
            return new NoContentResult();
        }
    }
}
