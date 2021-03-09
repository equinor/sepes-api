using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineDiskSizeImportService
    { 
        Task Import(CancellationToken cancellationToken = default);
    }
}
