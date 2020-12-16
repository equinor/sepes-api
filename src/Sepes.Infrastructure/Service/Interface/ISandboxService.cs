using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxService
    {
        Task<SandboxDetailsDto> GetSandboxDetailsAsync(int sandboxId);

        Task<SandboxDto> GetAsync(int sandboxId, UserOperation userOperation);

        Task<IEnumerable<SandboxDto>> GetAllForStudy(int studyId);
        Task<SandboxDetailsDto> CreateAsync(int studyId, SandboxCreateDto newSandbox);
        Task DeleteAsync(int sandboxId);

      

        Task MoveToNextPhase(int sandboxId);
    }
}
