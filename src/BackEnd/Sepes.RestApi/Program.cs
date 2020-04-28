using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sepes.RestApi.Services;
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
                 ConfigService.LoadDevEnv();

                 var config = configBuilder.AddEnvironmentVariables("SEPES_").Build();

                 var keyVaultUrl = config["KEYVAULT_URL"];
                 var clientId = config["CLIENT_ID"];
                 var clientSecret = config["CLIENT_SECRET"];

                 if (!string.IsNullOrWhiteSpace(keyVaultUrl))
                 {
                     //TODO: ADD BACK IN ONCE ACCESS POLICY ER KLAR
                    configBuilder.AddAzureKeyVault(keyVaultUrl, clientId, clientSecret);
                 }                

             })
                .UseStartup<Startup>();
    }
}
