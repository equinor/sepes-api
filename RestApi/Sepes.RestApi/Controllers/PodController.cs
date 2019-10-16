using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
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
        public IConfiguration Configuration { get; set; }

        private ISepesDb _sepesDb;
        private IAzureService _azPod;

        public PodController(ISepesDb sepesDb, IAzureService azPod) {
            _sepesDb = sepesDb;
            _azPod = azPod;
        }

        /*[HttpPost("create")]
        public int CreationVars([FromBody] Pod value)
        {
            return sepesDb.createPod(value); //Needs normal variables plus the studyID it belongs to.
        }*/

        [HttpPost("update")]
        public void UpdateVars([FromBody] string value)
        {
            throw new NotImplementedException();
        }

        /*[HttpGet("list")]
        public JObject Get([FromBody] Pod input)
        {
            return sepesDb.getPodList(input); //needs the study id to find pods from
        }*/
        //TODO view function

        [HttpPost("create")]
        public async Task<IActionResult> createPod([FromBody] Pod pod)
        {
            //Check for tags needed, if not found make them
            pod.podID =  await _sepesDb.createPod(pod);
            if (!(pod.podID == -1)){
                await _azPod.CreateNetwork(pod);
            }
            else{
                return StatusCode(500, "Unable to create pod in database");
            }
            

            //1. Create pod resource group in azure
            return Ok();
            //_azPod.CreateResourceGroup(input.studyID, input.podName, input.podTag);

            //2. Get info from azure to verify creation?
            //3. Commit info to database
        }
    }

}
