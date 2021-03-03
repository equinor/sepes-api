using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.Tests.Common.Constants;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class VirtualMachineSeed
    {     

        public static async Task<CloudResource> Create(
            Sandbox sandbox,
            string vmNameSuffix = VirtualMachineConstants.NAME,
            string size = VirtualMachineConstants.SIZE,
            string osCategory = VirtualMachineConstants.OS_CATEGORY_WINDOWS,
            string os = VirtualMachineConstants.OS_WINDOWS)
        {
            var sandboxResourceGroup = CloudResourceUtil.GetSandboxResourceGroupEntry(sandbox.Resources);

           var vmSettings = CreateVmSettingsString(size, osCategory, os);       

            var vmResource = CreateVmResource(sandbox, sandboxResourceGroup, sandbox.Study.Name, vmNameSuffix, vmSettings);

            return await SliceFixture.InsertAsync(vmResource);
        }

        static CloudResource CreateVmResource(Sandbox sandbox, CloudResource sandboxResourceGroup, string studyName, string nameSuffix, string configString = null)
        {
            var vmResourceName = AzureResourceNameUtil.VirtualMachine(studyName, sandbox.Name, nameSuffix);

            var vmResource = CloudResourceFactory.Create(sandboxResourceGroup.Region, AzureResourceType.VirtualMachine, sandboxResourceGroup.ResourceName, vmResourceName, parentResource: sandboxResourceGroup);
            vmResource.SandboxId = sandbox.Id;
            vmResource.ConfigString = configString;
            return vmResource;
        }

        static string CreateVmSettingsString(string size = VirtualMachineConstants.SIZE, string osCategory = "windows", string os = "win2019datacenter")
        {
            var vmSettings = new VmSettingsDto()
            {
                DiagnosticStorageAccountName = "diagstorageaccountname",
                NetworkName = "networkName",
                SubnetName = "subnetname",
                Size = size,
                Rules = AzureVmConstants.RulePresets.CreateInitialVmRules(1),
                OperatingSystemCategory = osCategory,
                OperatingSystem = os,
                Username = VirtualMachineConstants.USERNAME,
                Password = "nameinkeyvault",
            };

            return CloudResourceConfigStringSerializer.Serialize(vmSettings);
        }
    }
}
