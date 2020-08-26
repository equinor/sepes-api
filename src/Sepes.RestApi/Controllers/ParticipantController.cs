using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class ParticipantController : ControllerBase
    {
        readonly IParticipantService _participantService;

        public ParticipantController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        //Get list of lookup items
        [HttpGet("participants")]
        public async Task<IActionResult> GetLookupAsync()
        {
            var studies = await _participantService.GetLookupAsync();
            return new JsonResult(studies);
        }

        [HttpGet("participants/{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var participant = await _participantService.GetByIdAsync(id);
            return new JsonResult(participant);
        }

        /*
        [HttpPut("/studies/{studyId}/participants/{participantId}/{role}")]       
        public async Task<IActionResult> AddParticipantAsync(int studyId, int participantId, string role)
        {
            var updatedStudy = await _participantService.AddParticipantToStudyAsync(studyId, participantId, role);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("/studies/{studyId}/participants/{participantId}")]
        public async Task<IActionResult> RemoveParticipantAsync(int studyId, int participantId)
        {
            var updatedStudy = await _participantService.RemoveParticipantFromStudyAsync(studyId, participantId);
            return new JsonResult(updatedStudy);
        }*/
    }

}
