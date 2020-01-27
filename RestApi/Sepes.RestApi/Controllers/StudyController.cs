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
        public async Task<ActionResult<StudyInput>> SaveStudy([FromBody] StudyInput[] studies)
        {
            //Begin sketch, this code needs to be integrated in bellow checks. Reason for keeping it outside studyservice would be to be able to return actionresults for error handling in frontend
            //Error handling draft currently in [HttpPost("pod/delete")] and functional in there. 
            //Studies [1] is what the frontend claims the changes is based on while Studies [0] is the new version

            //These must happen only if no changes are needed. Maybe must be moved to studyservice
            List<ushort?> podIdBased = null;
            List<ushort?> podIdNew = null;

            foreach (PodInput podBased in studies[1].pods)
            {
                podIdBased.Add(podBased.podId);
            }
            foreach (PodInput podNew in studies[0].pods)
            {
                podIdNew.Add(podNew.podId);
            }

            var podIdDiff = podIdBased.Except(podIdNew);

            //Make sure only one deletion has been ordered
            if (podIdDiff.Count() == 1)
            {
                //Fetch the pod whose podId matches
                foreach (PodInput podIn in studies[1].pods)
                {
                    if (podIn.podId == podIdDiff.First())
                    {
                        //Perform delete
                        Pod podToDelete = podIn.ToPod();
                        var response = await _studyService.DeletePod(podToDelete);

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
                            return StatusCode(500, "Deletion status unknown. Refresh browser and check again.");
                        }
                    }
                }
            }
            else
            {
                return BadRequest("Too many deletion requests. Only single deletions supported at this time.");
            }
            //End sketch


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
