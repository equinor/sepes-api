using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Sepes.RestApi.Services;
using Sepes.RestApi.Model;

namespace Sepes.RestApi
{
    public class Startup
    {
        private IWebHostEnvironment _env;
        public IConfiguration Configuration { get;}
        public IConfiguration SepesConfiguration { get;}
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            Configuration = configuration;

            SepesConfiguration = new ConfigurationBuilder().AddEnvironmentVariables("SEPES_").Build();
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigService(Configuration, SepesConfiguration);

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
            .AddJwtBearer(options =>
            {
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

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    /* builder.WithOrigins("http://example.com",
                                        "http://www.contoso.com");
                    */
                    //Issue: 39  replace with above commented code. Preferably add config support for the URLs. Perhaps an if to check if environment is running in development so we can still easily debug without changing code
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.AddSingleton<ISepesDb>(new SepesDb(config.databaseConfig));
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IAzureService>(new AzureService(Configuration));
            services.AddSingleton<IPodService, PodService>();

            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
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

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseMvc();
        }
    }
}
