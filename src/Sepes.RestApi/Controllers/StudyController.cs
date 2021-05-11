using BrunoZell.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
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
            var study = await _studyReadService.GetStudyDetailsAsync(studyId);
            return new JsonResult(study);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudyAsync(
            [ModelBinder(BinderType = typeof(JsonModelBinder))] StudyCreateDto study,
            IFormFile image = null)
        {
            var createdStudy = await _studyCreateService.CreateAsync(study, image);          

            return new JsonResult(createdStudy);
        }

        [HttpPut("{studyId}/details")]       
        public async Task<IActionResult> UpdateStudyDetailsAsync(int studyId,
               [ModelBinder(BinderType = typeof(JsonModelBinder))] StudyUpdateDto study,
               IFormFile image = null)
        {
            var updatedStudy = await _studyUpdateService.UpdateMetadataAsync(studyId, study, image);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{studyId}")]
        public async Task<IActionResult> DeleteStudyAsync(int studyId)
        {
            await _studyDeleteService.DeleteStudyAsync(studyId);
            return new NoContentResult();
        }

        [HttpPut("{studyId}/close")]
        [Authorize]
        public async Task<IActionResult> CloseStudyAsync(int studyId)
        {
            await _studyDeleteService.CloseStudyAsync(studyId);
            return new NoContentResult();
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
