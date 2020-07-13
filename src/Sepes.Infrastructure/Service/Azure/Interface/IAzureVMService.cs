using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Sepes.Infrastructure.Util;
using Microsoft.Azure.Management.Compute.Fluent.Models;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureVMService
    {
        Task<IVirtualMachine> Create(Region region, string resourceGroupName, string sandboxName, INetwork primaryNetwork, 
                                    string subnetName, string userName, string password, string vmPerformanceProfile = "Cheap", 
                                    string os = "Windows Server 2012");
        Task ApplyVMStorageSettings(string resourceGroupName, string virtualMachineName);
        Task Delete(string resourceGroupName, string virtualMachineName);
    }
}

