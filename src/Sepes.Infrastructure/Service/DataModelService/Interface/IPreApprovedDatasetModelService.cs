using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IPreApprovedDatasetModelService
    { 
        Task<Dataset> GetByIdAsync(int datasetId, UserOperation userOperation);

        Task<List<Dataset>> GetAllAsync(UserOperation userOperation);

        Task<bool> IsStudySpecific(int datasetId);
        Task<Dataset> CreateAsync(Dataset newDataset);
    }
}
