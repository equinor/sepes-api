using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sepes.RestApi.Services;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Sepes.RestApi
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>


            WebHost.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((context, configBuilder) =>
             {
                 var config = configBuilder.AddEnvironmentVariables("SEPES_").Build();

                 var keyVaultUrl = config["KeyVault_Url"];

                 if (!string.IsNullOrWhiteSpace(keyVaultUrl))
                 {
                     var clientId = config["ClientId"];
                     var clientSecret = config["ClientSecret"];

                     configBuilder.AddAzureKeyVault(keyVaultUrl, clientId, clientSecret);
                 }

             })
              .ConfigureLogging((hostingContext, builder) => {

                  var iKey = hostingContext.Configuration["Appi_Key"];                 

                  builder.AddApplicationInsights(iKey);

                  // Adding the filter below to ensure logs of all severity from Program.cs
                  // is sent to ApplicationInsights.
                  builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                                   (typeof(Program).FullName, LogLevel.Trace);

                  // Adding the filter below to ensure logs of all severity from Startup.cs
                  // is sent to ApplicationInsights.
                  builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                                   (typeof(Startup).FullName, LogLevel.Trace);

              })
                .UseStartup<Startup>();
    }
}
