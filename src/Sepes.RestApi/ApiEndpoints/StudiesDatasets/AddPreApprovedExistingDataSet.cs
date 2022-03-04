using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudiesDatasets
{
    [Route("api/studies")]
    public class AddPreApprovedExistingDataSet : EndpointBase
    {
        readonly IStudyDatasetService _studyDatasetService;

        public AddPreApprovedExistingDataSet(IStudyDatasetService studyDatasetService)
        {
            _studyDatasetService = studyDatasetService;
        }

        [HttpPut("{studyId}/datasets/{datasetId}")]
        public async Task<IActionResult> Handle(int studyId, int datasetId)
        {
            var updatedStudy = await _studyDatasetService.AddPreApprovedDatasetToStudyAsync(studyId, datasetId);
            return new JsonResult(updatedStudy);
        }
    }
}
