using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Common.Util;
using Sepes.Infrastructure.Service.Interface;
using System;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class DatabaseConnectionStringProvider : IDatabaseConnectionStringProvider
    {
        readonly IConfiguration _configuration;

        public DatabaseConnectionStringProvider(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GetConnectionString()
        {
            return _configuration[ConfigConstants.DB_READ_WRITE_CONNECTION_STRING];
        }
    }
}
