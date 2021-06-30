using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class RegionsController : ControllerBase
    {
        readonly IRegionService _regionService;

        public RegionsController(IRegionService regionService)
        {           
            _regionService = regionService;
        }

        [HttpGet("regions")]
        public async Task<IActionResult> GetRegions(CancellationToken cancellationToken = default)
        {
            var regions = await _regionService.GetLookup(cancellationToken);
            return new JsonResult(regions);
        } 
    }
}
