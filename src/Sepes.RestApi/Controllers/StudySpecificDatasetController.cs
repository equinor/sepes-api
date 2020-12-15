using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task<IActionResult> CreateStudySpecificDataSetAsync(int studyId, DatasetCreateUpdateInputBaseDto newDataset, CancellationToken cancellation)
        {          
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);        

            var updatedStudy = await _studySpecificDatasetService.CreateStudySpecificDatasetAsync(studyId, newDataset, clientIp, cancellation);
            return new JsonResult(updatedStudy);
        }

        [HttpPut("{studyId}/datasets/studyspecific/{datasetId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateStudySpecificDataSet(int studyId, int datasetId, DatasetCreateUpdateInputBaseDto updatedDataset)
        {
            var updatedStudy = await _studySpecificDatasetService.UpdateStudySpecificDatasetAsync(studyId, datasetId, updatedDataset);
            return new JsonResult(updatedStudy);
        }
    }
}
