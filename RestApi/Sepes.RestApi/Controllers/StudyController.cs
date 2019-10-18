using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class StudyController : ControllerBase
    {
        private ISepesDb sepesDb;

        public StudyController(ISepesDb dbService) {
            sepesDb = dbService;
        }
        
        //Create study
        [HttpPost("create")]
        public int CreationVars([FromBody] Study value)
        {
            return sepesDb.createStudy(value);
        }
        
        //Update study
        [HttpPost("update")]
        public void UpdateVars([FromBody] string value)
        {
            throw new NotImplementedException();
        }

        //Get list of studies
        [HttpGet("list")]
        public string GetStudies()
        {
            return sepesDb.getStudies(false);
        }

        [HttpGet("archived")]
        public string GetArchivedStudies()
        {
            return sepesDb.getStudies(true);
        }
    }

}
