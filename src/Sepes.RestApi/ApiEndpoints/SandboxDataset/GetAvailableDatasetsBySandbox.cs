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
    public class GetAvailableDatasetsBySandbox : ControllerBase
    {
        readonly ISandboxDatasetModelService _sandboxDatasetModelService;
        public GetAvailableDatasetsBySandbox(ISandboxDatasetModelService sandboxDatasetModelService)
        {
            _sandboxDatasetModelService = sandboxDatasetModelService;
        }

        [HttpGet("{sandboxId}/availabledatasets")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Handle(int sandboxId)
        {
            var datasets = await _sandboxDatasetModelService.AllAvailable(sandboxId);
            return new JsonResult(datasets);
        }
    }
}
