//using System;
//using System.Diagnostics.CodeAnalysis;
//using System.IO;
//using System.Text;
//using dotenv.net;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using Sepes.Infrastructure.Dto;

//namespace Sepes.RestApi.Services
//{
//    public interface IConfigService
//    {
//        string DbOwnerConnectionString { get; }
//        string DbReadWriteConnectionString { get; }
    
//        AzureConfig AzureConfig { get; }
//        string InstrumentationKey { get; }
//        TokenValidationParameters TokenValidation { get; }
//        AuthConfig AuthConfig { get; }
//        bool HttpOnly { get; }
//    }

//    public class ConfigService : IConfigService
//    {

//        public string DbOwnerConnectionString { get; }

//        public string DbReadWriteConnectionString { get; }

//        public AzureConfig AzureConfig { get; }
//        public string InstrumentationKey { get; }
//        public TokenValidationParameters TokenValidation { get; }
//        public AuthConfig AuthConfig { get; }
//        public bool HttpOnly { get; } = false;

//        [ExcludeFromCodeCoverage]
//        public static IConfigService CreateConfig(IConfiguration configuration)
//        {
//            return new ConfigService(configuration);
//        }
       

//        public ConfigService(IConfiguration config)
//        { 
//            AzureConfig = new AzureConfig(
//                config["TenantId"],
//                config["ClientId"],
//                config["ClientSecret"],
//                config["SUBSCRIPTION_ID"],
//                $"{config["name"]}-{config["Azure:CommonResourceGroupName"]}",
//                config["JOIN_NETWORK_ROLE_NAME"]
//            );
//            InstrumentationKey = config["Appi_Key"];

//            TokenValidation = new TokenValidationParameters
//            {
//                ValidateIssuer = false,  //Issue: 39 set to true before MVP
//                ValidateAudience = false,
//                ValidateLifetime = false,
//                ValidateIssuerSigningKey = false          
//                //SaveSigninToken = true  
//            };

//            AuthConfig = new AuthConfig
//            {
//                Key = config["Jwt:Key"],
//                Issuer = config["Jwt:Issuer"],
//            };

//            if (config["HttpOnly"] == "true")
//            {
//                this.HttpOnly = true;
//            }

//            //var getWithKeyVaultKeys

//            var ownerConnectionStringFromKeyVault = config["sepesowner-connectionstring"];
//            var rwConnectionStringFromKeyVault = config["sepesrw-connectionstring"];

//            //If any of key vault DBs are empty, use from environment
//            if(String.IsNullOrWhiteSpace(ownerConnectionStringFromKeyVault)  || String.IsNullOrWhiteSpace(rwConnectionStringFromKeyVault) )
//            {
//                DbOwnerConnectionString = config["SEPES_DB_OWNER_CONNECTION_STRING"];
//                DbReadWriteConnectionString = config["SEPES_DB_RW_CONNECTION_STRING"];
//            }
//            else
//            {
//                DbOwnerConnectionString = ownerConnectionStringFromKeyVault;
//                DbReadWriteConnectionString = rwConnectionStringFromKeyVault;
//            }          
//        } 
//    }
//}