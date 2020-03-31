using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Sepes.RestApi.Services;
using Xunit;

namespace Sepes.Tests.Services
{
    public class ConfigServiceTests
    {
        public static ConfigService GetConfigService() {
            var aspConf = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string> {
                {"Jwt:Issuer", "SepesUnitTests"},
                {"Jwt:Key", "SecretKey"},
                {"Azure:CommonResourceGroupName", "SepesCommon"},
            }).Build();
            var sepesConf = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> {
                {"NAME", "Test"},
                {"TENANT_ID", "d1696363-d775-4580-b2b2-c7f80db35881"},
                {"CLIENT_ID", "bb8c9ce5-1f02-41e2-9449-2712bc4c2bd9"},
                {"CLIENT_SECRET", "It's a secret"},
                {"SUBSCRIPTION_ID", "38b95807-ff51-4781-80b9-50ab8b3b3f2a"},
                {"INSTRUMENTATION_KEY", "407c7027-9660-471d-9aa8-d1e4125794f2"},
                {"MSSQL_CONNECTION_STRING", "Server=db.example.com,1433;Initial Catalog=testdb;Persist Security Info=False;User ID=user;Password=password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"}
            }).Build();

            return new ConfigService(aspConf, sepesConf);
        }

        [Fact]
        public void DatabaseString()
        {
            var config = GetConfigService();
            Assert.Equal("Server=db.example.com,1433;Initial Catalog=testdb;Persist Security Info=False;User ID=user;Password=password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", config.connectionString);
        }

        [Fact]
        public void AzureConfig()
        {
            var config = GetConfigService();
            Assert.Equal("Test-SepesCommon", config.azureConfig.commonGroup);
        }

        [Fact]
        public void instrumentationKey()
        {
            var config = GetConfigService();
            Assert.Equal("407c7027-9660-471d-9aa8-d1e4125794f2", config.instrumentationKey);
        }

        [Fact]
        public void TokenValidationParameters()
        {
            var config = GetConfigService();
            Assert.Equal("SepesUnitTests", config.tokenValidation.ValidIssuer);
        }

        [Fact]
        public void authConfig()
        {
            var config = GetConfigService();
            Assert.Equal("SepesUnitTests", config.authConfig.Issuer);
        }
    }
}