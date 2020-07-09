using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IDatasetService
    {
        
        Task<IEnumerable<DatasetListItemDto>> GetDatasetsLookupAsync();
        Task<IEnumerable<DatasetDto>> GetDatasetsAsync();
        Task<DatasetDto> GetDatasetByDatasetIdAsync(int datasetId);
        Task<DatasetDto> GetDatasetByStudyIdAndDatasetIdAsync(int studyId, int datasetId);
        Task<DatasetDto> CreateDatasetAsync(DatasetDto newDataset);
        Task<DatasetDto> UpdateDatasetAsync(int datasetId, DatasetDto newDataset);
        Task<DatasetDto> UpdateStudySpecificDatasetAsync(int studyId, int datasetId, StudySpecificDatasetDto newDataset);

        // ------------------Does actions against study--------------
        Task<StudyDto> AddDatasetToStudyAsync(int studyId, int datasetId);
        Task<StudyDto> RemoveDatasetFromStudyAsync(int studyId, int datasetId);
        Task<StudyDto> AddStudySpecificDatasetAsync(int studyId, StudySpecificDatasetDto newDataset);
    }
}
