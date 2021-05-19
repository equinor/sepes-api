using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Model.Context;
using System.IO;

namespace Sepes.RestApi.Database
{

    public class SepesDbContextFactory : IDesignTimeDbContextFactory<SepesDbContext>
    {
        public SepesDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config[ConfigConstants.DB_READ_WRITE_CONNECTION_STRING];

            var optionsBuilder = new DbContextOptionsBuilder<SepesDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder.EnableSensitiveDataLogging(true);
            return new SepesDbContext(optionsBuilder.Options);
        }
    }
}