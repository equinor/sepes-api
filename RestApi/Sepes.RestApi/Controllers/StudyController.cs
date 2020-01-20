using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<StudyInput> SaveStudy([FromBody] StudyInput[] studies)
        {
            if (studies[1] == null)
            {
                Study study = await _studyService.Save(studies[0].ToStudy(), null);
                return study.ToStudyInput();
            }
            else
            {
                Study study = await _studyService.Save(studies[0].ToStudy(), studies[1].ToStudy());
                return study.ToStudyInput();
            }
        }

        [HttpPost("pod/delete")]
        //Gets the Based state from frontend to queue a delete
        //Checks pod against the study state
        public async Task<IActionResult> DeletePod([FromBody] PodInput podIn)
        {
            var response = await _studyService.DeletePod(podIn.ToPod());
            if (response == podIn.podId)
            {
                return Ok("Pod: " + podIn.podId + " deleted.");
            }
            else if (response == -1)
            {
                return BadRequest("Error: Pod does not match memory or is not found. Refresh and try again.");
            }
            else
            {
                return StatusCode(500,"Deletion may have suceeded. Refresh browser and check again");
            }
        }

        //Get list of studies
        [HttpGet("list")]
        public IEnumerable<StudyInput> GetStudies()
        {
            return _studyService.GetStudies(new User("", "", ""), false).Select(study => study.ToStudyInput());
        }

        [HttpGet("archived")]
        public IEnumerable<StudyInput> GetArchived()
        {
            return _studyService.GetStudies(new User("", "", ""), true).Select(study => study.ToStudyInput());
        }

        [HttpGet("dataset")]
        public async Task<string> GetDataset()
        {
            return await _sepesDb.getDatasetList();
        }
    }

}
