using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.VirtualMachines
{
    [Route("api/virtualmachines")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class UpdateVmSizeCache : ControllerBase
    {
        readonly IVirtualMachineSizeImportService _virtualMachineSizeImportService;

        public UpdateVmSizeCache(IVirtualMachineSizeImportService virtualMachineSizeImportService)
        {
            _virtualMachineSizeImportService = virtualMachineSizeImportService;
        }

        [HttpGet("updateVmSizeCache")]
        public async Task<IActionResult> Handle(CancellationToken cancellationToken = default)
        {
            await _virtualMachineSizeImportService.UpdateVmSizeCache(cancellationToken);
            return new NoContentResult();
        }
    }
}
