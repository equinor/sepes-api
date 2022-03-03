using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudiesParticipants
{
    [Route("api/")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class RemoveParticipant : ControllerBase
    {
        readonly IStudyParticipantRemoveService _studyParticipantRemoveService;

        public RemoveParticipant(IStudyParticipantRemoveService studyParticipantRemoveService)
        {
            _studyParticipantRemoveService = studyParticipantRemoveService;
        }

        [HttpDelete("studies/{studyId}/participants/{userId}/{roleName}")]
        public async Task<IActionResult> Handle(int studyId, int userId, string roleName)
        {
            await _studyParticipantRemoveService.RemoveAsync(studyId, userId, roleName);
            return new NoContentResult();
        }
    }
}
