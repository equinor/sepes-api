using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudiesDatasets
{
    [Route("api/studies")]
    public class Get : EndpointBase
    {
        readonly IStudySpecificDatasetService _studySpecificDatasetService;

        public Get(IStudySpecificDatasetService studySpecificDatasetService)
        {
            _studySpecificDatasetService = studySpecificDatasetService;
        }

        [HttpGet("{studyId}/datasets/{datasetId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Handle(int studyId, int datasetId)
        {
            var dataset = await _studySpecificDatasetService.GetDatasetAsync(studyId, datasetId);
            return new JsonResult(dataset);
        }
    }
}
