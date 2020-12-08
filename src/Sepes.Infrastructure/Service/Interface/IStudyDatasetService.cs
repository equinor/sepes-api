using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyDatasetService
    {
        //Add remove existing datasets to Study
        Task<StudyDatasetDto> AddDatasetToStudyAsync(int studyId, int datasetId);
        Task RemoveDatasetFromStudyAsync(int studyId, int datasetId);

        //Study specific data set operations
        Task<IEnumerable<StudyDatasetDto>> GetDatasetsForStudyAsync(int studyId);
  
        Task<StudyDatasetDto> GetDatasetByStudyIdAndDatasetIdAsync(int studyId, int datasetId);     
        Task<StudyDatasetDto> CreateStudySpecificDatasetAsync(int studyId, DatasetCreateUpdateInputDto newDataset, CancellationToken cancellationToken = default);

        Task<StudyDatasetDto> UpdateStudySpecificDatasetAsync(int studyId, int datasetId, DatasetCreateUpdateInputDto newDataset);

        string CalculateStorageAccountName(string userPrefix);
        Task SoftDeleteStudySpecificDatasetAsync(Study study, int datasetId, CancellationToken cancellationToken = default);
        Task HardDeleteStudySpecificDatasetAsync(Study study, int datasetId, CancellationToken cancellationToken = default);      
        Task SoftDeleteAllStudySpecificDatasetsAsync(Study study, CancellationToken cancellationToken = default);
        Task HardDeleteAllStudySpecificDatasetsAsync(Study study, CancellationToken cancellationToken = default);
    }
}
