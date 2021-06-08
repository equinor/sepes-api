using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class StudyDetailsController : ControllerBase
    {
        readonly IStudyDetailsService _studyDetailsService;        

        public StudyDetailsController(IStudyDetailsService studyDetailsService)
        {
            _studyDetailsService = studyDetailsService;             
        }      

        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetAsync(int studyId)
        {
            var study = await _studyDetailsService.Get(studyId);
            return new JsonResult(study);
        }                    
    }
}
