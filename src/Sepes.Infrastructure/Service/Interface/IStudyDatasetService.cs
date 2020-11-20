using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyDatasetService
    { 
        Task<IEnumerable<DataSetsForStudyDto>> GetDatasetsForStudy(int studyId);
        Task<DataSetsForStudyDto> UpdateStudySpecificDatasetAsync(int studyId, int datasetId, StudySpecificDatasetDto newDataset);
        Task<DataSetsForStudyDto> GetDatasetByStudyIdAndDatasetIdAsync(int studyId, int datasetId);

        Task<StudyDto> AddDatasetToStudyAsync(int studyId, int datasetId);
        Task<StudyDto> RemoveDatasetFromStudyAsync(int studyId, int datasetId);
        Task<StudyDto> AddStudySpecificDatasetAsync(int studyId, StudySpecificDatasetDto newDataset);
    }
}
