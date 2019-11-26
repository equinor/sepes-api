using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    //[Authorize]
    public class StudyController : ControllerBase
    {
        private ISepesDb _sepesDb;
        private IStudyService _studyService;

        public StudyController(ISepesDb dbService, IStudyService studyService)
        {
            _sepesDb = dbService;
            _studyService = studyService;
        }


        [HttpPost("save")]
        public async Task<StudyInput> SaveStudy([FromBody] StudyInput[] studies)
        {
            if (studies[1] == null) {
                Study study = await _studyService.Save(studies[0].ToStudy(), null);
                return study.ToStudyInput();
            }

            
            return studies[0];
        }


        //Create study
        [HttpPost("create")]
        public async Task<int> CreationVars([FromBody] Study value)
        {
            return await _sepesDb.createStudy(value.studyName, value.userIds, value.datasetIds);
        }

        //Update study
        [HttpPost("update")]
        public int UpdateVars([FromBody] Study study)
        {
            return 0;
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
