using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    //[Authorize]
    public class StudyController : ControllerBase
    {
        public IConfiguration Configuration {get; set;}
        private SepesDb sepesDb = new SepesDb();
        
        //Create study
        [HttpPost("create")]
        public int CreationVars([FromBody] JObject value)
        {
            /*
            Unpack data from browser
            Verify selections are member that exist
            if this is true then
                add study to database
            else
                return=Invalidselection
            */
            return sepesDb.createStudy(value);
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
        public ActionResult<string> Get()
        {
            return sepesDb.getDatasetList();
        }
    }


    public class Study {
        public string studyName { get; set; }
    }

}
