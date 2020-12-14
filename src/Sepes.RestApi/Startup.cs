﻿using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.OpenApi.Models;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model.Automapper;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Azure;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.Middelware;
using Sepes.RestApi.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Sepes.RestApi
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        readonly ILogger _logger;
        readonly IConfiguration _configuration;

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(ILogger<Startup> logger, IConfiguration configuration)
        {
            _logger = logger;

            var logMsg = "Sepes Startup Constructor";
            Trace.WriteLine(logMsg);
            _logger.LogWarning(logMsg);

            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var logMsg = "ConfigureServices starting";
            Trace.WriteLine(logMsg);
            _logger.LogWarning(logMsg);

            AddApplicationInsights(services);          

            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

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

            var enableSensitiveDataLogging = true;

            var readWriteDbConnectionString = _configuration[ConfigConstants.DB_READ_WRITE_CONNECTION_STRING];

            services.AddDbContext<SepesDbContext>(
              options => options.UseSqlServer(
                  readWriteDbConnectionString,
                  assembly => assembly.MigrationsAssembly(typeof(SepesDbContext).Assembly.FullName))
              .EnableSensitiveDataLogging(enableSensitiveDataLogging)
              );

            services.AddProtectedWebApi(_configuration, subscribeToJwtBearerMiddlewareDiagnosticsEvents: true)
                 .AddProtectedWebApiCallsProtectedWebApi(_configuration)
                 .AddInMemoryTokenCaches();

            // Token acquisition service based on MSAL.NET
            // and chosen token cache implementation
            services.AddWebAppCallsProtectedWebApi(_configuration)
               .AddInMemoryTokenCaches();

            DoMigration();

            services.AddHttpClient();

            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(AutoMappingConfigs));

            RegisterServices(services);

            SetFileUploadLimits(services);

            AddSwagger(services);

            var logMsgDone = "Configuring services done";
            Trace.WriteLine(logMsgDone);
            _logger.LogWarning(logMsgDone);
        }

        void AddApplicationInsights(IServiceCollection services)
        {
            // The following line enables Application Insights telemetry collection.
            // If this is left empty then no logs are made. Unknown if still affects performance.
            Trace.WriteLine("Configuring Application Insights");
            //services.AddApplicationInsightsTelemetry();

            Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions aiOptions
                = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions();
            // Disables adaptive sampling.
            aiOptions.EnableAdaptiveSampling = false;
            aiOptions.InstrumentationKey = _configuration[ConfigConstants.APPI_KEY];
            aiOptions.EnableDebugLogger = true;

            services.AddApplicationInsightsTelemetry(aiOptions);
        }

        void RegisterServices(IServiceCollection services)
        {
            //Plumbing
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserPermissionService, UserPermissionService>();
            services.AddScoped<IPrincipalService, PrincipalService>();
            services.AddTransient<IRequestIdService, RequestIdService>();
            services.AddTransient<IGraphServiceProvider, GraphServiceProvider>();

            //Domain Model Services
            services.AddScoped<IVariableService, VariableService>();
            services.AddTransient<ILookupService, LookupService>();
            services.AddTransient<IDatasetService, DatasetService>();
            services.AddTransient<ISandboxService, SandboxService>();
            services.AddTransient<IStudyService, StudyService>();
            services.AddTransient<IStudyDatasetService, StudyDatasetService>();
            services.AddTransient<IStudyParticipantService, StudyParticipantService>();
            services.AddTransient<ISandboxResourceService, SandboxResourceService>();
            services.AddTransient<ISandboxDatasetService, SandboxDatasetService>();
            services.AddTransient<IRegionService, RegionService>();

            //Ext System Facade Services
            services.AddTransient<IDatasetFileService, DatasetFileService>();
            services.AddTransient<IStudyLogoService, StudyLogoService>();
            services.AddTransient<IStudySpecificDatasetService, StudySpecificDatasetService>();
            services.AddTransient<IProvisioningQueueService, ProvisioningQueueService>();
            services.AddTransient<ISandboxResourceProvisioningService, SandboxResourceProvisioningService>();
            services.AddTransient<ISandboxResourceOperationService, SandboxResourceOperationService>();
            services.AddTransient<ISandboxResourceMonitoringService, SandboxResourceMonitoringService>();
            services.AddTransient<IVirtualMachineService, VirtualMachineService>();
            services.AddTransient<IVirtualMachineSizeService, VirtualMachineSizeService>();
            services.AddTransient<IVirtualMachineLookupService, VirtualMachineLookupService>();
            services.AddTransient<IVirtualMachineRuleService, VirtualMachineRuleService>();
            services.AddTransient<IDatasetCloudResourceService, DatasetCloudResourceService>();

            //Azure Services
            services.AddTransient<IAzureResourceGroupService, AzureResourceGroupService>();
            services.AddTransient<IAzureNetworkSecurityGroupService, AzureNetworkSecurityGroupService>();
            services.AddTransient<IAzureBastionService, AzureBastionService>();
            services.AddTransient<IAzureVNetService, AzureVNetService>();
            services.AddTransient<IAzureVmService, AzureVmService>();
            services.AddTransient<IAzureQueueService, AzureQueueService>();
            services.AddTransient<IAzureBlobStorageService, AzureBlobStorageService>();
            services.AddTransient<IAzureStorageAccountService, AzureStorageAccountService>();
            services.AddTransient<IAzureNetworkSecurityGroupRuleService, AzureNetworkSecurityGroupRuleService>();
            services.AddTransient<IAzureResourceSkuService, AzureResourceSkuService>();
            services.AddTransient<IAzureUserService, AzureUserService>();
            services.AddTransient<IAzureVmOsService, AzureVmOsService>();
            services.AddTransient<IAzureCostManagementService, AzureCostManagementService>();
            services.AddTransient<IAzureRoleAssignmentService, AzureRoleAssignmentService>(); 
        }

        void SetFileUploadLimits(IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
        }

        void AddSwagger(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sepes API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.OAuth2,
                    Scheme = "Bearer",
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{_configuration[ConfigConstants.AZ_TENANT_ID]}/oauth2/authorize"),
                            //Scopes = new Dictionary<string, string>
                            //{
                            //    { "https://graph.microsoft.com/User.Read", "MS Graph: Read for user" },
                            //    { "https://graph.microsoft.com/User.Read.All", "MS Graph: Read all users" }
                            //}
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,


                        },
                        new List<string>()
                    }
                });
            });
        }

        void DoMigration()
        {
            var disableMigrations = _configuration[ConfigConstants.DISABLE_MIGRATIONS];

            string logMessage;

            if (!String.IsNullOrWhiteSpace(disableMigrations) && disableMigrations.ToLower() == "false")
            {
                logMessage = "Migrations are disabled and will be skipped!";

            }
            else
            {
                logMessage = "Performing database migrations";
            }

            Trace.WriteLine(logMessage);
            _logger.LogWarning(logMessage);

            string sqlConnectionStringOwner = _configuration[ConfigConstants.DB_OWNER_CONNECTION_STRING];

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

            var logMsgDone = "Do migration done";

            Trace.WriteLine(logMsgDone);
            _logger.LogWarning(logMsgDone);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var logMsg = "Configure";

            Trace.WriteLine(logMsg);
            _logger.LogWarning(logMsg);


            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);

            var httpOnlyRaw = _configuration["HttpOnly"];

            // UseHttpsRedirection doesn't work well with docker.        
            if (!String.IsNullOrWhiteSpace(httpOnlyRaw) && httpOnlyRaw.ToLower() == "true")
            {
                var logMsgHttps = "Using HTTP only";
                Trace.WriteLine(logMsgHttps);
                _logger.LogWarning(logMsgHttps);
            }
            else
            {
                var logMsgHttps = "Also using HTTPS. Activating https redirection";
                Trace.WriteLine(logMsgHttps);
                _logger.LogWarning(logMsgHttps);
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.OAuthClientId(_configuration[ConfigConstants.AZ_CLIENT_ID]);
                c.OAuthClientSecret(_configuration[ConfigConstants.AZ_CLIENT_SECRET]);
                c.OAuthRealm(_configuration[ConfigConstants.AZ_CLIENT_ID]);
                c.OAuthAppName("Sepes Development");
                c.OAuthScopeSeparator(" ");
                c.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { ["resource"] = _configuration[ConfigConstants.AZ_CLIENT_ID] });
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sepes API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var logMsgDone = "Configure done";

            Trace.WriteLine(logMsgDone);
            _logger.LogWarning(logMsgDone);
        }
    }
}
