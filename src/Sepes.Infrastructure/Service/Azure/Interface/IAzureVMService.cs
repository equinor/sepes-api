using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureVMService
    {
        Task<IVirtualMachine> Create(Region region, string resourceGroupName, string sandboxName, INetwork primaryNetwork, 
                                    string subnetName, string userName, string password, string vmPerformanceProfile, 
                                    string os, string distro, IDictionary<string, string> tags, string diagStorageAccountName);
        Task ApplyVMStorageSettings(string resourceGroupName, string virtualMachineName, int size, string type);
        Task Delete(string resourceGroupName, string virtualMachineName);
        Task<bool> Exists(string resourceGroupName, string virtualMachineName);

        Task<string> GetProvisioningState(string resourceGroupName, string resourceName);
    }
}

