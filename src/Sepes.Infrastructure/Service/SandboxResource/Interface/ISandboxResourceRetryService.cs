using Sepes.Infrastructure.Dto.Sandbox;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxResourceRetryService
    {       
        Task<SandboxResourceLightDto> RetryLastOperation(int resourceId);
        Task ReScheduleSandboxResourceCreation(int sandboxId);     
       
     
    }   
}
