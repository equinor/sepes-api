using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxResourceProvisioningService
    {
        Task LookForWork();

        Task CarryOutWork(ProvisioningQueueParentDto work);

        //Task<SandboxWithCloudResourcesDto> CreateDiagStorageAccount(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags);
        //Task<SandboxWithCloudResourcesDto> CreateNetworkSecurityGroup(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags);
        //Task<SandboxWithCloudResourcesDto> CreateVirtualNetwork(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags);
        //Task<SandboxWithCloudResourcesDto> CreateBastion(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags);
        //Task<SandboxWithCloudResourcesDto> CreateVM(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags);
       
        //Task NukeUnitTestSandboxes();
    }
}
