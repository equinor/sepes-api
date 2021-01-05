using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class StudyParticipantController : ControllerBase
    {
        readonly IStudyParticipantService _studyParticipantService;

        public StudyParticipantController(IStudyParticipantService studyParticipantService)
        {
            _studyParticipantService = studyParticipantService;
        }

        //Get list of lookup items
        [HttpGet("participants")]
        [AuthorizeForScopes(Scopes = new[] { "User.Read.All" })]
        public async Task<IActionResult> GetLookupAsync(string search)
        {
            var studyParticipants = await _studyParticipantService.GetLookupAsync(search);
            return new JsonResult(studyParticipants);
        }

        [HttpPut("studies/{studyId}/participants/{role}")]
        public async Task<IActionResult> AddParticipantAsync(int studyId, ParticipantLookupDto user, string role)
        {
            var addedParticipantDto = await _studyParticipantService.AddAsync(studyId, user, role);
            return new JsonResult(addedParticipantDto);
        }    

        [HttpDelete("studies/{studyId}/participants/{userId}/{roleName}")]
        public async Task<IActionResult> RemoveParticipantAsync(int studyId, int userId, string roleName)
        {
            await _studyParticipantService.RemoveAsync(studyId, userId, roleName);
            return new NoContentResult();
        }
    }
}
