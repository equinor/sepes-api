using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
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
    public class StudyController : ControllerBase
    {
        readonly IStudyReadService _studyReadService;
        readonly IStudyCreateService _studyCreateService;
        readonly IStudyUpdateService _studyUpdateService;
        readonly IStudyDeleteService _studyDeleteService;
        readonly IStudyLogoService _studyLogoService;

        public StudyController(IStudyReadService studyReadService, IStudyCreateService studyCreateService, IStudyUpdateService studyUpdateService, IStudyDeleteService studyDeleteService,IStudyLogoService studyLogoService)
        {
            _studyReadService = studyReadService;
            _studyCreateService = studyCreateService;
            _studyUpdateService = studyUpdateService;
            _studyDeleteService = studyDeleteService;
            _studyLogoService = studyLogoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudiesAsync()
        {
            var studies = await _studyReadService.GetStudyListAsync();
            return new JsonResult(studies);
        }

        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetStudyAsync(int studyId)
        {
            var study = await _studyReadService.GetStudyDetailsDtoByIdAsync(studyId, UserOperation.Study_Read);
            return new JsonResult(study);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudyAsync(StudyCreateDto newStudy, IFormFile image = null)
        {
            var study = await _studyCreateService.CreateAsync(newStudy, image);          

            return new JsonResult(study);
        }     

        [HttpDelete("{studyId}")]
        public async Task<IActionResult> DeleteStudyAsync(int studyId)
        {
            await _studyDeleteService.CloseStudyAsync(studyId); //Todo: Switch to correct method
            return new NoContentResult();
        }

        //[HttpDelete("{studyId}")]
        //[Authorize]
        //public async Task<IActionResult> CloseStudyAsync(int studyId)
        //{
        //    await _studyService.CloseStudyAsync(studyId);
        //    return new NoContentResult();
        //}


        [HttpPut("{studyId}/details")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateStudyDetailsAsync(int studyId, StudyDto study)
        {
            var updatedStudy = await _studyUpdateService.UpdateMetadataAsync(studyId, study);
            return new JsonResult(updatedStudy);
        }

        [HttpGet("{studyId}/resultsandlearnings")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetResultsAndLearningsAsync(int studyId)
        {
            var resultsAndLearningsFromDb = await _studyReadService.GetResultsAndLearningsAsync(studyId);
            return new JsonResult(resultsAndLearningsFromDb);
        }

        [HttpPut("{studyId}/resultsandlearnings")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateResultsAndLearningsAsync(int studyId, StudyResultsAndLearningsDto resultsAndLearnings)
        {
            var resultsAndLearningsFromDb = await _studyUpdateService.UpdateResultsAndLearningsAsync(studyId, resultsAndLearnings);
            return new JsonResult(resultsAndLearningsFromDb);
        }

        // For local development, this method requires a running instance of Azure Storage Emulator
        [HttpPut("{studyId}/logo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddLogo(int studyId, [FromForm(Name = "image")] IFormFile studyLogo)
        {
            var logoUrl = await _studyLogoService.AddLogoAsync(studyId, studyLogo);
            return new JsonResult(logoUrl);
        }
    }
}
