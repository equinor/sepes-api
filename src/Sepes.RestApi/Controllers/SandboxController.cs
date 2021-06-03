using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.Sandbox;
using Sepes.Infrastructure.Service.Interface;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api")]
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

        [HttpPost("studies/{studyId}/sandboxes")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> CreateSandboxAsync(int studyId, SandboxCreateDto newSandbox)
        {
            var createdSandbox = await _sandboxService.CreateAsync(studyId, newSandbox);
            var sandboxDetails = await _sandboxService.GetSandboxDetailsAsync(createdSandbox.Id);
            return new JsonResult(sandboxDetails);
        }          

        [HttpGet("sandboxes/{sandboxId}")]
        public async Task<IActionResult> GetSandboxAsync(int sandboxId)
        {
            var sandboxDetails = await _sandboxService.GetSandboxDetailsAsync(sandboxId);
            return new JsonResult(sandboxDetails);
        }  

        [HttpDelete("sandboxes/{sandboxId}")]
        public async Task<IActionResult> RemoveSandboxAsync(int sandboxId)
        {
            await _sandboxService.DeleteAsync(sandboxId);
            return new NoContentResult();
        }     
    }
}
