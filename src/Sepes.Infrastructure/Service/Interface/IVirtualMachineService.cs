using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineService
    {
        Task<VmDto> CreateAsync(int sandboxId, CreateVmUserInputDto newSandbox);

        Task<VmDto> UpdateAsync(int sandboxId, CreateVmUserInputDto newSandbox);

        Task<VmDto> DeleteAsync(int id);

        string CalculateName(string studyName, string sandboxName, string userPrefix);
    }
}
