﻿using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureVMService : IHasProvisioningState, IHasTags, IPerformCloudResourceCRUD
    {
        Task<IVirtualMachine> Create(Region region, string resourceGroupName, string vmName, string primaryNetworkName, 
                                    string subnetName, string userName, string password, string vmSize, 
                                    string os, string distro, IDictionary<string, string> tags, string diagStorageAccountName, CancellationToken cancellationToken = default(CancellationToken));
        Task ApplyVmDataDisks(string resourceGroupName, string virtualMachineName, int size);
        Task Delete(string resourceGroupName, string virtualMachineName);

        
        Task<IEnumerable<VirtualMachineSize>> GetAvailableVmSizes(string region = null, CancellationToken cancellationToken = default);        
    }
}

