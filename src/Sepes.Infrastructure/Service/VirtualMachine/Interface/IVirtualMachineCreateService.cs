using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineCreateService
    {
        Task<VmDto> CreateAsync(int sandboxId, VirtualMachineCreateDto newSandbox);     
    }
}
