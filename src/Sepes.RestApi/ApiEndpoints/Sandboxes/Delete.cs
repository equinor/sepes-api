using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.RestApi.ApiEndpoints.Sandboxes
{
    [Route("api")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Delete : ControllerBase
    {
        private readonly ISandboxService _sandboxService;

        public Delete(ISandboxService sandboxService)
        {
            _sandboxService = sandboxService;
        }

        [HttpDelete("sandboxes/{sandboxId}")]
        public async Task<IActionResult> Handle(int sandboxId, CancellationToken cancellationToken = default)
        {
            await _sandboxService.DeleteAsync(sandboxId);
            return new NoContentResult();
        }
    }
}
