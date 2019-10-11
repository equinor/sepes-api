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
        private ISepesDb sepesDb = new SepesDb();
        private IAzPod  azPod   = new AzPod();

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
        public void createPod([FromBody] Pod input) //Create/implement pod model.
        {
            //1. Create pod resource group in azure
            azPod.CreatePodResourceGroup(input.podID, input.podName, input.podTag, azure);
            //2. Get info from azure to verify creation?
            //3. Commit info to database
        }
    }

}
