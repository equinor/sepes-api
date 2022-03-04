using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.Dataset;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudiesDatasets
{
    [Route("api/studies")]
    public class Update : EndpointBase
    {
        readonly IStudySpecificDatasetService _studySpecificDatasetService;

        public Update(IStudySpecificDatasetService studySpecificDatasetService)
        {
            _studySpecificDatasetService = studySpecificDatasetService;
        }

        [HttpPut("{studyId}/datasets/studyspecific/{datasetId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Handle(int studyId, int datasetId, DatasetCreateUpdateInputBaseDto updatedDataset)
        {
            var updatedDatasetResult = await _studySpecificDatasetService.UpdateStudySpecificDatasetAsync(studyId, datasetId, updatedDataset);
            return new JsonResult(updatedDatasetResult);
        }
    }
}
