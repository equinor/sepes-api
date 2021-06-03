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
    public class StudyReadController : ControllerBase
    {
        readonly IStudyEfReadService _studyEfReadService;        

        public StudyReadController(IStudyEfReadService studyEfReadService, IStudyCreateService studyCreateService, IStudyUpdateService studyUpdateService, IStudyDeleteService studyDeleteService)
        {
            _studyEfReadService = studyEfReadService;             
        }      

        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetStudyAsync(int studyId)
        {
            var study = await _studyEfReadService.GetStudyDetailsAsync(studyId);
            return new JsonResult(study);
        }                    
    }
}
