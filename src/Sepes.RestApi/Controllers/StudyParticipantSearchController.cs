using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
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
    public class StudyParticipantSearchController : ControllerBase
    {
        readonly IStudyParticipantSearchService _studyParticipantLookupService;  

        public StudyParticipantSearchController(
            IStudyParticipantSearchService studyParticipantLookupService)
        {
            _studyParticipantLookupService = studyParticipantLookupService;      
        }

        //Get list of lookup items
        [HttpGet("participants")]
        [AuthorizeForScopes(Scopes = new[] { "Group.Read.All", "GroupMember.Read.All" })]
        public async Task<IActionResult> SearchAsync(string search, CancellationToken cancellationToken = default)
        {
            var studyParticipants = await _studyParticipantLookupService.SearchAsync(search, cancellationToken: cancellationToken);
            return new JsonResult(studyParticipants);
        }       
    }
}
