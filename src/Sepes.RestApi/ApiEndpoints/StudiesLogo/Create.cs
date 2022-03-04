using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Handlers.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudiesLogo
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Create : ControllerBase
    {
        readonly IStudyCreateLogoHandler _studyCreateLogoHandler;

        public Create(IStudyCreateLogoHandler studyCreateLogoHandler)
        {
            _studyCreateLogoHandler = studyCreateLogoHandler;
        }

        // For local development, this method requires a running instance of Azure Storage Emulator
        [HttpPut("{studyId}/logo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Handle(int studyId, [FromForm(Name = "image")] IFormFile studyLogo)
        {
            var logoUrl = await _studyCreateLogoHandler.CreateAsync(studyId, studyLogo);
            return new JsonResult(logoUrl);
        }
    }
}
