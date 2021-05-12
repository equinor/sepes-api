using Sepes.Common.Response.Sandbox;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface ISandboxDatasetModelService
    {    
        Task<AvailableDatasets> AllAvailable(int sandboxId);

        Task<AvailableDatasets> Add(int sandboxId, int datasetId);

        Task<AvailableDatasets> Remove(int sandboxId, int datasetId);
        Task<List<SandboxDataset>> GetSandboxDatasetsForPhaseShiftAsync(int sandboxId);
    }
}
