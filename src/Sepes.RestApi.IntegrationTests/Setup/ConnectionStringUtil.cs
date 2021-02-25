using Microsoft.Extensions.Configuration;
using System;

namespace Sepes.RestApi.IntegrationTests.Setup
{
    public static class ConnectionStringUtil
    {
        public static string GetDatabaseConnectionString(IConfiguration config)
        {
            var dbConnectionStringKey = "SepesIntegrationTestConnectionString";
            var dbConnectionString = config.GetValue<string>(dbConnectionStringKey);

            if (string.IsNullOrEmpty(dbConnectionString))
                throw new ArgumentException($"Could not find a connection string in any configuration provider for {dbConnectionStringKey}");

            return dbConnectionString;
        }
    }
}
