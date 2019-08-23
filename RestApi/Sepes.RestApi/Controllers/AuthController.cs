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
        public IActionResult Token([FromBody] AzTokenClass AzToken) 
        {
            IActionResult response = Unauthorized();
            //Logic for testing if recieved data is a real token
            //Logic for swapping Az AD token for Sepes token
            var IsAuthentic = AuthenticateToken(AzToken);
            
            if (IsAuthentic){
                var SepesToken = GenerateJSONWebToken(AzToken, null);
                //response = Ok(new { Token = SepesToken}); //Alternative method to return wrapped in token. In case we need more attributes later.
                response = Ok(SepesToken);
            }
            else{
                //TODO look into error handling for identifiable conditions. Ex. if azure verification timed out.
                response = Unauthorized("Invalid Azure token.");
            }


            
            //Check if creation of token is automatically logged, if not add custom logging that reports what userid got what token.
            
            return response;
        }
        //Takes old token, and generates a new token.
        [Authorize]
        [HttpPost("refreshtoken")]
        public IActionResult RefreshToken([FromBody] SepesTokenClass OldSepesTokenString) 
        {
            IActionResult response = Unauthorized();
            //TODO logic for verifying and then making new sepestoken.
            //Authentication implied by [Authorize] but some content of old token may be transplanted to new token.
            
            var SepesToken = GenerateJSONWebToken(null ,new JwtSecurityTokenHandler().ReadJwtToken(OldSepesTokenString.idToken));
            //Either change GenerateJSONWebToken to support either AzToken or SepeToken. Or make new function for refresh
            response = Ok(SepesToken);
            return response;
        }

        private string GenerateJSONWebToken(AzTokenClass AZtoken,JwtSecurityToken OldSepesToken)
        {

            if (OldSepesToken != null){
                //Logic for bringing transplanted traits into next token
            }
            else if (AZtoken != null){
                //Logic for any transplanting traits from AzToken to sepestoken
            }
            else {
                //Throw exception as this code should never be reached
                throw new ArgumentNullException("Both arguments can not be null");
            }
            var JwtKey = _appSetting.Key;
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(JwtKey));  //Encoding.UTF8.GetBytes(configreader["Jwt:Key"])
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); 

            var token = new JwtSecurityToken(_appSetting.Issuer,  //Issuer
            _appSetting.Issuer,//Audience  
            null,  
            //TODO add in the token user so that will be usable for logging.
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(120),  //TODO judge how long tokens should live
            signingCredentials: credentials);  
  
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        private bool AuthenticateToken(AzTokenClass AZtoken)
        {
            //TODO Logic for authenticating the azure token.
            return true; //Always returns true for testing purpose
        }

    }

    public class AzTokenClass{
        //TODO add parameters to token
        public string Username { get; set; }
        public string idToken { get; set; }
        public string Expiration { get; set; }
    }

       public class SepesTokenClass{
        //Judge what parameters are needed for just refreshing. Username and such should be in token content body
        public string Username { get; set; }
        public string idToken { get; set; }
        public string Expiration { get; set; }
    }

}
