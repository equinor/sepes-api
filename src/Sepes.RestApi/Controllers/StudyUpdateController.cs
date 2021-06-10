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
    public class StudyUpdateController : ControllerBase
    { 
        readonly IStudyUpdateService _studyUpdateService;
        readonly IStudyDetailsService _studyDetailsService;

        public StudyUpdateController(IStudyUpdateService studyUpdateService, IStudyDetailsService studyDetailsService)
        { 
            _studyUpdateService = studyUpdateService;
            _studyDetailsService = studyDetailsService;
        } 
       

        [HttpPut("{studyId}/details")]       
        public async Task<IActionResult> UpdateStudyDetailsAsync(int studyId,
               [ModelBinder(BinderType = typeof(JsonModelBinder))] StudyUpdateDto study,
               IFormFile image = null)
        {
            var updatedStudy = await _studyUpdateService.UpdateMetadataAsync(studyId, study, image);
            var studyDetails = await _studyDetailsService.Get(studyId);
            return new JsonResult(studyDetails);
        }

        [HttpPut("{studyId}/resultsandlearnings")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateResultsAndLearningsAsync(int studyId, StudyResultsAndLearningsDto resultsAndLearnings)
        {
            var resultsAndLearningsFromDb = await _studyUpdateService.UpdateResultsAndLearningsAsync(studyId, resultsAndLearnings);
            return new JsonResult(resultsAndLearningsFromDb);
        }
    }
}
