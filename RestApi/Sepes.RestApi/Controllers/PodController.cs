using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Sepes.RestApi.Services;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [EnableCors("_myAllowSpecificOrigins")]
    public class PodController : ControllerBase
    {
        private readonly IPodService _pod;
        public PodController(IPodService podService)
        {
            _pod = podService;
        }

        [HttpPost("update")]
        public void UpdateVars([FromBody] string value)
        {
            throw new NotImplementedException();
        }

        [HttpPost("create")]
        public async Task<Pod> createPod([FromBody] PodInput input)
        {
            return await _pod.CreateNewPod(input.podName, input.studyID);
        }

        [HttpGet("list/{studyId}")]
        public async Task<string> getPods(int studyId)
        {
            return await _pod.GetPods(studyId);
        }
    }

}
