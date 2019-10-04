using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Sepes.RestApi.Services;
using Newtonsoft.Json.Linq;


namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SearchController : ControllerBase
    {
        private SepesDb sepesDb = new SepesDb();
        // GET api/search/user
        [HttpPost("user")]
        public IActionResult SearchUser([FromBody] JObject searchstring) //TODO change return type
        {
            Response.ContentType = "application/json";
            var result = sepesDb.searchUserList(searchstring);
            return Ok(result);
        }
        [HttpPost("dataset")]
        public IActionResult SearchDataset([FromBody] JObject searchstring)
        {
            Response.ContentType = "application/json";
            var result = sepesDb.searchDatasetList(searchstring);
            return Ok(result);
        }
        [HttpPost("study")]
        public IActionResult SearchStudy([FromBody] JObject searchstring)
        {
            Response.ContentType = "application/json";
            var result = sepesDb.searchStudyList(searchstring);
            return Ok(result);
        }
    }

}
