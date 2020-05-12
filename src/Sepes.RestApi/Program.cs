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
                 var config = configBuilder.AddEnvironmentVariables("SEPES_").Build();

                 var keyVaultUrl = config["KeyVault_Url"];

                 if (!string.IsNullOrWhiteSpace(keyVaultUrl))
                 {
                     var clientId = config["ClientId"];
                     var clientSecret = config["ClientSecret"];

                     configBuilder.AddAzureKeyVault(keyVaultUrl, clientId, clientSecret);
                 }

             })
                .UseStartup<Startup>();
    }
}
