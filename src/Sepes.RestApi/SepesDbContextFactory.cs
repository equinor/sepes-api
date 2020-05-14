

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Model.Context;
using Sepes.RestApi.Services;
using System;
using System.IO;

namespace Ras.Ui
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

            //if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")))
            //{
            //    ConfigService.LoadDevEnv();
            //}

            var _config = ConfigService.CreateConfig(config);           

            var optionsBuilder = new DbContextOptionsBuilder<SepesDbContext>();
            optionsBuilder.UseSqlServer(_config.DbReadWriteConnectionString);
            optionsBuilder.EnableSensitiveDataLogging(true);
            //TODO: Add support for created, createdBy, updated, updatedBy
            return new SepesDbContext(optionsBuilder.Options);
        }
    }
}