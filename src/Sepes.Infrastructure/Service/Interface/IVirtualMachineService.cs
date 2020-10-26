﻿using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineService
    {
        Task<VmDto> CreateAsync(int sandboxId, CreateVmUserInputDto newSandbox);

        Task<VmDto> UpdateAsync(int sandboxId, CreateVmUserInputDto newSandbox);

        Task<VmDto> DeleteAsync(int id);

        Task<List<VmDto>> VirtualMachinesForSandboxAsync(int sandboxId, CancellationToken cancellationToken = default(CancellationToken));

        Task<VmExtendedDto> GetExtendedInfo(int vmId);

        string CalculateName(string studyName, string sandboxName, string userPrefix);

        Task<List<VmSizeLookupDto>> AvailableSizes(int sandboxId, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<VmDiskLookupDto>> AvailableDisks();

        Task<List<VmOsDto>> AvailableOperatingSystems();
    }
}
