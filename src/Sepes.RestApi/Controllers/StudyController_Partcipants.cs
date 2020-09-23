using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{

    public partial class StudyController : StudyControllerBase
    {
        [HttpPut("{studyId}/participants/{role}")]
        public async Task<IActionResult> AddParticipantAsync(int studyId, ParticipantLookupDto user, string role)
        {
            var updatedStudy = await _studyService.HandleAddParticipantAsync(studyId, user, role);
            return new JsonResult(updatedStudy);
        }    

        [HttpDelete("{studyId}/participants/{participantId}")]
        public async Task<IActionResult> RemoveParticipantAsync(int studyId, int participantId)
        {
            var updatedStudy = await _studyService.RemoveParticipantFromStudyAsync(studyId, participantId);
            return new JsonResult(updatedStudy);
        }
    }
}
