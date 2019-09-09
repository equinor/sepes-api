using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudyController : ControllerBase
    {
        //Create study
        [HttpPost("create")]
        public void Post([FromBody] string value)
        {
            /*
            Unpack data from browser
            Verify selections are member that exist
            if this is true then
                add study to database
            else
                return=Invalidselection
            end
            
             */
        }
        //Get list of studies
        [HttpGet("list")]
        public void Get([FromBody] string value)
        {
            //Read from database and return list of current studies.
        }
    }

}
