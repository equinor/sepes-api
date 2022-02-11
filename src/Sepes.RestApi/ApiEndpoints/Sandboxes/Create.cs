using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.Sandbox;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.Sandbox
{
    [Route("api")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Create : ControllerBase
    {
        private readonly ISandboxService _sandboxService;

        public Create(ISandboxService sandboxService)
        {
            _sandboxService = sandboxService;
        }

        [HttpPost("studies/{StudyId}/sandboxes")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> Handle(int studyId, SandboxCreateDto newSandbox, CancellationToken cancellationToken = default)
        {
            var createdSandbox = await _sandboxService.CreateAsync(studyId, newSandbox);
            var sandboxDetails = await _sandboxService.GetSandboxDetailsAsync(createdSandbox.Id);
            return new JsonResult(sandboxDetails);
        }
    }
}
