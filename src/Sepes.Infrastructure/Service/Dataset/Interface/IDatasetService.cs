using Sepes.Infrastructure.Dto.Dataset;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IDatasetService
    {        
        Task<IEnumerable<DatasetListItemDto>> GetDatasetsLookupAsync();
        Task<IEnumerable<DatasetDto>> GetDatasetsAsync();
        Task<DatasetDto> GetDatasetByDatasetIdAsync(int datasetId);

        Task<DatasetDto> CreateDatasetAsync(PreApprovedDatasetCreateUpdateDto newDataset);
        Task<DatasetDto> UpdateDatasetAsync(int datasetId, DatasetDto newDataset);

        Task DeleteDatasetAsync(int datasetId);
        Task<bool> IsStudySpecific(int datasetId);
    }
}
