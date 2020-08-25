using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxWorkerService
    {
        Task DoWork();
        Task<AzureSandboxDto> CreateBasicSandboxResourcesAsync(int sandboxId, Region region, string studyName, Dictionary<string, string> tags);
        Task<AzureSandboxDto> CreateResourceGroup(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags);
        Task<AzureSandboxDto> CreateDiagStorageAccount(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags);
        Task<AzureSandboxDto> CreateNetworkSecurityGroup(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags);
        Task<AzureSandboxDto> CreateVirtualNetwork(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags);
        Task<AzureSandboxDto> CreateBastion(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags);
        Task<AzureSandboxDto> CreateVM(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags);
        Task NukeSandbox(string studyName, string sandboxName, string resourceGroupName);
        Task NukeUnitTestSandboxes();
    }
}
