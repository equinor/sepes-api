using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Service.Interface;
using BrunoZell.ModelBinding;
using Sepes.Common.Dto.Study;

namespace Sepes.RestApi.ApiEndpoints.Studies
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Create : ControllerBase
    {
        readonly IStudyCreateService _studyCreateService;
        readonly IStudyDetailsService _studyDetailsService;
        public Create(IStudyDetailsService studyDetailsService, IStudyCreateService studyCreateService)
        {
            _studyDetailsService = studyDetailsService;
            _studyCreateService = studyCreateService;
        }

        [HttpPost]
        public async Task<IActionResult> Handle([ModelBinder(BinderType = typeof(JsonModelBinder))] StudyCreateDto study, IFormFile image = null)
        {
            var createdStudy = await _studyCreateService.CreateAsync(study, image);
            var studyDetails = await _studyDetailsService.Get(createdStudy.Id);

            return new JsonResult(studyDetails);
        }
    }
}
