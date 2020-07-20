using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Sepes.Infrastructure.Util;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureVMService
    {
        Task<IVirtualMachine> Create(Region region, string resourceGroupName, string sandboxName, INetwork primaryNetwork, 
                                    string subnetName, string userName, string password, string vmPerformanceProfile, 
                                    string os, string distro, IDictionary<string, string> tags, string diagStorageAccountName);
        Task ApplyVMStorageSettings(string resourceGroupName, string virtualMachineName, int size, string type);
        Task Delete(string resourceGroupName, string virtualMachineName);
        Task<bool> Exists(string resourceGroupName, string virtualMachineName);
    }
}

