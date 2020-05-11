using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto;
using Sepes.RestApi.Services;
using System.Collections.Generic;
using System.Linq;
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
        private IStudyService _studyService;

        public StudyController(ISepesDb dbService, IStudyService studyService)
        {
            _sepesDb = dbService;
            _studyService = studyService;
        }

        [HttpPost("save")]
        public async Task<ActionResult<StudyInputDto>> SaveStudy([FromBody] StudyInputDto[] studies)
        {
            //Studies [1] is what the frontend claims the changes is based on while Studies [0] is the new version
            
            //If based is null it can be assumed this will be a newly created study
            if (studies[1] == null)
            {
                StudyDto study = await _studyService.Save(studies[0].ToStudy(), null);
                return study.ToStudyInput();
            }
            //Otherwise it must be a change.
            else
            {
                StudyDto study = await _studyService.Save(studies[0].ToStudy(), studies[1].ToStudy());
                return study.ToStudyInput();
            }
        }

        //Get list of studies
        [HttpGet("list")]
        public IEnumerable<StudyInputDto> GetStudies()
        {
            return _studyService.GetStudies(new UserDto("", "", ""), false).Select(study => study.ToStudyInput());
        }

        [HttpGet("archived")]
        public IEnumerable<StudyInputDto> GetArchived()
        {
            return _studyService.GetStudies(new UserDto("", "", ""), true).Select(study => study.ToStudyInput());
        }

        [HttpGet("dataset")]
        public async Task<string> GetDataset()
        {
            return await _sepesDb.getDatasetList();
        }
    }

}
