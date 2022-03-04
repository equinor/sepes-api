using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.Wbs
{
    [Route("api/wbsvalidation")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Validate : ControllerBase
    {
        readonly IWbsValidationService _wbsValidationService;

        public Validate(IWbsValidationService wbsValidationService)
        {
            _wbsValidationService = wbsValidationService;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> Handle(string code, CancellationToken cancellationToken = default)
        {
            var regions = await _wbsValidationService.IsValidWithAccessCheck(code, cancellationToken);
            return new JsonResult(regions);
        }
    }
}
