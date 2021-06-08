using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class StudyResultsAndLearningsController : ControllerBase
    {
        readonly IStudyResultsAndLearningsModelService _studyResultsAndLearningsModelService;      

        public StudyResultsAndLearningsController(IStudyResultsAndLearningsModelService studyResultsAndLearningsModelService)
        {
            _studyResultsAndLearningsModelService = studyResultsAndLearningsModelService;          
        }        

        [HttpGet("{studyId}/resultsandlearnings")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetResultsAndLearningsAsync(int studyId)
        {
            var resultsAndLearnings = await _studyResultsAndLearningsModelService.GetAsync(studyId);
            return new JsonResult(resultsAndLearnings);
        }       
    }
}
