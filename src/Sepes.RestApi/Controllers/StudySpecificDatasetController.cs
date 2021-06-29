using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.Dataset;
using Sepes.Infrastructure.Service.Interface;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Sepes.RestApi.Util;

namespace Sepes.RestApi.Controller
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class StudySpecificDatasetController : ControllerBase
    {
        readonly IStudySpecificDatasetService _studySpecificDatasetService;

        public StudySpecificDatasetController(IStudySpecificDatasetService studySpecificDatasetService)
        {
            _studySpecificDatasetService = studySpecificDatasetService;
        }
        
        [HttpPost("{studyId}/datasets/studyspecific")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> CreateStudySpecificDataSetAsync(int studyId, DatasetCreateUpdateInputBaseDto newDataset, CancellationToken cancellation = default)
        {          
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);        

            var createdDataset = await _studySpecificDatasetService.CreateStudySpecificDatasetAsync(studyId, newDataset, clientIp, cancellation);
            return new JsonResult(createdDataset);
        }

        [HttpPut("{studyId}/datasets/studyspecific/{datasetId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateStudySpecificDataSet(int studyId, int datasetId, DatasetCreateUpdateInputBaseDto updatedDataset)
        {
            var updatedDatasetResult = await _studySpecificDatasetService.UpdateStudySpecificDatasetAsync(studyId, datasetId, updatedDataset);
            return new JsonResult(updatedDatasetResult);
        }

        [HttpGet("{studyId}/datasets/{datasetId}/resources")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetDatasetResources(int studyId, int datasetId, CancellationToken cancellation = default)
        {
            var datasetResource = await _studySpecificDatasetService.GetDatasetResourcesAsync(studyId, datasetId, cancellation);
            return new JsonResult(datasetResource);
        }
    }
}
