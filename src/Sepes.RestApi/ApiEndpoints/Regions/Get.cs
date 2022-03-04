using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.Regions
{
    [Route("api")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Get : ControllerBase
    {
        readonly IRegionService _regionService;

        public Get(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet("regions")]
        public async Task<IActionResult> Handle(CancellationToken cancellationToken = default)
        {
            var regions = await _regionService.GetLookup(cancellationToken);
            return new JsonResult(regions);
        }
    }
}
