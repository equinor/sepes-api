using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Tests.Mocks
{
    class SandboxWorkerServiceMock : ISandboxWorkerService
    {
        public Task<AzureSandboxDto> CreateBasicSandboxResourcesAsync(int sandboxId, Region region, string studyName, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
        }

        public Task<AzureSandboxDto> CreateBastion(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
        }

        public Task<AzureSandboxDto> CreateDiagStorageAccount(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
        }

        public Task<AzureSandboxDto> CreateNetworkSecurityGroup(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
        }

        public Task<AzureSandboxDto> CreateResourceGroup(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
        }

        public Task<AzureSandboxDto> CreateVirtualNetwork(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
        }

        public Task<AzureSandboxDto> CreateVM(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags)
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
