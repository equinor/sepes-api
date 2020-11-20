using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Service.Interface;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize(Roles = AppRoles.Admin)] //Todo: Need wider access, but restricted for now
    public class StudyController : ControllerBase
    {
        readonly IStudyService _studyService;


        public StudyController(IStudyService studyService)
        {
            _studyService = studyService;
        }

        [HttpGet]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> GetStudiesAsync([FromQuery] bool? excludeHidden)
        {
            var studies = await _studyService.GetStudyListAsync(excludeHidden);
            return new JsonResult(studies);
        }

        [HttpGet("{studyId}")]
        [Authorize]
        public async Task<IActionResult> GetStudyAsync(int studyId)
        {

            var study = await _studyService.GetStudyDetailsDtoByIdAsync(studyId, UserOperations.StudyRead);

            return new JsonResult(study);
        }

        [HttpPost]
        [Authorize(Roles = RoleSets.AdminOrSponsor)]
        public async Task<IActionResult> CreateStudyAsync(StudyCreateDto newStudy)
        {
            var study = await _studyService.CreateStudyAsync(newStudy);
            return new JsonResult(study);
        }

        //[HttpPost()]
        //[Consumes(MediaTypeNames.Application.Json, "multipart/form-data")]
        //public async Task<IActionResult> CreateStudyWithPicture(StudyDto newStudy, IFormFile studyLogo)
        //{
        //    var study = await _studyService.CreateStudyAsync(newStudy, studyLogo);
        //    return new JsonResult(study);
        //}

        [HttpDelete("{studyId}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> DeleteStudyAsync(int studyId)
        {
            await _studyService.DeleteStudyAsync(studyId);
            return new NoContentResult();
        }


        [HttpPut("{studyId}/details")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Authorize]
        //TODO: Must also be possible for sponsor rep and other roles
        public async Task<IActionResult> UpdateStudyDetailsAsync(int studyId, StudyDto study)
        {
            var updatedStudy = await _studyService.UpdateStudyDetailsAsync(studyId, study);
            return new JsonResult(updatedStudy);
        }


        // For local development, this method requires a running instance of Azure Storage Emulator
        [HttpPut("{studyId}/logo")]
        [Consumes("multipart/form-data")]
        [Authorize]
        //TODO: Must also be possible for sponsor rep/vendor admin or other study specific roles
        public async Task<IActionResult> AddLogo(int studyId, [FromForm(Name = "image")] IFormFile studyLogo)
        {
            var updatedStudy = await _studyService.AddLogoAsync(studyId, studyLogo);
            return new JsonResult(updatedStudy);
        }

        [HttpGet("{studyId}/logo")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Octet)]
        [Authorize]
        // For local development, this method requires a running instance of Azure Storage Emulator
        public async Task<IActionResult> GetLogo(int studyId)
        {
            var logoResponse = await _studyService.GetLogoAsync(studyId);

            string fileType = logoResponse.LogoUrl.Split('.').Last();

            if (fileType.Equals("jpg"))
            {
                fileType = "jpeg";
            }

            return File(new System.IO.MemoryStream(logoResponse.LogoBytes), $"image/{fileType}", $"logo_{studyId}.{fileType}");
        }
    }
}
