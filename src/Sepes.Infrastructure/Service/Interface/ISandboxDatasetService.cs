using Sepes.Infrastructure.Dto.Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxDatasetService
    {
        Task<SandboxDatasetDto> Add(int sandboxId, int datasetId);
        Task<IEnumerable<SandboxDatasetDto>> GetAll(int sandboxId);
        Task<SandboxDatasetDto> Remove(int sandboxId, int datasetId);
    }
}
