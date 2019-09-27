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

        //Make values from appsetting availible to this code
        private AppSettings _appSetting;
        public AuthController(IOptions<AppSettings> appsettings)
        {
            _appSetting = appsettings.Value;
        }
         //POST api/auth/token
        [AllowAnonymous]
        [HttpPost("token")]
        public IActionResult Token([FromBody] AzTokenClass AzToken) 
        {
            IActionResult response = Unauthorized(); //Default response if bellow fails
            var IsAuthentic = AuthenticateToken(AzToken);
            
            if (IsAuthentic){
                var SepesToken = GenerateJSONWebToken(AzToken, null);
                //response = Ok(new { Token = SepesToken}); //Alternative method to return wrapped in token. In case we need more attributes later.
                response = Ok(SepesToken);
            }
            else{
                //Issue 41: look into error handling for identifiable conditions. Ex. if azure verification timed out.
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
            //Issue 38 this depending on if we have any traits we need to pass on or not this section of the controller may not need any logic at all.
            IActionResult response = Unauthorized(); //Default response if bellow fails
            var SepesToken = GenerateJSONWebToken(null ,new JwtSecurityTokenHandler().ReadJwtToken(OldSepesTokenString.idToken));
            response = Ok(SepesToken);
            return response;
        }

        private string GenerateJSONWebToken(AzTokenClass AZtoken,JwtSecurityToken OldSepesToken)
        {

            if (OldSepesToken != null){
                //Issue: 38 Logic for bringing transplanted traits into new token
            }
            else if (AZtoken != null){
                //Issue: 38 Logic for any transplanting traits from AzToken to SepesToken
            }
            else {
                //Throw exception as this code should never be reached
                throw new ArgumentNullException("Both arguments can not be null");
            }
            var JwtKey = _appSetting.Key;
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(JwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); 

            //Issue: 38 check if we can add in proper issuer and audience now. or if that has to be done at a later date. Likely going to be part of integration.
            var token = new JwtSecurityToken(_appSetting.Issuer,  //Issuer
            _appSetting.Issuer,//Audience  
            null,  
            //Issue: 38 suggested that a user is added to token for logging, also check how we can get user to be appended to actions performed in api
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(120),  
            signingCredentials: credentials);  
  
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        private bool AuthenticateToken(AzTokenClass AZtoken)
        {
            //Issue: 38  Logic for authenticating the azure token.
            return true;
        }

    }

    public class AzTokenClass{
        //Issue: 38  add parameters to token
        public string Username { get; set; }
        public string idToken { get; set; }
        public string Expiration { get; set; }
    }

       public class SepesTokenClass{
        //Issue: 38 Judge what parameters are needed for just refreshing. Username and such should be in token content body
        public string Username { get; set; }
        public string idToken { get; set; }
        public string Expiration { get; set; }
    }

}
