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
    public class StudyListController : ControllerBase
    {
        readonly IStudyListService _studyListService;
     

        public StudyListController(IStudyListService studyListService)
        {
            _studyListService = studyListService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudiesAsync()
        {
            var studies = await _studyListService.GetStudyListAsync();
            return new JsonResult(studies);
        }

           
    }
}
