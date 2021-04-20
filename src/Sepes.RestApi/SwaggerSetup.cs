using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Sepes.Infrastructure.Constants;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.RestApi
{
    public static class SwaggerSetup
    {
        public static void Configure(IConfiguration configuration, Dictionary<string, string> scopes, IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,

                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"https://login.microsoftonline.com/{configuration[ConfigConstants.AZ_TENANT_ID]}/oauth2/v2.0/token"),
                            AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{configuration[ConfigConstants.AZ_TENANT_ID]}/oauth2/v2.0/authorize"),
                            Scopes = scopes
                        }
                    },
                    Description = "Sepes Security Scheme"
                });
            });
        }
    }
}
