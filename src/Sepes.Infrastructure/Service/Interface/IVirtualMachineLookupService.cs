using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineLookupService
    {       
        string CalculateName(string studyName, string sandboxName, string userPrefix);

        Task<List<VmSizeLookupDto>> AvailableSizes(int sandboxId, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<VmDiskLookupDto>> AvailableDisks();


        Task<List<VmOsDto>> AvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<VmOsDto>> AvailableOperatingSystems(string region, CancellationToken cancellationToken = default(CancellationToken));

        Task<double> CalculatePrice(int sandboxId, CalculateVmPriceUserInputDto userInput);       
        
    }
}
