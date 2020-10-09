using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.RestApi.Controller
{
    [Route("api/lookup")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize(Roles = AppRoles.Admin)] //Todo: Need wider access, but keeping it restricted for now
    public class LookupController : ControllerBase
    {     
        readonly ILookupService _lookupService;

        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet("regions")]
        public IActionResult GetRegions()
        {
            var regions = _lookupService.AzureRegions();
            return new JsonResult(regions);
        }

        [HttpGet("studyroles")]
        public IActionResult GetStudyRoles()
        {
            var roles = _lookupService.StudyRoles();
            return new JsonResult(roles);
        }
    }

}
