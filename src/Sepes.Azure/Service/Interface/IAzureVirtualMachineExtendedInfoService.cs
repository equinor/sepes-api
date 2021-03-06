﻿using Sepes.Common.Dto.VirtualMachine;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureVirtualMachineExtendedInfoService
    {
        Task<VmExtendedDto> GetExtendedInfo(string resourceGroupName, string resourceName, CancellationToken cancellationToken = default);
    }
}

