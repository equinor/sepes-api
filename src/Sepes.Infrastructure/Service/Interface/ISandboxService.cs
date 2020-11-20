using Sepes.Infrastructure.Dto.Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxService
    {
        Task<SandboxDto> GetAsync(int sandboxId);

        Task<IEnumerable<SandboxDto>> GetForStudyAsync(int studyId);
        Task<SandboxDto> CreateAsync(int studyId, SandboxCreateDto newSandbox);
        Task<SandboxDto> DeleteAsync(int studyId, int sandboxId);

        Task<List<SandboxResourceLightDto>> GetSandboxResources(int studyId, int sandboxId);
        Task ReScheduleSandboxCreation(int studyId);
    }
}
