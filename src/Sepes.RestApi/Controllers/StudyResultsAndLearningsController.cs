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
        readonly IStudyRawQueryModelService _studyRawQueryModelService;      

        public StudyResultsAndLearningsController(IStudyRawQueryModelService studyRawQueryModelService)
        {
            _studyRawQueryModelService = studyRawQueryModelService;          
        }        

        [HttpGet("{studyId}/resultsandlearnings")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetResultsAndLearningsAsync(int studyId)
        {
            var resultsAndLearningsFromDb = await _studyRawQueryModelService.GetResultsAndLearningsAsync(studyId);
            return new JsonResult(resultsAndLearningsFromDb);
        }       
    }
}
