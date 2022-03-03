using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.SandboxDataset
{
    [Route("api/sandbox/")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class RemoveDataset : ControllerBase
    {
        readonly ISandboxDatasetModelService _sandboxDatasetModelService;
        public RemoveDataset(ISandboxDatasetModelService sandboxDatasetModelService)
        {
            _sandboxDatasetModelService = sandboxDatasetModelService;
        }

        [HttpDelete("{sandboxId}/datasets/{datasetId}")]
        public async Task<IActionResult> Handle(int sandboxId, int datasetId)
        {
            var datasets = await _sandboxDatasetModelService.Remove(sandboxId, datasetId);
            return new JsonResult(datasets);
        }
    }
}
