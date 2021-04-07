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
            CreateWebHostBuilder(args).Build().Run();
        }

        static void Log(string message)
        {
            Trace.WriteLine($"Program.cs: {message}");          
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
              CustomWebHost.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((context, configBuilder) =>
                 {
                     Log("ConfigureAppConfiguration");

                     var shouldAddKeyVault = TryGetKeyvaultConfig(out string keyVaultUrl, out string clientId, out string clientSecret);

                     if (shouldAddKeyVault)
                     {                       
                         configBuilder.AddAzureKeyVault(keyVaultUrl, clientId, clientSecret);
                     }                    
                 })
                 .ConfigureLogging((hostingContext, builder) =>
                 {
                     Log("ConfigureLogging");

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


        public static bool TryGetKeyvaultConfig(out string keyVaultUrl, out string clientId, out string clientSecret)
        {
            keyVaultUrl = Environment.GetEnvironmentVariable(ConfigConstants.KEY_VAULT, EnvironmentVariableTarget.Process);

            if (string.IsNullOrWhiteSpace(keyVaultUrl))
            {
                Log("Key vault url empty");
            }
            else
            {
                Log("Key vault url found. Initializing key vault");

                clientId = Environment.GetEnvironmentVariable(ConfigConstants.RADIX_SECRET_AZ_CLIENT_ID, EnvironmentVariableTarget.Process);
                clientSecret = Environment.GetEnvironmentVariable(ConfigConstants.RADIX_SECRET_AZ_CLIENT_SECRET, EnvironmentVariableTarget.Process);
                return true;
            }

            clientId = null;
            clientSecret = null;
            return false;
        }
    }
}
