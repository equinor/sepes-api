using Sepes.Infrastructure.Dto.Sandbox;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxPhaseService
    {
        Task<SandboxDetailsDto> MoveToNextPhaseAsync(int sandboxId, CancellationToken cancellation = default);
    }
}
