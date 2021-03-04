using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto.Sandbox;
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
            var updatedStudy = await _sandboxService.CreateAsync(studyId, newSandbox);
            return new JsonResult(updatedStudy);
        }

        [HttpGet("studies/{studyId}/sandboxes")]
        public async Task<IActionResult> GetSandboxesByStudyIdAsync(int studyId)
        {
            var sandboxes = await _sandboxService.GetAllForStudy(studyId);
            return new JsonResult(sandboxes);
        }     

        [HttpGet("sandboxes/{sandboxId}")]
        public async Task<IActionResult> GetSandboxAsync(int sandboxId)
        {
            var sandboxes = await _sandboxService.GetSandboxDetailsAsync(sandboxId);
            return new JsonResult(sandboxes);
        }  

        [HttpDelete("sandboxes/{sandboxId}")]
        public async Task<IActionResult> RemoveSandboxAsync(int sandboxId)
        {
            await _sandboxService.DeleteAsync(sandboxId);
            return new NoContentResult();
        }     
    }
}
