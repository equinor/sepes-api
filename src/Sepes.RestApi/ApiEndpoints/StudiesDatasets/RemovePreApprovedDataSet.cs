using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudiesDatasets
{
    [Route("api/studies")]
    public class RemovePreApprovedDataSet : EndpointBase
    {
        readonly IStudyDatasetService _studyDatasetService;

        public RemovePreApprovedDataSet(IStudyDatasetService studyDatasetService)
        {
            _studyDatasetService = studyDatasetService;
        }

        [HttpDelete("{studyId}/datasets/{datasetId}")]
        public async Task<IActionResult> Handle(int studyId, int datasetId)
        {
            await _studyDatasetService.RemovePreApprovedDatasetFromStudyAsync(studyId, datasetId);
            return new NoContentResult();
        }
    }
}
