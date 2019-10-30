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

        [HttpPost("nsg/create")]
        public async Task createNsg([FromBody] NsgModel data)
        {
            await _pod.createNsg(data.securityGroupName, data.resourceGroupName);
        }
        [HttpPost("nsg/delete")]
        public async Task deleteNsg([FromBody] NsgModel data)
        {
            await _pod.deleteNsg(data.securityGroupName, data.resourceGroupName);
        }



        [HttpPost("nsg/apply")]
        public async Task applyNsg([FromBody] NsgModel data)
        {
            await _pod.applyNsg(data.resourceGroupName, data.securityGroupName, data.subnetName, data.networkName);
        }

        [HttpPost("nsg/remove")]
        public async Task removeNsg([FromBody] NsgModel data)
        {

            await _pod.removeNsg(data.resourceGroupName, data.subnetName, data.networkName);
        }

        [HttpPost("nsg/switch")]
        public async Task switchNsg([FromBody] NsgModel data)
        {
            await _pod.switchNsg(data.resourceGroupName, data.securityGroupName, data.subnetName, data.networkName);
        }
        [HttpDelete("nsg/purgeunused")]
        public async Task<UInt16> deleteUnused()
        {
            return await _pod.deleteUnused();
        }
    }

}
