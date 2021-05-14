using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/wbsvalidation")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class WbsValidationController : ControllerBase
    {
        readonly IWbsValidationService _wbsValidationService;

        public WbsValidationController(IWbsValidationService wbsValidationService)
        {
            _wbsValidationService = wbsValidationService;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> Validate(string code, CancellationToken cancellationToken = default)
        {
            var regions = await _wbsValidationService.Exists(code, cancellationToken);
            return new JsonResult(regions);
        }       
    }

}
