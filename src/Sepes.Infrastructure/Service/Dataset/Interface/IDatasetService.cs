using Sepes.Common.Dto.Dataset;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IDatasetService
    {        
        Task<IEnumerable<DatasetLookupItemDto>> GetLookupAsync();
        Task<IEnumerable<DatasetDto>> GetAllAsync();
        Task<DatasetDto> GetByIdAsync(int datasetId);

        Task<DatasetDto> CreateAsync(PreApprovedDatasetCreateUpdateDto newDataset);
        Task<DatasetDto> UpdateAsync(int datasetId, DatasetDto newDataset);

        Task DeleteAsync(int datasetId);      
    }
}
