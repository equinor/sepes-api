using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets
{
    public static class CloudResourceBasicAsserts
    {
        public static void ResourceBasicAsserts(CloudResource cloudResource, string resourceType, string purpose)
        {
            Assert.NotNull(cloudResource);
            Assert.Equal(resourceType, cloudResource.ResourceType);
            Assert.Equal(purpose, cloudResource.Purpose);
        }

        public static void ResourceAfterCreationAsserts(CloudResource cloudResource)
        {
            Assert.NotNull(cloudResource.ResourceId);
            Assert.NotNull(cloudResource.ResourceGroupName);
            Assert.NotNull(cloudResource.ResourceName);
            Assert.NotNull(cloudResource.CreatedBy);
            Assert.NotNull(cloudResource.LastKnownProvisioningState);
        }

        public static void ResourceGroupAsserts(CloudResource cloudResource, string purpose)
        {
            ResourceBasicAsserts(cloudResource, AzureResourceType.ResourceGroup, purpose);
        }

        public static void StudyDatasetResourceGroupBasicAsserts(CloudResource cloudResource)
        {
            ResourceGroupAsserts(cloudResource, CloudResourcePurpose.StudySpecificDatasetContainer);

            Assert.Null(cloudResource.SandboxId);
            Assert.Null(cloudResource.Sandbox);
            Assert.Null(cloudResource.ParentResourceId);

            Assert.NotEmpty(cloudResource.Operations);

            foreach (var curOp in cloudResource.Operations)
            {               
                Assert.NotNull(curOp.CreatedBy);
                Assert.NotNull(curOp.CreatedBySessionId);
                Assert.Null(curOp.QueueMessageId);
                Assert.Null(curOp.QueueMessagePopReceipt);
                Assert.Null(curOp.QueueMessageVisibleAgainAt);
            }
        }

        public static void StudyDatasetResourceGroupBeforeProvisioningAssert(CloudResource cloudResource) {
           
            StudyDatasetResourceGroupBasicAsserts(cloudResource);

            Assert.Null(cloudResource.LastKnownProvisioningState);      

            foreach (var curOp in cloudResource.Operations)
            {
                Assert.Equal(CloudResourceOperationState.NEW, curOp.Status);
                Assert.NotNull(curOp.CreatedBy);                
                Assert.Null(curOp.CarriedOutBySessionId);
            }
        }

        public static void StudyDatasetResourceGroupAfterProvisioningAssert(CloudResource cloudResource)
        {
            StudyDatasetResourceGroupBasicAsserts(cloudResource);

            ResourceAfterCreationAsserts(cloudResource);

            Assert.Equal(CloudResourceProvisioningStates.SUCCEEDED, cloudResource.LastKnownProvisioningState);

            foreach (var curOp in cloudResource.Operations)
            {
                Assert.Equal(CloudResourceOperationState.DONE_SUCCESSFUL, curOp.Status);              
                Assert.NotNull(curOp.CarriedOutBySessionId);
            }
        }
    }
}
