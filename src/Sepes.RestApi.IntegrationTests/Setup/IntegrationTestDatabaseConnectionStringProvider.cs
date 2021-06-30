using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Service.Interface;
using System;

namespace Sepes.RestApi.IntegrationTests.Setup
{
    public class IntegrationTestDatabaseConnectionStringProvider : IDatabaseConnectionStringProvider
    {
        readonly IConfiguration _configuration;

        public IntegrationTestDatabaseConnectionStringProvider(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GetConnectionString()
        {
            return _configuration[ConfigConstants.DB_INTEGRATION_TEST_CONNECTION_STRING];
        }             
    }
}
