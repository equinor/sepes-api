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
using System.IO;
using System.Linq;

namespace Sepes.RestApi
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();

            // var isIntegrationTestRaw = System.Environment.GetEnvironmentVariable("SEPES_IS_INTEGRATION_TEST", EnvironmentVariableTarget.Process);
            // var isIntegrationTest = !String.IsNullOrWhiteSpace(isIntegrationTestRaw) && isIntegrationTestRaw.ToLower().Equals("true");

            //new WebHostBuilder()
            //    .UseKestrel()
            //    .UseContentRoot(Directory.GetCurrentDirectory())
            //    .UseIISIntegration()
            //    .UseStartup<Startup>()
            //    .ConfigureAppConfiguration((context, configBuilder) =>
            //    {

            //        configBuilder.AddJsonFile("appsettings.json", true, false);                  


            //        if (isIntegrationTest)
            //        {
            //            configBuilder.AddJsonFile("appsettings.UnitTest.json", true, false);
            //        }
            //        else
            //        {
            //            configBuilder.AddJsonFile("appsettings.Development.json", true, false);
            //        }
            //        configBuilder.AddUserSecrets<Program>();
            //        configBuilder.AddEnvironmentVariables(ConfigConstants.ENV_VARIABLE_PREFIX);

            //        var config = configBuilder.Build(); 

            //        var keyVaultUrl = config[ConfigConstants.KEY_VAULT];

            //        if (string.IsNullOrWhiteSpace(keyVaultUrl))
            //        {
            //            Trace.WriteLine("Program.cs: Key vault url empty");
            //        }
            //        else
            //        {
            //            Trace.WriteLine("Program.cs: Key vault url found. Initializing key vault");

            //            var clientId = config[ConfigConstants.AZ_CLIENT_ID];
            //            var clientSecret = config[ConfigConstants.AZ_CLIENT_SECRET];
            //            configBuilder.AddAzureKeyVault(keyVaultUrl, clientId, clientSecret);
            //        }
            //    })
            //     .ConfigureLogging((hostingContext, builder) => {

            //         var applicationInsightsInstrumentationKey = hostingContext.Configuration[ConfigConstants.APPI_KEY];

            //         builder.AddApplicationInsights(applicationInsightsInstrumentationKey);

            //         // Adding the filter below to ensure logs of all severity from Program.cs
            //         // is sent to ApplicationInsights.
            //         builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
            //                          (typeof(Program).FullName, LogLevel.Trace);

            //         // Adding the filter below to ensure logs of all severity from Startup.cs
            //         // is sent to ApplicationInsights.
            //         builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
            //                          (typeof(Startup).FullName, LogLevel.Trace);

            //     })
            //    .Build().Run();



        }

       

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var shouldAddKeyVault = TryFindingKeyvaultConnectionDetails(out string keyVaultUrl, out string clientId, out string clientSecret);

            var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory());

            configBuilder.AddJsonFile("appsettings.json", true, false);
            configBuilder.AddJsonFile("appsettings.Development.json", true, false);

            configBuilder.AddUserSecrets<Program>(optional: true, reloadOnChange: false);
            configBuilder.AddEnvironmentVariables(ConfigConstants.ENV_VARIABLE_PREFIX);
            configBuilder.AddCommandLine(args);

            if (shouldAddKeyVault)
            {
                configBuilder.AddAzureKeyVault(keyVaultUrl, clientId, clientSecret);
            }            

            return WebHost.CreateDefaultBuilder(args)
                     .UseConfiguration(configBuilder.Build())
                   //.ConfigureAppConfiguration((context, configBuilder) =>
                   //{
                   //    configBuilder.AddEnvironmentVariables(ConfigConstants.ENV_VARIABLE_PREFIX);

                   //    var config = configBuilder.Build();

                   //    var isIntegrationTest = ConfigUtil.GetBoolConfig(config, "IS_INTEGRATION_TEST");

                   //    foreach (var curSource in configBuilder.Sources.ToList())
                   //    {
                   //        if (curSource.GetType().Name == typeof(JsonConfigurationSource).Name)
                   //        {
                   //            var fileSource = curSource as JsonConfigurationSource;

                   //            if (isIntegrationTest && fileSource.Path.ToLower().Equals("appsettings.development.json"))
                   //            {
                   //                configBuilder.Sources.Remove(curSource);
                   //                configBuilder.AddJsonFile("appsettings.UnitTest.json", true, false);                       
                   //            }
                   //            else
                   //            {
                   //                fileSource.ReloadOnChange = false;
                   //            }                       
                   //        }
                   //    }

                   //    config = configBuilder.Build();              

                   //    var keyVaultUrl = config[ConfigConstants.KEY_VAULT];

                   //    if (string.IsNullOrWhiteSpace(keyVaultUrl))
                   //    {
                   //        Trace.WriteLine("Program.cs: Key vault url empty");
                   //    }
                   //    else
                   //    {
                   //        Trace.WriteLine("Program.cs: Key vault url found. Initializing key vault");

                   //        var clientId = config[ConfigConstants.AZ_CLIENT_ID];
                   //        var clientSecret = config[ConfigConstants.AZ_CLIENT_SECRET];
                   //        configBuilder.AddAzureKeyVault(keyVaultUrl, clientId, clientSecret);
                   //    }
                   //})
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

        }

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
