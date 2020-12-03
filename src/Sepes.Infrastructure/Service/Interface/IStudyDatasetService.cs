using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Dataset;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyDatasetService
    {
        //Add remove existing datasets to Study
        Task<StudyDatasetDto> AddDatasetToStudyAsync(int studyId, int datasetId);
        Task RemoveDatasetFromStudyAsync(int studyId, int datasetId);

        //Study specific data set operations
        Task<IEnumerable<StudyDatasetDto>> GetDatasetsForStudy(int studyId);
  
        Task<StudyDatasetDto> GetDatasetByStudyIdAndDatasetIdAsync(int studyId, int datasetId);     
        Task<StudyDatasetDto> CreateStudySpecificDatasetAsync(int studyId, DatasetCreateUpdateInputDto newDataset);

        Task<StudyDatasetDto> UpdateStudySpecificDatasetAsync(int studyId, int datasetId, DatasetCreateUpdateInputDto newDataset);
    }
}
