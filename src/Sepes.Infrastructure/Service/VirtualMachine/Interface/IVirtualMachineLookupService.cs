using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineLookupService
    {       
        string CalculateName(string studyName, string sandboxName, string userPrefix);

        VmUsernameValidateDto CheckIfUsernameIsValidOrThrow(VmUsernameDto input);

        Task<List<VmDiskLookupDto>> AvailableDisks(CancellationToken cancellationToken = default);

        Task<List<VmOsDto>> AvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default);
        Task<List<VmOsDto>> AvailableOperatingSystems(string region, CancellationToken cancellationToken = default);
    }
}
