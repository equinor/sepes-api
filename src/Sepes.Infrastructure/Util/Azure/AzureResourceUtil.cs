using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Dto.Provisioning;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Dto.Configuration;
using System;
using System.Text;

namespace Sepes.Infrastructure.Util
{
    public static class AzureResourceUtil
    {
        public static ResourceProvisioningResult CreateResultFromIResource(IResource resource)
        {
            return new ResourceProvisioningResult() { Resource = resource};
        }

        public static void ThrowIfResourceIsNull(IResource resource, string resourceType, string name, string errorMessagePrefix)
        {
            if (resource == null)
            {
                throw new System.Exception($"{errorMessagePrefix}: Resource {resourceType} with name {name} was not found");
            }
        }

        public static string CreateDescriptionForResourceOperation(string resourceType, string operationType, int resourceId = 0)
        {
            var description = $"{operationType} {resourceType}";

            if (resourceId > 0)
            {
                description += $" with id {resourceId}";
            }           

            return description;
        }

        public static string CreateDescriptionForStudyResourceOperation(string resourceType, string operationType, int studyId = 0, int resourceId = 0)
        {
            var description = CreateDescriptionForResourceOperation(resourceType, operationType, resourceId);

            if(studyId > 0)
            {
                description += $" for study {studyId}";
            }           

            return description;
        }

        public static string CreateDescriptionForSandboxResourceOperation(string resourceType, string operationType, int sandboxId, int resourceId = 0)
        {
            var description = CreateDescriptionForResourceOperation(resourceType, operationType, resourceId); 

            description += $" for sandbox {sandboxId}";

            return description;
        }

        public static string CreateResourceOperationErrorMessage(Exception ex)
        {
            var messageBuilder = new StringBuilder(ex.Message);

            if (ex.InnerException != null)
            {
                messageBuilder.AppendLine(CreateResourceOperationErrorMessage(ex.InnerException));
            }

            return messageBuilder.ToString();
        }

        public static string CreateResourceLink(IConfiguration config, CloudResource resource)
        {
            return CreateResourceLink(config, resource.ResourceId);
        }

        public static string CreateResourceLink(IConfiguration config, string resourceId)
        {
            if (String.IsNullOrWhiteSpace(resourceId))
            {
                return null;
            }           

            if (resourceId == AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME)
            {
                return null;
            }

            var domain = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.AZ_DOMAIN);

            return CreateResourceLink(domain, resourceId);
        }

        public static string CreateResourceLink(string domain, string resourceId)
        {
            var azureUrlPart = "https://portal.azure.com/#@";
            return $"{azureUrlPart}{domain}/resource{resourceId}";
        }

        public static string CreateResourceRetryLink(int resourceId)
        {
            return $"api/resources/{resourceId}/retry";
        }

        public static string CreateResourceCostLink(IConfiguration config, Sandbox sandbox)
        {
            var resourceGroupEntry = CloudResourceUtil.GetSandboxResourceGroupEntry(sandbox.Resources);

            if (resourceGroupEntry == null)

            {
                return null;
            }

            var resourceGroupId = resourceGroupEntry.ResourceId;

            if (String.IsNullOrWhiteSpace(resourceGroupId))
            {
                return null;
            }
            
            var domain = ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.AZ_DOMAIN);           

            if (resourceGroupId == AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME)
            {
                return null;
            }

            return CreateResourceCostLink(domain, resourceGroupId);
        }

        public static string CreateResourceCostLink(string domain, string resourceId)
        {
            var azureUrlPart = "https://portal.azure.com/#@";
            return $"{azureUrlPart}{domain}/resource{resourceId}/costanalysis";
        }
    }
}
