using BrunoZell.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Handlers.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class StudyMetadataUpdateController : ControllerBase
    { 
        readonly IStudyUpdateHandler _studyUpdateHandler;
        readonly IStudyDetailsService _studyDetailsService;

        public StudyMetadataUpdateController(IStudyUpdateHandler studyUpdateHandler, IStudyDetailsService studyDetailsService)
        {
            _studyUpdateHandler = studyUpdateHandler;
            _studyDetailsService = studyDetailsService;
        }        

        [HttpPut("{studyId}/details")]       
        public async Task<IActionResult> UpdateStudyDetailsAsync(int studyId,
               [ModelBinder(BinderType = typeof(JsonModelBinder))] StudyUpdateDto study,
               IFormFile image = null)
        {
            var spUpdate = Stopwatch.StartNew();

            var updatedStudy = await _studyUpdateHandler.UpdateAsync(studyId, study, image);

            var afterUpdate = spUpdate.ElapsedMilliseconds;
            spUpdate.Restart();

            var studyDetails = await _studyDetailsService.Get(studyId);

            var afterGetGetauls = spUpdate.ElapsedMilliseconds;          

            return new JsonResult(studyDetails);
        }       
    }
}
