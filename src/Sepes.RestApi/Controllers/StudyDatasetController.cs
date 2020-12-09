using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto.Dataset;
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
        public async Task<IActionResult> GetAllDatasetsAsync(int studyId)
        {
            var dataset = await _studyDatasetService.GetDatasetsForStudyAsync(studyId);
            return new JsonResult(dataset);
        }

        [HttpPut("{studyId}/datasets/{datasetId}")]        
        public async Task<IActionResult> AddExistingDataSetAsync(int studyId, int datasetId)
        {
            var updatedStudy = await _studyDatasetService.AddPreApprovedDatasetToStudyAsync(studyId, datasetId);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{studyId}/datasets/{datasetId}")]
        public async Task<IActionResult> RemoveDataSetAsync(int studyId, int datasetId)
        {
            await _studyDatasetService.RemovePreApprovedDatasetFromStudyAsync(studyId, datasetId);
            return new NoContentResult();
        }

        [HttpGet("{studyId}/datasets/{datasetId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetSpecificDatasetByStudyIdAsync(int studyId, int datasetId)
        {
            var dataset = await _studyDatasetService.GetDatasetByStudyIdAndDatasetIdAsync(studyId, datasetId);
            return new JsonResult(dataset);
        }        
    }
}
