using Sepes.Common.Dto;
using Sepes.Common.Response.Sandbox;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxResourceReadService
    {
        Task<List<CloudResourceDto>> GetSandboxResources(int sandboxId, CancellationToken cancellation = default);
        Task<List<SandboxResourceLight>> GetSandboxResourcesLight(int sandboxId);

        Task<string> GetSandboxCostanlysis(int sandboxId, CancellationToken cancellation = default);
    }   
}
