using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Azure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.VirtualMachines
{
    [Route("api/virtualmachines")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]

    public class GetSupportedLocations : ControllerBase
    {
        const string NAMESPACE = "Microsoft.Compute";
        const string RESOURCE_TYPE_NAME = "virtualMachines";
        private readonly IAzureResourceProviderApiService _azureResourceProviderApiService;
        public GetSupportedLocations(IAzureResourceProviderApiService azureResourceProviderApiService)
        {
            _azureResourceProviderApiService = azureResourceProviderApiService;
        }

        [HttpPost("locations")]
        public async Task<IActionResult> Handle(CancellationToken cancellationToken = default)
        {
            var locations = await _azureResourceProviderApiService.ListSupportedLocations(NAMESPACE, RESOURCE_TYPE_NAME, cancellationToken);
            return new JsonResult(locations);
        }
    }
}
