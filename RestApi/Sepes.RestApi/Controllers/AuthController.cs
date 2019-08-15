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
using Microsoft.IdentityModel.Tokens;

namespace Sepes.RestApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        //DEBUG

        //public IConfiguration configreader;
        byte[] tempsecretkey = System.Text.Encoding.UTF8.GetBytes("secretkey2asfafds56865"); 
        //END DEBUG
         //POST api/auth/token
        [AllowAnonymous]
        [HttpPost("token")]
        public IActionResult Token([FromBody] AzToken AzToken) //TODO Get a proper datatype for the token
        {
            IActionResult response = Unauthorized();
            //Logic for testing if recieved data is a real token
            //Logic for swapping Az AD token for Sepes token
            var IsAuthentic = AuthenticateToken(AzToken);
            
            if (IsAuthentic){
                var SepesToken = GenerateJSONWebToken(AzToken);
                response = Ok(new { Token = SepesToken});
            }


            
            //Check if creation of token is automatically logged, if not add custom logging that reports what userid got what token.
            
            return response;
            //return SepesToken.ToList(); Needs a return object.
        }

        private string GenerateJSONWebToken(AzToken AZtoken)
        {
            var securityKey = new SymmetricSecurityKey(tempsecretkey);  //Encoding.UTF8.GetBytes(configreader["Jwt:Key"])
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); 

            var token = new JwtSecurityToken("Testissuer",  
            "Testissuer",  
            null,  
            //TODO add in the token user so that will be usable for logging.
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(120),  
            signingCredentials: credentials);  //configreader["Jwt:Issuer"] instead of the string on two first parameters
  
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
