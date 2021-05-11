using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Common.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sepes.Tests.Util
{
    public class AzureResourceTypeUtilTest
    {
        [InlineData("Resource Group", AzureResourceType.ResourceGroup)]
        [InlineData("Storage Account", AzureResourceType.StorageAccount)]
        [InlineData("Virtual Network", AzureResourceType.VirtualNetwork)]
        [InlineData("Network Security Group", AzureResourceType.NetworkSecurityGroup)]
        [InlineData("Bastion", AzureResourceType.Bastion)]
        [InlineData("Virtual Machine", AzureResourceType.VirtualMachine)]
        [InlineData("n/a", "")]
        [InlineData("n/a", null)]
        [InlineData("n/a", "abc")]
        [Theory]
        public void StudySpecificDatasetResourceGroupName_ShouldContainStudyName(string expectedResult, string resourceType)
        {
            var resource = new CloudResource { ResourceType = resourceType };
            var result = AzureResourceTypeUtil.GetUserFriendlyName(resource);
            Assert.Equal(expectedResult, result);
        }
    }
}
