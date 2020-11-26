﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Service.Interface;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class SandboxController : ControllerBase
    {
        readonly ISandboxService _sandboxService;

        public SandboxController(ISandboxService sandboxService)
        {
            _sandboxService = sandboxService;
        }

        [HttpGet("{studyId}/sandboxes")]
        public async Task<IActionResult> GetSandboxesByStudyIdAsync(int studyId)
        {
            var sandboxes = await _sandboxService.GetAllForStudy(studyId);
            return new JsonResult(sandboxes);
        }

        [HttpGet("{studyId}/sandboxes/{sandboxId}")]
        public async Task<IActionResult> GetSandbox(int studyId, int sandboxId)
        {
            var sandboxes = await _sandboxService.GetSandboxDetailsAsync(sandboxId);
            return new JsonResult(sandboxes);
        }

        [HttpPost("{studyId}/sandboxes")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> CreateSandboxAsync(int studyId, SandboxCreateDto newSandbox)
        {
            var updatedStudy = await _sandboxService.CreateAsync(studyId, newSandbox);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{studyId}/sandboxes/{sandboxId}")]
        public async Task<IActionResult> RemoveSandboxAsync(int studyId, int sandboxId)
        {
           await _sandboxService.DeleteAsync(studyId, sandboxId);
            return new NoContentResult();
        }

        [HttpGet("{studyId}/sandboxes/{sandboxId}/resources")]
        public async Task<IActionResult> GetSandboxResources(int studyId, int sandboxId)
        {
            var sandboxes = await _sandboxService.GetSandboxResources(studyId, sandboxId);
            return new JsonResult(sandboxes);
        }

        //[HttpGet("sandboxes/templatelookup")]
        //[Authorize(Roles = AppRoles.Admin)]
        ////TODO: Must also be possible for sponsor rep and other roles
        //public async Task<IActionResult> GetTemplatesLookupAsync()
        //{
        //    var templates = new List<LookupDto>();
        //    return new JsonResult(templates);
        //}

        [HttpPut("{studyId}/sandboxes/{sandboxId}/rescheduleCreation")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> ReScheduleCreation(int studyId, int sandboxId)
        {
            await _sandboxService.ReScheduleSandboxCreation(sandboxId);
            return new NoContentResult();
        }
    }
}
