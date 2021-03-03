using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.Tests.Common.Constants;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class VirtualMachineSeed
    {
        public static async Task<CloudResource> CreateSimple(
          Sandbox sandbox,     
          string vmNameSuffix = VirtualMachineConstants.NAME)
        {
            var sandboxResourceGroup = CloudResourceUtil.GetSandboxResourceGroupEntry(sandbox.Resources);
            var vmResource = CreateVmResource(sandbox, sandboxResourceGroup, sandbox.Study.Name, vmNameSuffix);
            return await SliceFixture.InsertAsync(vmResource);
        }

        public static async Task<CloudResource> Create(
            Sandbox sandbox,
            string studyName,           
            string vmNameSuffix = VirtualMachineConstants.NAME,         
            string size = VirtualMachineConstants.SIZE,
            string os = VirtualMachineConstants.OS_WINDOWS,
            string username = VirtualMachineConstants.USERNAME)
        {

            var sandboxResourceGroup = CloudResourceUtil.GetSandboxResourceGroupEntry(sandbox.Resources);         
            //Todo: create the vm state object
            var vmResource = CreateVmResource(sandbox, sandboxResourceGroup, studyName, vmNameSuffix);           

            return await SliceFixture.InsertAsync(vmResource);
        }

        static CloudResource CreateVmResource(Sandbox sandbox, CloudResource sandboxResourceGroup, string studyName, string nameSuffix)
        {
            var vmResourceName = AzureResourceNameUtil.VirtualMachine(studyName, sandbox.Name, nameSuffix);

            var vmResource = CloudResourceFactory.Create(sandboxResourceGroup.Region, sandboxResourceGroup.ResourceName, $"{vmResourceName}-resourceId", $"{vmResourceName}-resourceKey", vmResourceName);
            vmResource.SandboxId = sandbox.Id;
            return vmResource;
        }      
    }
}
