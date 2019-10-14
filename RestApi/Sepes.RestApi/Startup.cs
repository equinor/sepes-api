﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting; //Adds IsDevelopment
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;

using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using System.Linq;

namespace Sepes.RestApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IWebHostEnvironment env)
        {
            var confbuilder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                confbuilder.AddUserSecrets<Startup>();
            }
            Configuration = confbuilder.Build();
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Adds the secret azure insight token. Make sure to set this if using logging via azure innsight
            //Use ' dotnet user-secrets set "AzureLogToken:ServiceApiKey" "YOURKEY" ' To add a secret key.

            // The following line enables Application Insights telemetry collection.
            //If this is left empty then no logs are made. Unknown if still affects performance.

            //Issue: 38 Check the all the logs thoroughly before you close out this issue. Test a the webapi functions and make sure none of them causes sensitive data to be logged. 

            //Secret key can be set up for with either dotnet secret key or in environment values, secret key will overwrite ENV.
            services.AddApplicationInsightsTelemetry(Configuration["AzureLogToken:ServiceApiKey"]);
            services.Configure<AppSettings>(Configuration.GetSection("Jwt"));
            services.AddOptions();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,  //Issue: 39 set to true before MVP
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                        //SaveSigninToken = true  
                    };
            });

            services.AddCors(options => {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder => {
                    /*
                    builder.WithOrigins("http://example.com",
                                        "http://www.contoso.com");
                    */
                    //Issue: 39  replace with above commented code. Preferably add config support for the URLs. Perhaps an if to check if environment is running in development so we can still easely debug without changing code

                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();

                });
            });

            services.AddSingleton<ISepesDb>(new SepesDb(Configuration));

            /////////////////////
            //// Azure setup
            string tenant = Configuration["Azure:TenantId"];
            string client = Configuration["Azure:ClientId"];
            string secret = Configuration["Azure:ClientSecret"];
            string subscription = Configuration["Azure:Subscription"];
            string resGroupName = Configuration["Azure:CommonResourceGroupNamePrefix"]+Configuration["Azure:CommonResourceGroupName"];

            var creds = new AzureCredentialsFactory().FromServicePrincipal(client, secret, tenant, AzureEnvironment.AzureGlobalCloud);
            var authenticated = Azure.Authenticate(creds);
            IAzure azure = authenticated.WithSubscription(subscription);
            
            if (!azure.ResourceGroups.Contain(resGroupName)) {
                azure.ResourceGroups
                    .Define(resGroupName)
                    .WithRegion(Region.EuropeNorth)
                    .Create();
            }

            services.AddSingleton<IAzure>(azure)
                .AddScoped<IAzureService, AzureService>();


            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(MyAllowSpecificOrigins);
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
