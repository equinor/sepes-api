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
    public class Delete : ControllerBase
    {
        readonly IStudyDeleteService _studyDeleteService;

        public Delete(IStudyDeleteService studyDeleteService)
        {
            _studyDeleteService = studyDeleteService;
        }

        [HttpDelete("{studyId}")]
        public async Task<IActionResult> Handle(int studyId)
        {
            await _studyDeleteService.DeleteStudyAsync(studyId);
            return new NoContentResult();
        }
    }
}
