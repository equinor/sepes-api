using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto;
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
        //TODO: Must also be possible for other study specific roles
        public async Task<IActionResult> GetDatasetsForStudy(int studyId)
        {
            var dataset = await _studyDatasetService.GetDatasetsForStudy(studyId);
            return new JsonResult(dataset);
        }

        [HttpPut("{studyId}/datasets/{datasetId}")]        
        //TODO: Must also be possible for sponsor rep/vendor admin
        public async Task<IActionResult> AddDataSetAsync(int studyId, int datasetId)
        {
            var updatedStudy = await _studyDatasetService.AddDatasetToStudyAsync(studyId, datasetId);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{studyId}/datasets/{datasetId}")]        
        //TODO: Must also be possible for sponsor rep/vendor admin
        public async Task<IActionResult> RemoveDataSetAsync(int studyId, int datasetId)
        {
            var updatedStudy = await _studyDatasetService.RemoveDatasetFromStudyAsync(studyId, datasetId);
            return new JsonResult(updatedStudy);
        }

        [HttpGet("{studyId}/datasets/{datasetId}")]
        [Consumes(MediaTypeNames.Application.Json)]        
        //TODO: Must also be possible for other study specific roles
        public async Task<IActionResult> GetSpecificDatasetByStudyIdAsync(int studyId, int datasetId)
        {
            var dataset = await _studyDatasetService.GetDatasetByStudyIdAndDatasetIdAsync(studyId, datasetId);
            return new JsonResult(dataset);
        }

        [HttpPost("{studyId}/datasets/studyspecific")]
        [Consumes(MediaTypeNames.Application.Json)]        
        //TODO: Must also be possible for sponsor rep/vendor admin or other study specific roles
        public async Task<IActionResult> AddStudySpecificDataSet(int studyId, StudySpecificDatasetDto newDataset)
        {
            var updatedStudy = await _studyDatasetService.AddStudySpecificDatasetAsync(studyId, newDataset);
            return new JsonResult(updatedStudy);
        }

        [HttpPut("{studyId}/datasets/studyspecific/{datasetId}")]
        [Consumes(MediaTypeNames.Application.Json)]        
        //TODO: Must also be possible for sponsor rep/vendor admin or other study specific roles
        public async Task<IActionResult> UpdateStudySpecificDataSet(int studyId, int datasetId, StudySpecificDatasetDto newDataset)
        {
            var updatedStudy = await _studyDatasetService.UpdateStudySpecificDatasetAsync(studyId, datasetId, newDataset);
            return new JsonResult(updatedStudy);
        }

    }

}
