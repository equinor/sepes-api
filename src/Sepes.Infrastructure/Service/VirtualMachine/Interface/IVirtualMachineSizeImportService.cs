using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineSizeImportService
    {     

        Task UpdateVmSizeCache(CancellationToken cancellationToken = default);    
        
    }
}
