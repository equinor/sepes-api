using BrunoZell.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        readonly IStudyCreateService _studyCreateService;
        readonly IStudyUpdateService _studyUpdateService;
        readonly IStudyDeleteService _studyDeleteService;

        public StudyController(IStudyCreateService studyCreateService, IStudyUpdateService studyUpdateService, IStudyDeleteService studyDeleteService)
        {           
            _studyCreateService = studyCreateService;
            _studyUpdateService = studyUpdateService;
            _studyDeleteService = studyDeleteService;           
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

        [HttpPut("{studyId}/resultsandlearnings")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateResultsAndLearningsAsync(int studyId, StudyResultsAndLearningsDto resultsAndLearnings)
        {
            var resultsAndLearningsFromDb = await _studyUpdateService.UpdateResultsAndLearningsAsync(studyId, resultsAndLearnings);
            return new JsonResult(resultsAndLearningsFromDb);
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
    }
}
