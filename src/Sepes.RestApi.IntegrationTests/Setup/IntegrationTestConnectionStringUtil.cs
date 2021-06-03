using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using System;

namespace Sepes.RestApi.IntegrationTests.Setup
{
    public static class IntegrationTestConnectionStringUtil
    {
        public static string GetDatabaseConnectionString(IConfiguration config)
        {           
            var dbConnectionString = config.GetValue<string>(ConfigConstants.DB_INTEGRATION_TEST_CONNECTION_STRING);

            if (string.IsNullOrEmpty(dbConnectionString))
                throw new ArgumentException($"Could not find a connection string in any configuration provider for {ConfigConstants.DB_INTEGRATION_TEST_CONNECTION_STRING}");

            return dbConnectionString;
        }
    }
}
