using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using System;

namespace Sepes.Azure.Service
{
    public class FunctionAzureCredentialService : IAzureCredentialService
    {
        protected string _subscriptionId;
        protected readonly IConfiguration _config;
        protected readonly ILogger _logger;

        public FunctionAzureCredentialService(IConfiguration config, ILogger<FunctionAzureCredentialService> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionId = _config[ConfigConstants.SUBSCRIPTION_ID];
        }

        public AzureCredentials GetAzureCredentials()
        {
            var defaultCredential = new DefaultAzureCredential();
            var defaultToken = defaultCredential.GetToken(new TokenRequestContext(new[] { "https://management.azure.com/.default" })).Token;
            var defaultTokenCredentials = new Microsoft.Rest.TokenCredentials(defaultToken);
            return new AzureCredentials(defaultTokenCredentials, defaultTokenCredentials, null, AzureEnvironment.AzureGlobalCloud);       
        }
    }
}
