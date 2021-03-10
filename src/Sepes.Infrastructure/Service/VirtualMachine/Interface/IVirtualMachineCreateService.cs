using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineCreateService
    {
        void ValidateVmPasswordOrThrow(string password);

        Task<VmDto> CreateAsync(int sandboxId, VirtualMachineCreateDto newSandbox);     
    }
}
