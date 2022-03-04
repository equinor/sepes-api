using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudiesDatasets
{
    [Route("api/studies")]
    public class GetDatasetResources : EndpointBase
    {
        readonly IStudySpecificDatasetService _studySpecificDatasetService;

        public GetDatasetResources(IStudySpecificDatasetService studySpecificDatasetService)
        {
            _studySpecificDatasetService = studySpecificDatasetService;
        }

        [HttpGet("{studyId}/datasets/{datasetId}/resources")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Handle(int studyId, int datasetId, CancellationToken cancellation = default)
        {
            var datasetResource = await _studySpecificDatasetService.GetDatasetResourcesAsync(studyId, datasetId, cancellation);
            return new JsonResult(datasetResource);
        }
    }
}
