using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
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
    [Authorize(Roles = AppRoles.Admin)] //Todo: Need wider access, but restricted for now
    public class StudyDatasetController : ControllerBase
    {
        readonly IDatasetService _datasetService;

        public StudyDatasetController(IDatasetService datasetService)
        {
            _datasetService = datasetService;
        }

        [HttpGet("{studyId}/datasets")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Authorize(Roles = RoleSets.AdminOrSponsor)]
        //TODO: Must also be possible for other study specific roles
        public async Task<IActionResult> GetDatasetsForStudy(int studyId)
        {
            var dataset = await _datasetService.GetDatasetsForStudy(studyId);
            return new JsonResult(dataset);
        }

        [HttpPut("{studyId}/datasets/{datasetId}")]
        [Authorize(Roles = AppRoles.Admin)]
        //TODO: Must also be possible for sponsor rep/vendor admin
        public async Task<IActionResult> AddDataSetAsync(int studyId, int datasetId)
        {
            var updatedStudy = await _datasetService.AddDatasetToStudyAsync(studyId, datasetId);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{studyId}/datasets/{datasetId}")]
        [Authorize(Roles = AppRoles.Admin)]
        //TODO: Must also be possible for sponsor rep/vendor admin
        public async Task<IActionResult> RemoveDataSetAsync(int studyId, int datasetId)
        {
            var updatedStudy = await _datasetService.RemoveDatasetFromStudyAsync(studyId, datasetId);
            return new JsonResult(updatedStudy);
        }

        [HttpGet("{studyId}/datasets/{datasetId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Authorize(Roles = RoleSets.AdminOrSponsor)]
        //TODO: Must also be possible for other study specific roles
        public async Task<IActionResult> GetSpecificDatasetByStudyIdAsync(int studyId, int datasetId)
        {
            var dataset = await _datasetService.GetDatasetByStudyIdAndDatasetIdAsync(studyId, datasetId);
            return new JsonResult(dataset);
        }

        [HttpPost("{studyId}/datasets/studyspecific")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Authorize(Roles = AppRoles.Admin)]
        //TODO: Must also be possible for sponsor rep/vendor admin or other study specific roles
        public async Task<IActionResult> AddStudySpecificDataSet(int studyId, StudySpecificDatasetDto newDataset)
        {
            var updatedStudy = await _datasetService.AddStudySpecificDatasetAsync(studyId, newDataset);
            return new JsonResult(updatedStudy);
        }

        [HttpPut("{studyId}/datasets/studyspecific/{datasetId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Authorize(Roles = AppRoles.Admin)]
        //TODO: Must also be possible for sponsor rep/vendor admin or other study specific roles
        public async Task<IActionResult> UpdateStudySpecificDataSet(int studyId, int datasetId, StudySpecificDatasetDto newDataset)
        {
            var updatedStudy = await _datasetService.UpdateStudySpecificDatasetAsync(studyId, datasetId, newDataset);
            return new JsonResult(updatedStudy);
        }

    }

}
