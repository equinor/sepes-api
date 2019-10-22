using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Sepes.RestApi.Services;
using Xunit;

namespace Sepes.RestApi.Tests.Services
{
    public class ConfigServiceTests
    {
        [Fact]
        public void DatabaseString()
        {
            //Given
            var aspConf = new ConfigurationBuilder().Build();
            var sepesConf = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> {
            {"MSSQL_CONNECTION_STRING", "Test string"}
        }).Build();

            //When
            var config = new ConfigService(aspConf, sepesConf);

            //Then
            Assert.Equal("Test string", config.databaseConfig.connectionString);
        }
    }
}