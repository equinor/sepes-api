using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/lookup")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class LookupController : ControllerBase
    {     
        readonly ILookupService _lookupService;
        readonly IRegionService _regionService;

        public LookupController(ILookupService lookupService, IRegionService regionService)
        {
            _lookupService = lookupService;
            _regionService = regionService;
        }

        [HttpGet("regions")]
        public async Task<IActionResult> GetRegions(CancellationToken cancellationToken = default)
        {
            var regions = await _regionService.GetLookup(cancellationToken);
            return new JsonResult(regions);
        }

        [HttpGet("studyroles")]
        public IActionResult GetStudyRoles()
        {
            var roles = _lookupService.StudyRoles();
            return new JsonResult(roles);
        }

        [HttpGet("studyroles/{studyId}")]
        public async Task<IActionResult> StudyRolesUserCanGive(int studyId)
        {
            var roles =  await _lookupService.StudyRolesUserCanGive(studyId);
            return new JsonResult(roles);
        }
    }

}
