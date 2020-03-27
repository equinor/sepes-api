using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Services
{
    public interface IConfigService
    {
        string connectionString { get; }
        AzureConfig azureConfig { get; }
        string instrumentationKey { get; }
        TokenValidationParameters tokenValidation { get; }
        AuthConfig authConfig { get; }
        bool httpOnly { get; } 
    }

    public class ConfigService : IConfigService
    {
        [ExcludeFromCodeCoverage]
        public static IConfigService CreateConfig(IConfiguration configuration){
            return new ConfigService(
                configuration,
                new ConfigurationBuilder().AddEnvironmentVariables("SEPES_").Build()
            );
        }
        [ExcludeFromCodeCoverage]
        public static void LoadDevEnv(){
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            // No standard loop fit.
            while(true){
                // We are at root before we found "RestApi"
                // Bail the function
                if(currentDirectory == null) return;
                // Found it bail the loop.
                if(currentDirectory.Name == "RestApi") break;
                // Try next parent.
                currentDirectory = currentDirectory.Parent;
            }
            var projectRoot = currentDirectory.Parent;
            var envPath = projectRoot.FullName + "/.env";

            DotEnv.Config(false, envPath);
            Console.WriteLine($"Found .env file at: {envPath}");
        }

        public string connectionString { get; }
        public AzureConfig azureConfig { get; }
        public string instrumentationKey { get; }
        public TokenValidationParameters tokenValidation { get; }
        public AuthConfig authConfig { get; }
        public bool httpOnly { get; } = false;

        public ConfigService(IConfiguration asp, IConfiguration sepes)
        {
            connectionString = sepes["MSSQL_CONNECTION_STRING"];
            azureConfig = new AzureConfig(
                sepes["TENANT_ID"],
                sepes["CLIENT_ID"],
                sepes["CLIENT_SECRET"],
                sepes["SUBSCRIPTION_ID"],
                $"{sepes["name"]}-{asp["Azure:CommonResourceGroupName"]}",
                sepes["JOIN_NETWORK_ROLE_NAME"]
            );
            instrumentationKey = sepes["INSTRUMENTATION_KEY"];

            tokenValidation = new TokenValidationParameters
            {
                ValidateIssuer = false,  //Issue: 39 set to true before MVP
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = asp["Jwt:Issuer"],
                ValidAudience = asp["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(asp["Jwt:Key"]))
                //SaveSigninToken = true  
            };

            authConfig = new AuthConfig
            {
                Key = asp["Jwt:Key"],
                Issuer = asp["Jwt:Issuer"],
            };

            if(sepes["HTTP_ONLY"] == "true"){
                this.httpOnly = true;
            }
        }
    }
}