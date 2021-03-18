using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/sandbox/")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class SandboxDatasetController : ControllerBase
    {
        readonly ISandboxDatasetModelService _sandboxDatasetModelService;

        public SandboxDatasetController(ISandboxDatasetModelService sandboxDatasetModelService)
        {
            _sandboxDatasetModelService = sandboxDatasetModelService;
        }

        [HttpGet("{sandboxId}/availabledatasets")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetAvailableDatasetsForSandbox(int sandboxId)
        {
            var datasets = await _sandboxDatasetModelService.AllAvailable(sandboxId);
            return new JsonResult(datasets);
        }

        [HttpPut("{sandboxId}/datasets/{datasetId}")]
        public async Task<IActionResult> AddDatasetAsync(int sandboxId, int datasetId)
        {
            var datasets = await _sandboxDatasetModelService.Add(sandboxId, datasetId);
            return new JsonResult(datasets);
        }

        [HttpDelete("{sandboxId}/datasets/{datasetId}")]
        public async Task<IActionResult> RemoveDatasetAsync(int sandboxId, int datasetId)
        {
            var datasets = await _sandboxDatasetModelService.Remove(sandboxId, datasetId);
            return new JsonResult(datasets);
        } 
    }
}
