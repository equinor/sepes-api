using Sepes.Infrastructure.Dto.Sandbox;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxCloudResourceService
    {
        Task<SandboxResourceCreationAndSchedulingDto> CreateBasicSandboxResourcesAsync(SandboxResourceCreationAndSchedulingDto dto);
        Task<SandboxResourceLightDto> RetryLastOperation(int resourceId);
        Task ReScheduleSandboxResourceCreation(int sandboxId);

        Task HandleSandboxDeleteAsync(int sandboxId);
    }   
}
