using Sepes.Infrastructure.Util;
using Xunit;

namespace Sepes.Tests.Util
{

    public class AzureResourceNameUtilTest
    {
        [Fact]
        public void ResourceGroupName_ShouldContainStudyAndSandboxName()
        {
            var resourceGroupName = AzureResourceNameUtil.ResourceGroup("Very Secret Software Inc", "First attempt at finding good data");
            Assert.InRange(resourceGroupName.Length, 6, 64);
         
            Assert.Contains("rg-study-", resourceGroupName);
            Assert.Contains("verysecretsoftwareinc", resourceGroupName);
            Assert.Contains("firstattemptatfindinggooddata", resourceGroupName);

        }

        [Fact]
        public void ResourceGroupName_ShouldNotExceed64Characters()
        {
            var resourceGroupName = AzureResourceNameUtil.ResourceGroup("Very Very Secret Software Inc", "First attempt at finding good data");
            Assert.InRange(resourceGroupName.Length, 6, 64);
            Assert.Contains("rg-study-", resourceGroupName);
            Assert.Contains("veryverysecretsoftwarei", resourceGroupName);
            Assert.Contains("firstattemptatfindinggoodda", resourceGroupName);

        }

        [Fact]
        public void DiagStorageAccountName_ShouldNotExceed24Characters()
        {
            var resourceName = AzureResourceNameUtil.DiagnosticsStorageAccount("Bestest Study Ever", "The third test we are going to to");
            Assert.InRange(resourceName.Length, 4, 24);
            Assert.Contains("stdiag", resourceName);
            Assert.Contains("best", resourceName);
            Assert.Contains("thethi", resourceName);


            var resourceName2 = AzureResourceNameUtil.DiagnosticsStorageAccount("Bestest Study Ever", "The third test we are going to too");
            Assert.InRange(resourceName2.Length, 4, 24);
            Assert.Contains("stdiag", resourceName2);
            Assert.Contains("best", resourceName2);
            Assert.Contains("thethi", resourceName2);

        }

    }
}
