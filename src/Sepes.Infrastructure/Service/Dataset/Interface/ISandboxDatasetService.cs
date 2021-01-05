using Sepes.Infrastructure.Dto.Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxDatasetService
    {       
        Task<IEnumerable<SandboxDatasetDto>> GetAll(int sandboxId);

        Task<IEnumerable<AvailableDatasetDto>> AllAvailable(int sandboxId);

        Task Add(int sandboxId, int datasetId);
        Task Remove(int sandboxId, int datasetId);
    }
}
