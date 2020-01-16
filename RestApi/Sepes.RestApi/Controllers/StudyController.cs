using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

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
            else {
                Study study = await _studyService.Save(studies[0].ToStudy(), studies[1].ToStudy());
                return study.ToStudyInput();
            }
        }

        //Get list of studies
        [HttpGet("list")]
        public IEnumerable<StudyInput> GetStudies()
        {
            return _studyService.GetStudies(new User("","",""), false).Select(study => study.ToStudyInput());
        }

        [HttpGet("archived")]
        public IEnumerable<StudyInput> GetArchived()
        {
            return _studyService.GetStudies(new User("","",""), true).Select(study => study.ToStudyInput());
        }

        [HttpGet("dataset")]
        public async Task<string> GetDataset()
        {
            return await _sepesDb.getDatasetList();
        }
    }

}
