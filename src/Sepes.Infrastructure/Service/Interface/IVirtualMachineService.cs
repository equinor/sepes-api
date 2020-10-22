using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineService
    {
        Task<VmDto> CreateAsync(int sandboxId, CreateVmUserInputDto newSandbox);

        Task<VmDto> UpdateAsync(int sandboxId, CreateVmUserInputDto newSandbox);

        Task<VmDto> DeleteAsync(int id);

        Task<List<VmDto>> VirtualMachinesForSandboxAsync(int sandboxId);

        string CalculateName(string studyName, string sandboxName, string userPrefix);

        Task<List<VmSizeDto>> AvailableSizes();

        Task<List<VmDiskDto>> AvailableDisks();

        Task<List<VmOsDto>> AvailableOperatingSystems();
    }
}
