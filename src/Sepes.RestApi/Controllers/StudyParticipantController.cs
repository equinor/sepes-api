using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Sepes.Common.Dto;
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
        readonly IStudyParticipantCreateService _studyParticipantCreateService;
        readonly IStudyParticipantRemoveService _studyParticipantRemoveService;

        public StudyParticipantController(         
            IStudyParticipantCreateService studyParticipantCreateService,
            IStudyParticipantRemoveService studyParticipantRemoveService)
        {          
            _studyParticipantCreateService = studyParticipantCreateService;
            _studyParticipantRemoveService = studyParticipantRemoveService;
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
