using Sepes.Azure.Util;
using Xunit;

namespace Sepes.Tests.Util
{

    public class AzureResourceNameUtilTest
    {

        [Fact]
        public void StudySpecificDatasetResourceGroupName_ShouldContainStudyName()
        {
            var resourceGroupName = AzureResourceNameUtil.StudySpecificDatasetResourceGroup("Very Secret Software Inc");
            Assert.InRange(resourceGroupName.Length, 6, 64);

            Assert.Contains("rg-study-verysecretsoftwareinc-datasets-", resourceGroupName);           
        }

        [Fact]
        public void SandboxResourceGroupName_ShouldContainStudyAndSandboxName()
        {
            var resourceGroupName = AzureResourceNameUtil.SandboxResourceGroup("Very Secret Software Inc", "First attempt at finding good data");
            Assert.InRange(resourceGroupName.Length, 6, 64);
         
            Assert.Contains("rg-study-", resourceGroupName);
            Assert.Contains("-verysecretsoftwareinc-", resourceGroupName);
            Assert.Contains("-firstattemptatfindinggooddata-", resourceGroupName);

        }

        [Fact]
        public void SandboxResourceGroupName_ShouldNotExceed64Characters()
        {
            var resourceGroupName = AzureResourceNameUtil.SandboxResourceGroup("Very Very Secret Software Inc", "First attempt at finding good data");
            Assert.InRange(resourceGroupName.Length, 6, 64);
            Assert.Contains("rg-study-", resourceGroupName);
            Assert.Contains("veryverysecretsoftwarei", resourceGroupName);
            Assert.Contains("firstattemptatfindinggood", resourceGroupName);
        }

        [Fact]
        public void DiagStorageAccountName_ShouldNotExceed24Characters()
        {
            var resourceName = AzureResourceNameUtil.DiagnosticsStorageAccount("Bestest Study Ever", "Strezztest1");
            Assert.InRange(resourceName.Length, 4, 24);
            Assert.Contains("stdiag", resourceName);
            Assert.Contains("bestes", resourceName);
            Assert.Contains("strezzte", resourceName);


            var resourceName2 = AzureResourceNameUtil.DiagnosticsStorageAccount("Bestest Study Ever", "The third test we are going to too");
            Assert.InRange(resourceName2.Length, 4, 24);
            Assert.Contains("stdiag", resourceName2);
            Assert.Contains("be", resourceName2);
            Assert.Contains("thethird", resourceName2);

        }

        [Fact]
        public void ResourceGroupName_ShouldFilterAwayNorwegianSpecialLetters()
        {
            var resourceName = AzureResourceNameUtil.SandboxResourceGroup("A revolutional Støddy with a long name", "Bæste sandbåx ju kæn tink");
            Assert.InRange(resourceName.Length, 4, 64);            
            Assert.Contains("rg-study-arevolutionalstddywithalongname-bstesandbxjukntink-", resourceName);
        }

        [Fact]
        public void DiagStorageAccountName_ShouldFilterAwayNorwegianSpecialLetters()
        {
            var resourceName = AzureResourceNameUtil.DiagnosticsStorageAccount("Støddy", "Bæste sandbåx");
            Assert.InRange(resourceName.Length, 4, 24);
     
            Assert.Contains("stdiagstddybstesandbx", resourceName);
   
        }

        [Fact]
        public void AzureResourceNameConstructor_should_work_with_long_names()
        {
            var resourceName = AzureResourceNameUtil.AzureResourceNameConstructor("stdiag",
                "loremipsumloremipsumdolorsitametconsecteturadipiscingeliloremipsumlloremipsumlloremipsumlloremipsumllorem",
                "aasdasasdas11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111", maxLength: 24, addUniqueEnding: true, avoidDash: true);
            //Assert.InRange(resourceName.Length, 4, 24);

            Assert.Contains("stdiagloremipsumloremip-aasdasasdas11111111111111111111111111111", resourceName);

        }

    }
}
