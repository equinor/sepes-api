using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Provisioning;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Service
{
    public class AzureServiceBase
    {
        protected readonly IConfiguration _config;
        protected readonly ILogger _logger;
        protected readonly IAzure _azure;
        protected readonly AzureCredentials _credentials;

        protected string _subscriptionId;


        public AzureServiceBase(IConfiguration config, ILogger logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var tenantId = config[ConfigConstants.AZ_TENANT_ID];
            var clientId = config[ConfigConstants.AZ_CLIENT_ID];
            var clientSecret = config[ConfigConstants.AZ_CLIENT_SECRET];

            _subscriptionId = config[ConfigConstants.SUBSCRIPTION_ID];        

            _credentials = new AzureCredentialsFactory().FromServicePrincipal(clientId, clientSecret, tenantId, AzureEnvironment.AzureGlobalCloud).WithDefaultSubscription(_subscriptionId);

            _azure = Microsoft.Azure.Management.Fluent.Azure.Configure()
                .WithLogLevel(Microsoft.Azure.Management.ResourceManager.Fluent.Core.HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(_credentials).WithSubscription(_subscriptionId);  
        }

        protected void EnsureResourceIsManagedByThisIEnvironmentThrowIfNot(string resourceName, IReadOnlyDictionary<string, string> resourceTags)
        {
            var convertedTags = AzureResourceTagsFactory.TagReadOnlyDictionaryToDictionary(resourceTags);
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceName, convertedTags);
        }     

        protected void CheckIfResourceHasCorrectManagedByTagThrowIfNot(string resourceName, IDictionary<string, string> resourceTags)
        {
            try
            {
                AzureResourceTagsFactory.CheckIfResourceIsManagedByThisInstanceThrowIfNot(_config, resourceTags);
            }
            catch (Exception ex)
            {
                throw new Exception($"Attempting to modify Azure resource not managed by this instance: {resourceName} ", ex);
            }
          
        }

        protected string GetSharedVariableThrowIfNotFoundOrEmpty(ResourceProvisioningParameters parameters, string variableName, string descriptionForErrorMessage)
        {
            if (!parameters.TryGetSharedVariable(variableName, out string sharedVariableValue))
            {
                throw new ArgumentException($"{this.GetType().Name}: Missing {descriptionForErrorMessage} from input");
            }
            else if (String.IsNullOrWhiteSpace(sharedVariableValue))
            {
                throw new ArgumentException($"{this.GetType().Name}: Empty {descriptionForErrorMessage} from input");
            }

            return sharedVariableValue;
        }        
    }
}
