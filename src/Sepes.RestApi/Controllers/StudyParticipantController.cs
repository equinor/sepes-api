using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
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
        readonly IStudyParticipantLookupService _studyParticipantLookupService;
        readonly IStudyParticipantCreateService _studyParticipantCreateService;
        readonly IStudyParticipantRemoveService _studyParticipantRemoveService;

        public StudyParticipantController(
            IStudyParticipantLookupService studyParticipantLookupService,
            IStudyParticipantCreateService studyParticipantCreateService,
            IStudyParticipantRemoveService studyParticipantRemoveService)
        {
            _studyParticipantLookupService = studyParticipantLookupService;
            _studyParticipantCreateService = studyParticipantCreateService;
            _studyParticipantRemoveService = studyParticipantRemoveService;
        }

        //Get list of lookup items
        [HttpGet("participants")]
        [AuthorizeForScopes(Scopes = new[] { "User.Read.All" })]
        public async Task<IActionResult> GetLookupAsync(string search, CancellationToken cancellationToken = default)
        {
            var studyParticipants = await _studyParticipantLookupService.GetLookupAsync(search, cancellationToken: cancellationToken);
            return new JsonResult(studyParticipants);
        }

        [HttpPut("studies/{studyId}/participants/{role}")]
        public async Task<IActionResult> AddParticipantAsync(int studyId, ParticipantLookupDto user, string role)
        {
            var addedParticipantDto = await _studyParticipantCreateService.AddAsync(studyId, user, role);
            return new JsonResult(addedParticipantDto);
        }    

        [HttpDelete("studies/{studyId}/participants/{userId}/{roleName}")]
        public async Task<IActionResult> RemoveParticipantAsync(int studyId, int userId, string roleName)
        {
            await _studyParticipantRemoveService.RemoveAsync(studyId, userId, roleName);
            return new NoContentResult();
        }
    }
}
