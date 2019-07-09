using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sepes.restapi.ValuesController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

/*         //GET test funtion, always with opposite of input value
        [HttpGet("FlipBool")]
        public FlipBool<bool> Get(bool blOrig)
        {
            return blInvert;
        }
*/
         // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class VController : ControllerBase
                //What is before Controller decides path
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Yep", "its running" };
        }

        // GET api/v/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value: " + id;
        }

        [HttpGet("{a}/{b}")]
        public ActionResult<string> Get(int a, int b)
        {
            return "value: " + (a + b);
        }
    }
}
