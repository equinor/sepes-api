using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto;
using Sepes.RestApi.Services;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IAuthService _auth;
        public AuthController(IAuthService authService)
        {
            _auth = authService;
        }

        //POST api/auth/token
        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<string> Token([FromBody] AzTokenClass AzToken)
        {
            ActionResult response = Unauthorized(); //Default response if bellow fail
            var SepesToken = await _auth.GenerateJSONWebToken();
            //Issue 41: look into error handling for identifiable conditions. Ex. if azure verification timed out.
            return SepesToken;
        }
        
        //Takes old token, and generates a new token.
        [Authorize]
        [HttpPost("refreshtoken")]
        public async Task<string> RefreshToken([FromBody] SepesTokenClass OldSepesTokenString)
        {
            //Issue 38 this depending on if we have any traits we need to pass on or not this section of the controller may not need any logic at all.
            ActionResult response = Unauthorized(); //Default response if bellow fails
            var SepesToken = await _auth.GenerateJSONWebToken();
            return SepesToken;
        }
    }
}
