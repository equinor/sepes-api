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
    public class AddParticipant : ControllerBase
    {
        readonly IStudyParticipantCreateService _studyParticipantCreateService;

        public AddParticipant(
            IStudyParticipantCreateService studyParticipantCreateService)
        {
            _studyParticipantCreateService = studyParticipantCreateService;
        }

        [HttpPut("studies/{studyId}/participants/{role}")]
        public async Task<IActionResult> Handle(int studyId, ParticipantLookupDto user, string role)
        {
            var addedParticipantDto = await _studyParticipantCreateService.AddAsync(studyId, user, role);
            return new JsonResult(addedParticipantDto);
        }
    }
}
