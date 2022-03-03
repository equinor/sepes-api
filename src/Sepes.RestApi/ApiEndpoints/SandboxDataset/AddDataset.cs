using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.SandboxDataset
{
    [Route("api/sandbox/")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class AddDataset
    {
        readonly ISandboxDatasetModelService _sandboxDatasetModelService;
        public AddDataset(ISandboxDatasetModelService sandboxDatasetModelService)
        {
            _sandboxDatasetModelService = sandboxDatasetModelService;
        }

        [HttpPut("{sandboxId}/datasets/{datasetId}")]
        public async Task<IActionResult> Handle(int sandboxId, int datasetId)
        {
            var datasets = await _sandboxDatasetModelService.Add(sandboxId, datasetId);
            return new JsonResult(datasets);
        }
    }
}
