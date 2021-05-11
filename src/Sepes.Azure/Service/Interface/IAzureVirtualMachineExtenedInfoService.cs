using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureVirtualMachineExtenedInfoService
    {
        Task<VmExtendedDto> GetExtendedInfo(string resourceGroupName, string resourceName, CancellationToken cancellationToken = default);
    }
}

