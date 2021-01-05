﻿using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureVmService : IHasProvisioningState, IHasTags, IPerformResourceProvisioning
    {
        Task<IVirtualMachine> CreateAsync(Region region, string resourceGroupName, string vmName, string primaryNetworkName, 
                                    string subnetName, string userName, string password, string vmSize, 
                                    string os, string distro, IDictionary<string, string> tags, string diagStorageAccountName, CancellationToken cancellationToken = default);
        Task ApplyVmDataDisks(string resourceGroupName, string virtualMachineName, int size);
        Task DeleteAsync(string resourceGroupName, string virtualMachineName, string networkSecurityGroupName, string configString);  
      
        Task<IVirtualMachine> GetAsync(string resourceGroupName, string resourceName);

        Task<VmExtendedDto> GetExtendedInfo(string resourceGroupName, string resourceName, CancellationToken cancellationToken = default);
    }
}

