using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Common.Util.Provisioning;
using System.Collections.Generic;
using System.Linq;
using Sepes.Azure.Util;

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

            var createOperationDescription = ResourceOperationDescriptionUtils.CreateDescriptionForResourceOperation(resourceType, CloudResourceOperationType.CREATE, studyId: studyId);
            SetOperationProperties(newResource, createOperationDescription);

            return newResource;
        }

        public static CloudResource CreateStudySpecificDatasetStorageAccountEntry(UserDto currentUser, string sessionId, int datasetId,
         string region, int resourceGroupId, string resourceGroupName, string resourceName, Dictionary<string, string> tags, int dependsOnId
          )
        {
            var resourceType = AzureResourceType.StorageAccount;
            var newResource = CreateBasicResource(currentUser, sessionId, region, resourceType, resourceName, tags, resourceGroupName);

            newResource.DatasetId = datasetId;
            newResource.ParentResourceId = resourceGroupId;

            newResource.Purpose = CloudResourcePurpose.StudySpecificDatasetStorageAccount;

            var createOperationDescription = ResourceOperationDescriptionUtils.CreateDescriptionForResourceOperation(resourceType, CloudResourceOperationType.CREATE);
            SetOperationProperties(newResource, createOperationDescription, operationDependsOn: dependsOnId);

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

            var createOperationDescription = ResourceOperationDescriptionUtils.CreateDescriptionForResourceOperation(resourceType, CloudResourceOperationType.CREATE, sandboxId: sandboxId);
            SetOperationProperties(newResource, createOperationDescription, batchId);

            return newResource;
        }

        public static CloudResource CreateSandboxResourceEntry(UserDto currentUser, string sessionId, int sandboxId, string region, string resourceType, int resourceGroupId, string resourceName, Dictionary<string, string> tags,
            string configString = null,
            string batchId = null,
            int dependsOn = 0,
            string resourceGroupName = null, bool sandboxControlled = true
        )
        {
            var newResource = CreateBasicResource(currentUser, sessionId, region, resourceType, resourceName, tags, resourceGroupName);

            newResource.SandboxId = sandboxId;
            newResource.SandboxControlled = sandboxControlled;
            newResource.ParentResourceId = resourceGroupId;
            newResource.Purpose = CloudResourcePurpose.SandboxResource;
            newResource.ConfigString = configString;

            var createOperationDescription = ResourceOperationDescriptionUtils.CreateDescriptionForResourceOperation(resourceType, CloudResourceOperationType.CREATE, sandboxId: sandboxId);
            SetOperationProperties(newResource, createOperationDescription, batchId, dependsOn);

            return newResource;
        }

        public static CloudResource CreateBasicResource(UserDto currentUser, string sessionId, string region, string resourceType, string resourceName, Dictionary<string, string> tags, string resourceGroupName = null
            )
        {
            var tagsString = TagUtils.TagDictionaryToString(tags);

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
                    Status = CloudResourceOperationState.NEW,
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
