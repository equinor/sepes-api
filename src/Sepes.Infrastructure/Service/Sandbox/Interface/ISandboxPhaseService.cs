using Sepes.Infrastructure.Response.Sandbox;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxPhaseService
    {
        Task<SandboxDetails> MoveToNextPhaseAsync(int sandboxId, CancellationToken cancellation = default);
    }
}
