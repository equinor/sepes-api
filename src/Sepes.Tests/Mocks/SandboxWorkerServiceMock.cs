using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Tests.Mocks
{
    class SandboxWorkerServiceMock : ISandboxResourceProvisioningService
    {
        public Task<SandboxWithCloudResourcesDto> CreateBastion(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
        }

        public Task<SandboxWithCloudResourcesDto> CreateDiagStorageAccount(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
        }

        public Task<SandboxWithCloudResourcesDto> CreateNetworkSecurityGroup(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
        }

        public Task CreateResourceGroupForSandbox(SandboxWithCloudResourcesDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<SandboxWithCloudResourcesDto> CreateVirtualNetwork(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
        }

        public Task<SandboxWithCloudResourcesDto> CreateVM(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
        }

        public Task DoWork()
        {
            throw new NotImplementedException();
        }

        public Task NukeSandbox(string studyName, string sandboxName, string resourceGroupName)
        {
            throw new NotImplementedException();
        }

        public Task NukeUnitTestSandboxes()
        {
            throw new NotImplementedException();
        }
    }
}
