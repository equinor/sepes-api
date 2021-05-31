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
    public class StudyDeleteController : ControllerBase
    {
        readonly IStudyDeleteService _studyDeleteService;

        public StudyDeleteController(IStudyDeleteService studyDeleteService)
        {
            _studyDeleteService = studyDeleteService;           
        }       

        [HttpDelete("{studyId}")]
        public async Task<IActionResult> DeleteStudyAsync(int studyId)
        {
            await _studyDeleteService.DeleteStudyAsync(studyId);
            return new NoContentResult();
        }

        [HttpPut("{studyId}/close")]
        [Authorize]
        public async Task<IActionResult> CloseStudyAsync(int studyId)
        {
            await _studyDeleteService.CloseStudyAsync(studyId);
            return new NoContentResult();
        }              
    }
}
