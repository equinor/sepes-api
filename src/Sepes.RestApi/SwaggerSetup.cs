using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Sepes.Infrastructure.Constants;
using System;
using System.Collections.Generic;

namespace Sepes.RestApi
{
    public static class SwaggerSetup
    {
        public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
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
                            Scopes = new Dictionary<string, string> { { $"{configuration[ConfigConstants.AZ_CLIENT_ID]}/User.Impersonation", "SEPES API" } },
                        }
                    },
                    Description = "Sepes Security Scheme"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "OAuth2",
                                    Type = ReferenceType.SecurityScheme                                    
                                } ,
                                Scheme = "OAuth2",
                                BearerFormat = "JWT",
                                Type = SecuritySchemeType.Http,
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            }, new List<string> { $"{configuration[ConfigConstants.AZ_CLIENT_ID]}/User.Impersonation" }

                        }
                    });
            });
        }

        public static void Configure(IConfiguration configuration, IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {                
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sepes API V1");
                c.OAuthClientId(configuration[ConfigConstants.AZ_CLIENT_ID]);                
                c.OAuthAppName("Sepes");
                c.OAuthScopeSeparator(" ");
                c.OAuthUsePkce();
            });
        }
    }
}
