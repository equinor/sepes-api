using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineLookupService
    {       
        string CalculateName(string studyName, string sandboxName, string userPrefix);

        Task<List<VmSizeLookupDto>> AvailableSizes(int sandboxId, CancellationToken cancellationToken = default);

        Task<List<VmSize>> AvailableSizes(string region, CancellationToken cancellationToken = default);
        Task<List<VmDiskLookupDto>> AvailableDisks(CancellationToken cancellationToken = default);


        Task<List<VmOsDto>> AvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default);
        Task<List<VmOsDto>> AvailableOperatingSystems(string region, CancellationToken cancellationToken = default);

        Task<double> CalculatePrice(int sandboxId, CalculateVmPriceUserInputDto userInput, CancellationToken cancellationToken = default);

        Task UpdateVmSizeCache(CancellationToken cancellationToken = default);
        
    }
}
