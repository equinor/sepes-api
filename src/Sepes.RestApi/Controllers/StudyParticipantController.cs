using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize(Roles = AppRoles.Admin)] //Todo: Need wider access, but restricted for now
    public class StudyParticipantController : ControllerBase
    {
        readonly IStudyParticipantService _studyParticipantService;

        public StudyParticipantController(IStudyParticipantService studyParticipantService)
        {
            _studyParticipantService = studyParticipantService;
        }

        //Get list of lookup items
        [HttpGet("participants")]
        public async Task<IActionResult> GetLookupAsync(string search)
        {
            var studies = await _studyParticipantService.GetLookupAsync(search);
            return new JsonResult(studies);
        }

        [HttpPut("studies/{studyId}/participants/{role}")]
        public async Task<IActionResult> AddParticipantAsync(int studyId, ParticipantLookupDto user, string role)
        {
            var updatedStudy = await _studyParticipantService.HandleAddParticipantAsync(studyId, user, role);
            return new JsonResult(updatedStudy);
        }    

        [HttpDelete("studies/{studyId}/participants/{userId}/{roleName}")]
        public async Task<IActionResult> RemoveParticipantAsync(int studyId, int userId, string roleName)
        {
            var updatedStudy = await _studyParticipantService.RemoveParticipantFromStudyAsync(studyId, userId, roleName);
            return new JsonResult(updatedStudy);
        }
    }
}
