using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxPhaseService
    {
        Task MoveToNextPhaseAsync(int sandboxId, CancellationToken cancellation = default);
    }
}
