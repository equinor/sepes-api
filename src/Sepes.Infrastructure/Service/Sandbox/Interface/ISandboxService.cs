using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Response.Sandbox;
using Sepes.Infrastructure.Model;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxService
    {
        Task<Sandbox> CreateAsync(int studyId, SandboxCreateDto newSandbox);
        
        Task<SandboxDetails> GetSandboxDetailsAsync(int sandboxId);

        Task DeleteAsync(int sandboxId);        
    }
}
