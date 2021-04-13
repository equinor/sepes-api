using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Response.Sandbox;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxService
    {
        Task<SandboxDetails> GetSandboxDetailsAsync(int sandboxId);
          
        Task<SandboxDetails> CreateAsync(int studyId, SandboxCreateDto newSandbox);
        Task DeleteAsync(int sandboxId);        
    }
}
