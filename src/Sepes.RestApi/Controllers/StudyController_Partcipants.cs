using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{

    public partial class StudyController : StudyControllerBase
    {
        [HttpPut("{studyId}/participants/{role}")]
        public async Task<IActionResult> AddParticipantAsync(int studyId, AzureADUserDto user, string role)
        {
            var updatedStudy = await _studyService.HandleAddParticipant(studyId, user, role);
            return new JsonResult(updatedStudy);
        }

        [HttpPut("{studyId}/participants")]
        public async Task<IActionResult> AddNewParticipantAsync(int studyId, UserCreateDto user)
        {
            var updatedStudy = await _studyService.AddNewParticipantToStudyAsync(studyId, user);
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
