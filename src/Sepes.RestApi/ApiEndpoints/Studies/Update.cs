﻿using BrunoZell.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Handlers.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.Studies
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Update : ControllerBase
    {
        readonly IStudyUpdateHandler _studyUpdateHandler;
        readonly IStudyDetailsService _studyDetailsService;

        public Update(IStudyUpdateHandler studyUpdateHandler, IStudyDetailsService studyDetailsService)
        {
            _studyUpdateHandler = studyUpdateHandler;
            _studyDetailsService = studyDetailsService;
        }

        [HttpPut("{studyId}/details")]
        public async Task<IActionResult> Handle(int studyId,
               [ModelBinder(BinderType = typeof(JsonModelBinder))] StudyUpdateDto study,
               IFormFile image = null)
        {
            _ = await _studyUpdateHandler.UpdateAsync(studyId, study, image);
            var studyDetails = await _studyDetailsService.Get(studyId);

            return new JsonResult(studyDetails);
        }
    }
}
