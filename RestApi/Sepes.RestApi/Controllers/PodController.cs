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
    [Authorize]
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

        [HttpPost("nsg/add")]
        public async Task addNsg([FromBody] string securityGroupName, string subnetName, string networkId)
        {
            await _pod.addNsg(securityGroupName, subnetName, networkId);
        }

        [HttpPost("nsg/remove")]
        public async Task removeNsg([FromBody] string securityGroupName, string subnetName, string networkId)
        {

            await _pod.removeNsg(securityGroupName, subnetName, networkId);
        }

        [HttpPost("nsg/switch")]
        public async Task switchNsg([FromBody] string securityGroupNameOld, string securityGroupNameNew, string subnetName, string networkId)
        {
            await _pod.switchNsg(securityGroupNameOld, securityGroupNameNew, subnetName, networkId);
        }
        [HttpDelete("nsg/purgeunused")]
        public async Task<UInt16> deleteUnused()
        {
            return await _pod.deleteUnused();
        }
    }

}
