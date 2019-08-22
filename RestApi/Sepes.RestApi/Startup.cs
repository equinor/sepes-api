using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sepes.RestApi.Model;

namespace Sepes.RestApi
{
    public class Startup
    {
        public IConfiguration Configuration {get; set;}
        public Startup(IHostingEnvironment env)
        {
            var confbuilder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
            Configuration = confbuilder.Build();
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Adds the secret azure insight token. Make sure to set this if using logging via azure innsight
            //Use ' dotnet user-secrets set "AzureLogToken:ServiceApiKey" "YOURKEY" ' To add a secret key.
            
            // The following line enables Application Insights telemetry collection.
            //TODO add support for local logging. If this is left empty then no logs are made. Unknown if still affects performance.
            //Secret key can be set up for with either secret key or in appsettings.json, secret key will overwrite json.
            services.AddApplicationInsightsTelemetry(Configuration["AzureLogToken:ServiceApiKey"]);
            services.Configure<AppSettings>(Configuration.GetSection("Jwt"));
            services.AddOptions();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  
            .AddJwtBearer(options =>  
            {  
            options.TokenValidationParameters = new TokenValidationParameters  
                {  
                ValidateIssuer = false,  //TODO set to true before MVP
                ValidateAudience = false,  
                ValidateLifetime = true,  
                ValidateIssuerSigningKey = true,  
                ValidIssuer = Configuration["Jwt:Issuer"],  
                ValidAudience = Configuration["Jwt:Issuer"],  
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))  
                };  
            }); 

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    /*
                    builder.WithOrigins("http://example.com",
                                        "http://www.contoso.com");
                    */
                    //TODO should be replaced with above commented code. Update URLs with what is required for your use case
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); 

                });
        });
    
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseMvc();
        }
    }
}
