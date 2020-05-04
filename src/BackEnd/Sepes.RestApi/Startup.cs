using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sepes.RestApi.Services;
using System.Diagnostics.CodeAnalysis;
using Sepes.Infrastructure.Model.Context;
using Microsoft.EntityFrameworkCore;
using System;

namespace Sepes.RestApi
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        IWebHostEnvironment _env;
        readonly IConfigService _configService;
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            //_env = env;

            //if (_env.EnvironmentName == "Development")
            //{
            //    ConfigService.LoadDevEnv();
            //}
          var secretFromConfig = configuration["CLIENT_SECRET"];
                _configService = ConfigService.CreateConfig(configuration);
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // The following line enables Application Insights telemetry collection.
            // If this is left empty then no logs are made. Unknown if still affects performance.
            services.AddApplicationInsightsTelemetry(_configService.InstrumentationKey);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = _configService.TokenValidation);

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    //builder.WithOrigins("http://example.com", "http://www.contoso.com");
                    // Issue: 39  replace with above commented code. Preferably add config support for the URLs. 
                    // Perhaps an if to check if environment is running in development so we can still easily debug without changing code
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            var azureService = new AzureService(_configService.AzureConfig);
            var dbService = new SepesDb(_configService.DbReadWriteConnectionString);
            var podService = new PodService(azureService);
            var studyService = new StudyService(dbService, podService);
            //studyService.LoadStudies();

            services.AddSingleton<ISepesDb>(dbService);
            services.AddSingleton<IAuthService>(new AuthService(_configService.AuthConfig));
            services.AddSingleton<IAzureService>(azureService);
            services.AddSingleton<IPodService>(podService);
            services.AddSingleton<IStudyService>(studyService);

            services.AddTransient<Sepes.Infrastructure.Service.StudyService2>();

            DoMigration();

            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            var enableSensitiveDataLogging = true;

            services.AddDbContext<SepesDbContext>(
              options => options.UseSqlServer(
                  _configService.DbReadWriteConnectionString,
                  assembly => assembly.MigrationsAssembly(typeof(SepesDbContext).Assembly.FullName))
              .EnableSensitiveDataLogging(enableSensitiveDataLogging)
              );

        }

        void DoMigration()
        {
            string sqlConnectionStringOwner = _configService.DbOwnerConnectionString;

            if (string.IsNullOrEmpty(sqlConnectionStringOwner))
            {
                throw new Exception("Could not obtain database OWNER connection string. Unable to run migrations");
            }

            DbContextOptionsBuilder<SepesDbContext> createDbOptions = new DbContextOptionsBuilder<SepesDbContext>();
            createDbOptions.UseSqlServer(sqlConnectionStringOwner);

            using (var ctx = new SepesDbContext(createDbOptions.Options))
            {
                ctx.Database.SetCommandTimeout(300);
                ctx.Database.Migrate();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            // UseHttpsRedirection doesn't work well with docker.
            if(!this._configService.HttpOnly) {
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
