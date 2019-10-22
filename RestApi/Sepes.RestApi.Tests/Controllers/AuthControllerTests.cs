using Xunit;
using Sepes.RestApi.Controller;
using System.IdentityModel.Tokens.Jwt;
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
            IAuthService _authService = new AuthService(_settings);
            AuthController controller = new AuthController(_authService); //Later move to test fixture, as same object can be reused.
            var token = controller.Token(JwtPackage);
            var tokencontent = token.Result.ToString();
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
            IAuthService _authService = new AuthService(_settings);
            var controller = new AuthController(_authService);
            var TestSepesToken = new SepesTokenClass();
            var token = controller.Token(JwtPackage);
            TestSepesToken.idToken = token.Result.ToString(); //Tokencontent must be renamed in previous test and moved to class.
            var result = controller.RefreshToken(TestSepesToken);
            var attempt = new JwtSecurityTokenHandler().ReadToken(result.Result.ToString());
        }
    }
}