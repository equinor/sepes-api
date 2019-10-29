using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sepes.RestApi.Services;
using System.Diagnostics.CodeAnalysis;


namespace Sepes.RestApi
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private IWebHostEnvironment _env;
        private readonly ConfigService _config;
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _config = new ConfigService(
                configuration,
                new ConfigurationBuilder().AddEnvironmentVariables("SEPES_").Build()
            );
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
            services.AddApplicationInsightsTelemetry(_config.instrumentationKey);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = _config.tokenValidation);

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

            services.AddSingleton<ISepesDb>(new SepesDb(_config.connectionString));
            services.AddSingleton<IAuthService>(new AuthService(_config.authConfig));
            services.AddSingleton<IAzureService>(new AzureService(_config.azureConfig));
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

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseMvc();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
