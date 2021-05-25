using Sepes.Common.Response.Sandbox;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxResourceReadService
    {       
        Task<List<SandboxResourceLight>> GetSandboxResourcesLight(int sandboxId);

        Task<string> GetSandboxCostanlysis(int sandboxId, CancellationToken cancellation = default);
    }   
}
