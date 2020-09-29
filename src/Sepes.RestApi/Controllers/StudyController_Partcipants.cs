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

        [HttpDelete("{studyId}/participants/{userId}/{roleName}")]
        public async Task<IActionResult> RemoveParticipantAsync(int studyId, int userId, string roleName)
        {
            var updatedStudy = await _studyService.RemoveParticipantFromStudyAsync(studyId, userId, roleName);
            return new JsonResult(updatedStudy);
        }
    }
}
