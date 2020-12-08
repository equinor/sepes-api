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
        readonly IStudyService _studyService;
        readonly IStudyLogoService _studyLogoService;

        public StudyController(IStudyService studyService, IStudyLogoService studyLogoService)
        {
            _studyService = studyService;
            _studyLogoService = studyLogoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudiesAsync([FromQuery] bool? excludeHidden)
        {
            var studies = await _studyService.GetStudyListAsync(excludeHidden);
            return new JsonResult(studies);
        }

        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetStudyAsync(int studyId)
        {
            var study = await _studyService.GetStudyDetailsDtoByIdAsync(studyId, UserOperation.Study_Read);
            return new JsonResult(study);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudyAsync(StudyCreateDto newStudy)
        {
            var study = await _studyService.CreateStudyAsync(newStudy);
            return new JsonResult(study);
        }

        [HttpDelete("{studyId}")]
        public async Task<IActionResult> DeleteStudyAsync(int studyId)
        {
            await _studyService.CloseStudyAsync(studyId); //Todo: Switch to correct method
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
            var updatedStudy = await _studyService.UpdateStudyMetadataAsync(studyId, study);
            return new JsonResult(updatedStudy);
        }

        [HttpGet("{studyId}/resultsandlearnings")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetResultsAndLearningsAsync(int studyId)
        {
            var resultsAndLearningsFromDb = await _studyService.GetResultsAndLearningsAsync(studyId);
            return new JsonResult(resultsAndLearningsFromDb);
        }

        [HttpPut("{studyId}/resultsandlearnings")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateResultsAndLearningsAsync(int studyId, StudyResultsAndLearningsDto resultsAndLearnings)
        {
            var resultsAndLearningsFromDb = await _studyService.UpdateResultsAndLearningsAsync(studyId, resultsAndLearnings);
            return new JsonResult(resultsAndLearningsFromDb);
        }

        // For local development, this method requires a running instance of Azure Storage Emulator
        [HttpPut("{studyId}/logo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddLogo(int studyId, [FromForm(Name = "image")] IFormFile studyLogo)
        {
            var updatedStudy = await _studyLogoService.AddLogoAsync(studyId, studyLogo);
            return new JsonResult(updatedStudy);
        }
    }
}
