using System.Text;
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
    }

    public class ConfigService : IConfigService
    {
        public string connectionString { get; }
        public AzureConfig azureConfig { get; }
        public string instrumentationKey { get; }
        public TokenValidationParameters tokenValidation { get; }
        public AuthConfig authConfig { get; }

        public ConfigService(IConfiguration asp, IConfiguration sepes)
        {
            connectionString = sepes["MSSQL_CONNECTION_STRING"];
            azureConfig = new AzureConfig(
                sepes["TENANT_ID"],
                sepes["CLIENT_ID"],
                sepes["CLIENT_SECRET"],
                sepes["SUBSCRIPTION_ID"],
                $"{sepes["name"]}-{asp["Azure:CommonResourceGroupName"]}"
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
        }
    }
}