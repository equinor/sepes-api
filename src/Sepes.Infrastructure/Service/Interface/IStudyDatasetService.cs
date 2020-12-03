using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Dto.Study;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyDatasetService
    {
        //Add remove existing datasets to Study
        Task<StudyDto> AddDatasetToStudyAsync(int studyId, int datasetId);
        Task<StudyDto> RemoveDatasetFromStudyAsync(int studyId, int datasetId);

        //Study specific data set operations
        Task<IEnumerable<StudyDatasetDto>> GetDatasetsForStudy(int studyId);
  
        Task<StudyDatasetDto> GetDatasetByStudyIdAndDatasetIdAsync(int studyId, int datasetId);     
        Task<StudyDatasetDto> CreateStudySpecificDatasetAsync(int studyId, DatasetCreateUpdateInputDto newDataset);

        Task<StudyDatasetDto> UpdateStudySpecificDatasetAsync(int studyId, int datasetId, DatasetCreateUpdateInputDto newDataset);
    }
}
