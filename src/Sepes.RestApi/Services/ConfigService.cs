using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sepes.Infrastructure.Dto;

namespace Sepes.RestApi.Services
{
    public interface IConfigService
    {
        string DbOwnerConnectionString { get; }
        string DbReadWriteConnectionString { get; }
    
        AzureConfig AzureConfig { get; }
        string InstrumentationKey { get; }
        TokenValidationParameters TokenValidation { get; }
        AuthConfig AuthConfig { get; }
        bool HttpOnly { get; }
    }

    public class ConfigService : IConfigService
    {

        public string DbOwnerConnectionString { get; }

        public string DbReadWriteConnectionString { get; }

        public AzureConfig AzureConfig { get; }
        public string InstrumentationKey { get; }
        public TokenValidationParameters TokenValidation { get; }
        public AuthConfig AuthConfig { get; }
        public bool HttpOnly { get; } = false;

        [ExcludeFromCodeCoverage]
        public static IConfigService CreateConfig(IConfiguration configuration)
        {
            return new ConfigService(
                configuration
                //,
                //new ConfigurationBuilder().AddEnvironmentVariables("SEPES_").Build()
            );
        }
        [ExcludeFromCodeCoverage]
        public static void LoadDevEnv()
        {

            try
            {
                var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

                // No standard loop fit.
                while (true)
                {
                    // We are at root before we found "RestApi"
                    // Bail the function
                    if (currentDirectory == null) return;
                    // Found it bail the loop.
                    if (currentDirectory.Name == "src") break;
                    // Try next parent.
                    currentDirectory = currentDirectory.Parent;
                }
                var projectRoot = currentDirectory.Parent;
                var envPath = projectRoot.FullName + "/.env";

                DotEnv.Config(true, envPath);
                Console.WriteLine($"Found .env file at: {envPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No .env file found! Skipping .env file variable fetch. Exception message: {ex.Message}");
            }
        }

        public ConfigService(IConfiguration config)
        { 
            AzureConfig = new AzureConfig(
                config["TenantId"],
                config["ClientId"],
                config["ClientSecret"],
                config["SUBSCRIPTION_ID"],
                $"{config["name"]}-{config["Azure:CommonResourceGroupName"]}",
                config["JOIN_NETWORK_ROLE_NAME"]
            );
            InstrumentationKey = config["SEPES_Appi_Key"];

            TokenValidation = new TokenValidationParameters
            {
                ValidateIssuer = false,  //Issue: 39 set to true before MVP
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false          
                //SaveSigninToken = true  
            };

            AuthConfig = new AuthConfig
            {
                Key = config["Jwt:Key"],
                Issuer = config["Jwt:Issuer"],
            };

            if (config["HTTP_ONLY"] == "true")
            {
                this.HttpOnly = true;
            }

            //var getWithKeyVaultKeys

            var ownerConnectionStringFromKeyVault = config["sepesowner-connectionstring"];
            var rwConnectionStringFromKeyVault = config["sepesrw-connectionstring"];

            //If any of key vault DBs are empty, use from environment
            if(String.IsNullOrWhiteSpace(ownerConnectionStringFromKeyVault)  || String.IsNullOrWhiteSpace(rwConnectionStringFromKeyVault) )
            {
                DbOwnerConnectionString = config["SEPES_DB_OWNER_CONNECTION_STRING"];
                DbReadWriteConnectionString = config["SEPES_DB_RW_CONNECTION_STRING"];
            }
            else
            {
                DbOwnerConnectionString = ownerConnectionStringFromKeyVault;
                DbReadWriteConnectionString = rwConnectionStringFromKeyVault;
            }          
        } 
    }
}