using System;
using System.Collections.Generic;
using System.Linq;
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
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;


namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class UserController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }
        //private SepesDb sepesDb;

        /*[HttpPost("create")]
        public int CreationVars([FromBody] User value)
        {
            return sepesDb.createUser(value);
        }*/

        [HttpPost("update")]
        public void UpdateVars([FromBody] string value)
        {
            throw new NotImplementedException();
        }

        [HttpGet("list")]
        public JObject Get()
        {
            throw new NotImplementedException("Function not yet implemented, use search api instead");
            //return sepesDb.getUserList(); //May not be implemented or might be a limited search
        }
        //TODO view action
    }

}
