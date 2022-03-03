using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudiesParticipants
{
    [Route("api/studies/")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class RolesAvailableForUser : ControllerBase
    {
        readonly IStudyParticipantRolesService _studyParticipantRolesService;

        public RolesAvailableForUser(IStudyParticipantRolesService studyParticipantRolesService)
        {
            _studyParticipantRolesService = studyParticipantRolesService;
        }

        [HttpGet("{studyId}/availableroles/")]
        public async Task<IActionResult> Handle(int studyId)
        {
            var roles = await _studyParticipantRolesService.RolesAvailableForUser(studyId);
            return new JsonResult(roles);
        }
    }
}
