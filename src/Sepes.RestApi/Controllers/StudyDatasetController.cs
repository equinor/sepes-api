using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class StudyDatasetController : ControllerBase
    {
        readonly IStudyDatasetService _studyDatasetService;

        public StudyDatasetController(IStudyDatasetService studyDatasetService)
        {
            _studyDatasetService = studyDatasetService;
        }

        [HttpGet("{studyId}/datasets")]
        [Consumes(MediaTypeNames.Application.Json)]        
        public async Task<IActionResult> GetAllStudyDatasetsAsync(int studyId)
        {
            var dataset = await _studyDatasetService.GetDatasetsForStudyAsync(studyId);
            return new JsonResult(dataset);
        }

        [HttpPut("{studyId}/datasets/{datasetId}")]        
        public async Task<IActionResult> AddPreApprovedExistingDataSetAsync(int studyId, int datasetId)
        {
            var updatedStudy = await _studyDatasetService.AddPreApprovedDatasetToStudyAsync(studyId, datasetId);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{studyId}/datasets/{datasetId}")]
        public async Task<IActionResult> RemovePreApprovedDataSetAsync(int studyId, int datasetId)
        {
            await _studyDatasetService.RemovePreApprovedDatasetFromStudyAsync(studyId, datasetId);
            return new NoContentResult();
        }     
    }
}
