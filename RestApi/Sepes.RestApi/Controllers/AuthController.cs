using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;

namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IAuthService _auth;
        public AuthController(IAuthService authService)
        {
            _auth = authService;
        }

        //POST api/auth/token
        [AllowAnonymous]
        [HttpPost("token")]
        public IActionResult Token([FromBody] AzTokenClass AzToken)
        {
            IActionResult response = Unauthorized(); //Default response if bellow fail
            var SepesToken = _auth.GenerateJSONWebToken(AzToken.idToken, null);
            response = Ok(SepesToken);
            //Issue 41: look into error handling for identifiable conditions. Ex. if azure verification timed out.
            return response;
        }
        //Takes old token, and generates a new token.
        [Authorize]
        [HttpPost("refreshtoken")]
        public IActionResult RefreshToken([FromBody] SepesTokenClass OldSepesTokenString)
        {
            //Issue 38 this depending on if we have any traits we need to pass on or not this section of the controller may not need any logic at all.
            IActionResult response = Unauthorized(); //Default response if bellow fails
            var SepesToken = _auth.GenerateJSONWebToken(null, OldSepesTokenString.idToken);
            response = Ok(SepesToken);
            return response;
        }
    }
}
