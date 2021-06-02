using BrunoZell.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class StudyCreateController : ControllerBase
    {     
        readonly IStudyCreateService _studyCreateService;
        readonly IStudyDetailsService _studyDetailsService;


        public StudyCreateController(IStudyCreateService studyCreateService, IStudyDetailsService studyDetailsService)
        {           
            _studyCreateService = studyCreateService;
            _studyDetailsService = studyDetailsService;
        }         

        [HttpPost]
        public async Task<IActionResult> CreateStudyAsync(
            [ModelBinder(BinderType = typeof(JsonModelBinder))] StudyCreateDto study,
            IFormFile image = null)
        {
            var createdStudy = await _studyCreateService.CreateAsync(study, image);
            var studyDetails = await _studyDetailsService.Get(createdStudy.Id);

            return new JsonResult(studyDetails);
        }
    }
}
