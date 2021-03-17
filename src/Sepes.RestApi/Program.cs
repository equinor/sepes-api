using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Sepes.RestApi
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
          CreateWebHostBuilder(args)
            .Build().Run();
         
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
              CustomWebHost.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((context, configBuilder) =>
                 {
                     var shouldAddKeyVault = TryFindingKeyvaultConnectionDetails(out string keyVaultUrl, out string clientId, out string clientSecret);

                     if (shouldAddKeyVault)
                     {
                         Trace.WriteLine("Program.cs: Adding key vault");
                         configBuilder.AddAzureKeyVault(keyVaultUrl, clientId, clientSecret);
                     }
                     else
                     {
                         Trace.WriteLine("Program.cs: Key vault url empty");
                     }
                 })
                 .ConfigureLogging((hostingContext, builder) =>
                 {

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

        public static bool TryFindingKeyvaultConnectionDetails(out string keyVaultUrl, out string clientId, out string clientSecret)
        {
            keyVaultUrl = System.Environment.GetEnvironmentVariable(ConfigConstants.KEY_VAULT, EnvironmentVariableTarget.Process);

            if (string.IsNullOrWhiteSpace(keyVaultUrl))
            {
                Trace.WriteLine("Program.cs: Key vault url empty");
            }
            else
            {
                Trace.WriteLine("Program.cs: Key vault url found. Initializing key vault");

                clientId = System.Environment.GetEnvironmentVariable(ConfigConstants.RADIX_SECRET_AZ_CLIENT_ID, EnvironmentVariableTarget.Process);
                clientSecret = System.Environment.GetEnvironmentVariable(ConfigConstants.RADIX_SECRET_AZ_CLIENT_SECRET, EnvironmentVariableTarget.Process);
                return true;
            }

            clientId = null;
            clientSecret = null;
            return false;
        }
    }
}
