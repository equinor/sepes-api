using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Azure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StorageAccounts
{
    [Route("api/storageaccounts")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class GetSupportedLocations : ControllerBase
    {
        const string NAMESPACE = "Microsoft.Storage";
        const string RESOURCE_TYPE_NAME = "storageAccounts";
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
