using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudiesDatasets
{
    [Route("api/studies")]
    public class List : EndpointBase
    {
        readonly IStudyDatasetService _studyDatasetService;

        public List(IStudyDatasetService studyDatasetService)
        {
            _studyDatasetService = studyDatasetService;
        }

        [HttpGet("{studyId}/datasets")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Handle(int studyId)
        {
            var dataset = await _studyDatasetService.GetDatasetsForStudyAsync(studyId);
            return new JsonResult(dataset);
        }
    }
}
