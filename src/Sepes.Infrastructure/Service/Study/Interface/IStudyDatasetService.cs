using Sepes.Common.Dto;
using Sepes.Common.Dto.Dataset;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyDatasetService
    {        
        Task<DatasetDto> AddPreApprovedDatasetToStudyAsync(int studyId, int datasetId);
        Task RemovePreApprovedDatasetFromStudyAsync(int studyId, int datasetId);
      
        Task<IEnumerable<DatasetDto>> GetDatasetsForStudyAsync(int studyId);
  
        Task<DatasetDto> GetDatasetByStudyIdAndDatasetIdAsync(int studyId, int datasetId);         
    }
}
