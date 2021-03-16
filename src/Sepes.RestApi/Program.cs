using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Util;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
                 configBuilder.AddEnvironmentVariables(ConfigConstants.ENV_VARIABLE_PREFIX);

                 var config = configBuilder.Build();

                 var isIntegrationTest = ConfigUtil.GetBoolConfig(config, "IS_INTEGRATION_TEST");

                 foreach (var curSource in configBuilder.Sources.ToList())
                 {
                     if (curSource.GetType().Name == typeof(JsonConfigurationSource).Name)
                     {
                         var fileSource = curSource as JsonConfigurationSource;

                         if (isIntegrationTest && fileSource.Path.ToLower().Equals("appsettings.development.json"))
                         {
                             configBuilder.Sources.Remove(curSource);
                             configBuilder.AddJsonFile("appsettings.UnitTest.json", true, false);                       
                         }
                         else
                         {
                             fileSource.ReloadOnChange = false;
                         }                       
                     }
                 }

                 config = configBuilder.Build();              

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
