using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.Infrastructure.Model.Factory
{
    public static class CloudResourceFactory
    {
        public static CloudResource CreateStudyResourceGroupEntry(UserDto currentUser, string sessionId,
          int studyId, string region, string resourceGroupName, Dictionary<string, string> tags
           )
        {
            var resourceType = AzureResourceType.ResourceGroup;
            var newResource = CreateBasicResource(currentUser, sessionId, region, resourceType, resourceGroupName, tags, resourceGroupName);

            newResource.StudyId = studyId;
            newResource.Purpose = CloudResourcePurpose.StudySpecificDatasetContainer;

            var createOperationDescription = AzureResourceUtil.CreateDescriptionForStudyResourceOperation(resourceType, CloudResourceOperationType.CREATE, studyId);
            SetOperationProperties(newResource, createOperationDescription);

            return newResource;
        }

        public static CloudResource CreateStudySpecificDatasetStorageAccountEntry(UserDto currentUser, string sessionId, int datasetId,
         string region, int resourceGroupId, string resourceGroupName, string resourceName, Dictionary<string, string> tags
          )
        {
            var resourceType = AzureResourceType.StorageAccount;
            var newResource = CreateBasicResource(currentUser, sessionId, region, resourceType, resourceName, tags, resourceGroupName);
            
            newResource.DatasetId = datasetId;
            newResource.ParentResourceId = resourceGroupId;

            newResource.Purpose = CloudResourcePurpose.StudySpecificDatasetStorageAccount;

            var createOperationDescription = AzureResourceUtil.CreateDescriptionForStudyResourceOperation(resourceType, CloudResourceOperationType.CREATE);
            SetOperationProperties(newResource, createOperationDescription);

            return newResource;
        }

        public static CloudResource CreateSandboxResourceGroupEntry(UserDto currentUser, string sessionId,
           int sandboxId, string region, string resourceGroupName, Dictionary<string, string> tags,
           string batchId
            )
        {
            var resourceType = AzureResourceType.ResourceGroup;
            var newResource = CreateBasicResource(currentUser, sessionId, region, resourceType, resourceGroupName, tags, resourceGroupName);

            newResource.SandboxId = sandboxId;
            newResource.SandboxControlled = true;
            newResource.Purpose = CloudResourcePurpose.SandboxResourceGroup;

            var createOperationDescription = AzureResourceUtil.CreateDescriptionForSandboxResourceOperation(resourceType, CloudResourceOperationType.CREATE, sandboxId);
            SetOperationProperties(newResource, createOperationDescription, batchId);

            return newResource;
        }

        public static CloudResource CreateSandboxResourceEntry(UserDto currentUser, string sessionId, int sandboxId, string region, string resourceType, int resourceGroupId, string resourceName, Dictionary<string, string> tags,
       string configString = null,
       string batchId = null,
       int dependsOn = 0,
      string resourceGroupName = null
        )
        {
            var newResource = CreateBasicResource(currentUser, sessionId, region, resourceType, resourceName, tags, resourceGroupName);

            newResource.SandboxId = sandboxId;
            newResource.SandboxControlled = true;
            newResource.ParentResourceId = resourceGroupId;
            newResource.Purpose = CloudResourcePurpose.SandboxResource;
            newResource.ConfigString = configString;

            var createOperationDescription = AzureResourceUtil.CreateDescriptionForSandboxResourceOperation(resourceType, CloudResourceOperationType.CREATE, sandboxId);
            SetOperationProperties(newResource, createOperationDescription, batchId, dependsOn);

            return newResource;
        }

        public static CloudResource CreateBasicResource(UserDto currentUser, string sessionId, string region, string resourceType, string resourceName, Dictionary<string, string> tags, string resourceGroupName = null
            )
        {
            var tagsString = AzureResourceTagsFactory.TagDictionaryToString(tags);

            var newResource = new CloudResource()
            {
                ResourceType = resourceType,
                Region = region,
                ResourceGroupName = resourceGroupName,
                ResourceName = resourceName,
                Tags = tagsString,
                ResourceKey = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME,
                ResourceId = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME,
                Operations = new List<CloudResourceOperation> {
                    new CloudResourceOperation()
                    {
                    OperationType = CloudResourceOperationType.CREATE,
                    CreatedBy = currentUser.UserName,
                    CreatedBySessionId = sessionId,
                    MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT
                    }
                },
                CreatedBy = currentUser.UserName
            };

            return newResource;
        }

        static void SetOperationProperties(CloudResource resource, string description, string batchId = null, int operationDependsOn = 0)
        {
            var operation = resource.Operations.SingleOrDefault();
            operation.Description = description;
            operation.BatchId = batchId;
            operation.DependsOnOperationId = operationDependsOn > 0 ? operationDependsOn : default(int?);
        }
    }
}
