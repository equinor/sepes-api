using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json.Linq;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;

namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    //[Authorize]
    public class StudyController : ControllerBase
    {
        private ISepesDb sepesDb;

        public StudyController(ISepesDb dbService) {
            sepesDb = dbService;
        }
        
        //Create study
        /*[HttpPost("create")]
        public int CreationVars([FromBody] Study value)
        {
            return sepesDb.createStudy(value);
        }*/
        
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
        public string Get()
        {
            return sepesDb.getDatasetList();
        }
    }

}
