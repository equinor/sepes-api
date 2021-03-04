using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class CloudResourceFactory
    {
        public static CloudResource CreateResourceGroup(string region, string name, string resourceId = null, string resourceKey = null, string purpose = null, bool sandboxControlled = false)
        {
            return Create(region, AzureResourceType.ResourceGroup, name, name, resourceId, resourceKey, purpose, sandboxControlled: sandboxControlled);
        }

            public static CloudResource Create(string region, string resourceType, string resourceGroup, string resourceName, string resourceId = null, string resourceKey = null, string purpose = null, bool sandboxControlled = false, CloudResource parentResource = null)
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
                Operations = new List<CloudResourceOperation>() { CreateOperationSucceeded("create" + resourceName) },
               LastKnownProvisioningState =  CloudResourceProvisioningStates.SUCCEEDED,
                CreatedBy = "seed",
                Created = DateTime.UtcNow,
                UpdatedBy = "seed",
                Updated = DateTime.UtcNow

            };

            if(parentResource != null)
            {
                if(parentResource.Id > 0)
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

        public static CloudResourceOperation CreateOperationNew(string description, string operationType = CloudResourceOperationType.CREATE, string batchId = null)
        {
            var operation = CreateOpeartionBasic(description, operationType, batchId: batchId);

            return operation;
        }

        public static CloudResourceOperation CreateOperationSucceeded(string description, string operationType = CloudResourceOperationType.CREATE, string batchId = null)
        {
            var operation = CreateOpeartionBasic(description, operationType, status: CloudResourceOperationState.DONE_SUCCESSFUL, batchId: batchId);
            return operation;
        }

        public static CloudResourceOperation CreateOpeartionBasic(string description, string operationType, string status = CloudResourceOperationState.NEW, string batchId = null)
        {
            return new CloudResourceOperation()
            {
                CreatedBySessionId = Guid.NewGuid().ToString(),
                Description = description,
                MaxTryCount = 5,
                OperationType = operationType,
                Status = status,
                BatchId = batchId,
                CreatedBy = "seed",
                Created = DateTime.UtcNow,
                UpdatedBy = "seed",
                Updated = DateTime.UtcNow

            };
        }

    }
}
