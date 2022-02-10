using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.Sandboxes
{
    [Route("api")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class GetById:ControllerBase
    {
        private readonly ISandboxService _sandboxService;
        public GetById(ISandboxService sandboxService)
        {
            _sandboxService = sandboxService;
        }

        [HttpGet("sandboxes/{sandboxId}")]
        public async Task<IActionResult> Handle(int sandboxId, CancellationToken cancellationToken = default)
        {
            var sandboxDetails = await _sandboxService.GetSandboxDetailsAsync(sandboxId);
            return new JsonResult(sandboxDetails);
        }
    }
}
