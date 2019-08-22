using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        //DEBUG
        private AppSettings _appSetting;
        public AuthController(IOptions<AppSettings> appsettings)
        {
            _appSetting = appsettings.Value;
        }
        //END DEBUG
         //POST api/auth/token
        [AllowAnonymous]
        [HttpPost("token")]
        public IActionResult Token([FromBody] AzToken AzToken) 
        {
            IActionResult response = Unauthorized();
            //Logic for testing if recieved data is a real token
            //Logic for swapping Az AD token for Sepes token
            var IsAuthentic = AuthenticateToken(AzToken);
            //TODO Defauult response to a unauthorized or similar response
            
            if (IsAuthentic){
                var SepesToken = GenerateJSONWebToken(AzToken);
                //response = Ok(new { Token = SepesToken}); //Alternative method to return wrapped in token. In case we need more attributes later.
                response = Ok(SepesToken);
            }
            else{
                //TODO look into error handling if azure verification timed out.
                response = Unauthorized("Invalid Azure token.");
            }


            
            //Check if creation of token is automatically logged, if not add custom logging that reports what userid got what token.
            
            return response;
        }

        private string GenerateJSONWebToken(AzToken AZtoken)
        {
            var JwtKey = _appSetting.Key;
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(JwtKey));  //Encoding.UTF8.GetBytes(configreader["Jwt:Key"])
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); 

            var token = new JwtSecurityToken(_appSetting.Issuer,  
            "Testaudience",  
            null,  
            //TODO add in the token user so that will be usable for logging.
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(120),  
            signingCredentials: credentials);  
  
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private bool AuthenticateToken(AzToken AZtoken)
        {
            //TODO Logic for authenticating the azure token.
            return true; //Always returns true for testing purpose
        }

    }

    public class AzToken{
        //TODO add parameters to token
        public string Username { get; set; }
        public string idToken { get; set; }
        public string Expiration { get; set; }
    }

}
