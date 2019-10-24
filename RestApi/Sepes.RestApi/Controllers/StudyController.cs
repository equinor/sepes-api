using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class StudyController : ControllerBase
    {
        private ISepesDb _sepesDb;

        public StudyController(ISepesDb dbService)
        {
            _sepesDb = dbService;
        }

        //Create study
        [HttpPost("create")]
        public async Task<int> CreationVars([FromBody] Study value)
        {
            return await _sepesDb.createStudy(value.studyName, value.userIds, value.datasetIds);
        }

        //Update study
        [HttpPost("update")]
        public async Task<int> UpdateVars([FromBody] Study study)
        {
            return await _sepesDb.updateStudy(study);
        }

        //Get list of studies
        [HttpGet("list")]
        public async Task<string> GetStudies()
        {
            return await _sepesDb.getStudies(false);
        }

        [HttpGet("archived")]
        public async Task<string> GetArchivedStudies()
        {
            return await _sepesDb.getStudies(true);
        }

        [HttpGet("dataset")]
        public async Task<string> GetDataset()
        {
            return await _sepesDb.getDatasetList();
        }
    }

}
