using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Response.Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{    public interface ISandboxDatasetService
    {       
        Task<IEnumerable<SandboxDatasetDto>> GetAll(int sandboxId);

        Task<AvailableDatasets> AllAvailable(int sandboxId);

        Task<AvailableDatasets> Add(int sandboxId, int datasetId);
        Task<AvailableDatasets> Remove(int sandboxId, int datasetId);
    }
}
