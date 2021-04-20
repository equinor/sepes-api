using AutoMapper;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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
using Microsoft.OpenApi.Models;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model.Automapper;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Azure;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.DataModelService;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using Sepes.RestApi.Middelware;
using Sepes.RestApi.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace Sepes.RestApi
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        readonly ILogger _logger;
        readonly IConfiguration _configuration;

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        readonly Dictionary<string, string> Scopes = new Dictionary<string, string>() { { "api://e90cbb61-896e-4ec7-aa37-23511700e1ed/User.Impersonation", "Access SEPES" } };

        //public Startup(ILogger<Startup> logger, IConfiguration configuration)
        public Startup(ILogger<Startup> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            Log("Sepes Startup Constructor");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log("ConfigureServices starting");

            AddApplicationInsights(services);

            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            var corsDomainsFromConfig = ConfigUtil.GetCommaSeparatedConfigValueAndThrowIfEmpty(_configuration, ConfigConstants.ALLOW_CORS_DOMAINS);

            Log("Startup - ConfigureServices - Cors domains: *");

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    var domainsAsArray = new string[corsDomainsFromConfig.Count];
                    corsDomainsFromConfig.CopyTo(domainsAsArray);

                    builder.WithOrigins(domainsAsArray);
                    builder.AllowAnyHeader().AllowAnyMethod();
                });
            });

            var enableSensitiveDataLogging = true;

            var isIntegrationTest = ConfigUtil.GetBoolConfig(_configuration, ConfigConstants.IS_INTEGRATION_TEST);

            if (!isIntegrationTest)
            {
                DoMigration();

                var readWriteDbConnectionString = _configuration[ConfigConstants.DB_READ_WRITE_CONNECTION_STRING];

                if (string.IsNullOrWhiteSpace(readWriteDbConnectionString))
                {
                    throw new Exception("Could not obtain database READWRITE connection string. Unable to add DB Context");
                }

                services.AddDbContext<SepesDbContext>(
                  options => options.UseSqlServer(
                      readWriteDbConnectionString,
                      assembly => assembly.MigrationsAssembly(typeof(SepesDbContext).Assembly.FullName))
                  .EnableSensitiveDataLogging(enableSensitiveDataLogging)
                  );
            }

            //  services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //.AddMicrosoftIdentityWebApi(_configuration)
            //  .EnableTokenAcquisitionToCallDownstreamApi()
            //  .AddInMemoryTokenCaches();


            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(a => { }, b =>
                 {
                    //_configuration.Bind("AzureAd", o);
                    b.UsePkce = true;
                     b.ClientId = _configuration[ConfigConstants.AZ_CLIENT_ID]; // "<client_id>";
                    b.TenantId = _configuration[ConfigConstants.AZ_TENANT_ID]; //"<tenant_id>";
                    b.Domain = _configuration[ConfigConstants.AZ_DOMAIN]; //"yourdomain.com";
                    b.Instance = _configuration[ConfigConstants.AZ_INSTANCE]; //"https://login.microsoftonline.com";
                    b.CallbackPath = "/signin-oidc";
                     b.ResponseType = "code";

                     var defaultBackChannel = new HttpClient();
                     defaultBackChannel.DefaultRequestHeaders.Add("Origin", "sepes");
                     b.Backchannel = defaultBackChannel;
                 });

            services.AddHttpClient();

            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(AutoMappingConfigs));

            RegisterServices(services);

            SetFileUploadLimits(services);

            AddSwagger(services);

            Log("Configuring services done");
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
            services.AddSingleton<IPublicIpFromThirdPartyService, PublicIpFromThirdPartyService>();
            services.AddSingleton<IPublicIpService, PublicIpService>();
            services.AddScoped<IHealthService, HealthService>();

            //Data model services v2
            services.AddTransient<IStudyModelService, StudyModelService>();
            services.AddTransient<IStudySpecificDatasetModelService, StudySpecificDatasetModelService>();
            services.AddTransient<IPreApprovedDatasetModelService, PreApprovedDatasetModelService>();
            services.AddTransient<ISandboxModelService, SandboxModelService>();
            services.AddTransient<ISandboxDatasetModelService, SandboxDatasetModelService>();

            //Domain Model Services
            services.AddTransient<IStudyReadService, StudyReadService>();
            services.AddTransient<IStudyCreateService, StudyCreateService>();
            services.AddTransient<IStudyUpdateService, StudyUpdateService>();
            services.AddTransient<IStudyDeleteService, StudyDeleteService>();
            services.AddTransient<IDatasetService, DatasetService>();
            services.AddTransient<ISandboxService, SandboxService>();
            services.AddTransient<ISandboxPhaseService, SandboxPhaseService>();
            services.AddTransient<ISandboxResourceReadService, SandboxResourceReadService>();
            services.AddTransient<IStudyDatasetService, StudyDatasetService>();
            services.AddTransient<IStudyParticipantLookupService, StudyParticipantLookupService>();
            services.AddTransient<IStudyParticipantCreateService, StudyParticipantCreateService>();
            services.AddTransient<IStudyParticipantRemoveService, StudyParticipantRemoveService>();
            services.AddTransient<ICloudResourceReadService, CloudResourceReadService>();
            services.AddTransient<ICloudResourceCreateService, CloudResourceCreateService>();
            services.AddTransient<ICloudResourceUpdateService, CloudResourceUpdateService>();
            services.AddTransient<ICloudResourceDeleteService, CloudResourceDeleteService>();
            services.AddTransient<IResourceOperationModelService, ResourceOperationModelService>();
            services.AddTransient<ICloudResourceOperationCreateService, CloudResourceOperationCreateService>();
            services.AddTransient<ICloudResourceOperationReadService, CloudResourceOperationReadService>();
            services.AddTransient<ICloudResourceOperationUpdateService, CloudResourceOperationUpdateService>();

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
            services.AddTransient<IVirtualMachineCreateService, VirtualMachineCreateService>();
            services.AddTransient<IVirtualMachineReadService, VirtualMachineReadService>();
            services.AddTransient<IVirtualMachineDeleteService, VirtualMachineDeleteService>();
            services.AddTransient<IVirtualMachineSizeService, VirtualMachineSizeService>();
            services.AddTransient<IVirtualMachineDiskSizeService, VirtualMachineDiskSizeService>();
            services.AddTransient<IVirtualMachineOperatingSystemService, VirtualMachineOperatingSystemService>();
            services.AddTransient<IVirtualMachineRuleService, VirtualMachineRuleService>();
            services.AddTransient<IVirtualMachineValidationService, VirtualMachineValidationService>();
            services.AddTransient<IDatasetCloudResourceService, DatasetCloudResourceService>();

            //Import Services
            services.AddTransient<IVirtualMachineDiskSizeImportService, VirtualMachineDiskSizeImportService>();
            services.AddTransient<IVirtualMachineSizeImportService, VirtualMachineSizeImportService>();

            //Azure Services
            services.AddTransient<IAzureResourceGroupService, AzureResourceGroupService>();
            services.AddTransient<IAzureNetworkSecurityGroupService, AzureNetworkSecurityGroupService>();
            services.AddTransient<IAzureBastionService, AzureBastionService>();
            services.AddTransient<IAzureVirtualNetworkService, AzureVirtualNetworkService>();
            services.AddTransient<IAzureVirtualMachineService, AzureVirtualMachineService>();
            services.AddTransient<IAzureVirtualMachineExtenedInfoService, AzureVirtualMachineExtendedInfoService>();
            services.AddTransient<IAzureQueueService, AzureQueueService>();
            services.AddTransient<IAzureBlobStorageService, AzureBlobStorageService>();
            services.AddTransient<IAzureBlobStorageUriBuilderService, AzureBlobStorageUriBuilderService>();
            services.AddTransient<IAzureStorageAccountService, AzureStorageAccountService>();
            services.AddTransient<IAzureStorageAccountAccessKeyService, AzureStorageAccountAccessKeyService>();
            services.AddTransient<IAzureStorageAccountNetworkRuleService, AzureStorageAccountNetworkRuleService>();
            services.AddTransient<IAzureNetworkSecurityGroupRuleService, AzureNetworkSecurityGroupRuleService>();
            services.AddTransient<IAzureResourceSkuService, AzureResourceSkuService>();
            services.AddTransient<IAzureUserService, AzureUserService>();
            services.AddTransient<IAzureVirtualNetworkOperatingSystemService, AzureVirtualNetworkOperatingSystemService>();
            services.AddTransient<IAzureCostManagementService, AzureCostManagementService>();
            services.AddTransient<IAzureRoleAssignmentService, AzureRoleAssignmentService>();
            services.AddTransient<IAzureKeyVaultSecretService, AzureKeyVaultSecretService>();
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
                c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                {
                    //Description =
                    //    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    //Name = "Authorization",
                    //In = ParameterLocation.Header,
                    Type = SecuritySchemeType.OAuth2,
                    //Scheme = "Bearer",
                    Flows = new OpenApiOAuthFlows
                    {

                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            Scopes = Scopes,
                            TokenUrl = new Uri($"https://login.microsoftonline.com/{_configuration[ConfigConstants.AZ_TENANT_ID]}/oauth2/v2.0/token"),
                            AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{_configuration[ConfigConstants.AZ_TENANT_ID]}/oauth2/v2.0/authorize")
                            //Implicit = new OpenApiOAuthFlow
                            //{
                            //    AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{_configuration[ConfigConstants.AZ_TENANT_ID]}/oauth2/authorize"),
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

            if (!String.IsNullOrWhiteSpace(disableMigrations) && disableMigrations.ToLower() == "true")
            {
                Log("Migrations are disabled and will be skipped!");
                return;
            }
            else
            {
                Log("Performing database migrations");
            }

            string sqlConnectionStringOwner = _configuration[ConfigConstants.DB_OWNER_CONNECTION_STRING];

            if (string.IsNullOrWhiteSpace(sqlConnectionStringOwner))
            {
                throw new Exception("Could not obtain database OWNER connection string. Unable to run migrations");
            }

            var createDbOptions = new DbContextOptionsBuilder<SepesDbContext>();
            createDbOptions.UseSqlServer(sqlConnectionStringOwner);

            using (var ctx = new SepesDbContext(createDbOptions.Options))
            {
                ctx.Database.SetCommandTimeout(300);
                ctx.Database.Migrate();
            }

            Log("Do migration done");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log("Configure");

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
                Log("Using HTTP only");
            }
            else
            {
                Log("Also using HTTPS. Activating https redirection");
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
                c.OAuthUsePkce();
                c.OAuthAppName("Sepes Development");
                c.OAuthScopeSeparator(" ");
                c.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { ["resource"] = _configuration[ConfigConstants.AZ_CLIENT_ID] });
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sepes API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Log("Configure done");
        }
        void Log(string message)
        {
            Trace.WriteLine(message);
            _logger.LogInformation(message);
        }
    }
}
