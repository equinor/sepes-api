﻿using System;
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
        private readonly IPodService _pod;
        public PodController(IPodService podService) {
            _pod = podService;
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
        public async Task<ActionResult<Pod>> createPod([FromBody] PodInput input)
        {
            var pod = await _pod.CreateNewPod(input.podName, input.studyID);
            return Ok(pod);
        }
    }

}
