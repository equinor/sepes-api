using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System;

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
        public async Task<ActionResult<StudyInput>> SaveStudy([FromBody] StudyInput[] studies)
        {
            //Studies [1] is what the frontend claims the changes is based on while Studies [0] is the new version
            
            
            //If based is null it can be assumed this will be a newly created study
            if (studies[1] == null)
            {
                Study study = await _studyService.Save(studies[0].ToStudy(), null);
                return study.ToStudyInput();
            }
            //If based has more pods than new version then it can be assumed pods are requested to be deleted.
            else if (studies[1].pods.Count() > studies[0].pods.Count())
            {
                
                List<ushort?> podIdBased = new List<ushort?>();
                List<ushort?> podIdNew = new List<ushort?>();
                
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
                                return StatusCode(500, "Deletion status unknown. Refresh browser and check again.");
                            }
                        }
                    }
                }
                else
                {
                    return BadRequest("Too many deletion requests. Only single deletions supported at this time.");
                }
                return BadRequest("Unable to process request");
            }
            //Otherwise it must be a change.
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
