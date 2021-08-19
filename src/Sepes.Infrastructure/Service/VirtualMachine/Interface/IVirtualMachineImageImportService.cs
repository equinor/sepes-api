using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineImageImportService
    { 
        Task Import(CancellationToken cancellationToken = default);
    }
}
