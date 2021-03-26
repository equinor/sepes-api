using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Response.Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxService
    {
        Task<SandboxDetails> GetSandboxDetailsAsync(int sandboxId);

        Task<SandboxDto> GetAsync(int sandboxId, UserOperation userOperation, bool withIncludes = false);     
        Task<SandboxDetails> CreateAsync(int studyId, SandboxCreateDto newSandbox);
        Task DeleteAsync(int sandboxId);        
    }
}
