using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class ParticipantController : ControllerBase
    {
        readonly IStudyParticipantService _studyParticipantService;

        public ParticipantController(IStudyParticipantService studyParticipantService)
        {
            _studyParticipantService = studyParticipantService;
        }

        //Get list of lookup items
        [HttpGet("participants")]
        public async Task<IActionResult> GetLookupAsync()
        {
            var studies = await _studyParticipantService.GetLookupAsync();
            return new JsonResult(studies);
        }     
    }
}
