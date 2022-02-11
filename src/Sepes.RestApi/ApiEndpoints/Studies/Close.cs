using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.Studies
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Close : ControllerBase
    {
        readonly IStudyDeleteService _studyDeleteService;

        public Close(IStudyDeleteService studyDeleteService)
        {
            _studyDeleteService = studyDeleteService;
        }

        [HttpPut("{studyId}/close")]
        public async Task<IActionResult> Handle(int studyId)
        {
            await _studyDeleteService.CloseStudyAsync(studyId);
            return new NoContentResult();
        }
    }
}
