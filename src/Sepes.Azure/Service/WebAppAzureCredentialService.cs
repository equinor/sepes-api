using Azure.Identity;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Util;
using System;

namespace Sepes.Azure.Service
{
    public class WebAppAzureCredentialService : IAzureCredentialService
    {
        protected string _subscriptionId;

        protected readonly IConfiguration _config;
        protected readonly ILogger _logger;

        public WebAppAzureCredentialService(IConfiguration config, ILogger<WebAppAzureCredentialService> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionId = _config[ConfigConstants.SUBSCRIPTION_ID];
        }

        public AzureCredentials GetAzureCredentials()
        {
            var tenantId = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.AZ_TENANT_ID);
            var clientId = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.AZ_CLIENT_ID); 
            var clientSecret = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.AZ_CLIENT_SECRET);

            return new AzureCredentialsFactory().FromServicePrincipal(clientId, clientSecret, tenantId, AzureEnvironment.AzureGlobalCloud).WithDefaultSubscription(_subscriptionId);
        }
    }
}
