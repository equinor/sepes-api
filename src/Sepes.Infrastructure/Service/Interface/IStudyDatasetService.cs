using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyDatasetService
    {        
        Task<StudyDatasetDto> AddPreApprovedDatasetToStudyAsync(int studyId, int datasetId);
        Task RemovePreApprovedDatasetFromStudyAsync(int studyId, int datasetId);
      
        Task<IEnumerable<StudyDatasetDto>> GetDatasetsForStudyAsync(int studyId);
  
        Task<StudyDatasetDto> GetDatasetByStudyIdAndDatasetIdAsync(int studyId, int datasetId);         
    }
}
