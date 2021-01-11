using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineDiskService
    {  
        //Task<List<VmSizeLookupDto>> AvailableSizes(int sandboxId, CancellationToken cancellationToken = default);

        //Task<List<VmSize>> AvailableSizes(string region, CancellationToken cancellationToken = default);      

        Task UpdateVmDiskSizeCache(CancellationToken cancellationToken = default);

        //Task<double> CalculateVmPrice(int sandboxId, CalculateVmPriceUserInputDto input, CancellationToken cancellationToken = default);
        
    }
}
