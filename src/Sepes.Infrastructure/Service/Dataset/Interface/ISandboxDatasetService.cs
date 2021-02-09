using Sepes.Infrastructure.Dto.Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxDatasetService
    {       
        Task<IEnumerable<SandboxDatasetDto>> GetAll(int sandboxId);

        Task<AvailableDatasetResponseDto> AllAvailable(int sandboxId);

        Task<AvailableDatasetResponseDto> Add(int sandboxId, int datasetId);
        Task<AvailableDatasetResponseDto> Remove(int sandboxId, int datasetId);
    }
}
