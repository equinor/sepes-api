using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudiesDatasets
{
    [Route("api/studies")]
    public class Delete : EndpointBase
    {
        readonly IStudySpecificDatasetService _studySpecificDatasetService;
        public Delete(IStudySpecificDatasetService studySpecificDatasetService)
        {
            _studySpecificDatasetService = studySpecificDatasetService;
        }

        [HttpDelete("datasets/studyspecific/{datasetId}")]
        public async Task<IActionResult> Handle(int datasetId, CancellationToken cancellationToken = default)
        {
            await _studySpecificDatasetService.HardDeleteStudySpecificDatasetAsync(datasetId, cancellationToken);

            return new NoContentResult();
        }
    }
}
