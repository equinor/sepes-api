using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Config;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
                 var config = configBuilder.AddEnvironmentVariables(ConfigConstants.ENV_VARIABLE_PREFIX).Build();
                 
                 var keyVaultUrl = config[ConfigConstants.KEY_VAULT];

                 if (string.IsNullOrWhiteSpace(keyVaultUrl))
                 {
                     Trace.WriteLine("Program.cs: Key vault url empty");
                 }
                 else
                 {
                     Trace.WriteLine("Program.cs: Key vault url found. Initializing key vault");
                     
                     var clientId = config[ConfigConstants.AZ_CLIENT_ID];
                     var clientSecret = config[ConfigConstants.AZ_CLIENT_SECRET];
                     configBuilder.AddAzureKeyVault(keyVaultUrl, clientId, clientSecret);
                 }
             })
              .ConfigureLogging((hostingContext, builder) => {

                  var applicationInsightsInstrumentationKey = hostingContext.Configuration[ConfigConstants.APPI_KEY];                 

                  builder.AddApplicationInsights(applicationInsightsInstrumentationKey);

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
