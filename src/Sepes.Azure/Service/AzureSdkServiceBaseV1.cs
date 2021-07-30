using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Azure.Util;
using Sepes.Common.Constants;
using Sepes.Common.Util;
using System;
using System.Collections.Generic;

namespace Sepes.Azure.Service
{
    public class AzureSdkServiceBaseV1
    {
        protected readonly IConfiguration _config;
        protected readonly ILogger _logger;

        protected readonly IAzure _azure;
        protected readonly AzureCredentials _credentials;

        protected string _subscriptionId;

        public AzureSdkServiceBaseV1(IConfiguration config, ILogger logger, IAzureCredentialService azureCredentialService)
     
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _subscriptionId = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.SUBSCRIPTION_ID);
         
            _credentials = azureCredentialService.GetAzureCredentials();          

            _azure = Microsoft.Azure.Management.Fluent.Azure.Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(_credentials).WithSubscription(_subscriptionId);  
        }

        protected void EnsureResourceIsManagedByThisIEnvironmentThrowIfNot(string resourceName, IReadOnlyDictionary<string, string> resourceTags)
        {
            var convertedTags = TagUtils.TagReadOnlyDictionaryToDictionary(resourceTags);
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceName, convertedTags);
        }

        public static void ContainsTagWithValueThrowIfError(IDictionary<string, string> resourceTags, string tagName, string expectedTagValue)
        {
            string actualTagValue;

            if (resourceTags.TryGetValue(tagName, out actualTagValue))
            {
                if (String.IsNullOrWhiteSpace(actualTagValue))
                {
                    throw new Exception($"Value of tag {tagName} was empty");
                }
                else
                {
                    if (expectedTagValue == actualTagValue)
                    {
                        return;
                    }
                    else
                    {
                        throw new Exception($"Value of tag {tagName} was different. Expected value: {expectedTagValue}, Actual value: {actualTagValue}");
                    }
                }
            }
            else
            {
                throw new Exception($"Resource is missing tag: {tagName}");
            }
        }

        protected void CheckIfResourceHasCorrectManagedByTagThrowIfNot(string resourceName, IDictionary<string, string> resourceTags)
        {
            try
            {
                var expectedTagValueFromConfig = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.MANAGED_BY);

                ContainsTagWithValueThrowIfError(resourceTags, CloudResourceConstants.MANAGED_BY_TAG_NAME, expectedTagValueFromConfig);

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

        protected Region GetRegionFromString(string regionName)
        {
            return RegionStringConverter.Convert(regionName);
        }
    }
}
