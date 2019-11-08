using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Sepes.RestApi.Model;
using System.Diagnostics.CodeAnalysis;

namespace Sepes.RestApi.Controller
{
    [ExcludeFromCodeCoverage]//Only until this controller gets a function
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class UserController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }

        [HttpPost("create")]
        public int CreationVars([FromBody] User value)
        {
            throw new NotImplementedException();
        }

        [HttpPost("update")]
        public void UpdateVars([FromBody] string value)
        {
            throw new NotImplementedException();
        }

        [HttpGet("list")]
        public JObject Get()
        {
            throw new NotImplementedException();
        }
    }

}
