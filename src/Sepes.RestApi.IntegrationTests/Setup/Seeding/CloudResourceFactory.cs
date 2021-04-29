using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class CloudResourceFactory
    {
        public static CloudResource CreateResourceGroup(string region, string name, string resourceId = null, string resourceKey = null, string purpose = null, bool sandboxControlled = false, string batchId = null, bool createOperationFinished = true)
        {
            return Create(region, AzureResourceType.ResourceGroup, name, name, resourceId, resourceKey, purpose, sandboxControlled: sandboxControlled, batchId: batchId, createOperationFinished: createOperationFinished);
        }

        public static CloudResource CreateResourceGroupFailing(string region, string name, string resourceId = null, string resourceKey = null, string purpose = null, bool sandboxControlled = false, string batchId = null,
               string statusOfFailedResource = CloudResourceOperationState.FAILED, int tryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT, int maxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT)
        {
            return CreateFailing(region, AzureResourceType.ResourceGroup, name, name, resourceId, resourceKey, purpose, sandboxControlled: sandboxControlled, batchId: batchId, statusOfFailedResource: statusOfFailedResource, tryCount: tryCount, maxTryCount: maxTryCount);
        }

        public static CloudResource Create(string region,
            string resourceType,
            string resourceGroup,
            string resourceName,
            string resourceId = null,
            string resourceKey = null,
            string purpose = null,
            bool sandboxControlled = false,
            string batchId = null,
            CloudResource parentResource = null,
            bool createOperationFinished = true,
            bool deleted = false,
            bool deleteSucceeded = false)
        {
            var cloudResource = CreateBasic(region, resourceType, resourceGroup, resourceName, resourceId, resourceKey, purpose, sandboxControlled, parentResource);
            cloudResource.Operations.Add(createOperationFinished ? CloudResourceOperationFactory.SucceededOperation("create" + resourceName, batchId: batchId) : CloudResourceOperationFactory.NewOperation("create" + resourceName, batchId: batchId));
            
            if(deleted)
            {
                cloudResource.Operations.Add(deleteSucceeded ?
                    CloudResourceOperationFactory.SucceededOperation("delete" + resourceName, operationType: CloudResourceOperationType.DELETE)
                    : CloudResourceOperationFactory.NewOperation("delete" + resourceName, operationType: CloudResourceOperationType.DELETE));
            }
            
            return cloudResource;
        }

        public static CloudResource CreateFailing(string region,
            string resourceType,
            string resourceGroup,
            string resourceName,
            string resourceId = null,
            string resourceKey = null,
            string purpose = null,
            bool sandboxControlled = false,
            string batchId = null,
            CloudResource parentResource = null,
            string statusOfFailedResource = CloudResourceOperationState.FAILED,
            int tryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT,
            int maxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT,
            bool deleted = false,
            bool deleteSucceeded = false

            )
        {
            var cloudResource = CreateBasic(region, resourceType, resourceGroup, resourceName, resourceId, resourceKey, purpose, sandboxControlled, parentResource);
            cloudResource.LastKnownProvisioningState = null;
            cloudResource.Operations.Add(CloudResourceOperationFactory.FailedOperation("create" + resourceName, batchId: batchId, status: statusOfFailedResource, tryCount: tryCount, maxTryCount: maxTryCount));

            if (deleted)
            {
                cloudResource.Operations.Add(deleteSucceeded ?
                    CloudResourceOperationFactory.SucceededOperation("delete" + resourceName, operationType: CloudResourceOperationType.DELETE)
                    : CloudResourceOperationFactory.NewOperation("delete" + resourceName, operationType: CloudResourceOperationType.DELETE));
            }

            return cloudResource;
        }

        static CloudResource CreateBasic(string region,
            string resourceType,
            string resourceGroup,
            string resourceName,
            string resourceId = null,
            string resourceKey = null,
            string purpose = null,
            bool sandboxControlled = false,
            CloudResource parentResource = null)
        {
            var cloudResource = new CloudResource
            {
                Region = region,
                ResourceType = resourceType,
                ResourceGroupName = resourceGroup,
                Purpose = purpose,
                SandboxControlled = sandboxControlled,
                ResourceId = String.IsNullOrWhiteSpace(resourceId) ? resourceName + "-id" : resourceId,
                ResourceKey = String.IsNullOrWhiteSpace(resourceKey) ? resourceName + "-id" : resourceKey,
                ResourceName = resourceName,
                Operations = new List<CloudResourceOperation>(),
                LastKnownProvisioningState = CloudResourceProvisioningStates.SUCCEEDED,
                CreatedBy = "seed",
                Created = DateTime.UtcNow,
                UpdatedBy = "seed",
                Updated = DateTime.UtcNow
            };

            if (parentResource != null)
            {
                if (parentResource.Id > 0)
                {
                    cloudResource.ParentResourceId = parentResource.Id;
                }
                else
                {
                    cloudResource.ParentResource = parentResource;
                }
            }

            return cloudResource;
        }

    }
}
