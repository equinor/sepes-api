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
    public class Details : ControllerBase
    {
        readonly IStudyDetailsService _studyDetailsService;

        public Details(IStudyDetailsService studyDetailsService)
        {
            _studyDetailsService = studyDetailsService;
        }

        [HttpGet("{studyId}")]
        public async Task<IActionResult> Handle(int studyId)
        {
            var study = await _studyDetailsService.Get(studyId);
            return new JsonResult(study);
        }
    }
}
