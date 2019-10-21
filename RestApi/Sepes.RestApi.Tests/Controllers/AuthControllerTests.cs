using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Sepes.RestApi;
using Sepes.RestApi.Controller;
using System.Linq;
using System.Text;
using System;
using System.Text.Json;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Sepes.RestApi.Model;
using Microsoft.Extensions.Options;
using Sepes.RestApi.Services;

namespace Sepes.RestApi.Tests.Controller
{
    public class AuthControllerTests
    {
        private static string key = "erfagidfi9yhjeropgbhsrietjjksdjgtjklenfgophirgdhtjfxpiogjh";
        [Fact]
        public void GetToken()
        {
            var JwtPackage = new AzTokenClass();
            JwtPackage.idToken = "r52sefsdf";
            AppSettings _settings = new AppSettings();
            _settings.Key = key;
            _settings.Issuer = "TestIssuer";
            var _option = Options.Create(_settings);
            IAuthService _authService = new AuthService(_option);
            AuthController controller = new AuthController(_authService); //Later move to test fixture, as same object can be reused.
            var token = controller.Token(JwtPackage);
            var tokencontent = token.Value.ToString();
            var attempt = new JwtSecurityTokenHandler().ReadToken(tokencontent);
        }
        [Fact]
        public void RefreshToken()
        {
            var JwtPackage = new AzTokenClass();
            JwtPackage.idToken = "dsfgdsfs";
            AppSettings _settings = new AppSettings();
            _settings.Key = key;
            _settings.Issuer = "TestIssuer";
            var _option = Options.Create(_settings);
            IAuthService _authService = new AuthService(_option);
            var controller = new AuthController(_authService); 
            var TestSepesToken = new SepesTokenClass();
            var token = controller.Token(JwtPackage);
            TestSepesToken.idToken = token.Value.ToString(); //Tokencontent must be renamed in previous test and moved to class.
            var result = controller.RefreshToken(TestSepesToken);
            var attempt = new JwtSecurityTokenHandler().ReadToken(result.Value.ToString());
        }
    }
}