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
        public void CreationVars([FromBody] string value)
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
        //Update study
        [HttpPost("update")]
        public void UpdateVars([FromBody] string value)
        {
            /*  
                Check if user is authorized to modify the study
                Compare updated values with old values of study with same ID
                Authorized = Athorize(UpdateVars); //Ask OPA if user authorized
                if Authorized then{
                    if StudyIDfound then{//Can likely be removed as that would be part of authorisation check
                        compare records
                        if RecordChangeValid then{
                            update record;
                            return= succes;
                        }
                        else{
                            return =Invalid change
                        }
                    }
                    else{
                        return= Record not found
                    }
                }
                else{
                    return=unauthorized;
                }
             */
        }
        //Get list of studies
        [HttpGet("list")]
        public void Get([FromBody] string value)
        {
            //Read from database and return list of current studies.
            //Might need to make custom class so we can get an array with multiple fields to each position
        }
    }

}
