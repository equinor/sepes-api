using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
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
    [Authorize]
    public class StudyControllerBase : ControllerBase
    {
       protected bool CanViewRestrictedStudy()
        {
            //TODO: Open up for more than admins
            //TODO: ADdd relevant study specific roles
            return User.IsInRole(Roles.Admin);
        }
    }

  
    public partial class StudyController : StudyControllerBase
    {        
        readonly IStudyService _studyService;
        readonly ISandboxService _sandboxService;
        readonly IDatasetService _datasetService;    


        public StudyController(IStudyService studyService, ISandboxService sandboxService, IDatasetService datasetService)
        {
            _studyService = studyService;
            _sandboxService = sandboxService;
            _datasetService = datasetService;      
        }       

        [HttpGet]
        public async Task<IActionResult> GetStudiesAsync([FromQuery] bool? includeRestricted)
        {
          
            if(includeRestricted.HasValue && includeRestricted.Value && CanViewRestrictedStudy() == false)
            {
                return new ForbidResult();
            }
         
            var studies = await _studyService.GetStudiesAsync(includeRestricted);
            return new JsonResult(studies);
        }

        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetStudyAsync(int studyId)
        {
            //TODO: Require a role for this?
            var study = await _studyService.GetStudyByIdAsync(studyId);

            if(study.Restricted && CanViewRestrictedStudy() == false)
            {
                return new ForbidResult();
            }

            return new JsonResult(study);
        }

        [HttpPost()]
        [Authorize(Roles = RoleSets.AdminOrSponsor)]
        public async Task<IActionResult> CreateStudyAsync(StudyDto newStudy)
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
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteStudyAsync(int studyId)
        {
            var study = await _studyService.DeleteStudyAsync(studyId);
            return new JsonResult(study);
        }
       
       
        [HttpPut("{studyId}/details")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Authorize(Roles = RoleSets.AdminOrSponsor)]
        //TODO: Must also be possible for sponsor rep and other roles
        public async Task<IActionResult> UpdateStudyDetailsAsync(int studyId, StudyDto study)
        {
            var updatedStudy = await _studyService.UpdateStudyDetailsAsync(studyId, study);
            return new JsonResult(updatedStudy);
        }
        
        
        // For local development, this method requires a running instance of Azure Storage Emulator
        [HttpPut("{studyId}/logo")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = RoleSets.AdminOrSponsor)]
        //TODO: Must also be possible for sponsor rep/vendor admin or other study specific roles
        public async Task<IActionResult> AddLogo(int studyId, [FromForm(Name = "image")] IFormFile studyLogo)
        {
            var updatedStudy = await _studyService.AddLogoAsync(studyId, studyLogo);
            return new JsonResult(updatedStudy);
        }

        [HttpGet("{studyId}/logo")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Octet)]

        //Is study restricted? Then check if user can view restricted studies
        // For local development, this method requires a running instance of Azure Storage Emulator
        public async Task<IActionResult> GetLogo(int studyId)
        {
            var study = await _studyService.GetStudyByIdAsync(studyId);

            if (study.Restricted && CanViewRestrictedStudy() == false)
            {
                return new ForbidResult();
            }

            byte[] logo = await _studyService.GetLogoAsync(studyId);
            var studyDtoFromDb = await _studyService.GetStudyByIdAsync(studyId);
            string fileType = studyDtoFromDb.LogoUrl.Split('.').Last();
            if (fileType.Equals("jpg"))
            {
                fileType = "jpeg";
            }
            return File(new System.IO.MemoryStream(logo), $"image/{fileType}", $"logo_{studyId}.{fileType}");
            //return new ObjectResult(logo);
        }     

          
    }

}
