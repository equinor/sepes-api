using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sepes.Tests.Util
{
    public class AzureResourceStatusUtilTest
    {
        [Fact]
        public void VmRule_IsSameRule_withSameRule_shouldBeTrue()
        {
            var cloudResource = new CloudResource() { };

            var ex = Assert.Throws<ArgumentNullException>(() => AzureResourceStatusUtil.DecideWhatOperationToBaseStatusOn(cloudResource));

        }

        [Fact]
        public void VmRule_IsSameRule_withSameRule_shouldBeTrue2()
        {
            var cloudResourceOperation = new List<CloudResourceOperation>() { };
            var cloudResource = new CloudResource() { Operations = cloudResourceOperation };
            var result = AzureResourceStatusUtil.DecideWhatOperationToBaseStatusOn(cloudResource);
            Assert.Null(result);
        }

        [Fact]
        public void VmRule_IsSameRule_withSameRule_shouldBeTrue3()
        {
            var cloudResourceOperationList = new List<CloudResourceOperation>() { };
            var cloudOperation1 = new CloudResourceOperation() { Status = CloudResourceOperationState.DONE_SUCCESSFUL };
            cloudResourceOperationList.Add(cloudOperation1);
            var cloudResource = new CloudResource() { Operations = cloudResourceOperationList };
            var result = AzureResourceStatusUtil.DecideWhatOperationToBaseStatusOn(cloudResource);
            Assert.Equal(cloudOperation1, result);
        }

        [Fact]
        public void VmRule_IsSameRule_withSameRule_shouldBeTrue5()
        {
            var cloudResourceOperationList = new List<CloudResourceOperation>() { };
            var cloudOperation1 = new CloudResourceOperation() { Status = CloudResourceOperationState.FAILED };
            cloudResourceOperationList.Add(cloudOperation1);
            var cloudResource = new CloudResource() { Operations = cloudResourceOperationList };
            var result = AzureResourceStatusUtil.DecideWhatOperationToBaseStatusOn(cloudResource);
            Assert.Equal(cloudOperation1, result);
        }

        [Fact]
        public void VmRule_IsSameRule_withSameRule_shouldBeTrue4()
        {
            var cloudResourceOperationList = new List<CloudResourceOperation>() { };
            var cloudOperation1 = new CloudResourceOperation() { Status = CloudResourceOperationState.ABORTED };
            var cloudOperation2 = new CloudResourceOperation() { Status = CloudResourceOperationState.DONE_SUCCESSFUL };
            cloudResourceOperationList.Add(cloudOperation1);
            cloudResourceOperationList.Add(cloudOperation2);
            var cloudResource = new CloudResource() { Operations = cloudResourceOperationList };
            var result = AzureResourceStatusUtil.DecideWhatOperationToBaseStatusOn(cloudResource);
            Assert.Equal(cloudOperation2, result);
        }

        [Fact]
        public void VmRule_IsSameRule_withSameRule_shouldBeTrue6()
        {
            var cloudResourceOperationList = new List<CloudResourceOperation>() { };
            var cloudOperation1 = new CloudResourceOperation() { Status = CloudResourceOperationState.ABORTED };
            var cloudOperation2 = new CloudResourceOperation() { Status = CloudResourceOperationState.FAILED };
            cloudResourceOperationList.Add(cloudOperation1);
            cloudResourceOperationList.Add(cloudOperation2);
            var cloudResource = new CloudResource() { Operations = cloudResourceOperationList };
            var result = AzureResourceStatusUtil.DecideWhatOperationToBaseStatusOn(cloudResource);
            Assert.Equal(cloudOperation2, result);
        }

        [InlineData(CloudResourceOperationState.IN_PROGRESS, CloudResourceOperationType.CREATE, "Creating")]
        [InlineData(CloudResourceOperationState.IN_PROGRESS, CloudResourceOperationType.DELETE, "Deleting")]
        [InlineData(CloudResourceOperationState.IN_PROGRESS, CloudResourceOperationType.UPDATE, "Updating")]
        [InlineData(CloudResourceOperationState.NEW, CloudResourceOperationType.CREATE, "Creating (queued)")]
        [InlineData(CloudResourceOperationState.NEW, CloudResourceOperationType.DELETE, "Deleting (queued)")]
        [InlineData(CloudResourceOperationState.NEW, CloudResourceOperationType.UPDATE, "Updating (queued)")]
        [InlineData(CloudResourceOperationState.FAILED, CloudResourceOperationType.CREATE, "Create failed (0/0)")]
        [InlineData(CloudResourceOperationState.FAILED, CloudResourceOperationType.DELETE, "Delete failed (0/0)")]
        [InlineData(CloudResourceOperationState.FAILED, CloudResourceOperationType.UPDATE, "Update failed (0/0)")]
        [Theory]
        public void VmRule_IsSameRule_withSameRule_shouldBeTrue10(string status, string operationType, string expectedResult)
        {
            var cloudResourceOperationList = new List<CloudResourceOperation>() { };
            var cloudOperation1 = new CloudResourceOperation() { Status = status, OperationType = operationType };
            cloudResourceOperationList.Add(cloudOperation1);
            var cloudResource = new CloudResource() { Operations = cloudResourceOperationList };
            var result = AzureResourceStatusUtil.ResourceStatus(cloudResource);
            Assert.Equal(expectedResult, result);
        }

    }
}
