using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Services
{
    public class AuthService : IAuthService
    {
        private AppSettings _appSetting;
        public AuthService(AppSettings appsettings)
        {
            _appSetting = appsettings;
        }

        public Task<string> GenerateJSONWebToken(string token)
        {
            var JwtKey = _appSetting.Key;
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(JwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Issue: 38 check if we can add in proper issuer and audience now. or if that has to be done at a later date. Likely going to be part of integration.
            var newtoken = new JwtSecurityToken(_appSetting.Issuer,  //Issuer
            _appSetting.Issuer,//Audience  
            null,
            //Issue: 38 suggested that a user is added to token for logging, also check how we can get user to be appended to actions performed in api
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(newtoken));
        }
    }
}