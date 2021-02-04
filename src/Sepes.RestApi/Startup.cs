using AutoMapper;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
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
using Sepes.Infrastructure.Service.DataModelService;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.Middelware;
using Sepes.RestApi.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;

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

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddMicrosoftIdentityWebApi(_configuration)
            .EnableTokenAcquisitionToCallDownstreamApi()
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
            Trace.WriteLine("Configuring Application Insights");
        
            var aiOptions = new ApplicationInsightsServiceOptions
                {
                    // Disables adaptive sampling.
                    EnableAdaptiveSampling = false,
                    InstrumentationKey = _configuration[ConfigConstants.APPI_KEY],
                    EnableDebugLogger = true
                };

            services.AddApplicationInsightsTelemetry(aiOptions);
        }

        void RegisterServices(IServiceCollection services)
        {
            //Plumbing
            
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserPermissionService, UserPermissionService>();
            services.AddScoped<IPrincipalService, PrincipalService>();
            services.AddTransient<IRequestIdService, RequestIdService>();
            services.AddTransient<IGraphServiceProvider, GraphServiceProvider>();

            //Data model services v2
            services.AddTransient<IStudyModelService, StudyModelService>();

            //Domain Model Services
            services.AddTransient<IStudyReadService, StudyReadService>();
            services.AddTransient<IStudyCreateUpdateService, StudyCreateUpdateService>();
            services.AddTransient<IStudyDeleteService, StudyDeleteService>();
            services.AddTransient<IDatasetService, DatasetService>();
            services.AddTransient<ISandboxService, SandboxService>();
            services.AddTransient<ISandboxPhaseService, SandboxPhaseService>();           
            services.AddTransient<IStudyDatasetService, StudyDatasetService>();
            services.AddTransient<IStudyParticipantLookupService, StudyParticipantLookupService>();
            services.AddTransient<IStudyParticipantCreateService, StudyParticipantCreateService>();
            services.AddTransient<IStudyParticipantRemoveService, StudyParticipantRemoveService>();
            services.AddTransient<ICloudResourceReadService, CloudResourceReadService>();
            services.AddTransient<ICloudResourceCreateService, CloudResourceCreateService>();
            services.AddTransient<ICloudResourceUpdateService, CloudResourceUpdateService>();
            services.AddTransient<ICloudResourceDeleteService, CloudResourceDeleteService>();
            services.AddTransient<ICloudResourceOperationCreateService, CloudResourceOperationCreateService>();
            services.AddTransient<ICloudResourceOperationReadService, CloudResourceOperationReadService>();
            services.AddTransient<ICloudResourceOperationUpdateService, CloudResourceOperationUpdateService>();     
            services.AddTransient<ISandboxDatasetService, SandboxDatasetService>();
            services.AddTransient<IRegionService, RegionService>();
            services.AddScoped<IVariableService, VariableService>();
            services.AddTransient<ILookupService, LookupService>();

            //Ext System Facade Services
            services.AddTransient<IDatasetFileService, DatasetFileService>();          
            services.AddTransient<IStudyLogoService, StudyLogoService>();
            services.AddTransient<IStudySpecificDatasetService, StudySpecificDatasetService>();
            services.AddTransient<IProvisioningQueueService, ProvisioningQueueService>();            
            services.AddTransient<IResourceProvisioningService, ResourceProvisioningService>();
            services.AddTransient<ISandboxResourceCreateService, SandboxResourceCreateService>();
            services.AddTransient<ISandboxResourceRetryService, SandboxResourceRetryService>();
            services.AddTransient<ISandboxResourceDeleteService, SandboxResourceDeleteService>();    
            services.AddTransient<ICloudResourceMonitoringService, CloudResourceMonitoringService>();
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

            //To get actual Client IP even though behind load balancer
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

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
                c.OAuthClientId(_configuration[ConfigConstants.AZ_SWAGGER_CLIENT_ID]);
                c.OAuthClientSecret(_configuration[ConfigConstants.AZ_SWAGGER_CLIENT_SECRET]);
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
